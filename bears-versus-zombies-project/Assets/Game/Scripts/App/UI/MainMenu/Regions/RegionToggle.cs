using Fusion;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame.App.UI
{
    public sealed class RegionToggle : ToggleBase<RegionToggle>
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Image _background;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private TMP_Text _regionCodeLabel;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private TMP_Text _pingLabel;
        
        public RegionInfo RegionInfo { get; private set; }

        public void Initialize(RegionInfo regionInfo, string regionCode, string ping)
        {
            RegionInfo = regionInfo;
            _regionCodeLabel.text = regionCode;
            _pingLabel.text = ping;
        }
    }
}