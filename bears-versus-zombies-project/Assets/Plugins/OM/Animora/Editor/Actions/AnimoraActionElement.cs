using System.Reflection;
using OM.Editor;
using OM.Animora.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Animora.Editor
{
    /// <summary>
    /// Represents a UI element for displaying and interacting with a single <see cref="AnimoraAction"/>.
    /// Supports drag-and-drop, renaming, context menu actions, and reorderable layout.
    /// </summary>
    public class AnimoraActionElement : VisualElement
    {
        public AnimoraPlayer Player { get; private set; }
        public AnimoraAction Action { get; }
        public AnimoraActionsManager ActionsManager { get; }
        public AnimoraActionManagerDrawer ActionManagerDrawer { get; }
        public SerializedProperty Property { get; }

        private readonly OM_FoldoutGroup _foldoutGroup;
        private readonly VisualElement _draggingArea;
        private Vector2 _startDragPosition;
        private AnimoraActionDropArea _currentDropArea;
        private readonly AnimoraClip _clip;
        private readonly TextField _actionNameTextField;

        /// <summary>
        /// Initializes the component element with all required data and UI setup.
        /// </summary>
        public AnimoraActionElement(
            AnimoraAction action,
            AnimoraActionsManager actionsManager,
            AnimoraActionManagerDrawer actionManagerDrawer,
            SerializedProperty property,
            AnimoraClip clip)
        {
            Player = property.serializedObject.targetObject as AnimoraPlayer;
            Action = action;
            ActionsManager = actionsManager;
            ActionManagerDrawer = actionManagerDrawer;
            Property = property.Copy();
            _clip = clip;

            property.serializedObject.Update();

            _draggingArea = new VisualElement().SetName("DraggingArea").AddTo(this).SetPickingMode(PickingMode.Ignore);
            Add(_draggingArea);

            _foldoutGroup = OM_FoldoutGroup.CreateSettingsGroup(Property, action.GetType().Name, "Action");
            _draggingArea.Add(_foldoutGroup);

            // Context menu
            var button = new Button { text = "More" };
            button.RegisterCallback<ClickEvent>(e =>
            {
                OnContextMenuClicked();
                e.StopImmediatePropagation();
            });
            _foldoutGroup.HeaderRightContainer.Add(button);

            // Toggle switch for enabling/disabling
            var isEnabledProperty = property.FindPropertyRelative("isEnabled");
            var enablePropertyField = OM_Switcher.CreateSwitcher(null, isEnabledProperty, newValue =>
            {
                _foldoutGroup.Content.SetEnabled(newValue);
            });
            _foldoutGroup.HeaderLeftContainer.Add(enablePropertyField);
            enablePropertyField.SendToBack();
            enablePropertyField.RegisterCallback<ClickEvent>(e => e.StopImmediatePropagation());

            // Action name field
            var nameProp = property.FindPropertyRelative("actionName");
            _foldoutGroup.Label.SetDisplay(false);

            _actionNameTextField = new TextField(nameProp.stringValue)
            {
                label = ""
            };
            _actionNameTextField.BindProperty(nameProp);
            _actionNameTextField.Bind(nameProp.serializedObject);
            _actionNameTextField.style.minWidth = new StyleLength(new Length(50, LengthUnit.Pixel));
            _actionNameTextField.schedule.Execute(() =>
            {
                var input = _actionNameTextField.Q("unity-text-input");
                input.style.flexGrow = 0;
                input.style.minWidth = new StyleLength(new Length(50, LengthUnit.Pixel));
            });

            _actionNameTextField.RegisterValueChangedCallback(e =>
            {
                ActionManagerDrawer.Player.RecordUndo("Record Action Name Change");
                action.ActionName = e.newValue;
                ActionManagerDrawer.Player.OnValidate();
            });
            _actionNameTextField.RegisterCallback<ClickEvent>(e => e.StopImmediatePropagation());
            _foldoutGroup.HeaderLeftContainer.Add(_actionNameTextField);

            // Drag handle
            var behaviourElementDragArea = new AnimoraActionElementDragArea(this);
            _foldoutGroup.HeaderLeftContainer.Insert(0, behaviourElementDragArea);

            // Full property content
            var field = new PropertyField(property);
            field.Bind(property.serializedObject);
            _foldoutGroup.AddToContent(field);

            Undo.undoRedoPerformed += OnUndoRedoPerformed;
            this.RegisterCallback<DetachFromPanelEvent>(_ => Undo.undoRedoPerformed -= OnUndoRedoPerformed);
        }

        /// <summary>
        /// Restores name value after undo/redo.
        /// </summary>
        private void OnUndoRedoPerformed()
        {
            _actionNameTextField.value = Action.ActionName;
        }

        /// <summary>
        /// Builds and displays the context menu for the component.
        /// </summary>
        private void OnContextMenuClicked()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Edit"), false, () =>
            {
                OMEditorUtility.OpenScriptInEditorByName(Action.GetType().Name);
            });
            menu.AddItem(new GUIContent("Reset"), false, () =>
            {
                Action.Reset(Player, _clip);
            });
            menu.AddItem(new GUIContent("Remove"), false, () =>
            {
                ActionsManager.RemoveComponent(Action, ActionManagerDrawer.Player);
            });

            // Add methods marked with [ContextMenu]
            var methodInfos = Action.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methodInfos)
            {
                if (method.GetCustomAttribute<ContextMenu>() != null)
                {
                    menu.AddItem(new GUIContent(method.Name), false, () => method.Invoke(Action, null));
                }
            }

            menu.ShowAsContext();
        }

        /// <summary>
        /// Begins the drag operation.
        /// </summary>
        public void StartDrag(Vector2 mousePosition)
        {
            var worldToLocal = ActionManagerDrawer.DraggingArea.WorldToLocal(_draggingArea.worldBound);
            _startDragPosition = new Vector2(
                _draggingArea.resolvedStyle.left + worldToLocal.x,
                _draggingArea.localBound.yMin + worldToLocal.y
            );

            style.minHeight = resolvedStyle.height;
            _draggingArea.style.width = resolvedStyle.width;
            _draggingArea.style.height = resolvedStyle.height;

            _draggingArea.RemoveFromHierarchy();
            ActionManagerDrawer.DraggingArea.Add(_draggingArea);

            _draggingArea.style.left = _startDragPosition.x;
            _draggingArea.style.top = _startDragPosition.y;
        }

        /// <summary>
        /// Updates the position of the element during dragging.
        /// </summary>
        public void Drag(Vector2 delta, Vector2 mousePosition)
        {
            var newPosition = _startDragPosition + delta;
            _draggingArea.style.left = newPosition.x;
            _draggingArea.style.top = newPosition.y;

            TryAddElementToDropArea(mousePosition);
        }

        /// <summary>
        /// Ends the drag and inserts the element in the drop area.
        /// </summary>
        public void EndDrag(Vector2 delta, Vector2 mousePosition)
        {
            TryAddElementToDropArea(mousePosition);

            _draggingArea.RemoveFromHierarchy();
            Add(_draggingArea);
            style.minHeight = StyleKeyword.Auto;
            _draggingArea.style.left = StyleKeyword.Auto;
            _draggingArea.style.top = StyleKeyword.Auto;
            _draggingArea.style.width = StyleKeyword.Auto;
            _draggingArea.style.height = StyleKeyword.Auto;
        }

        /// <summary>
        /// Checks if a drop area is valid and moves the component accordingly.
        /// </summary>
        private void TryAddElementToDropArea(Vector2 mousePosition)
        {
            if (!ActionManagerDrawer.TryGetDropArea(mousePosition, out var dropArea)) return;

            if (parent != dropArea)
            {
                Apply(-1);
                return;
            }

            int index = -1;
            foreach (var child in dropArea.Children())
            {
                if (child == this) continue;

                var localRect = ActionManagerDrawer.DraggingArea.WorldToLocal(child.worldBound);
                if (!localRect.Contains(mousePosition)) continue;

                index = mousePosition.y < localRect.center.y
                    ? dropArea.IndexOf(child)
                    : dropArea.IndexOf(child) + 1;

                break;
            }

            if (index <= -1 || dropArea.IndexOf(this) == index) return;
            Apply(index);

            void Apply(int indexToApply)
            {
                RemoveFromHierarchy();
                indexToApply = Mathf.Clamp(indexToApply, -1, dropArea.childCount);
                if (indexToApply == -1)
                    dropArea.Add(this);
                else
                    dropArea.Insert(indexToApply, this);

                ActionManagerDrawer.Player.RecordUndo("Record Action Change");

                Action.ActionGroup = dropArea.ActionGroup;
                Action.OrderIndex = dropArea.IndexOf(this);

                foreach (var child in dropArea.Children())
                {
                    if (child is AnimoraActionElement element)
                        element.Action.OrderIndex = dropArea.IndexOf(element);
                }

                if (_currentDropArea != null)
                {
                    foreach (var child in _currentDropArea.Children())
                    {
                        if (child is AnimoraActionElement element)
                            element.Action.OrderIndex = _currentDropArea.IndexOf(element);
                    }
                }

                _currentDropArea = dropArea;
            }
        }
    }
}
