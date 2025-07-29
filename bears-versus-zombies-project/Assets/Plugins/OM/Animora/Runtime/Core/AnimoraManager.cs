using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OM.Animora.Runtime
{
    public class AnimoraManager : MonoBehaviour
    {
        public struct AnimoraPlayerInstance
        {
            public string PlayerName;
            public List<AnimoraPlayer> Players;
            
            public AnimoraPlayerInstance(string playerName)
            {
                PlayerName = playerName;
                Players = new List<AnimoraPlayer>();
            }
        }

        #region Singleton

        public static AnimoraManager Instance { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void SpawnOnStart()
        {
            var obj = new GameObject("AnimoraManager");
            Instance = obj.AddComponent<AnimoraManager>();
            DontDestroyOnLoad(obj);
            Instance.Init();
        }

        #endregion

        public Dictionary<string, AnimoraPlayerInstance> AnimoraPlayers { get; } = new Dictionary<string, AnimoraPlayerInstance>();

        private void Init()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnSceneUnloaded(Scene arg0)
        {
            AnimoraPlayers.Clear();
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            
        }

        public static void RegisterAnimoraPlayer(AnimoraPlayer player)
        {
            if(Instance == null) return;
            
            if (player == null) return;
            var playerName = player.PlayerUniqueID;
            if (Instance.AnimoraPlayers.ContainsKey(playerName))
            {
                Instance.AnimoraPlayers[playerName].Players.Add(player);
            }
            else
            {
                var instance = new AnimoraPlayerInstance(playerName);
                instance.Players.Add(player);
                Instance.AnimoraPlayers.Add(playerName, instance);
            }
        }
        
        public static void UnregisterAnimoraPlayer(AnimoraPlayer player)
        {
            if(Instance == null) return;

            if (player == null) return;
            var playerName = player.PlayerUniqueID;
            if (Instance.AnimoraPlayers.ContainsKey(playerName))
            {
                Instance.AnimoraPlayers[playerName].Players.Remove(player);
                if (Instance.AnimoraPlayers[playerName].Players.Count == 0)
                {
                    Instance.AnimoraPlayers.Remove(playerName);
                }
            }
        }
        
        public static AnimoraPlayer GetFirstPlayerByName(string playerName)
        {
            if(Instance == null) return null;
            return Instance.AnimoraPlayers.TryGetValue(playerName, out var player) ? player.Players.FirstOrDefault() : null;
        }

        public static AnimoraPlayer PlayFirstPlayerByName(string playerName)
        {
            if(Instance == null) return null;
            if (!Instance.AnimoraPlayers.TryGetValue(playerName, out var player)) return null;
            
            var firstPlayer = player.Players.FirstOrDefault();
            firstPlayer?.PlayAnimation();
            return firstPlayer;
        }
        
        public static List<AnimoraPlayer> GetAllPlayersByName(string playerName)
        {
            if(Instance == null) return new List<AnimoraPlayer>();
            return Instance.AnimoraPlayers.TryGetValue(playerName, out var player) ? player.Players : new List<AnimoraPlayer>();
        }

        public static List<AnimoraPlayer> PlayAllPlayersByName(string playerName)
        {
            if(Instance == null) return new List<AnimoraPlayer>();
            if (!Instance.AnimoraPlayers.TryGetValue(playerName, out var player)) return new List<AnimoraPlayer>();
            
            foreach (var animoraPlayer in player.Players)
            {
                animoraPlayer?.PlayAnimation();
            }
            return player.Players;
        }

    }
}