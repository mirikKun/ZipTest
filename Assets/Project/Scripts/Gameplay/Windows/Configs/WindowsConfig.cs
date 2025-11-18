using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Gameplay.Windows.Configs
{
    [CreateAssetMenu(fileName = "windowConfig", menuName = "ECS Survivors/Window Config")]
    public class WindowsConfig : ScriptableObject
    {
        public List<WindowConfig> WindowConfigs;
    }
}