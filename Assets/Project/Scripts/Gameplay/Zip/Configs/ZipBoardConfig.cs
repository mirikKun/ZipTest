using System;
using Project.Scripts.Gameplay.Zip.Board;
using Project.Scripts.Gameplay.Zip.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.Configs
{
    [CreateAssetMenu(fileName = "ZipBoardConfig", menuName = "Configs/Zip/ZipBoardConfig", order = 0)]
    public class ZipBoardConfig : ScriptableObject
    {
        [field: SerializeField] public Vector2Int Size { get; private set; }
        [field: SerializeField] public Vector2Int[] CheckpointPositions { get; private set; }
        [field: SerializeField] public ZipCellWallsConfig[] Walls { get; private set; }
        [field: SerializeField] public float OrientedTimeToFinish { get; private set; } = 10;

        public int Width => Size.x;
        public int Height => Size.y;
        public int MaxSize => Mathf.Max(Size.y, Size.x);

        public ZipDefaultCell[,] GetCells()
        {
            ZipDefaultCell[,] cells = new ZipDefaultCell[Size.x, Size.y];
            for (var x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    cells[x, y] = new ZipDefaultCell(ZipCellType.Empty, position,
                        haveRightWall: HaveRightWall(position), haveDownWall: HaveDownWall(position));
                }
            }

            for (var i = 0; i < CheckpointPositions.Length; i++)
            {
                Vector2Int checkpointPosition = CheckpointPositions[i];
                cells[checkpointPosition.x, checkpointPosition.y] = new ZipDefaultCell(ZipCellType.Checkpoint,
                    checkpointPosition, i, haveRightWall: HaveRightWall(checkpointPosition),
                    haveDownWall: HaveDownWall(checkpointPosition));
            }


            return cells;
        }

        private bool HaveRightWall(Vector2Int position)
        {
            foreach (var wall in Walls)
            {
                if (wall.Position == position) return wall.HaveRightWall;
            }

            return false;
        }

        private bool HaveDownWall(Vector2Int position)
        {
            foreach (var wall in Walls)
            {
                if (wall.Position == position) return wall.HaveDownWall;
            }

            return false;
        }
    }

    [Serializable]
    public class ZipCellWallsConfig
    {
        [SerializeField] private Vector2Int _position;
        [SerializeField] private bool _haveRightWall;
        [SerializeField] private bool _haveDownWall;

        public Vector2Int Position => _position;
        public bool HaveRightWall => _haveRightWall;
        public bool HaveDownWall => _haveDownWall;

        // Конструктор по умолчанию для сериализации Unity
        public ZipCellWallsConfig()
        {
        }

        public ZipCellWallsConfig(Vector2Int position, bool haveRightWall, bool haveDownWall)
        {
            _position = position;
            _haveRightWall = haveRightWall;
            _haveDownWall = haveDownWall;
        }
    }
}