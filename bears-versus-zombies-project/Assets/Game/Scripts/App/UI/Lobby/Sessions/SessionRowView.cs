using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame.App.UI
{
    public sealed class SessionRowView : MonoBehaviour
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Button _button;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private TMP_Text _sessionNameLabel;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private TMP_Text _playersCounterLabel;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private TMP_Text _regionLabel;

        private readonly ReactiveCommand _clickedCommand = new();
        
        public Observable<Unit> Clicked => _clickedCommand.AsObservable();

        private void Awake()
        {
            _button
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _clickedCommand.Execute(Unit.Default);
                })
                .AddTo(this);
        }

        public void Initialize(string sessionName, string playersCount, string region)
        {
            _sessionNameLabel.text = sessionName;
            _playersCounterLabel.text = playersCount;
            _regionLabel.text = region;
        }
    }
}