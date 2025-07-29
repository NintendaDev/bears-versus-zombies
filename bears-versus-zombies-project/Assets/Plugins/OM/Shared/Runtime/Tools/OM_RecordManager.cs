using System;
using System.Collections.Generic;

namespace OM
{
    /// <summary>
    /// Provides a lightweight undo system by recording and restoring values via delegates.
    /// </summary>
    public static class OM_RecordManager
    {
        /// <summary>
        /// Interface for record data entries that can perform undo operations.
        /// </summary>
        private interface IRecordData
        {
            /// <summary>
            /// Reverts the recorded value using the stored setter.
            /// </summary>
            void UndoAction();
        }

        /// <summary>
        /// Concrete implementation for storing a struct value and its setter.
        /// </summary>
        /// <typeparam name="T">The struct type being recorded.</typeparam>
        private class RecordData<T> : IRecordData where T : struct
        {
            private readonly T _value;
            private readonly Action<T> _setter;

            /// <summary>
            /// Constructs a new record entry with the initial value and a setter delegate.
            /// </summary>
            /// <param name="startValue">The value to restore on undo.</param>
            /// <param name="setter">The method to call with the recorded value.</param>
            public RecordData(T startValue, Action<T> setter)
            {
                _setter = setter;
                _value = startValue;
            }

            /// <summary>
            /// Applies the stored value using the setter delegate.
            /// </summary>
            public void UndoAction()
            {
                _setter(_value);
            }
        }

        /// <summary>
        /// Stores undo history per target object.
        /// </summary>
        private static Dictionary<object, List<IRecordData>> UndoRedoActionsList { get; set; } = new();

        /// <summary>
        /// Records the start value of a field or performs an undo depending on the preview state.
        /// </summary>
        /// <typeparam name="T">The value type being recorded (must be a struct).</typeparam>
        /// <param name="isPreviewOn">If true, records the value. If false, undoes all changes for the target.</param>
        /// <param name="target">The target object used as the key in the dictionary.</param>
        /// <param name="startValue">The value to record for potential undo.</param>
        /// <param name="setter">The delegate used to apply the recorded value.</param>
        public static void RecordOrUndoObject<T>(bool isPreviewOn, object target, T startValue, Action<T> setter)
            where T : struct
        {
            if (target == null) return;

            if (isPreviewOn)
            {
                if (UndoRedoActionsList.ContainsKey(target))
                {
                    UndoRedoActionsList[target].Add(new RecordData<T>(startValue, setter));
                }
                else
                {
                    UndoRedoActionsList.Add(target, new List<IRecordData> { new RecordData<T>(startValue, setter) });
                }
            }
            else
            {
                if (!UndoRedoActionsList.TryGetValue(target, out var recordDataList)) return;

                foreach (var data in recordDataList)
                {
                    data.UndoAction();
                }
            }
        }

        /// <summary>
        /// Clears all recorded undo entries for all objects.
        /// </summary>
        public static void Clear()
        {
            UndoRedoActionsList.Clear();
        }
    }
}
