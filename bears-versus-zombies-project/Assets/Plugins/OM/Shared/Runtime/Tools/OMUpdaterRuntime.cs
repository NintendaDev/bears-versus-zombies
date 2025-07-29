using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OM
{
    /// <summary>
    /// Interface for classes that require periodic updates.
    /// </summary>
    public interface IOMUpdater
    {
        bool IsDontDestroyOnLoad() => false;
        bool IsUpdaterCompleted();
        void OnUpdate();
        void Stop();
    }
    
    /// <summary>
    /// Singleton class that manages and executes periodic updates for various components.
    /// </summary>
    public class OMUpdaterRuntime : MonoBehaviour
    {
        public const string GameObjectName = "HCCUpdaterRuntime";
        public static OMUpdaterRuntime Instance { get; private set; }

        private readonly List<IOMUpdater> _updaters = new List<IOMUpdater>();
        
        /// <summary>
        /// Initializes the singleton instance of the OMUpdaterRuntime.
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Subscribes to the scene loaded event to stop updaters that are not marked as "DontDestroyOnLoad".
        /// </summary>
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        /// <summary>
        /// Unsubscribes from the scene loaded event to prevent memory leaks.
        /// </summary>
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        /// <summary>
        /// Handles the scene loaded event and stops updaters that are not marked as "DontDestroyOnLoad".
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="loadSceneMode"></param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            foreach (var updater in _updaters.Where(x => !x.IsDontDestroyOnLoad()))
            {
                updater.Stop();
            }
        }
        
        /// <summary>
        /// Executes the update method for each registered updater.
        /// </summary>
        private void Update()
        {
            for (var i = _updaters.Count - 1; i >= 0; i--)
            {
                var updater = _updaters[i];
                updater.OnUpdate();
                if (updater.IsUpdaterCompleted())
                {
                    _updaters.RemoveAt(i);
                }
            }
        }
        
        /// <summary>
        /// Adds an updater to the list of updaters.
        /// </summary>
        /// <param name="updater"></param>
        public static void AddUpdater(IOMUpdater updater)
        {
            if (Instance == null)
            {
                Instance = new GameObject(GameObjectName).AddComponent<OMUpdaterRuntime>();
            }
            Instance._updaters.Add(updater);
        }

        /// <summary>
        /// Removes an updater from the list of updaters.
        /// </summary>
        /// <param name="updater"></param>
        public static void RemoveUpdater(IOMUpdater updater)
        {
            if(Instance == null) return;
            updater.Stop();
        }
    }
}