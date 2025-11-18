using UnityEngine;

namespace Project.Scripts.Gameplay.Levels
{
    public interface ILevelDataProvider
    {
        Vector3 StartPoint { get; }
        void SetStartPoint(Vector3 startPoint);
    }
}