using System;
using System.Collections.Generic;
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
        private Vector2Int _startPoint;
        private Vector2Int _currentPosition;

        private bool _isFinished;
        private bool _isOrderCorrect;

        public event Action BoardFinished;
        public event Action OrderBecameWrong;
        public event Action OrderBecameCorrect;

        public void CreateBoard(Vector2Int size, Vector2Int startPoint, ZipDefaultCell[,] cells)
        {
            _startPoint = startPoint;
            _currentPosition = startPoint;
            _isOrderCorrect = true;
            _size = size;
            _defaultCells = cells;
            InitializeZipCurrentCells(size, startPoint);
        }

        private void InitializeZipCurrentCells(Vector2Int size, Vector2Int startPoint)
        {
            _zipCurrentCells = new ZipCurrentCell[size.x, size.y];
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    _zipCurrentCells[x, y] = new ZipCurrentCell(new Vector2Int(size.x, size.y));
                }
            }

            _zipCurrentCells[startPoint.x, startPoint.y].SetStepIndex(0);
        }

        public void Move(Direction direction, int steps = 1)
        {
            for (int i = 0; i < steps; i++)
            {
                Vector2Int nextPosition = _currentPosition + DirectionToVector2Int(direction);
                if (!IsValidPosition(nextPosition) || HaveWall(_currentPosition, direction)) break;
                if (CanMoveForward(nextPosition))
                {
                    MoveForward(nextPosition);
                }
                else if (CanMoveBackward(_currentPosition, nextPosition))
                {
                    MoveBackwards(nextPosition);
                }
            }

            if (_zipCurrentCells[_currentPosition.x, _currentPosition.y].StepIndex == CellsCount&&_isOrderCorrect)
            {
                _isFinished = true;
                BoardFinished?.Invoke();
            }
        }

        private int CellsCount => _size.x*_size.y-1;

        private void MoveBackwards(Vector2Int nextPosition)
        {
            if (_defaultCells[_currentPosition.x, _currentPosition.y].Type == ZipCellType.Checkpoint)
            {
                RemoveCheckpoint(_defaultCells[_currentPosition.x, _currentPosition.y]);
            }

            _zipCurrentCells[_currentPosition.x, _currentPosition.y].SetStepIndex(-1);
            _currentPosition = nextPosition;
        }

        private void MoveForward(Vector2Int nextPosition)
        {
            _zipCurrentCells[nextPosition.x, nextPosition.y].SetStepIndex(
                _zipCurrentCells[_currentPosition.x, _currentPosition.y].StepIndex + 1);
            _currentPosition = nextPosition;

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
                }

                _isOrderCorrect = false;
            }

            _checkpoints.Add(cell);
        }

        private void RemoveCheckpoint(ZipDefaultCell cell)
        {
            if (_checkpoints.Contains(cell))
            {
                _checkpoints.Remove(cell);

                _isOrderCorrect = true;
                for (int i = 1; i < _checkpoints.Count; i++)
                {
                    if (_checkpoints[i].Index != _checkpoints[i - 1].Index + 1)
                    {
                        _isOrderCorrect = false;
                        break;
                    }
                }

                if (_isOrderCorrect)
                {
                    OrderBecameCorrect?.Invoke();
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
                   position.y >= 0 && position.y < _size.y;
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
            return _zipCurrentCells[currentPosition.x, currentPosition.y].StepIndex ==
                   _zipCurrentCells[previousPosition.x, previousPosition.y].StepIndex - 1;
        }
    }
}