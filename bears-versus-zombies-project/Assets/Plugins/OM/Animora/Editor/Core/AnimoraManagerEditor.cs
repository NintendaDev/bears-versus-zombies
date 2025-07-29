using System;
using OM.Animora.Runtime;
using UnityEditor;
using UnityEngine;

namespace OM.Animora.Editor
{
    [CustomEditor(typeof(AnimoraManager),true)]
    public class AnimoraManagerEditor : UnityEditor.Editor
    {
        private AnimoraManager _animoraManager;

        private void OnEnable()
        {
            _animoraManager = (AnimoraManager) target;
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            foreach (var pair in _animoraManager.AnimoraPlayers)
            {
                var playerName = pair.Key;
                var playerInstance = pair.Value;
                
                GUILayout.Label($"Player Name: {playerName}" + $" | Player Count: {playerInstance.Players.Count}");

                foreach (var player in playerInstance.Players)
                {
                    if (player == null) continue;

                    if (GUILayout.Button(player.name))
                    {
                        EditorGUIUtility.PingObject(player);
                    }
                }
            }
        }
    }
}