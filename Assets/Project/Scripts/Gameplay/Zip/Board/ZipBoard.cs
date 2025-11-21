using System;
using System.Collections.Generic;
using Project.Scripts.Gameplay.Zip.Configs;
using Project.Scripts.Gameplay.Zip.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.Board
{
    public class ZipBoard
    {
        private Vector2Int _size;
        private ZipDefaultCell[,] _defaultCells;
        private ZipCurrentCell[,] _zipCurrentCells;
        private List<ZipDefaultCell> _checkpoints = new List<ZipDefaultCell>();
        private List<ZipCurrentCell> _lineCells = new List<ZipCurrentCell>();
        private Vector2Int _startPoint;
        private Vector2Int _currentPosition;

        private bool _isFinished;
        private bool _isOrderCorrect;
        public ZipDefaultCell[,] DefaultCells => _defaultCells;
        public List<ZipCurrentCell> LineCells => _lineCells;
        public ZipCurrentCell[,] ZipCurrentCells => _zipCurrentCells;

        public event Action<ZipCurrentCell, List<ZipCurrentCell>> OnCellChanged;
        public event Action BoardFinished;
        public event Action<ZipCurrentCell> CheckpointReached;
        public event Action OrderBecameWrong;
        public event Action OrderBecameCorrect;

        public ZipBoard(ZipBoardConfig config)
        {
            _startPoint = config.CheckpointPositions[0];
            _currentPosition = _startPoint;
            _isOrderCorrect = true;
            _size = config.Size;
            _defaultCells = config.GetCells();
            InitializeZipCurrentCells(_size, _startPoint);
            _lineCells.Add(_zipCurrentCells[_startPoint.x, _startPoint.y]);
            AddCheckpoint(_defaultCells[_startPoint.x, _startPoint.y]);
        }


        public ZipBoard(Vector2Int size, Vector2Int startPoint, ZipDefaultCell[,] cells)
        {
            _startPoint = startPoint;
            _currentPosition = startPoint;
            _isOrderCorrect = true;
            _size = size;
            _defaultCells = cells;
            InitializeZipCurrentCells(_size, _startPoint);
            _lineCells.Add(_zipCurrentCells[startPoint.x, startPoint.y]);
        }

        private void InitializeZipCurrentCells(Vector2Int size, Vector2Int startPoint)
        {
            _zipCurrentCells = new ZipCurrentCell[size.x, size.y];
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    _zipCurrentCells[x, y] = new ZipCurrentCell(new Vector2Int(x, y));
                }
            }

            _zipCurrentCells[startPoint.x, startPoint.y].SetStepIndex(0);
        }

        public void TryMoveToPoint(Vector2Int point, bool canGoBack = true)
        {
            if (!IsValidPosition(point)||_isFinished) return;

            if (_zipCurrentCells[point.x, point.y].StepIndex >= 0 && canGoBack)
            {
                MoveBackwardsUntilPoint(point);
            }
            else if (point.x == _currentPosition.x)
            {
                int steps = Mathf.Abs(point.y - _currentPosition.y);
                Direction direction = point.y > _currentPosition.y ? Direction.Up : Direction.Down;
                Move(direction, steps);
            }
            else if (point.y == _currentPosition.y)
            {
                int steps = Mathf.Abs(point.x - _currentPosition.x);
                Direction direction = point.x > _currentPosition.x ? Direction.Right : Direction.Left;
                Move(direction, steps);
            }
        }

        public void Move(Direction direction, int steps = 1, bool canGoBack = true)
        {
            //Debug.Log($"Move {direction} {steps}");

            if (steps <= 0) return;
            for (int i = 0; i < steps; i++)
            {
                Vector2Int nextPosition = _currentPosition + DirectionToVector2Int(direction);
                if (!IsValidPosition(nextPosition) || HaveWall(_currentPosition, direction)) break;
                if (CanMoveForward(nextPosition))
                {
                    MoveForward(nextPosition);
                }
                else if (canGoBack && CanMoveBackward(_currentPosition, nextPosition))
                {
                    MoveBackwards(nextPosition);
                }
            }

            if (_zipCurrentCells[_currentPosition.x, _currentPosition.y].StepIndex == CellsCount && _isOrderCorrect)
            {
                _isFinished = true;
                BoardFinished?.Invoke();
                Debug.Log("Finished");
            }
        }

        private int CellsCount => _size.x * _size.y - 1;

        private void MoveBackwards(Vector2Int nextPosition)
        {
            if (_defaultCells[_currentPosition.x, _currentPosition.y].Type == ZipCellType.Checkpoint)
            {
                RemoveCheckpoint(_defaultCells[_currentPosition.x, _currentPosition.y]);
            }

            _zipCurrentCells[_currentPosition.x, _currentPosition.y].SetStepIndex(-1);
            _lineCells.Remove(_zipCurrentCells[_currentPosition.x, _currentPosition.y]);
            OnCellChanged?.Invoke(_zipCurrentCells[_currentPosition.x, _currentPosition.y], _lineCells);
            _currentPosition = nextPosition;
            //_zipCurrentCells[_currentPosition.x, _currentPosition.y].SetPreviousCell(null);
        }

        private void MoveBackwardsUntilPoint(Vector2Int point)
        {
            while (_currentPosition != point && _lineCells.Count > 1 && _currentPosition != _startPoint)
            {
                MoveBackwards(_lineCells[^2].Position);
            }

            OnCellChanged?.Invoke(_zipCurrentCells[_currentPosition.x, _currentPosition.y], _lineCells);
        }

        private void MoveForward(Vector2Int nextPosition)
        {
            _zipCurrentCells[nextPosition.x, nextPosition.y].SetStepIndex(
                _zipCurrentCells[_currentPosition.x, _currentPosition.y].StepIndex + 1);

            _zipCurrentCells[nextPosition.x, nextPosition.y].SetPreviousCell(
                _zipCurrentCells[_currentPosition.x, _currentPosition.y]);

            _currentPosition = nextPosition;
            _lineCells.Add(_zipCurrentCells[_currentPosition.x, _currentPosition.y]);

            OnCellChanged?.Invoke(_zipCurrentCells[_currentPosition.x, _currentPosition.y], _lineCells);

            if (_defaultCells[_currentPosition.x, _currentPosition.y].Type == ZipCellType.Checkpoint)
            {
                AddCheckpoint(_defaultCells[_currentPosition.x, _currentPosition.y]);
            }
        }

        private void AddCheckpoint(ZipDefaultCell cell)
        {
            if (_checkpoints.Count != 0 && _checkpoints[^1].Index != cell.Index - 1)
            {
                if (_isOrderCorrect)
                {
                    OrderBecameWrong?.Invoke();
                    Debug.Log("Wrong order");
                }

                _isOrderCorrect = false;
            }

            _checkpoints.Add(cell);
            if (_isOrderCorrect)
            {
                CheckpointReached?.Invoke(_zipCurrentCells[cell.Position.x, cell.Position.y]);
            }
        }

        private void RemoveCheckpoint(ZipDefaultCell cell)
        {
            if (_checkpoints.Contains(cell))
            {
                _checkpoints.Remove(cell);

                bool isOrderCorrect = true;
                for (int i = 1; i < _checkpoints.Count; i++)
                {
                    if (_checkpoints[i].Index != _checkpoints[i - 1].Index + 1)
                    {
                        isOrderCorrect = false;
                        break;
                    }
                }

                if (!_isOrderCorrect && isOrderCorrect)
                {
                    OrderBecameCorrect?.Invoke();
                    _isOrderCorrect = true;
                    Debug.Log("Correct order");
                }
            }
        }


        private Vector2Int DirectionToVector2Int(Direction direction)
        {
            return direction switch
            {
                Direction.Up => Vector2Int.up,
                Direction.Down => Vector2Int.down,
                Direction.Left => Vector2Int.left,
                Direction.Right => Vector2Int.right,
                _ => Vector2Int.zero
            };
        }

        private bool IsValidPosition(Vector2Int position)
        {
            return position.x >= 0 && position.x < _size.x &&
                   position.y >= 0 && position.y < _size.y &&position!=_currentPosition;
        }

        private bool HaveWall(Vector2Int currentPosition, Direction direction)
        {
            return direction switch
            {
                Direction.Up => _defaultCells[currentPosition.x, currentPosition.y + 1].HaveDownWall,
                Direction.Down => _defaultCells[currentPosition.x, currentPosition.y].HaveDownWall,
                Direction.Left => _defaultCells[currentPosition.x - 1, currentPosition.y].HaveRightWall,
                Direction.Right => _defaultCells[currentPosition.x, currentPosition.y].HaveRightWall,
                _ => false
            };
        }

        private bool CanMoveForward(Vector2Int newPosition)
        {
            return _zipCurrentCells[newPosition.x, newPosition.y].StepIndex == -1;
        }

        private bool CanMoveBackward(Vector2Int currentPosition, Vector2Int previousPosition)
        {
            return _zipCurrentCells[previousPosition.x, previousPosition.y].StepIndex >= 0 &&
                   _zipCurrentCells[currentPosition.x, currentPosition.y].StepIndex ==
                   _zipCurrentCells[previousPosition.x, previousPosition.y].StepIndex +1;
        }
    }
}