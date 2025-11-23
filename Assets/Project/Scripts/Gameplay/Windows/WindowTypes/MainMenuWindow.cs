using Project.Scripts.Infrastructure.States.GameStates;
using Project.Scripts.Infrastructure.States.StateMachine;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.Scripts.Gameplay.Windows.WindowTypes
{
    public class MainMenuWindow:BaseWindow
    {
        [SerializeField] private Button _startZipGameButton;
        [SerializeField] private Button _startZipEndlessGameButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;
        private IGameStateMachine _gameStateMachine;
        private IWindowService _windowService;

        [Inject]
        private void Construct(IGameStateMachine gameStateMachine,IWindowService windowService)
        {
            _gameStateMachine = gameStateMachine;
            _windowService = windowService;
        }

        protected override void Initialize()
        {
            _startZipGameButton.onClick.AddListener(OpenZipGame);
            _startZipEndlessGameButton.onClick.AddListener(OpenZipEndlessGame);
            _settingsButton.onClick.AddListener(OpenSettingsMenu);
            _exitButton.onClick.AddListener(Application.Quit);
        }


    
        private void OpenSettingsMenu()
        {
            _windowService.Open(WindowId.Settings);
        }
        private void OpenZipGame()
        {
            _gameStateMachine.Enter<LoadingZipGameSceneState>();
        }
        private void OpenZipEndlessGame()
        {
            _gameStateMachine.Enter<LoadingZipEndlessGameSceneState>();
        }
    }
}