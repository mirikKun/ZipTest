using System;
using System.Collections.Generic;
using Project.Scripts.Gameplay.Zip.Board;
using Project.Scripts.Gameplay.Zip.Configs;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.View
{
    public class ZipBoardView : MonoBehaviour
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private GameObject _wrongOrderLabel;
        [SerializeField] private Vector2 _boardSize;
        [SerializeField] private Vector2 _offset;

        [Header("Prefabs")] [SerializeField] private ZipCellView _cellPrefab;

        private Vector2Int _current;

        private ZipCellView[,] _cells;
        private ZipBoard _board;
        private bool _active;
        public ZipCellView[,] Cells => _cells;
        public event Action BoardFinished;
        public event Action<ZipCurrentCell> CellClicked;


        public void CreateBoard(ZipBoardData data, bool autoClear = true)
        {
            if (autoClear) ClearBoard();
            _board = new ZipBoard(data);
            _board.OnCellChanged += OnCellChanged;
            _board.CheckpointReached += OnCheckpointReached;
            _board.BoardFinished += OnBoardFinished;
            _board.OrderBecameWrong += OnOrderBecameWrong;
            _board.OrderBecameCorrect += OnOrderBecameCorrect;

            float cellSize = _boardSize.x / data.MaxSize;
            _grid.cellSize = Vector3.one * cellSize;


            _cells = new ZipCellView[data.Width, data.Height];

            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    Vector3 centerOffset = (((Vector2)data.Size * cellSize)) / 2 - Vector2.one * cellSize / 2;
                    Vector3 position = _grid.CellToWorld(new Vector3Int(x, y, 0)) +
                        new Vector3(_offset.x, _offset.y, 0) - centerOffset;
                    ZipCellView cell = Instantiate(_cellPrefab, position, Quaternion.identity, transform);
                    cell.InitCell(_board.DefaultCells[x, y], cellSize, data.Size);
                    _cells[x, y] = cell;
                    _cells[x, y].OnCellClicked += OnCellClicked;
                    _cells[x, y].OnCellOver += OnCellOver;
                }
            }

            Vector2Int start = data.CheckpointPositions[0];
            _cells[start.x, start.y].UpdateCell(_board.ZipCurrentCells[start.x, start.y], _board.LineCells);
            _wrongOrderLabel.SetActive(false);
            _active = true;
        }

        public void ClearBoard()
        {
            if (_cells != null)
            {
                for (int x = 0; x < _cells.GetLength(0); x++)
                {
                    for (int y = 0; y < _cells.GetLength(1); y++)
                    {
                        if (_cells[x, y] != null)
                        {
                            _cells[x, y].Destroy();
                            //Destroy(_cells[x, y].gameObject);
                        }
                    }
                }

                _cells = null;
            }
        }

        public void SetActive(bool active)
        {
            _active = active;
        }

        private void OnCheckpointReached(ZipCurrentCell cell)
        {
            _cells[cell.Position.x, cell.Position.y].OnCheckpointReached();
        }

        private void OnOrderBecameCorrect()
        {
            _wrongOrderLabel.SetActive(false);
        }


        private void OnOrderBecameWrong()
        {
            _wrongOrderLabel.SetActive(true);
        }

        private void OnBoardFinished()
        {
            _active = false;
            BoardFinished?.Invoke();
        }

        private void OnCellChanged(ZipCurrentCell cell, List<ZipCurrentCell> lineCells)
        {
            _cells[cell.Position.x, cell.Position.y].UpdateCell(cell, lineCells);
            if (cell.PreviousCell == null) return;
            _cells[cell.PreviousCell.Position.x, cell.PreviousCell.Position.y].UpdateCell(cell.PreviousCell, lineCells);
        }

        private void OnCellClicked(ZipDefaultCell cell, bool canGoBack)
        {
            if (!_active) return;
            _board.TryMoveToPoint(cell.Position, canGoBack);
            CellClicked?.Invoke(_board.ZipCurrentCells[cell.Position.x, cell.Position.y]);
        }

        private void OnCellOver(ZipDefaultCell cell, bool canGoBack)
        {
            _board.TryMoveToPoint(cell.Position, canGoBack);
        }
    }
}