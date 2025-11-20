using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.Scripts.Gameplay.Windows.WindowTypes
{
    public class MainMenuWindow:MonoBehaviour
    {
        [SerializeField] private Button _startButton;

        public void SetNextLevelButtonAction(UnityAction action)
        {
            _startButton.onClick.AddListener(action);
        }

 
    }
}