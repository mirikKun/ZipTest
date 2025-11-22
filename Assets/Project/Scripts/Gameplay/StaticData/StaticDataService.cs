using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Gameplay.Windows;
using Project.Scripts.Gameplay.Windows.Configs;
using Project.Scripts.Gameplay.Zip.Configs;
using UnityEngine;

namespace Project.Scripts.Gameplay.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private Dictionary<WindowId, GameObject> _windowPrefabsById;
        private ZipBoardConfigsList _zipBoardConfigsList;

        public void LoadAll()
        {
            LoadWindows();
            LoadZipBoardConfigsList();
        }

        private void LoadZipBoardConfigsList()
        {
            _zipBoardConfigsList = Resources.Load<ZipBoardConfigsList>("Configs/Zip/ZipBoardConfigsList");
        }


        public GameObject GetWindowPrefab(WindowId id) =>
            _windowPrefabsById.TryGetValue(id, out GameObject prefab)
                ? prefab
                : throw new Exception($"Prefab config for window {id} was not found");

        public ZipBoardConfigsList GetZipBoardConfigList()
        {
            return _zipBoardConfigsList;
        }

        private void LoadWindows()
        {
            _windowPrefabsById = Resources
                .Load<WindowsConfig>("Configs/Windows/WindowsConfig")
                .WindowConfigs
                .ToDictionary(x => x.Id, x => x.Prefab);
        }
    }
}