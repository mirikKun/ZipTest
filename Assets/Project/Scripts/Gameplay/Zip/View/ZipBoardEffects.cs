using System.Collections;
using Cysharp.Threading.Tasks;
using Project.Scripts.Gameplay.EffectAnimations;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.View
{
    public class ZipBoardEffects:MonoBehaviour
    {
     
        [SerializeField] private AnimationsHolder _finishAnimation;

        public IEnumerator PlayFinishAnimation()
        {
            yield return _finishAnimation.PlayAllAnimations().ToCoroutine();
        }

    }
}