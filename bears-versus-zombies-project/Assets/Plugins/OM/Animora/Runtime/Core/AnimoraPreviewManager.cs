
using System;
using System.Collections.Generic;

namespace OM.Animora.Runtime
{
    /// <summary>
    /// A static utility class responsible for managing state changes during editor preview scrubbing.
    /// It allows recording the initial state of properties when preview begins and restoring
    /// that state when preview ends, effectively providing an undo mechanism for changes
    /// made solely during the preview scrubbing process. This prevents preview modifications
    /// from permanently altering scene object values.
    /// </summary>
    public static class AnimoraPreviewManager
    {
        /// <summary>
        /// Internal interface defining a common contract for objects that store undo actions.
        /// Allows storing different types of undo operations in a single collection.
        /// </summary>
        private interface IRecordData
        {
            /// <summary>
            /// Executes the action required to undo the recorded change (e.g., restore the original value).
            /// </summary>
            void UndoAction();
        }

        /// <summary>
        /// Internal generic class that implements <see cref="IRecordData"/>.
        /// It specifically stores the original value of a property (<typeparamref name="T"/>)
        /// and an Action delegate (<see cref="Action{T}"/>) that can be used to set the property back to that original value.
        /// </summary>
        /// <typeparam name="T">The type of the value being recorded (must be a struct).</typeparam>
        private class RecordData<T> : IRecordData where T : struct // Constraint: T must be a value type
        {
            /// <summary>
            /// Stores the original value captured when recording started. Readonly after construction.
            /// </summary>
            private readonly T _value;
            /// <summary>
            /// Stores the delegate (method) used to set the property back to its original value. Readonly after construction.
            /// </summary>
            private readonly Action<T> _setter;

            /// <summary>
            /// Initializes a new instance of the <see cref="RecordData{T}"/> class.
            /// </summary>
            /// <param name="startValue">The original value of the property to record.</param>
            /// <param name="setter">The Action delegate that allows setting the property's value.</param>
            public RecordData(T startValue, Action<T> setter)
            {
                _setter = setter; // Store the setter delegate
                _value = startValue; // Store the original value
            }

            /// <summary>
            /// Executes the stored setter delegate, passing the originally recorded value to restore the state.
            /// Implements the <see cref="IRecordData.UndoAction"/> method.
            /// </summary>
            public void UndoAction()
            {
                // Call the setter Action, passing the stored original value (_value) back to it.
                _setter(_value);
            }
        }

        /// <summary>
        /// Static dictionary storing the list of recorded undo actions (<see cref="IRecordData"/>)
        /// keyed by the target object instance whose properties were modified during preview.
        /// This allows tracking undo actions separately for multiple objects affected by the preview.
        /// Initialized with a new empty dictionary.
        /// </summary>
        private static Dictionary<object, List<IRecordData>> UndoRedoActionsList { get; set; } = new Dictionary<object, List<IRecordData>>();

        /// <summary>
        /// Records the initial state of a property when preview starts (`isPreviewOn` is true),
        /// or applies the recorded undo actions to restore the initial state when preview stops (`isPreviewOn` is false).
        /// </summary>
        /// <typeparam name="T">The struct type of the property value being managed.</typeparam>
        /// <param name="isPreviewOn">True if preview mode is starting (record state), false if preview mode is stopping (undo changes).</param>
        /// <param name="target">The object instance whose property is being managed (used as the key in the dictionary).</param>
        /// <param name="startValue">The current value of the property when preview starts (used for recording).</param>
        /// <param name="setter">An Action delegate that can set the property's value (used for both recording the action and undoing).</param>
        public static void RecordOrUndoObject<T>(bool isPreviewOn, object target, T startValue, Action<T> setter)
            where T : struct // Constraint: T must be a value type
        {
            // Ensure the target object is not null.
            if (target == null) return;

            // --- Record Logic (When Preview Starts) ---
            if (isPreviewOn)
            {
                // Check if the dictionary already contains an entry for this target object.
                if (UndoRedoActionsList.ContainsKey(target))
                {
                    // If an entry exists, add a new RecordData instance for this specific property
                    // to the existing list of undo actions for this target.
                    UndoRedoActionsList[target].Add(new RecordData<T>(startValue, setter));
                }
                else
                {
                    // If no entry exists for this target, create a new list containing the new RecordData
                    // instance and add it to the dictionary with the target object as the key.
                    UndoRedoActionsList.Add(target, new List<IRecordData> { new RecordData<T>(startValue, setter) });
                }
            }
            // --- Undo Logic (When Preview Stops) ---
            else
            {
                // Try to retrieve the list of recorded undo actions for the specified target object.
                if (!UndoRedoActionsList.TryGetValue(target, out var recordDataList))
                {
                    // If no actions were recorded for this target (e.g., preview started and stopped without changes), simply return.
                    return;
                }

                // If undo actions were found, iterate through each recorded action in the list.
                foreach (var data in recordDataList)
                {
                    // Execute the UndoAction for each recorded data item.
                    // This calls the _setter delegate within the RecordData instance, restoring the original value.
                    data.UndoAction();
                }

                // After undoing actions for this target, remove its entry from the dictionary
                // to clear the state for the next preview session.
                // It's important to do this *after* iterating and undoing.
                UndoRedoActionsList.Remove(target);
            }
        }

        /// <summary>
        /// Clears all recorded undo actions for all target objects.
        /// Should be called when the editor state is reset or the timeline is closed
        /// to prevent stale undo data.
        /// </summary>
        public static void Clear()
        {
            // Clear the dictionary containing all undo actions.
            UndoRedoActionsList.Clear();
        }
    }
}