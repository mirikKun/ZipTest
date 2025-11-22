using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Gameplay.Windows
{
    public class WindowService : IWindowService
    {
        private readonly IWindowFactory _windowFactory;

        private readonly Dictionary<WindowId, BaseWindow> _openedWindows = new();


        public WindowService(IWindowFactory windowFactory) =>
            _windowFactory = windowFactory;

        public void Open(WindowId windowId)
        {
            if (_openedWindows.ContainsKey(windowId))
            {
                _openedWindows[windowId].gameObject.SetActive(true);
                return;
            }

            var baseWindow = _windowFactory.CreateWindow(windowId);
            _openedWindows.Add(windowId, baseWindow);
        }

        public T Open<T>(WindowId windowId) where T : BaseWindow
        {
            if (_openedWindows.ContainsKey(windowId))
            {
                _openedWindows[windowId].gameObject.SetActive(true);
                return _openedWindows[windowId] as T;
            }

            var window = _windowFactory.CreateWindow<T>(windowId);
            _openedWindows.Add(windowId, window);
            return window;
        }

        public void Hide(WindowId windowId)
        {
            if (_openedWindows.ContainsKey(windowId))
                _openedWindows[windowId].gameObject.SetActive(false);
        }

        public void Close(WindowId windowId)
        {
            var window = _openedWindows[windowId];
            _openedWindows.Remove(windowId);

            Object.Destroy(window.gameObject);
        }

        public void CloseAll()
        {
            foreach (var window in _openedWindows.Values)
            {
                if (window)
                {
                    Object.Destroy(window.gameObject);
                }
            }

            _openedWindows.Clear();
        }
    }
}