using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OM
{
    /// <summary>
    /// Central manager for handling object copy-paste functionality using JSON serialization.
    /// </summary>
    public static class OM_CopyPasteManager
    {
        private static readonly Dictionary<Type, OM_CopyPasteInstance> CopyPasteInstances = new();

        /// <summary>
        /// Gets the copy-paste instance for a specific type.
        /// </summary>
        public static OM_CopyPasteInstance GetCopyPasteInstance<T>()
        {
            if (CopyPasteInstances.TryGetValue(typeof(T), out var instance))
                return instance;

            var newInstance = new OM_CopyPasteInstance();
            CopyPasteInstances.Add(typeof(T), newInstance);
            return newInstance;
        }

        /// <summary>
        /// Creates a deep duplicate of the object using JSON serialization.
        /// </summary>
        public static T Duplicate<T>(T toConvert)
        {
            var asJson = new OM_AsJson
            {
                JsonText = JsonUtility.ToJson(toConvert),
                Type = toConvert.GetType()
            };

            return (T)JsonUtility.FromJson(asJson.JsonText, asJson.Type);
        }

        /// <summary>
        /// Copies a single object to the internal clipboard.
        /// </summary>
        public static void Copy<T>(T toCopyObject)
        {
            GetCopyPasteInstance<T>().Copy(toCopyObject);
        }

        /// <summary>
        /// Copies a list of objects to the internal clipboard.
        /// </summary>
        public static void CopyList<T>(List<T> toCopyObjects)
        {
            GetCopyPasteInstance<T>().CopyList(toCopyObjects);
        }

        /// <summary>
        /// Pastes all copied objects as a list of the given type.
        /// </summary>
        public static List<T> Paste<T>()
        {
            return GetCopyPasteInstance<T>().Paste<T>();
        }

        /// <summary>
        /// Checks if there is a valid paste buffer for a given type.
        /// </summary>
        public static bool CanPaste<T>()
        {
            return GetCopyPasteInstance<T>().CanPaste();
        }

        /// <summary>
        /// Gets whether the current buffer contains one or multiple objects.
        /// </summary>
        public static OM_CopyType GetCopyType<T>()
        {
            return GetCopyPasteInstance<T>().GetCopyType();
        }
    }

    /// <summary>
    /// Struct representing a serialized object along with its type.
    /// </summary>
    public struct OM_AsJson
    {
        public Type Type;
        public string JsonText;

        public override string ToString()
        {
            return $"Type: {Type} Json: {JsonText}";
        }
    }

    /// <summary>
    /// Enum indicating if a copy action involved a single or multiple objects.
    /// </summary>
    public enum OM_CopyType
    {
        Single,
        Multiple,
    }

    /// <summary>
    /// Represents a single copy-paste instance for a specific type.
    /// </summary>
    public class OM_CopyPasteInstance
    {
        public event Action<bool, OM_CopyType> OnCopyPasteCallback;

        private List<OM_AsJson> _copiedObjects = new();

        /// <summary>
        /// Copies a single object to the buffer.
        /// </summary>
        public void Copy<T>(T toCopyObject)
        {
            if (toCopyObject == null) return;

            _copiedObjects ??= new List<OM_AsJson>();
            _copiedObjects.Clear();
            _copiedObjects.Add(GetAsJson(toCopyObject));

            OnCopyPasteCallback?.Invoke(true, OM_CopyType.Single);
        }

        /// <summary>
        /// Copies a list of objects to the buffer.
        /// </summary>
        public void CopyList<T>(List<T> toCopyObjects)
        {
            if (toCopyObjects == null || toCopyObjects.Count == 0) return;

            _copiedObjects ??= new List<OM_AsJson>();
            _copiedObjects.Clear();

            foreach (var copy in toCopyObjects)
            {
                _copiedObjects.Add(GetAsJson(copy));
            }

            OnCopyPasteCallback?.Invoke(true, OM_CopyType.Multiple);
        }

        /// <summary>
        /// Pastes all copied objects as a list.
        /// </summary>
        public List<T> Paste<T>()
        {
            if (_copiedObjects == null) return null;

            var toPasteList = _copiedObjects.Select(GetObjectFromJson<T>).ToList();
            _copiedObjects = null;

            OnCopyPasteCallback?.Invoke(false, toPasteList.Count > 1 ? OM_CopyType.Multiple : OM_CopyType.Single);
            return toPasteList;
        }

        /// <summary>
        /// Returns the number of copied objects currently stored.
        /// </summary>
        public int GetCopyCount() => _copiedObjects?.Count ?? 0;

        /// <summary>
        /// Checks if pasting is possible.
        /// </summary>
        public bool CanPaste() => _copiedObjects is { Count: > 0 };

        /// <summary>
        /// Returns the copy type: single or multiple.
        /// </summary>
        public OM_CopyType GetCopyType() =>
            _copiedObjects == null || _copiedObjects.Count <= 1 ? OM_CopyType.Single : OM_CopyType.Multiple;

        private static OM_AsJson GetAsJson(object o)
        {
            return new OM_AsJson
            {
                Type = o.GetType(),
                JsonText = JsonUtility.ToJson(o)
            };
        }

        private static T GetObjectFromJson<T>(OM_AsJson asJson)
        {
            try
            {
                return (T)JsonUtility.FromJson(asJson.JsonText, asJson.Type);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
    }
}
