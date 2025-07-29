using Modules.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Modules.LoadingCurtain
{
    public sealed class LoadingCurtain : MonoBehaviour, ILoadingCurtain
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Canvas _curtain;
        
        [SerializeField, Required, ChildGameObjectsOnly] 
        private GameObject _logoRoot;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private GameObject _messageRoot;
        
        [SerializeField, Required, ChildGameObjectsOnly] 
        private GameObject _spinner;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private TMP_Text _messageLabel;
        
        [SerializeField, Required, ChildGameObjectsOnly]
        private ProgressBar _progressBar;

        private void Awake()
        {
            Reset();
        }

        [Button, HideInEditorMode]
        public void Show()
        {
            Reset();
            _curtain.gameObject.SetActive(true);
        }

        [Button, HideInEditorMode]
        public void Hide()
        {
            _curtain.gameObject.SetActive(false);
        }

        [Button, HideInEditorMode]
        public void ShowProgress()
        {
            HideSpinner();
            _progressBar.Reset();
            _progressBar.Show();
        }
        
        public void UpdateProgress(float percent)
        {
            _progressBar.SetProgress(percent);
        }

        [Button, HideInEditorMode]
        public void HideProgress()
        {
            _progressBar.Hide();
            ShowSpinner();
        }

        [Button, HideInEditorMode]
        public void ShowMessage(string message)
        {
            _messageRoot.SetActive(true);
            _messageLabel.text = message;
        }

        [Button, HideInEditorMode]
        public void HideMessage()
        {
            _messageRoot.SetActive(false);
        }
        
        public void HideLogo() => _logoRoot.SetActive(false);

        private void Reset()
        {
            ShowSpinner();
            HideProgress();
            HideMessage();
            ShowLogo();
        }
        
        private void ShowSpinner() => _spinner.SetActive(true);
        
        private void HideSpinner() => _spinner.SetActive(false);
        
        private void ShowLogo() => _logoRoot.SetActive(true);
    }
}