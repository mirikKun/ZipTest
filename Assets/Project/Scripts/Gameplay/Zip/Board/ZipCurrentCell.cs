using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.Board
{
    public class ZipCurrentCell
    {
        public int StepIndex { get; private set; }
        public Vector2Int Position { get;private set; }
        public ZipCurrentCell(Vector2Int position)
        {
            Position = position;
            StepIndex = -1;
        }
        public void SetStepIndex(int stepIndex)
        {
            StepIndex = stepIndex;
        }
    }
}