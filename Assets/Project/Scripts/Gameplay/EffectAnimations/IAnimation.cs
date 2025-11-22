using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Project.Scripts.Gameplay.EffectAnimations
{
    public interface IAnimation
    {

        UniTask PlayAnimation();
        float GetAnimationDuration();
        void SetStartState();
    }
}

