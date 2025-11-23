using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Project.Scripts.Gameplay.EffectAnimations;
using Project.Scripts.Gameplay.Zip.Board;
using Project.Scripts.Gameplay.Zip.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.View
{
    public class ZipLineView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _line;
        [SerializeField] private SpriteMask _spriteMask;
        [SerializeField] private ZipLineSprite[] _lineSprites;
        [SerializeField] private AnimationsHolder _onClickAnimation;

        public void UpdateLineSprite(List<ZipCurrentCell> lineCells, int index)
        {
            if (lineCells.Count == 1)
            {
                 SetSprite(LineSpriteType.Start);
            }
            else if (index == 0)
            {
                Vector2Int offset = lineCells[0].Position - lineCells[1].Position;
                GetCurrentLineSprite(offset);
            }
            else if (index == lineCells.Count - 1)
            {
                Vector2Int offset = lineCells[^1].Position - lineCells[^2].Position;
                GetCurrentLineSprite(offset);
            }
            else
            {
                GetStraightLineSprite(lineCells[index + 1].Position - lineCells[index - 1].Position);
                GetAngleLineSprite(lineCells[index - 1].Position, lineCells[index].Position,
                    lineCells[index + 1].Position);
            }
        }

        public void PlayOnClickAnimation()
        {
            _onClickAnimation.PlayAllAnimations().Forget();
        }

        private void GetStraightLineSprite(Vector2Int offset)
        {
            if (offset == Vector2Int.right * 2 || offset == Vector2Int.left * 2)
            {
                 SetSprite(LineSpriteType.StraightHorizontal);
            }
            else if (offset == Vector2Int.up * 2 || offset == Vector2Int.down * 2)
            {
                 SetSprite(LineSpriteType.StraightVertical);
            }
        }

        private void GetAngleLineSprite(Vector2Int previous, Vector2Int current, Vector2Int next)
        {
            Vector2Int totalOffset = next - previous;
            Vector2Int currentOffset = next - current;
            if ((totalOffset.x == 1 && totalOffset.y == 1 && currentOffset.y == 1)
                || (totalOffset.x == -1 && totalOffset.y == -1 && currentOffset.x == -1))
            {
                 SetSprite(LineSpriteType.AngleLeftUp);
            }
            else if ((totalOffset.x == 1 && totalOffset.y == -1 && currentOffset.x == 1) ||
                     (totalOffset.x == -1 && totalOffset.y == 1 && currentOffset.y == 1))
            {
                 SetSprite(LineSpriteType.AngleRightUp);
            }
            else if ((totalOffset.x == -1 && totalOffset.y == 1 && currentOffset.x == -1)
                     || (totalOffset.x == 1 && totalOffset.y == -1 && currentOffset.y == -1))
            {
                 SetSprite(LineSpriteType.AngleLeftDown);
            }
            else if ((totalOffset.x == -1 && totalOffset.y == -1 && currentOffset.y == -1)
                     || (totalOffset.x == 1 && totalOffset.y == 1 && currentOffset.x == 1))
            {
                 SetSprite(LineSpriteType.AngleRightDown);
            }
        }

        private void GetCurrentLineSprite(Vector2Int offset)
        {
            if (offset == Vector2Int.right)
            {
                 SetSprite(LineSpriteType.CurrentRight);
            }
            else if (offset == Vector2Int.left)
            {
                 SetSprite(LineSpriteType.CurrentLeft);
            }
            else if (offset == Vector2Int.up)
            {
                 SetSprite(LineSpriteType.CurrentUp);
            }
            else if (offset == Vector2Int.down)
            {
                 SetSprite(LineSpriteType.CurrentDown);
            }
        }

        private void SetSprite(LineSpriteType spriteType)
        {
            foreach (var cellBackground in _lineSprites)
            {
                if (cellBackground.SpriteType == spriteType)
                {
                    _line.sprite= cellBackground.Sprite;
                    _spriteMask.sprite= cellBackground.Sprite;
                }
            }

        }

        public void SetScale(float scale)
        {
            _line.transform.localScale = Vector3.one * scale;
        }
    }

    [Serializable]
    public class ZipLineSprite
    {
        public Sprite Sprite;
        public LineSpriteType SpriteType;
    }
}