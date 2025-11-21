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


        private ZipCellView[,] _cells;
        private ZipBoard _board;
        private float _timeUsed;
        private bool _active;
        public event Action<float> BoardFinished;
        public float TimeUsed => _timeUsed;

        private void Update()
        {
            if (!_active) return;
            _timeUsed += Time.deltaTime;
        }

        public void CreateBoard(ZipBoardConfig config)
        {
            ClearBoard();
            _board = new ZipBoard(config);
            _board.OnCellChanged += OnCellChanged;
            _board.CheckpointReached += OnCheckpointReached;
            _board.BoardFinished += OnBoardFinished;
            _board.OrderBecameWrong += OnOrderBecameWrong;
            _board.OrderBecameCorrect += OnOrderBecameCorrect;

            float cellSize = _boardSize.x / config.MaxSize;
            _grid.cellSize = Vector3.one*cellSize;


            _cells = new ZipCellView[config.Width, config.Height];

            for (int x = 0; x < config.Width; x++)
            {
                for (int y = 0; y < config.Height; y++)
                {
                    Vector3 centerOffset = (((Vector2)config.Size * cellSize)) / 2-Vector2.one*cellSize/2;
                    Vector3 position = _grid.CellToWorld(new Vector3Int(x, y, 0)) +
                        new Vector3(_offset.x, _offset.y, 0) - centerOffset;
                    ZipCellView cell = Instantiate(_cellPrefab, position, Quaternion.identity, transform);
                    cell.InitCell(_board.DefaultCells[x, y], cellSize, config.Size);
                    _cells[x, y] = cell;
                    _cells[x, y].OnCellClicked += OnCellClicked;
                }
            }

            Vector2Int start = config.CheckpointPositions[0];
            _cells[start.x, start.y].UpdateCell(_board.ZipCurrentCells[start.x, start.y], _board.LineCells);
            _timeUsed = 0;
            _active = true;
            _wrongOrderLabel.SetActive(false);
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
            BoardFinished?.Invoke(_timeUsed);
            _active = false;
        }

        private void ClearBoard()
        {
            if (_cells != null)
            {
                for (int x = 0; x < _cells.GetLength(0); x++)
                {
                    for (int y = 0; y < _cells.GetLength(1); y++)
                    {
                        if (_cells[x, y] != null)
                            Destroy(_cells[x, y].gameObject);
                    }
                }

                _cells = null;
            }
        }

        private void OnCellChanged(ZipCurrentCell cell, List<ZipCurrentCell> lineCells)
        {
            _cells[cell.Position.x, cell.Position.y].UpdateCell(cell, lineCells);
            if (cell.PreviousCell == null) return;
            _cells[cell.PreviousCell.Position.x, cell.PreviousCell.Position.y].UpdateCell(cell.PreviousCell, lineCells);
        }

        private void OnCellClicked(ZipDefaultCell cell, bool canGoBack)
        {
            _board.TryMoveToPoint(cell.Position, canGoBack);
        }
    }
}