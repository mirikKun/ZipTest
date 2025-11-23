using Cysharp.Threading.Tasks;
using Project.Scripts.Gameplay.EffectAnimations;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.View
{
    public class TimerView:MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timer;
        [SerializeField] private TextMeshProUGUI _additionalTime;
        [SerializeField] private AnimationsHolder _additionalTimeAnimation;
        
        public void UpdateTimer(float timeUsed)
        {
            int minutes = Mathf.FloorToInt(timeUsed / 60f);
            int seconds = Mathf.FloorToInt(timeUsed % 60f);
            _timer.text = $"{minutes:00}:{seconds:00}";
        }

        public void PlayNewTimeEffect(float additionalTime)
        {
            
            int seconds = Mathf.FloorToInt(additionalTime);

            _additionalTime.text = $"+{seconds}";
            _additionalTimeAnimation.PlayAllAnimations().Forget();
        }
    }
}