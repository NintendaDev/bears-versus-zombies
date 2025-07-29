using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace OM.Editor
{
    /// <summary>
    /// A comprehensive utility class for extending Unity's SerializedProperty.
    /// Adds support for setting/getting values, copying, reflection, traversal, and attribute access.
    /// </summary>
    public static class OM_SerializedPropertyExtensions
    {
        private static readonly Regex PropertyPathRegex = new Regex(@"(?<=\.|^)(?:Array\.data\[\d+\]|[^.]+)");

        /// <summary>Resets the property's value to its default.</summary>
        public static void ResetPropertyValue(this SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = 0;
                    break;
                case SerializedPropertyType.Vector3:
                    property.vector3Value = Vector3.zero;
                    break;
                case SerializedPropertyType.Vector2:
                    property.vector2Value = Vector2.zero;
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = 0f;
                    break;
                case SerializedPropertyType.Boolean:
                    property.boolValue = false;
                    break;
                case SerializedPropertyType.String:
                    property.stringValue = string.Empty;
                    break;
                case SerializedPropertyType.Color:
                    property.colorValue = Color.white;
                    break;
                case SerializedPropertyType.ObjectReference:
                    property.objectReferenceValue = null;
                    break;
                case SerializedPropertyType.Enum:
                    property.enumValueIndex = 0;
                    break;
                // Add more cases as needed
            }

            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
        
        /// <summary>Sets the property's value using boxed object input.</summary>
        public static void SetValue(this SerializedProperty property, object value)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = (int)value;
                    break;
                case SerializedPropertyType.Boolean:
                    property.boolValue = (bool)value;
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = (float)value;
                    break;
                case SerializedPropertyType.String:
                    property.stringValue = (string)value;
                    break;
                case SerializedPropertyType.Color:
                    property.colorValue = (Color)value;
                    break;
                case SerializedPropertyType.ObjectReference:
                    property.objectReferenceValue = value as UnityEngine.Object;
                    break;
                case SerializedPropertyType.LayerMask:
                    property.intValue = (int)value;
                    break;
                case SerializedPropertyType.Enum:
                    property.enumValueIndex = (int)value;
                    break;
                case SerializedPropertyType.Vector2:
                    property.vector2Value = (Vector2)value;
                    break;
                case SerializedPropertyType.Vector3:
                    property.vector3Value = (Vector3)value;
                    break;
                case SerializedPropertyType.Vector4:
                    property.vector4Value = (Vector4)value;
                    break;
                case SerializedPropertyType.Rect:
                    property.rectValue = (Rect)value;
                    break;
                case SerializedPropertyType.ArraySize:
                    property.arraySize = (int)value;
                    break;
                case SerializedPropertyType.Character:
                    property.stringValue = (string)value;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    property.animationCurveValue = (AnimationCurve)value;
                    break;
                case SerializedPropertyType.Bounds:
                    property.boundsValue = (Bounds)value;
                    break;
                case SerializedPropertyType.Gradient:
                    property.gradientValue = (Gradient)value;
                    break;
                case SerializedPropertyType.Quaternion:
                    property.quaternionValue = (Quaternion)value;
                    break;
                case SerializedPropertyType.ExposedReference:
                    property.exposedReferenceValue = (UnityEngine.Object)value;
                    break;
                case SerializedPropertyType.Vector2Int:
                    property.vector2IntValue = (Vector2Int)value;
                    break;
                case SerializedPropertyType.Vector3Int:
                    property.vector3IntValue = (Vector3Int)value;
                    break;
                case SerializedPropertyType.RectInt:
                    property.rectIntValue = (RectInt)value;
                    break;
                case SerializedPropertyType.BoundsInt:
                    property.boundsIntValue = (BoundsInt)value;
                    break;
                default:
                    
                    Debug.LogWarning($"Unsupported property type: {property.propertyType}");
                    break;
            }
            
            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>Gets the current value of the property as a boxed object.</summary>
        public static object GetValue(this SerializedProperty property)
        {
            if (property?.serializedObject == null || property.serializedObject.targetObject == null)
            {
                return null;
            }

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return property.intValue;
                case SerializedPropertyType.Boolean:
                    return property.boolValue;
                case SerializedPropertyType.Float:
                    return property.floatValue;
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Color:
                    return property.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue;
                case SerializedPropertyType.LayerMask:
                    return property.intValue;
                case SerializedPropertyType.Enum:
                    return property.enumValueIndex;
                case SerializedPropertyType.Vector2:
                    return property.vector2Value;
                case SerializedPropertyType.Vector3:
                    return property.vector3Value;
                case SerializedPropertyType.Vector4:
                    return property.vector4Value;
                case SerializedPropertyType.Rect:
                    return property.rectValue;
                case SerializedPropertyType.ArraySize:
                    return property.arraySize;
                case SerializedPropertyType.Character:
                    return property.stringValue;
                case SerializedPropertyType.AnimationCurve:
                    return property.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return property.boundsValue;
                case SerializedPropertyType.Gradient:
                    return property.gradientValue;
                case SerializedPropertyType.Quaternion:
                    return property.quaternionValue;
                case SerializedPropertyType.ExposedReference:
                    return property.exposedReferenceValue;
                case SerializedPropertyType.Vector2Int:
                    return property.vector2IntValue;
                case SerializedPropertyType.Vector3Int:
                    return property.vector3IntValue;
                case SerializedPropertyType.RectInt:
                    return property.rectIntValue;
                case SerializedPropertyType.BoundsInt:
                    return property.boundsIntValue;
                default:
                    return property.GetValueCustom();
            }
        }
        
        /// <summary>Copies the value from one SerializedProperty to another.</summary>
        public static void CopyFromSerializedProperty(this SerializedProperty to, SerializedProperty from)
        {
            switch (to.propertyType)
            {
                case SerializedPropertyType.Integer:
                    to.intValue = from.intValue;
                    break;
                case SerializedPropertyType.Boolean:
                    to.boolValue = from.boolValue;
                    break;
                case SerializedPropertyType.Float:
                    to.floatValue = from.floatValue;
                    break;
                case SerializedPropertyType.String:
                    to.stringValue = from.stringValue;
                    break;
                case SerializedPropertyType.Color:
                    to.colorValue = from.colorValue;
                    break;
                case SerializedPropertyType.ObjectReference:
                    to.objectReferenceValue = from.objectReferenceValue;
                    break;
                case SerializedPropertyType.LayerMask:
                    to.intValue = from.intValue;
                    break;
                case SerializedPropertyType.Enum:
                    to.enumValueIndex = from.enumValueIndex;
                    break;
                case SerializedPropertyType.Vector2:
                    to.vector2Value = from.vector2Value;
                    break;
                case SerializedPropertyType.Vector3:
                    to.vector3Value = from.vector3Value;
                    break;
                case SerializedPropertyType.Vector4:
                    to.vector4Value = from.vector4Value;
                    break;
                case SerializedPropertyType.Rect:
                    to.rectValue = from.rectValue;
                    break;
                case SerializedPropertyType.ArraySize:
                    to.arraySize = from.arraySize;
                    break;
                case SerializedPropertyType.Character:
                    to.stringValue = from.stringValue;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    to.animationCurveValue = from.animationCurveValue;
                    break;
                case SerializedPropertyType.Bounds:
                    to.boundsValue = from.boundsValue;
                    break;
                case SerializedPropertyType.Gradient:
                    to.gradientValue = from.gradientValue;
                    break;
                case SerializedPropertyType.Quaternion:
                    to.quaternionValue = from.quaternionValue;
                    break;
                case SerializedPropertyType.ExposedReference:
                    to.exposedReferenceValue = from.exposedReferenceValue;
                    break;
                case SerializedPropertyType.Vector2Int:
                    to.vector2IntValue = from.vector2IntValue;
                    break;
                case SerializedPropertyType.Vector3Int:
                    to.vector3IntValue = from.vector3IntValue;
                    break;
                case SerializedPropertyType.RectInt:
                    to.rectIntValue = from.rectIntValue;
                    break;
                case SerializedPropertyType.BoundsInt:
                    to.boundsIntValue = from.boundsIntValue;
                    break;
                default:
                    Debug.LogWarning($"Unsupported property type: {to.propertyType}");
                    break;
            }
        }
        
        /// <summary>Enumerates all visible children of a property.</summary>
        public static IEnumerable<SerializedProperty> GetAllProperties(this SerializedProperty property,
            bool enterChildren = true)
        {
            if (property == null)
            {
                yield break;
            }

            var currentProperty = property.Copy();
            var startDepth = currentProperty.depth;

            while (currentProperty.NextVisible(enterChildren) && currentProperty.depth > startDepth)
            {
                enterChildren = false;
                yield return currentProperty;
            }
        }
        
        /// <summary>Gets properties by name from a SerializedObject.</summary>
        public static IEnumerable<SerializedProperty> GetPropertiesByName(this SerializedObject targetSerializedObject,params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                var serializedProperty = targetSerializedObject.FindProperty(propertyName);
                if (serializedProperty != null) yield return serializedProperty;
            }
        }
        
        /// <summary>Gets properties by name from a SerializedProperty.</summary>
        public static IEnumerable<SerializedProperty> GetPropertiesByName(this SerializedProperty targetProperty,params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                var serializedProperty = targetProperty.FindPropertyRelative(propertyName);
                if (serializedProperty != null) yield return serializedProperty;
            }
        }

        /// <summary>Enumerates all visible top-level properties of a SerializedObject.</summary>
        public static IEnumerable<SerializedProperty> GetAllProperties(this SerializedObject serializedObject,
            bool enterChildren = true)
        {
            if (serializedObject == null)
            {
                yield break;
            }

            var currentProperty = serializedObject.GetIterator();
            var startDepth = currentProperty.depth;

            while (currentProperty.NextVisible(enterChildren) && currentProperty.depth > startDepth)
            {
                enterChildren = false;
                yield return currentProperty;
            }
        }
        
        /// <summary>Splits a property path into its individual segments.</summary>
        public static string[] SplitPropertyPath(string propertyPath)
        {
            if (string.IsNullOrEmpty(propertyPath)) return new string[0];

            var matches = PropertyPathRegex.Matches(propertyPath);
            var parts = new List<string>();

            foreach (Match match in matches)
            {
                parts.Add(match.Value);
            }

            return parts.ToArray();
        }

        /// <summary>Gets the parent SerializedProperty of a nested property.</summary>
        public static SerializedProperty GetParentProperty(this SerializedProperty property)
        {
            if (property == null) return null;

            string[] parts = SplitPropertyPath(property.propertyPath);
            if (parts.Length <= 1) return null;

            SerializedProperty parent = property.serializedObject.FindProperty(parts[0]);
            for (var i = 1; i < parts.Length - 1; i++)
            {
                parent = parent.FindPropertyRelative(parts[i]);
            }

            return parent;
        }
        
        /// <summary>Gets the FieldInfo of the parent of the property.</summary>
        public static FieldInfo GetFieldInfoOfTheParent(this SerializedProperty property,bool checkAllParents)
        {
            if (property == null) return null;

            object obj = property.serializedObject.targetObject;
            if (property.propertyPath.Contains(".") == false)
            {
                var type = obj.GetType();
                FieldInfo field = null;
                if (checkAllParents)
                {
                    field = type.GetFieldInfoFromAllParents(property.name);
                }
                else
                {
                    field = type.GetField(property.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                }
                return field;
            }
            var parts = SplitPropertyPath(property.propertyPath);

            FieldInfo lastField = null;

            for (var index = 0; index < parts.Length - 1; index++)
            {
                var part = parts[index];
                Debug.Log(part);
                if (obj == null) return null;

                if (part.StartsWith("Array.data["))
                {
                    // Handle arrays and lists
                    obj = HandleArrayOrList(part, obj);
                }
                else
                {
                    // Handle regular fields
                    var type = obj.GetType();
                    
                    FieldInfo field = null;
                    if (checkAllParents)
                    {
                        field = type.GetFieldInfoFromAllParents(part);
                    }
                    else
                    {
                        field = type.GetField(part, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    }
                    if (field == null) return null;

                    lastField = field; // Update the last valid FieldInfo
                    obj = field.GetValue(obj); // Traverse deeper
                }
            }

            return lastField;
        }

        /// <summary>Gets the FieldInfo for a property based on a specific object.</summary>
        public static FieldInfo GetFieldInfo(this SerializedProperty property,object target,bool checkAllParents)
        {
            if (property == null) return null;

            var type1 = target.GetType();
            if (checkAllParents)
            {
                return type1.GetFieldInfoFromAllParents(property.name);
            }
            
            var fieldInfo = type1.GetField(property.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            return fieldInfo;
        }
        
        /// <summary>Gets FieldInfo from all parent types in hierarchy.</summary>
        public static FieldInfo GetFieldInfoFromAllParents(this Type type, string fieldName)
        {
            if (type == null || string.IsNullOrEmpty(fieldName)) return null;

            // Traverse up the class hierarchy
            while (type != null)
            {
                // Look for the field in the current type (including private, public, and protected fields)
                var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                if (field != null)
                {
                    return field; // Return the found field
                }

                // Move up to the parent class
                type = type.BaseType;
            }

            return null; // Return null if not found in the class hierarchy
        }
        
        /// <summary>Gets the FieldInfo of the final property segment using reflection.</summary>
        public static FieldInfo GetFieldInfo(this SerializedProperty property,bool checkAllParents)
        {
            if (property == null) return null;

            object obj = property.serializedObject.targetObject;
            string[] parts = SplitPropertyPath(property.propertyPath);

            FieldInfo lastField = null;

            foreach (var part in parts)
            {
                if (obj == null) return null;

                if (part.StartsWith("Array.data["))
                {
                    // Handle arrays and lists
                    obj = HandleArrayOrList(part, obj);
                }
                else
                {
                    // Handle regular fields
                    var type = obj.GetType();
                    FieldInfo field = null;
                    if (checkAllParents)
                    {
                        field = type.GetFieldInfoFromAllParents(part);
                    }
                    else
                    {
                        field = type.GetField(part, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    }
                    if (field == null) return null;

                    lastField = field; // Update the last valid FieldInfo
                    obj = field.GetValue(obj); // Traverse deeper
                }
            }
            return lastField;
        }

        /// <summary>Returns the parent value object of a serialized property.</summary>
        public static object GetParentValue(this SerializedProperty property)
        {
            if (property == null) return null;

            var parent = property.GetParentProperty();
            if (parent != null)
            {
                return parent != null ? parent.GetValue() : null;
            }

            return property.serializedObject.targetObject;
        }

        public static bool IsSerializedPropertyValid(this SerializedProperty property)
        {
            if (property == null) return false;
            if (property.serializedObject == null) return false;
            if (property.serializedObject.targetObject == null) return false;

            try
            {
                var _ = property.propertyPath;
            }
            catch
            {
                return false;
            }

            return true;
        }
        
        /// <summary>Uses custom reflection logic to retrieve the actual value behind a property path.</summary>
        public static object GetValueCustom(this SerializedProperty property,bool checkAllParents = true,bool debug = false)
        {
            if (property == null) return null;

            object obj = property.serializedObject.targetObject;
            string[] parts = SplitPropertyPath(property.propertyPath);

            if (debug)
            {
                Debug.Log(parts.Length);
                var stringBuilder = new StringBuilder();
                foreach (var part in parts)
                {
                    stringBuilder.Append(part);
                    stringBuilder.Append(" - ");
                }
                Debug.Log(stringBuilder.ToString());
            }

            foreach (var part in parts)
            {
                if(debug) Debug.Log(part);
                if (obj == null)
                {
                    if(debug) Debug.Log("Object is null");
                    return null;
                }

                if (part.Contains("Array"))
                {
                    // Handle arrays and lists
                    obj = HandleArrayOrList(part, obj);
                }
                else
                {
                    // Handle regular fields
                    var type = obj.GetType();
                    FieldInfo field = null;
                    if (checkAllParents)
                    {
                        field = type.GetFieldInfoFromAllParents(part);
                    }
                    else
                    {
                        field = type.GetField(part, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    }
                    //var field = type.GetField(part, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field == null)
                    {
                        if(debug) Debug.Log("Field is null Part: " + part + " Type: " + type + " Object: " + obj);
                        return null;
                    }

                    obj = field.GetValue(obj);
                }
            }
            
            if(debug) Debug.Log("Returning: " + obj);
            return obj;
        }

        /// <summary>Handles list/array element access for reflection.</summary>
        private static object HandleArrayOrList(string part, object obj)
        {
            if (!part.StartsWith("Array.data["))
            {
                return obj;
            }

            // Extract the index from the "Array.data[index]" format
            int startIdx = part.IndexOf('[') + 1;
            int endIdx = part.IndexOf(']');
            if (startIdx < 0 || endIdx < 0) return null;

            if (int.TryParse(part.Substring(startIdx, endIdx - startIdx), out int index))
            {
                if (obj is IList list && index >= 0 && index < list.Count)
                {
                    return list[index];
                }
            }
            return null;
        }
        
        /// <summary>Gets a single attribute of the specified type from the property’s field.</summary>
        public static T GetAttribute<T>(this SerializedProperty property,bool checkAllParents) where T : Attribute
        {
            var fieldInfo = GetFieldInfo(property,checkAllParents);
            return fieldInfo != null ? fieldInfo.GetCustomAttribute<T>() : null;
        }

        /// <summary>Gets all attributes applied to the property's field.</summary>
        public static Attribute[] GetAttributes(this SerializedProperty property,bool checkAllParents)
        {
            var fieldInfo = GetFieldInfo(property,checkAllParents);
            if (fieldInfo != null)
            {
                return (Attribute[])fieldInfo.GetCustomAttributes(false);
            }
            return Array.Empty<Attribute>();
        }
        
        /// <summary>Finds a nested property by string path, supporting array access.</summary>
        public static SerializedProperty GetSerializedPropertyByPath(this SerializedObject target, string path)
        {
            if (target == null || string.IsNullOrEmpty(path)) 
                return null;

            string[] parts = path.Split('.');
            SerializedProperty property = target.FindProperty(parts[0]);

            for (int i = 1; i < parts.Length; i++)
            {
                if (property == null)
                    return null;

                // Handle array indexing
                if (parts[i].StartsWith("Array.data["))
                {
                    // Extract the index from "Array.data[index]"
                    int startIndex = parts[i].IndexOf('[') + 1;
                    int endIndex = parts[i].IndexOf(']');
                    if (startIndex >= 0 && endIndex > startIndex)
                    {
                        string indexString = parts[i].Substring(startIndex, endIndex - startIndex);
                        if (int.TryParse(indexString, out int index))
                        {
                            property = property.GetArrayElementAtIndex(index);
                        }
                        else
                        {
                            Debug.LogError($"Invalid array index in path: {parts[i]}");
                            return null;
                        }
                    }
                    else
                    {
                        Debug.LogError($"Malformed array path segment: {parts[i]}");
                        return null;
                    }
                }
                else
                {
                    // Handle regular property access
                    property = property.FindPropertyRelative(parts[i]);
                }
            }

            return property;
        }
        
    }
}