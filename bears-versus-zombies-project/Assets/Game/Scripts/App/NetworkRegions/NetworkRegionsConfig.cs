using UnityEngine;

namespace SampleGame.App
{
    [CreateAssetMenu(fileName = "NetworkRegionsConfig", 
        menuName = "SampleGame/NetworkRegionsConfig")]
    public sealed class NetworkRegionsConfig : ScriptableObject
    {
        [SerializeField] private string[] _regionsFilter = new[] { "eu", "us", "asia" };
        
        public string[] RegionsFilter => _regionsFilter;
        
        [field: SerializeField] public string DefaultRegion { get; private set; }  = "eu";
    }
}