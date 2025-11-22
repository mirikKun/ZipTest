using Project.Scripts.Gameplay.Windows;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Infrastructure.Installers
{
    public class UIInitializer : MonoBehaviour, IInitializable
    {
        [SerializeField] private RectTransform _uiRoot;
        private IWindowFactory _windowFactory;


        [Inject]
        private void Construct(IWindowFactory windowFactory)
        {
            _windowFactory = windowFactory;
            Initialize();
        }

        public void Initialize() =>
            _windowFactory.SetUIRoot(_uiRoot);
    }
}