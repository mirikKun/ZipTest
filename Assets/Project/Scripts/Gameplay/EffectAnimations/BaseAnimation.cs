using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Gameplay.EffectAnimations
{
    public abstract class BaseAnimation :MonoBehaviour, IAnimation
    {
        [SerializeField] protected float _duration = 1f;
        [SerializeField] protected Ease _easeType = Ease.OutQuad;
        [SerializeField] protected AnimationCurve _customCurve;
        [SerializeField] protected bool _useCustomCurve = false;

        public abstract UniTask PlayAnimation();
        
        public virtual float GetAnimationDuration()
        {
            return _duration;
        }

        public abstract void SetStartState();
    }
}

