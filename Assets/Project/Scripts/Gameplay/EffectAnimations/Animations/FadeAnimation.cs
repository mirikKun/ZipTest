using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Gameplay.EffectAnimations.Animations
{
    [Serializable]
    public class FadeAnimation : BaseAnimation
    {
        [SerializeField] private Graphic _graphic;
        [SerializeField] private float _startAlpha = 1f;
        [SerializeField] private float _endAlpha = 0f;
        [SerializeField] private bool _useGraphicAlpha = false;

        public override async UniTask PlayAnimation()
        {
            if (_graphic == null) return;

            float startAlpha = _useGraphicAlpha ? _graphic.color.a : _startAlpha;

            Color currentColor = _graphic.color;
            currentColor.a = startAlpha;
            _graphic.color = currentColor;

            Tween tween = _graphic.DOFade(_endAlpha, _duration);

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
            if (_graphic == null) return;

            float startAlpha = _useGraphicAlpha ? _graphic.color.a : _startAlpha;

            Color currentColor = _graphic.color;
            currentColor.a = startAlpha;
            _graphic.color = currentColor;
        }
    }
}