using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Project.Scripts.Gameplay.Zip.View
{
    public class ZipCheckPointView:MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _checkPoint;
        [SerializeField] private TextMeshProUGUI _index;
        [Header("Reach animation")]
        [SerializeField] private float _animationDuration = 0.2f;
        [SerializeField] private AnimationCurve _animationCurve;
        [SerializeField] private float _targetScale = 1.2f;
        
        public void SetIndex(int index)
        {
            _index.text = index.ToString();
        }

        public void OnCheckPointReached()
        {
            transform.DOScale(_targetScale, _animationDuration).SetEase(_animationCurve);
        }
    }
}