using System;
using Project.Scripts.Gameplay.Zip.Board;
using Project.Scripts.Gameplay.Zip.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Gameplay.Zip.View
{
    public class ZipCellView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _backGround;
        [SerializeField] private ZipLineView _line;
        [SerializeField] private ZipCheckPointView _checkPoint;
        [SerializeField] private BoxCollider2D _collider;
        private ZipDefaultCell _cell;

        public event Action<ZipDefaultCell> OnCellClicked;

        public void InitCell(ZipDefaultCell cell,Vector2 size)
        {
            _cell = cell;
            if (cell.Type == ZipCellType.Checkpoint)
            {
                _checkPoint.gameObject.SetActive(true);
                _checkPoint.SetIndex(cell.Index+1);
                if (cell.Index == 0)
                {
                    _line.gameObject.SetActive(true);

                }
            }
            //_backGround.size = size;
            _collider.size = size;
        }

        public void UpdateCell(ZipCurrentCell cell)
        {
            _line.gameObject.SetActive(cell.StepIndex > 0);
        }

        private void OnMouseDown()
        {
            Debug.Log($"Click on{_cell.Position}");
            OnCellClicked?.Invoke(_cell);
        }
        
    }
}