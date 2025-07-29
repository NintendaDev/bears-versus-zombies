using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace OM
{
    /// <summary>
    /// Extension methods for converting Unity coroutines to async tasks and integrating async/await with MonoBehaviours.
    /// </summary>
    public static class OM_CoroutineExtensions
    {
        /// <summary>
        /// Converts a coroutine to a Task that completes when the coroutine finishes.
        /// </summary>
        /// <param name="coroutine">The IEnumerator coroutine to run.</param>
        /// <param name="runner">MonoBehaviour to run the coroutine on.</param>
        /// <returns>A Task that completes when the coroutine is done.</returns>
        public static Task MakeAsync(this IEnumerator coroutine, MonoBehaviour runner)
        {
            var tcs = new TaskCompletionSource<bool>();
            runner.StartCoroutine(WrapCoroutine(coroutine, tcs));
            return tcs.Task;
        }

        /// <summary>
        /// Converts a coroutine to a Task and also returns the Coroutine instance.
        /// </summary>
        /// <param name="coroutine">The IEnumerator coroutine to run.</param>
        /// <param name="runner">MonoBehaviour to run the coroutine on.</param>
        /// <param name="coroutineResult">The Coroutine instance that is started.</param>
        /// <returns>A Task that completes when the coroutine is done.</returns>
        public static Task MakeAsync(this IEnumerator coroutine, MonoBehaviour runner, out Coroutine coroutineResult)
        {
            var tcs = new TaskCompletionSource<bool>();
            coroutineResult = runner.StartCoroutine(WrapCoroutine(coroutine, tcs));
            return tcs.Task;
        }

        /// <summary>
        /// Wraps a coroutine and completes a Task when finished.
        /// </summary>
        private static IEnumerator WrapCoroutine(IEnumerator coroutine, TaskCompletionSource<bool> tcs)
        {
            yield return coroutine;
            tcs.SetResult(true);
        }

        /// <summary>
        /// Runs a coroutine and returns a Task that completes when the coroutine finishes.
        /// </summary>
        /// <param name="runner">MonoBehaviour to run the coroutine on.</param>
        /// <param name="coroutine">The IEnumerator coroutine to run.</param>
        /// <returns>A Task that completes when the coroutine is done.</returns>
        public static Task RunCoroutineAsTask(MonoBehaviour runner, IEnumerator coroutine)
        {
            var tcs = new TaskCompletionSource<bool>();
            runner.StartCoroutine(WrapCoroutine(coroutine, tcs));
            return tcs.Task;
        }

        /// <summary>
        /// Runs an async method inside a coroutine.
        /// </summary>
        /// <param name="asyncFunc">The async function to run.</param>
        /// <returns>An IEnumerator that can be yielded in a coroutine.</returns>
        public static IEnumerator RunAsync(Func<Task> asyncFunc)
        {
            var task = asyncFunc();
            while (!task.IsCompleted)
                yield return null;

            if (task.IsFaulted)
                Debug.LogException(task.Exception);
        }

        /// <summary>
        /// Awaits a Unity-style delay using Time.time in an async method.
        /// </summary>
        /// <param name="seconds">Time to wait in seconds.</param>
        public static async Task WaitForSecondsAsync(float seconds)
        {
            float endTime = Time.time + seconds;
            while (Time.time < endTime)
                await Task.Yield();
        }

        /// <summary>
        /// Starts an async method from a MonoBehaviour without waiting for it to complete.
        /// </summary>
        /// <param name="runner">The MonoBehaviour to start from.</param>
        /// <param name="asyncFunc">The async method to run.</param>
        public static void FireAndForget(MonoBehaviour runner, Func<Task> asyncFunc)
        {
            runner.StartCoroutine(RunAsync(asyncFunc));
        }
    }
}
