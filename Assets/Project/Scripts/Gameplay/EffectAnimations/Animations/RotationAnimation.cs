using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Gameplay.EffectAnimations.Animations
{
    [System.Serializable]
    public class RotationAnimation : BaseAnimation
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _targetRotation = Vector3.zero;
        [SerializeField] private Vector3 _startRotation = Vector3.zero;
        [SerializeField] private bool _relativeRotation = false;
        [SerializeField] private bool _fromCurrentRotation = true;
        [SerializeField] private RotateMode _rotateMode = RotateMode.Fast;

        public override async UniTask PlayAnimation()
        {
            Vector3 startRotation = _fromCurrentRotation ? _target.eulerAngles : _startRotation;
            Vector3 endRotation = _relativeRotation ? startRotation + _targetRotation : _targetRotation;

            Tween tween = _target.DORotate(endRotation, _duration, _rotateMode);

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
            Vector3 startRotation = _fromCurrentRotation ? _target.eulerAngles : _startRotation;
            _target.eulerAngles = startRotation;
        }
    }
}