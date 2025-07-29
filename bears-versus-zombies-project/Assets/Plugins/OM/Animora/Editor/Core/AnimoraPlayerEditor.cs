using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OM.Animora.Runtime;
using OM.Editor;
using OM.TimelineCreator.Editor;
using OM.TimelineCreator.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Animora.Editor
{
    /// <summary>
    /// Custom inspector for <see cref="AnimoraPlayer"/>, providing timeline editing, clip management,
    /// settings UI, and dynamic action inspection.
    /// </summary>
    [CustomEditor(typeof(AnimoraPlayer))]
    public class AnimoraPlayerEditor : UnityEditor.Editor, IOM_SearchPopupOwner, IOM_TimelineEditorOwner<AnimoraClip>
    {
        public OM_VisualElementsManager VisualElementsManager { get; private set; }
        public UnityEditor.Editor Editor => this;
        public IOM_TimelinePlayer<AnimoraClip> TimelinePlayer => Player;

        public AnimoraTimeline AnimoraTimeline { get; private set; }
        public AnimoraPlayer Player { get; private set; }
        public VisualElement Root { get; private set; }
        public AnimoraInspector Inspector { get; private set; }

        private List<Type> _cachedClipsTypes;
        private AnimoraPlayerEditorControlSection _animoraPlayerEditorControlSection;

        private List<AnimoraPlayer> _allPlayersInScene;

        /// <summary>
        /// Called when the editor is enabled.
        /// </summary>
        private void OnEnable()
        {
            VisualElementsManager = new OM_VisualElementsManager();
            Player = (AnimoraPlayer)target;

            Undo.undoRedoPerformed += UndoRedoPerformed;
            Player.OnPlayerValidateCallback += OnPlayerValidate;

            _allPlayersInScene = FindObjectsByType<AnimoraPlayer>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .ToList();
            _allPlayersInScene.Remove(Player);
        }

        /// <summary>
        /// Called when the editor is disabled.
        /// </summary>
        private void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            Player.OnPlayerValidateCallback -= OnPlayerValidate;
            _animoraPlayerEditorControlSection?.OnDisable();
        }

        private void UndoRedoPerformed() => VisualElementsManager.TriggerUndoRedo();
        private void OnPlayerValidate() => VisualElementsManager.TriggerValidate();

        /// <summary>
        /// Creates the full inspector UI.
        /// </summary>
        public override VisualElement CreateInspectorGUI()
        {
            Root = new VisualElement();
            Root.AddStyleSheet("Animora");

            DrawSettings();
            DrawTimeline();
            DrawInspector();

            // Ensure all clip actions are up to date
            foreach (var clip in Player.GetClips())
            {
                if (clip is IAnimoraComponentsManagerOwner actionOwner)
                {
                    actionOwner.ActionsManager.CheckActions(Player);
                }
            }

            return Root;
        }

        /// <summary>
        /// Draws the player settings UI as a foldout group.
        /// </summary>
        private void DrawSettings()
        {
            OM_FoldoutGroup foldoutGroup = null;

            foreach (var property in serializedObject.GetAllProperties())
            {
                if (property.name == "m_Script") continue;

                if (foldoutGroup == null)
                {
                    foldoutGroup = OM_FoldoutGroup.CreateSettingsGroup(property.Copy(), "Player Settings", "");
                    foldoutGroup.AddClassNames("main-settings-group");
                    Root.Add(foldoutGroup);
                }

                var hideInInspector = property.GetAttribute<OM_HideInInspector>(true);
                if (hideInInspector != null) continue;

                var field = new PropertyField(property);
                field.Bind(serializedObject);

                if (property.name == "animoraPlayerEvents")
                {
                    Root.Add(field);
                    field.style.marginBottom = 5;
                    continue;
                }
                
                foldoutGroup.AddToContent(field);
                
                if (property.name == "playerUniqueID")
                {
                    var helpBox = new HelpBox();
                    helpBox.messageType = HelpBoxMessageType.Info;
                    helpBox.text = "";
                    foldoutGroup.AddToContent(helpBox);
                    field.RegisterValueChangeCallback(e =>
                    {
                        var any = _allPlayersInScene.Any(x=>x.PlayerUniqueID == e.changedProperty.stringValue);
                        if (any)
                        {
                            helpBox.text = "This ID is already used by another player in the scene.";
                            helpBox.messageType = HelpBoxMessageType.Warning;
                        }
                        else
                        {
                            helpBox.text = "This ID is unique in the scene.";
                            helpBox.messageType = HelpBoxMessageType.Info;
                        }
                    });
                }
            }

            var settings = OM_TimelineSettings.Instance;
            if (settings != null && foldoutGroup != null)
            {
                var editor = CreateEditor(settings);
                foldoutGroup.AddToContent(editor.CreateInspectorGUI());
            }

            if (foldoutGroup != null)
                foldoutGroup.style.marginBottom = 5;
        }

        /// <summary>
        /// Draws the timeline editor and header control section.
        /// </summary>
        private void DrawTimeline()
        {
            AnimoraTimeline = new AnimoraTimeline(this, this);
            Root.Add(AnimoraTimeline);

            _animoraPlayerEditorControlSection = new AnimoraPlayerEditorControlSection(this);
            _animoraPlayerEditorControlSection.OnEnable();

            AnimoraTimeline.Header.OnContextButtonClicked += () =>
            {
                var menu = new GenericMenu();

                menu.AddItem(new GUIContent("Edit Script"), false, () =>
                {
                    var scriptProp = serializedObject.FindProperty("m_Script");
                    var script = scriptProp.objectReferenceValue as MonoScript;
                    if (script != null)
                    {
                        AssetDatabase.OpenAsset(script);
                    }
                });

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Save Timeline"), false, () => AnimoraSaveManager.Save(Player));
                menu.AddItem(new GUIContent("Load Timeline"), false, () => AnimoraSaveManager.Load(Player));

                menu.ShowAsContext();
            };
        }

        /// <summary>
        /// Draws the inspector section for the currently selected clip.
        /// </summary>
        private void DrawInspector()
        {
            Inspector = new AnimoraInspector(this);
            Root.Add(Inspector);
        }

        /// <summary>
        /// Supplies the searchable list of available clip types.
        /// </summary>
        public List<OM_SearchPopupItemData> GetSearchItems()
        {
            _cachedClipsTypes ??= GetAllClipsFromAssembly();
            var searchItems = new List<OM_SearchPopupItemData>();

            foreach (var clipType in _cachedClipsTypes)
            {
                var createAttr = clipType.GetCustomAttribute<AnimoraCreateAttribute>();
                if (createAttr == null) continue;

                var iconAttr = clipType.GetCustomAttribute<AnimoraIconAttribute>();
                var iconPath = iconAttr?.IconName;
                var iconType = iconAttr?.IconType ?? OM_IconType.ResourcesFolder;
                var description = clipType.GetCustomAttribute<AnimoraDescriptionAttribute>()?.Description;
                var keywords = clipType.GetCustomAttribute<AnimoraKeywordsAttribute>()?.Keywords;

                searchItems.Add(new OM_SearchPopupItemData(
                    createAttr.Name, createAttr.Path, clipType,
                    iconType, iconPath, description, keywords));
            }

            return searchItems;
        }

        /// <summary>
        /// Handles the creation and registration of a new clip from the search popup.
        /// </summary>
        public void OnSearchItemSelected(OM_SearchPopupItemData data)
        {
            if (data.Data is not Type clipType) return;

            var instance = (AnimoraClip)Activator.CreateInstance(clipType);
            instance.OnCreate(Player);
            instance.ClipName = data.Name;
            instance.ClipDescription = data.Description;
            instance.Duration = Player.GetTimelineDuration() * .5f; // half the timeline duration
            instance.HighlightColor = GetNextColor();

            Player.AddClip(instance);
            serializedObject.Update();
            AnimoraTimeline.SelectTrack(Player.GetClips().Count() - 1);
        }

        /// <summary>
        /// Returns all non-abstract <see cref="AnimoraClip"/> types in the current AppDomain.
        /// </summary>
        private static List<Type> GetAllClipsFromAssembly()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => typeof(AnimoraClip).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList();
        }

        /// <summary>
        /// A rotating list of colors used to differentiate clips (optional).
        /// </summary>
        private static readonly List<Color> GetColors = new()
        {
            new Color(0.8f, 0.33f, 0.41f),
            new Color(0.4f, 0.8f, 0.41f),
            new Color(0.39f, 0.38f, 0.8f),
            new Color(0.8f, 0.78f, 0.51f),
            new Color(0.8f, 0.49f, 0.8f),
            new Color(0.38f, 0.78f, 0.8f),
        };

        /// <summary>
        /// Cycles through predefined colors to assign to new clips.
        /// </summary>
        private static Color GetNextColor()
        {
            var i = PlayerPrefs.GetInt("AnimoraColorIndex", 0);
            PlayerPrefs.SetInt("AnimoraColorIndex", i + 1);

            if (i < GetColors.Count) return GetColors[i];
            i = 0;
            PlayerPrefs.SetInt("AnimoraColorIndex", 0);

            return GetColors[i];
        }
    }
}
