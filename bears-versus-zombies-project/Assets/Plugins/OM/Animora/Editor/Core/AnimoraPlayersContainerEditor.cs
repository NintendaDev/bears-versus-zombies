using OM.Animora.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Animora.Editor
{
    [CustomEditor(typeof(AnimoraPlayersContainer),true)]
    public class AnimoraPlayersContainerEditor : UnityEditor.Editor
    {
        private AnimoraPlayersContainer _animoraPlayersContainer;
        
        private void OnEnable()
        {
            _animoraPlayersContainer = (AnimoraPlayersContainer)target;
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var listOfPlayers = _animoraPlayersContainer.GetSameIDPlayers();
            if (listOfPlayers.Count > 0)
            {
                EditorGUILayout.HelpBox(
                    "There are players with the same name in the container. Please make sure all players have unique names.", 
                    MessageType.Warning);

                EditorGUILayout.BeginVertical();
                
                foreach (var player in listOfPlayers)
                {
                    if (GUILayout.Button(player.name))
                    {
                        EditorGUIUtility.PingObject(player);
                    }
                    var o = new SerializedObject(player);
                    o.Update();
                    var prop = o.FindProperty("playerUniqueID");
                    EditorGUILayout.PropertyField(prop);
                    o.ApplyModifiedProperties();
                    
                }
                
                EditorGUILayout.EndVertical();
            }
            
        }
    }
}