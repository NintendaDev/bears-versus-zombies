using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SampleGame.Gameplay.GameContext
{
    public sealed class RandomPointsService : MonoBehaviour
    {
        [SerializeField]
        private Transform[] _points;
        
        public Transform NextPoint()
        {
            return _points[Random.Range(0, _points.Length)];
        }

        [Button, HideInPlayMode]
        private void Scan()
        {
            List<Transform> rootTransforms = new();
            
            foreach (Transform childTransform in transform)
                rootTransforms.Add(childTransform);
            
            _points = rootTransforms.ToArray();
        }
    }
}