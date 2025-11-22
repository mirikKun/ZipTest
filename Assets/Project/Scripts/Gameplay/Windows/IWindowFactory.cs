using UnityEngine;

namespace Project.Scripts.Gameplay.Windows
{
    public interface IWindowFactory
    {
        public void SetUIRoot(RectTransform uiRoot);
        public BaseWindow CreateWindow(WindowId windowId);
        T CreateWindow<T>(WindowId windowId) where T : BaseWindow;
    }
}