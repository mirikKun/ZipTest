using Cysharp.Threading.Tasks;
using Project.Scripts.Gameplay.EffectAnimations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.Scripts.Gameplay.Windows.WindowTypes
{
    public class LoseMenu : BaseWindow
    {
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button[] _mainMenuButtons;
        [SerializeField] private TextMeshProUGUI _timeUsed;
        [SerializeField] private TextMeshProUGUI _levelPassed;
        [SerializeField] private AnimationsHolder _progressAnimation;

        private bool _initialized;

        public void Init(UnityAction openHomeScreen, UnityAction openCurrentLevel)
        {
            if (_initialized) return;
            _initialized = true;
            SetMainMenuButtons(openHomeScreen);
            SetRestartButtonAction(openCurrentLevel);
        }

        public void SetMainMenuButtons(UnityAction action)
        {
            foreach (var button in _mainMenuButtons)
            {
                button.onClick.AddListener(action);
            }
        }


        public void SetRestartButtonAction(UnityAction action)
        {
            _restartButton.onClick.AddListener(action);
        }

        public void SetData(float timeUsed, int levelPassed)
        {
            int minutes = Mathf.FloorToInt(timeUsed / 60f);
            int seconds = Mathf.FloorToInt(timeUsed % 60f);
            _timeUsed.text = $"{minutes:00}:{seconds:00}";

            _levelPassed.text = levelPassed.ToString();


            _progressAnimation.SetAnimationsStartState();
            _progressAnimation.PlayAllAnimations().Forget();
        }
    }
}