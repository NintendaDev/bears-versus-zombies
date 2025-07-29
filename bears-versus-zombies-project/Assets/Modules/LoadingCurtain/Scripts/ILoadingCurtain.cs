namespace Modules.LoadingCurtain
{
    public interface ILoadingCurtain
    {
        public void Show();
        
        public void Hide();

        public void ShowProgress();

        public void UpdateProgress(float percent);
        
        public void HideProgress();

        public void HideLogo();

        public void ShowMessage(string message);
        
        public void HideMessage();
    }
}