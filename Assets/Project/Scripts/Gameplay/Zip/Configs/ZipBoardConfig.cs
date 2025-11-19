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
        [field: SerializeField] public Vector2Int[] CheckpointIndexes { get; private set; }
        [field: SerializeField] public ZipCellWallsConfig[] Walls { get; private set; }
        [field: SerializeField] public Vector2Int StartPosition { get; private set; }

        public int Width => Size.x;
        public int Height => Size.y;

        public ZipDefaultCell[,] GetCells()
        {
            ZipDefaultCell[,] cells = new ZipDefaultCell[Size.x, Size.y];
            for (var x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    cells[x, y] = new ZipDefaultCell(ZipCellType.Empty, position,haveRightWall:HaveRightWall(position),haveDownWall:HaveDownWall(position));
                }
            }

            for (var i = 0; i < CheckpointIndexes.Length; i++)
            {
                Vector2Int checkpointIndex = CheckpointIndexes[i];
                cells[checkpointIndex.x, checkpointIndex.y] = new ZipDefaultCell(ZipCellType.Checkpoint, checkpointIndex, i,haveRightWall:HaveRightWall(StartPosition),haveDownWall:HaveDownWall(StartPosition));
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
        public Vector2Int Position { get; private set; }
        public bool HaveRightWall { get; private set; }
        public bool HaveDownWall { get; private set; }
    }
}