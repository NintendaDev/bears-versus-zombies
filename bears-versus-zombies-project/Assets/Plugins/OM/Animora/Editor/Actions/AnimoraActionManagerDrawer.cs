using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OM.Editor;
using OM.Animora.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Animora.Editor
{
    /// <summary>
    /// Custom property drawer for <see cref="AnimoraActionsManager"/>.
    /// Displays grouped and reorderable Animora actions using foldouts, drag-and-drop, and a searchable popup.
    /// </summary>
    [CustomPropertyDrawer(typeof(AnimoraActionsManager), true)]
    public class AnimoraActionManagerDrawer : PropertyDrawer, IOM_SearchPopupOwner
    {
        private readonly HashSet<AnimoraActionElement> _actionElements = new();
        private readonly List<AnimoraActionDropArea> _dropAreas = new();

        public OM_FoldoutGroup FoldoutGroup { get; private set; }
        public VisualElement DraggingArea { get; private set; }
        public SerializedObject SerializedObject { get; private set; }
        public AnimoraPlayer Player { get; private set; }
        public AnimoraActionsManager ActionsManager { get; private set; }
        public SerializedProperty ActionsContainerProperty { get; private set; }
        public AnimoraClip Clip { get; private set; }

        private AnimoraActionGroup _currentActionGroup;

        /// <summary>
        /// Attempts to get the drop area under the given mouse position.
        /// </summary>
        public bool TryGetDropArea(Vector2 mousePosition, out AnimoraActionDropArea dropArea)
        {
            foreach (var data in _dropAreas)
            {
                if (data.localBound.Contains(mousePosition))
                {
                    dropArea = data;
                    return true;
                }
            }

            dropArea = null;
            return false;
        }

        /// <summary>
        /// Creates the UI layout for the property drawer.
        /// </summary>
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _dropAreas.Clear();
            _actionElements.Clear();

            SerializedObject = property.serializedObject;
            ActionsContainerProperty = property.Copy();

            ActionsManager = property.GetValueCustom(true) as AnimoraActionsManager;
            if (ActionsManager == null) return new Label("No Actions Manager found");

            Player = property.serializedObject.targetObject as AnimoraPlayer;
            if (Player == null) return new Label("No AnimoraPlayer found");

            ActionsManager.CheckActions(Player);

            FoldoutGroup = OM_FoldoutGroup.CreateSettingsGroup(property.Copy(), "Actions", "Actions");
            FoldoutGroup.AddStyleSheet("AnimoraActionsManager");
            FoldoutGroup.Content.AddManipulator(new OM_DragControlManipulator());
            FoldoutGroup.Content.AddClassNames("actions-manager-container");

            var serializedProperty = property.GetParentProperty();
            Clip = serializedProperty.GetValueCustom(true) as AnimoraClip;
            if (Clip == null) return new Label("No Clip Found");

            // Create drop areas for each group
            foreach (AnimoraActionGroup groupType in Enum.GetValues(typeof(AnimoraActionGroup)))
            {
                var groupHeader = new AnimoraActionDropAreaHeader(groupType);
                FoldoutGroup.AddToContent(groupHeader);

                var groupElement = new AnimoraActionDropArea(groupType, this);
                FoldoutGroup.AddToContent(groupElement);
                _dropAreas.Add(groupElement);
            }

            // Create invisible dragging overlay
            DraggingArea = new VisualElement().AddTo(FoldoutGroup.Content).SetPickingMode(PickingMode.Ignore);
            DraggingArea.style.position = Position.Absolute;
            DraggingArea.style.left = 0;
            DraggingArea.style.top = 0;
            DraggingArea.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            DraggingArea.style.height = new StyleLength(new Length(100, LengthUnit.Percent));

            CreateOrRemoveActions();

            Undo.undoRedoPerformed += UndoRedoPerformed;
            ActionsManager.OnActionsChanged += ActionsManagerOnOnActionsChanged;

            FoldoutGroup.RegisterCallback<DetachFromPanelEvent>(_ =>
            {
                Undo.undoRedoPerformed -= UndoRedoPerformed;
                ActionsManager.OnActionsChanged -= ActionsManagerOnOnActionsChanged;
            });

            return FoldoutGroup;
        }

        /// <summary>
        /// Called when the actions manager fires a change event.
        /// </summary>
        private void ActionsManagerOnOnActionsChanged()
        {
            CreateOrRemoveActions();
        }

        /// <summary>
        /// Handles changes after undo/redo operations.
        /// </summary>
        private void UndoRedoPerformed()
        {
            if (Player.ClipsManager.GetClips().Contains(Clip))
            {
                CreateOrRemoveActions();
            }
        }

        /// <summary>
        /// Synchronizes the displayed elements with the actual state of the <see cref="AnimoraActionsManager"/>.
        /// </summary>
        private void CreateOrRemoveActions()
        {
            if (ActionsManager == null || ActionsContainerProperty == null)
                return;

            SerializedObject.Update();
            if (ActionsContainerProperty.GetValueCustom(true) == null)
                return;

            var currentActions = ActionsManager.GetComponents();
            var actionsProperty = ActionsContainerProperty.FindPropertyRelative("actions");
            if (actionsProperty == null)
                return;

            // Remove stale elements
            _actionElements.RemoveWhere(element =>
            {
                if (element.parent is AnimoraActionDropArea dropArea &&
                    element.Action.ActionGroup != dropArea.ActionGroup)
                {
                    element.RemoveFromHierarchy();
                    return true;
                }

                if (element.Action == null || !currentActions.Contains(element.Action))
                {
                    element.RemoveFromHierarchy();
                    return true;
                }

                return false;
            });

            // Add new elements
            var changedDropAreas = new List<AnimoraActionDropArea>();
            foreach (var action in currentActions)
            {
                if (_actionElements.All(x => x.Action != action))
                {
                    if (action == null) continue;

                    var p = actionsProperty.GetArrayElementAtIndex(currentActions.IndexOf(action));
                    var element = new AnimoraActionElement(action, ActionsManager, this, p.Copy(), Clip);
                    _actionElements.Add(element);

                    var dropArea = _dropAreas.First(x => x.ActionGroup == action.ActionGroup);
                    dropArea.Add(element);

                    if (!changedDropAreas.Contains(dropArea))
                    {
                        changedDropAreas.Add(dropArea);
                    }
                }
            }

            // Reorder elements
            foreach (var dropArea in changedDropAreas)
            {
                var ordered = _actionElements
                    .Where(x => x.Action.ActionGroup == dropArea.ActionGroup)
                    .OrderBy(x => x.Action.OrderIndex)
                    .ToList();

                for (int i = 0; i < ordered.Count; i++)
                {
                    var elem = ordered[i];
                    if (dropArea.IndexOf(elem) == i) continue;

                    dropArea.Remove(elem);
                    dropArea.Insert(i, elem);
                    elem.Action.OrderIndex = i;
                }
            }
        }

        /// <summary>
        /// Displays a search popup for adding new actions.
        /// </summary>
        public void ShowSearchPopup(AnimoraActionGroup actionGroup, VisualElement target = null)
        {
            _currentActionGroup = actionGroup;
            OM_SearchPopup.Open(target ?? FoldoutGroup.Content, this);
        }

        /// <summary>
        /// Supplies the search popup with available action types.
        /// </summary>
        public List<OM_SearchPopupItemData> GetSearchItems()
        {
            var allComponents = GetAllActionsFromAssembly();
            var list = new List<OM_SearchPopupItemData>();

            foreach (var component in allComponents)
            {
                var createAttr = component.GetCustomAttribute<AnimoraCreateAttribute>();
                if (createAttr == null) continue;

                var iconAttr = component.GetCustomAttribute<AnimoraIconAttribute>();
                var iconPath = iconAttr?.IconName;
                var iconType = iconAttr?.IconType ?? OM_IconType.ResourcesFolder;
                var description = component.GetCustomAttribute<AnimoraDescriptionAttribute>()?.Description;
                var keywords = component.GetCustomAttribute<AnimoraKeywordsAttribute>()?.Keywords;

                var actionAttr = component.GetCustomAttribute<AnimoraActionAttribute>();
                if (actionAttr != null && !actionAttr.IsTargetType(Clip.GetTargetType()))
                    continue;

                list.Add(new OM_SearchPopupItemData(
                    createAttr.Name,
                    createAttr.Path,
                    component,
                    iconType,
                    iconPath,
                    description,
                    keywords));
            }

            return list;
        }

        /// <summary>
        /// Instantiates and adds a selected component from the search popup.
        /// </summary>
        public void OnSearchItemSelected(OM_SearchPopupItemData data)
        {
            if (data.Data is Type actionType)
            {
                var instance = (AnimoraAction)Activator.CreateInstance(actionType);
                instance.ActionGroup = _currentActionGroup;
                instance.ActionName = data.Name;
                ActionsManager.AddComponent(instance, Player);
            }
        }

        /// <summary>
        /// Gets all valid <see cref="AnimoraAction"/> types in the current AppDomain.
        /// </summary>
        private static List<Type> GetAllActionsFromAssembly()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => typeof(AnimoraAction).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                .ToList();
        }
    }
}
