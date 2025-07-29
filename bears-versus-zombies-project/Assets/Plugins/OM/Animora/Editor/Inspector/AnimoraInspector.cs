using System.Linq;
using System.Reflection;
using OM.Editor;
using OM.Animora.Runtime;
using OM.TimelineCreator.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Animora.Editor
{
    /// <summary>
    /// Custom inspector UI that displays detailed settings for the currently selected <see cref="AnimoraClip"/>.
    /// Dynamically reacts to timeline selection changes and supports clip interaction, renaming, toggling, and context actions.
    /// </summary>
    public class AnimoraInspector : VisualElement
    {
        private readonly AnimoraPlayerEditor _playerEditor;
        private readonly AnimoraPlayer _player;
        private readonly AnimoraTimeline _timeline;

        private OM_Track<AnimoraClip, AnimoraTrack> SelectedTrack { get; set; }

        private readonly VisualElement _container;
        private PropertyField _clipPropertyField;

        /// <summary>
        /// Initializes the inspector for the given <see cref="AnimoraPlayerEditor"/>.
        /// </summary>
        /// <param name="playerEditor">The parent editor instance.</param>
        public AnimoraInspector(AnimoraPlayerEditor playerEditor)
        {
            _playerEditor = playerEditor;
            _player = playerEditor.Player;
            _timeline = playerEditor.AnimoraTimeline;

            _container = new VisualElement();
            _container.AddClassNames("inspector");
            Add(_container);

            _timeline.OnSelectedTrackChanged += TimelineOnOnSelectedTrackChanged;
            TimelineOnOnSelectedTrackChanged(_timeline.SelectedTrack);
        }

        /// <summary>
        /// Called when the selected track changes in the timeline.
        /// </summary>
        private void TimelineOnOnSelectedTrackChanged(OM_Track<AnimoraClip, AnimoraTrack> track)
        {
            SelectedTrack = track;
            RefreshInspector();
        }

        /// <summary>
        /// Hides the inspector UI.
        /// </summary>
        private void HideInspector()
        {
            _container.Clear();
            _container.SetDisplay(false);
        }

        /// <summary>
        /// Rebuilds the inspector UI for the currently selected track/clip.
        /// </summary>
        public virtual void RefreshInspector()
        {
            if (SelectedTrack == null)
            {
                HideInspector();
                return;
            }

            var selectedClipIndex = _player.GetClips().ToList().IndexOf(SelectedTrack.Clip);
            if (selectedClipIndex == -1)
            {
                HideInspector();
                return;
            }

            var clipsManagerProp = _playerEditor.serializedObject.FindProperty("clipsManager");
            var clipsProp = clipsManagerProp.FindPropertyRelative("clips");

            if (clipsProp == null || selectedClipIndex >= clipsProp.arraySize)
            {
                HideInspector();
                return;
            }

            _container.Clear();
            _container.SetDisplay(true);

            var selectedClipProp = clipsProp.GetArrayElementAtIndex(selectedClipIndex);
            var clip = selectedClipProp.GetValueCustom(true);

            // Header
            var header = new VisualElement();
            header.AddClassNames("inspector-header");
            _container.Add(header);

            // IsActive toggle
            var isActiveProp = selectedClipProp.FindPropertyRelative("isActive");
            var isActiveSwitcher = OM_Switcher.CreateSwitcher("", isActiveProp.Copy(), isActive =>
            {
                isActiveProp.boolValue = isActive;
                _clipPropertyField.SetEnabled(isActive);
            });
            header.Add(isActiveSwitcher);

            // Clip name field
            var clipNameProp = selectedClipProp.FindPropertyRelative("clipName");
            var nameField = new TextField(clipNameProp.stringValue)
            {
                label = ""
            };
            nameField.AddClassNames("inspector-header-name-field");
            nameField.BindProperty(clipNameProp);
            nameField.Bind(_playerEditor.serializedObject);
            header.Add(nameField);

            // Context menu button
            var contextMenuButton = new Button
            {
                text = "⋮"
            };
            contextMenuButton.AddToClassList("inspector-header-context-menu-button");
            contextMenuButton.clicked += () =>
            {
                var menu = new GenericMenu();

                menu.AddItem(new GUIContent("Edit Script"), false, () =>
                {
                    OMEditorUtility.OpenScriptInEditorByName(clip.GetType().Name);
                });

                menu.AddItem(new GUIContent("Reset"), false, () =>
                {
                    if (clip is AnimoraClip animoraClip)
                    {
                        animoraClip.Reset();
                    }
                    else
                    {
                        Debug.LogError("Clip is null or not of type AnimoraClip");
                    }
                });

                // Context menu actions via [ContextMenu]
                var value = selectedClipProp.GetValue();
                var methodInfos = value.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var separatorAdded = false;

                foreach (var methodInfo in methodInfos)
                {
                    var contextAttr = methodInfo.GetCustomAttribute<ContextMenu>();
                    if (contextAttr != null)
                    {
                        if (!separatorAdded)
                        {
                            menu.AddSeparator("");
                            separatorAdded = true;
                        }

                        menu.AddItem(new GUIContent(contextAttr.menuItem), false, () =>
                        {
                            var parameters = methodInfo.GetParameters();
                            methodInfo.Invoke(value, parameters.Length == 0 ? null : new object[] { _player });
                        });
                    }
                }

                menu.ShowAsContext();
            };
            header.Add(contextMenuButton);

            if (OMEditorUtility.GetScriptByName(SelectedTrack.Clip.GetType().Name,out var script))
            {
                var scriptField = new ObjectField("Script")
                {
                    objectType = typeof(MonoScript),
                    value = script
                };
                scriptField.AddClassNames("inspector-header-script-field");
                _container.Add(scriptField);
                scriptField.SetEnabled(false);
            }
            else
            {
                var clipName = new Label(SelectedTrack.Clip.GetType().Name);
                clipName.AddClassNames("inspector-header-clip-name");
                _container.Add(clipName);
            }

            // Full clip inspector
            _clipPropertyField = new PropertyField(selectedClipProp);
            _clipPropertyField.Bind(_playerEditor.serializedObject);
            _container.Add(_clipPropertyField);
        }
    }
}
