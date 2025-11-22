using Project.Scripts.Gameplay.Windows;
using Project.Scripts.Gameplay.Zip.Configs;
using UnityEngine;

namespace Project.Scripts.Gameplay.StaticData
{
    public interface IStaticDataService
    {
        void LoadAll();
        GameObject GetWindowPrefab(WindowId id);
        ZipBoardConfigsList GetZipBoardConfigList();
    }
}