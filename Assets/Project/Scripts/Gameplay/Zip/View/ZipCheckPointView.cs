using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Project.Scripts.Gameplay.EffectAnimations;

namespace Project.Scripts.Gameplay.Zip.View
{
    public class ZipCheckPointView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _checkPoint;
        [SerializeField] private TextMeshProUGUI _index;

        [Header("Reach animation")] [SerializeField]
        private AnimationsHolder _reachAnimation;

        public void SetIndex(int index)
        {
            _index.text = index.ToString();
        }

        public void OnCheckPointReached()
        {
            _reachAnimation.PlayAllAnimations().Forget();
        }
    }
}