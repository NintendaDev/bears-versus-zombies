using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace OM.Editor
{
    /// <summary>
    /// A static utility class for drawing serialized properties and editor elements in Unity,
    /// as well as reflection and script handling tools for the Editor.
    /// </summary>
    public static class OMEditorUtility
    {
        /// <summary>
        /// Draws all visible properties of a SerializedProperty using IMGUI.
        /// </summary>
        public static void DrawAllProperties(this SerializedProperty property, bool enterChildren = true)
        {
            EditorGUI.indentLevel++;

            var currentProperty = property.Copy();
            var startDepth = currentProperty.depth;

            while (currentProperty.NextVisible(enterChildren) && currentProperty.depth > startDepth)
            {
                enterChildren = false;
                if (currentProperty.name == "m_Script")
                    continue;

                EditorGUILayout.PropertyField(currentProperty, true);
            }

            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Draws all visible properties in a given Rect (manual layout).
        /// </summary>
        public static void DrawAllProperties(this SerializedProperty property, Rect position, bool enterChildren = true)
        {
            if (property == null)
            {
                EditorGUI.HelpBox(position, "SerializedProperty is null", MessageType.Error);
                return;
            }

            EditorGUI.indentLevel++;

            var currentProperty = property.Copy();
            var startDepth = currentProperty.depth;

            while (currentProperty.NextVisible(enterChildren) && currentProperty.depth > startDepth)
            {
                enterChildren = false;
                if (currentProperty.name == "m_Script")
                    continue;

                EditorGUI.PropertyField(position, currentProperty, true);
                position.y += EditorGUI.GetPropertyHeight(currentProperty, true);
            }

            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Calculates the total height required to draw all visible child properties.
        /// </summary>
        public static float GetHeightOfAllProperties(this SerializedProperty property, bool enterChildren = true)
        {
            if (property == null)
                return 20;

            float height = 20;
            var currentProperty = property.Copy();
            var startDepth = currentProperty.depth;

            while (currentProperty.NextVisible(enterChildren) && currentProperty.depth > startDepth)
            {
                enterChildren = false;
                if (currentProperty.name == "m_Script")
                    continue;

                height += EditorGUI.GetPropertyHeight(currentProperty, true);
            }

            return height;
        }

        /// <summary>
        /// Draws all properties of a SerializedObject inside a VisualElement using IMGUI.
        /// </summary>
        public static void DrawAllFieldsInSerializedObject(VisualElement root, SerializedObject serializedObject, Action onChanged = null)
        {
            var imguiContainer = new IMGUIContainer(() =>
            {
                if (serializedObject.targetObject == null) return;

                serializedObject.Update();
                serializedObject.GetIterator().DrawAllProperties();
                if (GUI.changed)
                    onChanged?.Invoke();

                serializedObject.ApplyModifiedProperties();
            });

            imguiContainer.style.marginTop = 5;
            imguiContainer.style.marginBottom = 5;
            root.Add(imguiContainer);
        }

        /// <summary>
        /// Draws all fields of a SerializedObject using IMGUI inside the provided VisualElement.
        /// </summary>
        public static void DrawAllFields(this SerializedObject serializedObject, VisualElement root, Action onChanged = null)
        {
            var imguiContainer = new IMGUIContainer(() =>
            {
                if (serializedObject.targetObject == null) return;

                serializedObject.Update();
                serializedObject.GetIterator().DrawAllProperties();
                if (GUI.changed)
                    onChanged?.Invoke();

                serializedObject.ApplyModifiedProperties();
            });

            imguiContainer.style.marginTop = 5;
            imguiContainer.style.marginBottom = 5;
            imguiContainer.style.marginLeft = -5;
            root.Add(imguiContainer);
        }

        /// <summary>
        /// Draws elements in a grid layout using IMGUI inside a VisualElement.
        /// </summary>
        public static void DrawInGrid(VisualElement root, float windowWidth, int count, Action<int> itemCallback)
        {
            root.Add(new IMGUIContainer(() =>
            {
                const float buttonWidth = 60;
                var buttonsCountInRow = (int)(windowWidth / buttonWidth);

                GUILayout.BeginHorizontal();

                for (var i = 0; i < count; i++)
                {
                    if (i % buttonsCountInRow == 0)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }

                    itemCallback?.Invoke(i);
                }

                GUILayout.EndHorizontal();
            }));
        }

        /// <summary>
        /// Draws all MonoBehaviour objects of type T in the current scene in a grid layout.
        /// </summary>
        public static void DrawAllObjectsInSceneInGrid<T>(this EditorWindow editorWindow, ref IMGUIContainer container, float buttonWidth, Action<T> itemCreate) where T : MonoBehaviour
        {
            container.onGUIHandler += () =>
            {
                var buttonsCountInRow = (int)(editorWindow.position.width / buttonWidth);
                var objects = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);

                GUILayout.BeginHorizontal();

                for (var i = 0; i < objects.Length; i++)
                {
                    if (i % buttonsCountInRow == 0)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }

                    itemCreate?.Invoke(objects[i]);
                }

                GUILayout.EndHorizontal();
            };
        }

        /// <summary>
        /// Draws objects returned by getObjects in a grid layout using IMGUI.
        /// </summary>
        public static void DrawObjectsInGrid<T>(this EditorWindow editorWindow, ref IMGUIContainer container, float buttonWidth, Func<T[]> getObjects, Action<T> itemCreate)
        {
            container.onGUIHandler += () =>
            {
                var buttonsCountInRow = (int)(editorWindow.position.width / buttonWidth);
                var objects = getObjects.Invoke();
                if (objects == null) return;

                GUILayout.BeginHorizontal();

                for (var i = 0; i < objects.Length; i++)
                {
                    if (i % buttonsCountInRow == 0)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }

                    itemCreate?.Invoke(objects[i]);
                }

                GUILayout.EndHorizontal();
            };
        }

        /// <summary>
        /// Finds a MonoScript asset by its type name and opens it in the default script editor.
        /// </summary>
        public static void FindScriptInAssetsAndOpen(object target)
        {
            if (target == null) return;

            var script = target.GetType().Name;
            var guids = AssetDatabase.FindAssets(script);
            if (guids.Length == 0)
            {
                Debug.LogError("Script not found in assets");
                return;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            AssetDatabase.OpenAsset(asset);
        }

        /// <summary>
        /// Iterates through all values of an enum type.
        /// </summary>
        public static IEnumerable<T> LoopThroughEnum<T>() where T : Enum
        {
            var list = new List<T>();
            foreach (T value in Enum.GetValues(typeof(T)))
                list.Add(value);

            return list;
        }

        /// <summary>
        /// Searches for a script asset by name and returns it if found.
        /// </summary>
        public static bool GetScriptByName(string scriptName, out MonoScript resultScript)
        {
            var guids = AssetDatabase.FindAssets(scriptName + " t:Script");
            if (guids.Length == 0)
            {
                Debug.LogWarning("Script not found!");
                resultScript = null;
                return false;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            resultScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            return resultScript != null;
        }

        /// <summary>
        /// Opens a C# script file by its class name using Unity's asset database.
        /// </summary>
        public static void OpenScriptInEditorByName(string scriptName)
        {
            if (GetScriptByName(scriptName, out var script))
            {
                AssetDatabase.OpenAsset(script);
            }
            else
            {
                Debug.LogError($"Script {scriptName} not found");
            }
        }

        /// <summary>
        /// Opens a MonoScript asset in the default script editor.
        /// </summary>
        public static void OpenScriptInEditor(MonoScript script)
        {
            AssetDatabase.OpenAsset(script);
        }
    }
}
