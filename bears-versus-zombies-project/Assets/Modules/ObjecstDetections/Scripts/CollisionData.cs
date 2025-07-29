using UnityEngine;

namespace Modules.ObjectsDetection
{
    public struct CollisionData
    {
        public GameObject Object;
        
        public Vector3 Point;
        
        public bool IsValid => Object != null;
    }
}