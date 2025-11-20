using System;
using System.Collections.Generic;
using Project.Scripts.Gameplay.Zip.Board;
using Project.Scripts.Gameplay.Zip.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Gameplay.Zip.View
{
    public class ZipCellView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _backGround;
        [SerializeField] private float _spriteScaleMultiplier = 1.1f;
        [SerializeField] private ZipLineView _line;
        [SerializeField] private ZipCheckPointView _checkPoint;
        [SerializeField] private BoxCollider2D _collider;

        [SerializeField] private ZipCellBackgroundSprite[] _cellBackgrounds;

        private ZipDefaultCell _cell;
        private float _spritePixelPerUnit = 100;

        public event Action<ZipDefaultCell, bool> OnCellClicked;

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

            float scale = (1 / (_backGround.sprite.rect.height / _spritePixelPerUnit)) * size;
            _backGround.transform.localScale = Vector3.one * _spriteScaleMultiplier * scale;
            _line.SetScale(scale);
            _collider.size = Vector2.one*size;
            UpdateSprite(cell.Position, boardSize);
        }

        public void UpdateCell(ZipCurrentCell cell, List<ZipCurrentCell> lineCells)
        {
            _line.gameObject.SetActive(cell.StepIndex >= 0);
            if (cell.StepIndex >= 0)
            {
                _line.UpdateLineSprite(lineCells, cell.StepIndex);
            }
        }

        private void OnMouseDown()
        {
            Debug.Log($"Click on{_cell.Position}");
            OnCellClicked?.Invoke(_cell, true);
        }

        private void OnMouseOver()
        {
            if (UnityEngine.Input.GetMouseButton(0))
            {
                OnCellClicked?.Invoke(_cell, false);
            }
        }

        private void UpdateSprite(Vector2Int position, Vector2Int size)
        {
            if (position.x == 0 && position.y == 0)
            {
                _backGround.sprite = GetSprite(CellBackgroundSpriteType.LeftDown);
            }
            else if (position.x == 0 && position.y == size.y - 1)
            {
                _backGround.sprite = GetSprite(CellBackgroundSpriteType.LeftUp);
            }
            else if (position.x == size.x - 1 && position.y == size.y - 1)
            {
                _backGround.sprite = GetSprite(CellBackgroundSpriteType.RightUp);
            }
            else if (position.x == size.x - 1 && position.y == 0)
            {
                _backGround.sprite = GetSprite(CellBackgroundSpriteType.RightDown);
            }
            else
            {
                _backGround.sprite = GetSprite(CellBackgroundSpriteType.Center);
            }
        }

        private Sprite GetSprite(CellBackgroundSpriteType spriteType)
        {
            foreach (var cellBackground in _cellBackgrounds)
            {
                if (cellBackground._spriteType == spriteType) return cellBackground.Sprite;
            }

            return null;
        }
    }

    [Serializable]
    public class ZipCellBackgroundSprite
    {
        public Sprite Sprite;
        public CellBackgroundSpriteType _spriteType;
    }
}