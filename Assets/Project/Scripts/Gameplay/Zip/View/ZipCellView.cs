using System;
using System.Collections.Generic;
using DG.Tweening;
using Project.Scripts.Gameplay.Zip.Board;
using Project.Scripts.Gameplay.Zip.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.View
{
    public class ZipCellView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _backGround;
        [SerializeField] private SpriteRenderer _frame;
        [SerializeField] private float _spriteScaleMultiplier = 1.1f;
        [SerializeField] private ZipLineView _line;
        [SerializeField] private ZipCheckPointView _checkPoint;
        [SerializeField] private BoxCollider2D _collider;

        [SerializeField] private ZipCellSprites[] _cellBackgrounds;

        private ZipDefaultCell _cell;
        private float _spritePixelPerUnit = 100;
        private Tween _tween;

        public event Action<ZipDefaultCell, bool> OnCellClicked;
        public event Action<ZipDefaultCell, bool> OnCellOver;
        public  ZipLineView Line=>_line;

        public void InitCell(ZipDefaultCell cell, float size, Vector2Int boardSize)
        {
            _cell = cell;
            if (cell.Type == ZipCellType.Checkpoint)
            {
                _checkPoint.gameObject.SetActive(true);
                _checkPoint.SetIndex(cell.Index + 1);
                if (cell.Index == 0)
                {
                    _line.gameObject.SetActive(true);
                }
            }

            float scale = (1 / (_frame.sprite.rect.height / _spritePixelPerUnit)) * size;
            _frame.transform.localScale = Vector3.one * _spriteScaleMultiplier * scale;
            _backGround.transform.localScale = Vector3.one * scale;
            _line.SetScale(scale);
            _collider.size = Vector2.one * size;
            UpdateSprite(cell.Position, boardSize);
        }

        public void UpdateCell(ZipCurrentCell cell, List<ZipCurrentCell> lineCells)
        {
            _line.gameObject.SetActive(cell.StepIndex >= 0);
            _backGround.gameObject.SetActive(cell.StepIndex >= 0);
            if (cell.StepIndex >= 0)
            {
                _line.UpdateLineSprite(lineCells, cell.StepIndex);
            }
        }

        public void OnCheckpointReached()
        {
            _checkPoint.OnCheckPointReached();
        }

        public void Destroy()
        {
            _tween.Kill();
            Destroy(gameObject);
        }

        public void ApplyTween(Tween tween)
        {
            _tween = tween;
        }

        private void OnMouseDown()
        {
            OnCellClicked?.Invoke(_cell, true);
        }

        private void OnMouseOver()
        {
            if (Input.GetMouseButton(0))
            {
                OnCellOver?.Invoke(_cell, false);
            }
        }

        private void UpdateSprite(Vector2Int position, Vector2Int size)
        {
            if (position.x == 0 && position.y == 0)
            {
                SetSpritesByType(CellBackgroundSpriteType.LeftDown);
            }
            else if (position.x == 0 && position.y == size.y - 1)
            {
                SetSpritesByType(CellBackgroundSpriteType.LeftUp);
            }
            else if (position.x == size.x - 1 && position.y == size.y - 1)
            {
                SetSpritesByType(CellBackgroundSpriteType.RightUp);
            }
            else if (position.x == size.x - 1 && position.y == 0)
            {
                SetSpritesByType(CellBackgroundSpriteType.RightDown);
            }
            else
            {
                SetSpritesByType(CellBackgroundSpriteType.Center);
            }
        }

        private void SetSpritesByType(CellBackgroundSpriteType spriteType)
        {
            foreach (var cellBackground in _cellBackgrounds)
            {
                if (cellBackground.SpriteType == spriteType)
                {
                    _frame.sprite = cellBackground.FrameSprite;
                    _backGround.sprite = cellBackground.BackgroundSprite;

                    return;
                }
            }
        }
    }

    [Serializable]
    public class ZipCellSprites
    {
        public Sprite FrameSprite;
        public Sprite BackgroundSprite;
        public CellBackgroundSpriteType SpriteType;
    }
}