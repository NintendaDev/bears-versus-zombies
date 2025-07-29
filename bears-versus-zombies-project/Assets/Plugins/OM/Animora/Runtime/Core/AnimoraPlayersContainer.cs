using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OM.Animora.Runtime
{
    public class AnimoraPlayersContainer : MonoBehaviour
    {
        [SerializeField] private List<AnimoraPlayer> players;
        
        public AnimoraPlayer GetAnimoraPlayerByName(string playerName)
        {
            return players.FirstOrDefault(x => x.PlayerUniqueID == playerName);
        }
        
        public List<AnimoraPlayer> GetAnimoraPlayersByName(string playerName)
        {
            return players.Where(x => x.PlayerUniqueID == playerName).ToList();
        }
        
        public AnimoraPlayer PlayFirstPlayerByName(string playerName)
        {
            var player = GetAnimoraPlayerByName(playerName);
            if (player != null)
            {
                player.PlayAnimation();
            }
            return player;
        }

        public List<AnimoraPlayer> GetSameIDPlayers()
        {
            var sameIDPlayers = new List<AnimoraPlayer>();
            var playerNames = players.Where(x=>x != null).Select(x => x.PlayerUniqueID).ToList();
            var duplicates = playerNames.GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .ToList();

            foreach (var player in players)
            {
                if(player == null) continue;
                if (duplicates.Contains(player.PlayerUniqueID))
                {
                    sameIDPlayers.Add(player);
                }
            }

            return sameIDPlayers;
        }
    }
}