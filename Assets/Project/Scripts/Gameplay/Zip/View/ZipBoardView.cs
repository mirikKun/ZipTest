using System;
using Project.Scripts.Gameplay.Zip.Board;
using Project.Scripts.Gameplay.Zip.Configs;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.View
{
    public class ZipBoardView:MonoBehaviour
    {
        [SerializeField] private ZipBoardConfig _config;
        [SerializeField] private Grid _grid;
        [SerializeField] private Vector2 _cellSize;
        [SerializeField] private Vector2 _offset;

        [Header("Prefabs")]

        [SerializeField] private ZipCellView _cellPrefab;


        private ZipCellView[,] _cells;
        private ZipBoard _board;
        private void Start()
        {
            CreateBoard();
        }

        private void CreateBoard()
        {
            _board = new ZipBoard(_config);
            _board.OnCellChanged += OnCellChanged;
            _grid.cellSize = _cellSize;
            

            _cells = new ZipCellView[_config.Width, _config.Height];

            for (int x = 0; x < _config.Width; x++)
            {
                for (int y = 0; y < _config.Height; y++)
                {
                    Vector3 centerOffset = (_config.Size-Vector2Int.one) * _cellSize/2;
                    Vector3 position = _grid.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(_offset.x, _offset.y, 0)-centerOffset;
                    ZipCellView cell = Instantiate(_cellPrefab, position, Quaternion.identity, transform);
                    cell.InitCell(_board.DefaultCells[x,y],_cellSize);
                    _cells[x, y] = cell;
                    _cells[x, y].OnCellClicked += OnCellClicked;
                }
            }
        }

        private void OnCellChanged(ZipCurrentCell cell)
        {
            _cells[cell.Position.x, cell.Position.y].UpdateCell(cell);
        }

        private void OnCellClicked(ZipDefaultCell cell)
        {
            _board.TryMoveToPoint(cell.Position);
        }
    }
}