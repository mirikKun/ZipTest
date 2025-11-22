namespace Project.Scripts.Gameplay.Windows
{
    public interface IWindowService
    {
        void Open(WindowId windowId);
        void Close(WindowId windowId);
        T Open<T>(WindowId windowId) where T : BaseWindow;
        void CloseAll();
        void Hide(WindowId windowId);
    }
}