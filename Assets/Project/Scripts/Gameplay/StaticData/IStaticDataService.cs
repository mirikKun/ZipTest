using Project.Scripts.Gameplay.Windows;
using UnityEngine;

namespace Project.Scripts.Gameplay.StaticData
{
    public interface IStaticDataService
    {
        void LoadAll();
        GameObject GetWindowPrefab(WindowId id);
    }
}