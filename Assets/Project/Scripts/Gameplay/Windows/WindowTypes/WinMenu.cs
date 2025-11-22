using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Gameplay.EffectAnimations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.Scripts.Gameplay.Windows.WindowTypes
{
    public class WinMenu : MonoBehaviour
    {
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button[] _mainMenuButtons;
        [SerializeField] private TextMeshProUGUI _timeUsed;
        [SerializeField] private AnimationsHolder _winAnimation;
        [SerializeField] private Image[] _stars;
        [SerializeField] private float _timeToMaxStars = 10f;

        public void SetNextLevelButtonAction(UnityAction action)
        {
            _nextLevelButton.onClick.AddListener(action);
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

        public void SetData(float timeUsed)
        {
            int minutes = Mathf.FloorToInt(timeUsed / 60f);
            int seconds = Mathf.FloorToInt(timeUsed % 60f);
            _timeUsed.text = $"{minutes:00}:{seconds:00}";
            for (int i = 0; i < _stars.Length; i++)
            {
                _stars[i].enabled = _timeToMaxStars * (i+1) > timeUsed;
            }

            _winAnimation.SetAnimationsStartState();
            _winAnimation.PlayAllAnimations().Forget();
        }
    }
}