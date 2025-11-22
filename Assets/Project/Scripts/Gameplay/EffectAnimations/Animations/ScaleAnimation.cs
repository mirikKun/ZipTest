using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Gameplay.EffectAnimations.Animations
{
    [System.Serializable]
    public class ScaleAnimation : BaseAnimation
    {
        [SerializeField] private Transform _target;

        [SerializeField] private Vector3 _startScale = Vector3.zero;
        [SerializeField] private Vector3 _targetScale = Vector3.one;
        [SerializeField] private bool _relativeScale = false;
        [SerializeField] private bool _fromCurrentScale = true;

        public override async UniTask PlayAnimation()
        {
            Vector3 startScale = _fromCurrentScale ? _target.localScale : _startScale;
            Vector3 endScale = _relativeScale ? startScale + _targetScale : _targetScale;

            Tween tween = _target.DOScale(endScale, _duration);
            _target.localScale = startScale;

            if (_useCustomCurve && _customCurve != null && _customCurve.length > 0)
            {
                tween.SetEase(_customCurve);
            }
            else
            {
                tween.SetEase(_easeType);
            }

            await UniTask.WaitUntil(() => !tween.IsActive());
        }

        public override void SetStartState()
        {
            Vector3 startScale = _fromCurrentScale ? _target.localScale : _startScale;
            _target.localScale = startScale;
        }
    }
}