using UnityEngine;

namespace SampleGame.Gameplay.GameObjects
{
    public sealed class ParentTransformConstraint : MonoBehaviour
    {
        [SerializeField] private string _parentName = "[World]";

        private void OnEnable()
        {
            GameObject parent = GameObject.Find(_parentName);
            
            if (parent != null)
                transform.SetParent(parent.transform);
        }
    }
}