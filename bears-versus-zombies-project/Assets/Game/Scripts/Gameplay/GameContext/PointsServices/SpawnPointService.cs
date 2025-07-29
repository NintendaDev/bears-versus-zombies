using System.Collections.Generic;
using UnityEngine;

namespace SampleGame.Gameplay.GameContext
{
    public class SpawnPointService : MonoBehaviour
    {
        [SerializeField]
        private Transform[] _spawnPoints;
        
        private readonly List<Transform> _freePoints = new();
        
        public Transform NextPoint()
        {
            if (_freePoints.Count == 0) 
                _freePoints.AddRange(_spawnPoints);

            int randomIndex = Random.Range(0, _freePoints.Count);
            Transform result = _freePoints[randomIndex];
            _freePoints.RemoveAt(randomIndex);
            
            return result;
        }
    }
}