using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Gameplay.EffectAnimations.Animations
{
    [System.Serializable]
    public class MoveAnimation : BaseAnimation
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _targetPosition;
        [SerializeField] private Vector3 _startPosition = Vector3.zero;
        [SerializeField] private Transform _startPositionTransform;
        [SerializeField] private Transform _targetPositionTransform;
        [SerializeField] private bool _useTransformTarget = false;

        public override async UniTask PlayAnimation()
        {


            Vector3 endPosition;

            if (_useTransformTarget)
            {
                endPosition = _targetPositionTransform.position;
                _target.position = _startPositionTransform.position;
                
                if (_targetPositionTransform is RectTransform targetPositionRectTransform)
                {
                    endPosition = targetPositionRectTransform.anchoredPosition;
                }
            }
            else
            {
                endPosition = _targetPosition;
                _target.localPosition = _startPosition;

            }

            Tween tween;
            if (_target is RectTransform targetRectTransform)
            {
                tween = targetRectTransform.DOAnchorPos3D(endPosition, _duration);
            }
            else
            {
                tween = _useTransformTarget
                    ? _target.DOLocalMove(endPosition, _duration)
                    : _target.DOMove(endPosition, _duration);
            }
            

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
            if (_useTransformTarget)
            {
                _target.position = _startPositionTransform.position;

            }
            else
            {
                _target.position = _startPosition;
            }        
        }
    }
}

