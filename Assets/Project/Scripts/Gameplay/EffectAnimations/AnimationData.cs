using System;
using UnityEngine;

namespace Project.Scripts.Gameplay.EffectAnimations
{
    [Serializable]
    public class AnimationData
    {
        [Header("Animation Settings")]
        [SerializeField] private BaseAnimation _baseAnimation;
        
        [Header("Delays")]
        [SerializeField] private float _delayBefore = 0f;
        [SerializeField] private float _delayAfter = 0f;
        
        [Header("Play Mode")]
        [SerializeField] private AnimationPlayMode _playMode = AnimationPlayMode.Sequential;
        

        public float DelayBefore => _delayBefore;
        public float DelayAfter => _delayAfter;
        public AnimationPlayMode PlayMode => _playMode;

        public IAnimation GetAnimation()
        {
            return _baseAnimation;
        }

        public float GetTotalDuration()
        {
            var animation = GetAnimation();
            if (animation == null) return 0f;
            
            return _delayBefore + animation.GetAnimationDuration() + _delayAfter;
        }
    }


}

