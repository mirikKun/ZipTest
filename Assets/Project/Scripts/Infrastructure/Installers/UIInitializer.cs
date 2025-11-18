using Code.Gameplay.Windows;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class UIInitializer : MonoBehaviour, IInitializable
    {
        [SerializeField] private RectTransform _uiRoot;
        private IWindowFactory _windowFactory;


        [Inject]
        private void Construct(IWindowFactory windowFactory) =>
            _windowFactory = windowFactory;

        public void Initialize() =>
            _windowFactory.SetUIRoot(_uiRoot);
    }
}