using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.Scripts.Gameplay.Windows.WindowTypes
{
    public class WinMenu:MonoBehaviour
    {
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button[] _mainMenuButtons;
        [SerializeField] private TextMeshProUGUI _timeUsed;

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

        public void SetData(float timeUsed)
        {
            int minutes = Mathf.FloorToInt(timeUsed / 60f);
            int seconds = Mathf.FloorToInt(timeUsed % 60f);
            _timeUsed.text = $"{minutes:00}:{seconds:00}";
        }
    }
}