using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.Configs
{
    [CreateAssetMenu(fileName = "ZipBoardConfigsList", menuName = "Configs/Zip/ZipBoardConfigsList", order = 0)]
    public class ZipBoardConfigsList:ScriptableObject
    {
        [field:SerializeField] public ZipBoardConfig[] Configs { get; private set; }
        
        public ZipBoardConfig GetConfigByIndex(int index) => Configs[index%Configs.Length];
    }
}