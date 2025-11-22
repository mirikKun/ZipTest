using Project.Scripts.Gameplay.StaticData;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Gameplay.Windows
{
    public class WindowFactory : IWindowFactory
    {
        private readonly IStaticDataService _staticData;
        private readonly IInstantiator _instantiator;
        private RectTransform _uiRoot;

        public WindowFactory(IStaticDataService staticData, IInstantiator instantiator)
        {
            _staticData = staticData;
            _instantiator = instantiator;
        }

        public void SetUIRoot(RectTransform uiRoot) =>
            _uiRoot = uiRoot;

        public BaseWindow CreateWindow(WindowId windowId) =>
            _instantiator.InstantiatePrefabForComponent<BaseWindow>(PrefabFor(windowId), _uiRoot);        
        public T CreateWindow<T>(WindowId windowId) where T : BaseWindow
        {
            return _instantiator.InstantiatePrefabForComponent<T>(PrefabFor(windowId), _uiRoot);
        }

        private GameObject PrefabFor(WindowId id) =>
            _staticData.GetWindowPrefab(id);
    }
}