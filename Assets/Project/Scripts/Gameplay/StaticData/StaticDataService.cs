using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Gameplay.Windows;
using Project.Scripts.Gameplay.Windows.Configs;
using UnityEngine;

namespace Project.Scripts.Gameplay.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private Dictionary<WindowId, GameObject> _windowPrefabsById;

        public void LoadAll()
        {
            LoadWindows();
        }


        public GameObject GetWindowPrefab(WindowId id) =>
            _windowPrefabsById.TryGetValue(id, out GameObject prefab)
                ? prefab
                : throw new Exception($"Prefab config for window {id} was not found");

        private void LoadWindows()
        {
            _windowPrefabsById = Resources
                .Load<WindowsConfig>("Configs/Windows/windowsConfig")
                .WindowConfigs
                .ToDictionary(x => x.Id, x => x.Prefab);
        }
    }
}