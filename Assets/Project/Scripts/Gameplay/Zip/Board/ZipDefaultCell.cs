using Project.Scripts.Gameplay.Zip.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.Board
{
    public class ZipDefaultCell
    {
        public Vector2Int Position { get;private set; }
        public int Index{get;private set;}
        public ZipCellType Type { get; private set; }
        public bool HaveRightWall { get; private set; }
        public bool HaveDownWall { get; private set; }
        
        public ZipDefaultCell(ZipCellType zipCellType,Vector2Int position,int index=0,bool haveRightWall=false,bool haveDownWall=false)
        {
            Position = position;
            Type=zipCellType;
            Index = index;
            HaveRightWall = haveRightWall;
            HaveDownWall = haveDownWall;
            
        }
    }
}