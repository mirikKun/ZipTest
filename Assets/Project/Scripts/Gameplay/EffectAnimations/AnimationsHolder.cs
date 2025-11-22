using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Project.Scripts.Gameplay.EffectAnimations
{
    [System.Serializable]
    public class AnimationsHolder
    {
        [SerializeField] private List<AnimationData> _animations = new List<AnimationData>();

        public List<AnimationData> Animations => _animations;
        
        public async UniTask PlayAllAnimations()
        {
            
            for (int i = 0; i < _animations.Count; i++)
            {
                AnimationData animationData = _animations[i];

                IAnimation animation = animationData.GetAnimation();
                if (animation == null) continue;

                if (animationData.PlayMode == AnimationPlayMode.Sequential)
                {

                    await  PlayAnimationWithDelays(animation, animationData.DelayBefore, animationData.DelayAfter);

                }
                if (animationData.PlayMode == AnimationPlayMode.Parallel)
                {
                    PlayAnimationWithDelays(animation, animationData.DelayBefore, animationData.DelayAfter).Forget();
                }
            }
        }

        public void SetAnimationsStartState()
        {
            foreach (var animationData in _animations)
            {
                animationData.GetAnimation().SetStartState();
            }
        }

        private async UniTask PlayAnimationWithDelays(IAnimation animation, float delayBefore, float delayAfter)
        {
            if (delayBefore > 0f)
            {
                await UniTask.WaitForSeconds(delayBefore);
            }
            await animation.PlayAnimation();
            if (delayAfter > 0f)
            {
                await UniTask.WaitForSeconds(delayAfter);
            }
        }
        
    }
}

