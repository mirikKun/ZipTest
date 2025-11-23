using System;
using Cysharp.Threading.Tasks;
using Project.Scripts.Gameplay.EffectAnimations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.Scripts.Gameplay.Windows.WindowTypes
{
    public class WinMenu : BaseWindow
    {
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button[] _mainMenuButtons;
        [SerializeField] private TextMeshProUGUI _timeUsed;
        [SerializeField] private AnimationsHolder _winAnimation;
        [SerializeField] private WinStar[] _stars;
        private bool _initialized;

        public void Init(UnityAction openHomeScreen, UnityAction openNextLevel, UnityAction openCurrentLevel)
        {
            if (_initialized) return;
            _initialized = true;
            SetMainMenuButtons(openHomeScreen);
            SetNextLevelButtonAction(openNextLevel);
            SetRestartButtonAction(openCurrentLevel);
        }


        public void SetMainMenuButtons(UnityAction action)
        {
            foreach (var button in _mainMenuButtons)
            {
                button.onClick.AddListener(action);
            }
        }

        public void SetNextLevelButtonAction(UnityAction action)
        {
            _nextLevelButton.onClick.AddListener(action);
        }

        public void SetRestartButtonAction(UnityAction action)
        {
            _restartButton.onClick.AddListener(action);
        }

        public void SetData(float timeUsed, float timeToMaxStars)
        {
            int minutes = Mathf.FloorToInt(timeUsed / 60f);
            int seconds = Mathf.FloorToInt(timeUsed % 60f);
            _timeUsed.text = $"{minutes:00}:{seconds:00}";

            foreach (var star in _stars)
            {
                star.StarAnimation.SetStartState();
            }


            _winAnimation.SetAnimationsStartState();
            PlayAnimations(timeUsed, timeToMaxStars).Forget();
        }

        private async UniTask PlayAnimations(float timeUsed, float timeToMaxStars)
        {
            await _winAnimation.PlayAllAnimations();
            for (int i = 0; i < _stars.Length; i++)
            {
                if (timeToMaxStars * (i + 1) / _stars.Length > timeUsed)
                {
                    await UniTask.WaitForSeconds(_stars[i].Delay);
                    _stars[i].StarParticles.Play();
                    _stars[i].StarAnimation.PlayAnimation().Forget();
                }
            }
        }
    }

    [Serializable]
    public class WinStar
    {
        public BaseAnimation StarAnimation;
        public ParticleSystem StarParticles;
        public float Delay;
    }
}