using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame.App.UI
{
    public sealed class SessionConnectButton : MonoBehaviour
    {
        [SerializeField, Required] private Button _button;
        [SerializeField, Required] private TMP_Text _sessionNameLabel;

        private readonly ReactiveCommand _clickedCommand = new();
        private GameObject _gameObject;

        public Observable<Unit> Clicked => _clickedCommand.AsObservable();
        
        public void Initialize()
        {
            _gameObject = gameObject;
            
            _button.OnClickAsObservable()
                .Subscribe(_clickedCommand.Execute)
                .AddTo(this);
        }

        public void Enable(string sessionName)
        {
            _gameObject.SetActive(true);
            _sessionNameLabel.text = sessionName;
        }

        public void Disable()
        {
            _gameObject.SetActive(false);
        }
    }
}