using System;
using System.Collections.Generic;
using Project.Scripts.Gameplay.Zip.Board;
using Project.Scripts.Gameplay.Zip.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.View
{
    public class ZipLineView:MonoBehaviour
    {
        [SerializeField] private ZipLineSprite[] _lineSprites;
        [SerializeField] private SpriteRenderer _line;

        public void UpdateLineSprite(List<ZipCurrentCell> lineCells, int index)
        {
            if (lineCells.Count == 1)
            {
                _line.sprite = GetSprite(LineSpriteType.Start);
            }
            else if (index == 0)
            {
                Vector2Int offset = lineCells[0].Position - lineCells[1].Position;
                GetCurrentLineSprite(offset);
            }
            else if(index == lineCells.Count - 1)
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

        private void GetStraightLineSprite(Vector2Int offset)
        {
            if (offset == Vector2Int.right * 2 || offset == Vector2Int.left * 2)
            {
                _line.sprite = GetSprite(LineSpriteType.StraightHorizontal);
            }
            else if (offset == Vector2Int.up * 2 || offset == Vector2Int.down * 2)
            {
                _line.sprite = GetSprite(LineSpriteType.StraightVertical);
            }

        }

        private void GetAngleLineSprite(Vector2Int previous, Vector2Int current, Vector2Int next)
        {
            Vector2Int totalOffset=next-previous;
            Vector2Int currentOffset=next-current;
             if ((totalOffset.x == 1 && totalOffset.y == 1 && currentOffset.y == 1)
                 || (totalOffset.x == -1 && totalOffset.y == -1 && currentOffset.x == -1))
            {
                _line.sprite = GetSprite(LineSpriteType.AngleLeftUp);
            }
            else if ((totalOffset.x == 1 && totalOffset.y == -1 && currentOffset.x == 1)||
                     (totalOffset.x == -1 && totalOffset.y == 1 && currentOffset.y == 1))
            {
                _line.sprite = GetSprite(LineSpriteType.AngleRightUp);
                
            }
            else if ((totalOffset.x == -1 && totalOffset.y == 1 && currentOffset.x == -1)
                     || (totalOffset.x == 1 && totalOffset.y == -1 && currentOffset.y == -1))
            {
                _line.sprite = GetSprite(LineSpriteType.AngleLeftDown);
            }
            else if ((totalOffset.x == -1 && totalOffset.y == -1 && currentOffset.y == -1)
                     || (totalOffset.x == 1 && totalOffset.y == 1 && currentOffset.x == 1))
            {
                _line.sprite = GetSprite(LineSpriteType.AngleRightDown);
            }
        }
        private void GetCurrentLineSprite(Vector2Int offset)
        {
            if (offset == Vector2Int.right)
            {
                _line.sprite = GetSprite(LineSpriteType.CurrentRight);
            }
            else if (offset == Vector2Int.left)
            {
                _line.sprite = GetSprite(LineSpriteType.CurrentLeft);
            }
            else if (offset == Vector2Int.up)
            {
                _line.sprite = GetSprite(LineSpriteType.CurrentUp);
            }
            else if (offset == Vector2Int.down)
            {
                _line.sprite = GetSprite(LineSpriteType.CurrentDown);
            }
        }

        private Sprite GetSprite(LineSpriteType spriteType)
        {
            foreach (var cellBackground in _lineSprites)
            {
                if (cellBackground.SpriteType == spriteType) return cellBackground.Sprite;
            }
            return null;
        }
    }
    [Serializable]
    public class ZipLineSprite
    {
     
        public Sprite Sprite;
        public LineSpriteType SpriteType;
    }
}