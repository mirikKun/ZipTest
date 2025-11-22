using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Project.Scripts.Gameplay.EffectAnimations;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.View
{
    public class ZipBoardEffects : MonoBehaviour
    {
        [SerializeField] private ZipBoardView _zipBoardView;
        [SerializeField] private AnimationsHolder _finishAnimation;

        [SerializeField] private float _boardAppearAnimationDuration = 1.0f;
        [SerializeField] private float _cellAppearAnimationDuration = 0.2f;

        public void PlayPlayBoardAppearAnimation(Vector3 scaleFrom, Vector3 scaleTo)
        {
            ZipCellView[,] cells = _zipBoardView.Cells;
            int rows = cells.GetLength(0);
            int columns = cells.GetLength(1);

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    cells[x, y].transform.localScale = scaleFrom;
                }
            }

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                      PlayCellAppearAnimation(cells[x, y], (x+y)/ (rows+columns-2f)* _boardAppearAnimationDuration,scaleTo).Forget();      
                }
            }
        }

        private async UniTask PlayCellAppearAnimation(ZipCellView cell, float delay,Vector3 scaleTo)
        {
            await UniTask.WaitForSeconds(delay);
            if(cell)
            {
                Tween tween = cell.transform.DOScale(scaleTo, _cellAppearAnimationDuration);
                cell.ApplyTween(tween);
            }
        }

        public IEnumerator PlayFinishAnimation()
        {
            yield return _finishAnimation.PlayAllAnimations().ToCoroutine();
        }
    }
}