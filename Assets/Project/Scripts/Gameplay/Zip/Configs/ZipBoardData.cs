using Project.Scripts.Gameplay.Zip.Board;
using Project.Scripts.Gameplay.Zip.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.Configs
{
    public class ZipBoardData
    {
        public Vector2Int Size { get; }
        public Vector2Int[] CheckpointPositions { get; }
        public ZipCellWallsConfig[] Walls { get; }
        public float OrientedTimeToFinish { get; }

        public int Width => Size.x;
        public int Height => Size.y;
        public int MaxSize => Mathf.Max(Size.y, Size.x);

        public ZipBoardData(Vector2Int size, Vector2Int[] checkpointPositions, ZipCellWallsConfig[] walls, float orientedTimeToFinish)
        {
            Size = size;
            CheckpointPositions = checkpointPositions ?? new Vector2Int[0];
            Walls = walls ?? new ZipCellWallsConfig[0];
            OrientedTimeToFinish = orientedTimeToFinish;
        }

        public ZipDefaultCell[,] GetCells()
        {
            ZipDefaultCell[,] cells = new ZipDefaultCell[Size.x, Size.y];
            for (var x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    cells[x, y] = new ZipDefaultCell(ZipCellType.Empty, position, 
                        haveRightWall: HaveRightWall(position), 
                        haveDownWall: HaveDownWall(position));
                }
            }

            for (var i = 0; i < CheckpointPositions.Length; i++)
            {
                Vector2Int checkpointPosition = CheckpointPositions[i];
                cells[checkpointPosition.x, checkpointPosition.y] = new ZipDefaultCell(
                    ZipCellType.Checkpoint, checkpointPosition, i,
                    haveRightWall: HaveRightWall(checkpointPosition),
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
}

