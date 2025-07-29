using OM.Animora.Runtime;
using UnityEditor;
using UnityEngine;

namespace OM.Animora.Editor
{
    /// <summary>
    /// Provides utility methods to save and load <see cref="AnimoraPlayer"/> state to/from a `.animora` JSON file.
    /// </summary>
    public static class AnimoraSaveManager
    {
        /// <summary>
        /// Serializes the current <see cref="AnimoraPlayer"/> state to JSON and saves it to a file.
        /// </summary>
        /// <param name="player">The <see cref="AnimoraPlayer"/> to save.</param>
        public static void Save(AnimoraPlayer player)
        {
            if (player == null) return;

            var json = JsonUtility.ToJson(player.GetSaveData());

            var path = EditorUtility.SaveFilePanel("Save Animora Player", Application.dataPath, "AnimoraPlayer.animora", "animora");
            if (string.IsNullOrEmpty(path)) return;

            System.IO.File.WriteAllText(path, json);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Loads a previously saved `.animora` file and applies it to the given <see cref="AnimoraPlayer"/>.
        /// </summary>
        /// <param name="player">The <see cref="AnimoraPlayer"/> to apply the loaded data to.</param>
        public static void Load(AnimoraPlayer player)
        {
            var path = EditorUtility.OpenFilePanel("Load Timeline", Application.dataPath, "animora");
            if (string.IsNullOrEmpty(path)) return;

            var json = System.IO.File.ReadAllText(path);
            var animoraPlayerSaveData = JsonUtility.FromJson<AnimoraPlayerSaveData>(json);

            player.LoadSaveData(animoraPlayerSaveData);
        }
    }
}