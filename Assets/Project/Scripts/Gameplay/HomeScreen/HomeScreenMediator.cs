using Project.Scripts.Gameplay.Windows;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Gameplay.HomeScreen
{
    public class HomeScreenMediator : MonoBehaviour
    {
        private IWindowService _windowService;

        [Inject]
        private void Construct(IWindowService windowService)
        {
            _windowService = windowService;
        }

        private void Start()
        {
            OpenMainMenu();
        }

        private void OnDestroy()
        {
            _windowService.CloseAll();
        }

        private void OpenMainMenu()
        {
            _windowService.Open(WindowId.MainMenu);
        }
    }
}