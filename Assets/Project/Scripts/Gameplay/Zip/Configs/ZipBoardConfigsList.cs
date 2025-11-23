using Project.Scripts.Gameplay.Zip.LevelGeneration;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.Configs
{
    [CreateAssetMenu(fileName = "ZipBoardConfigsList", menuName = "Configs/Zip/ZipBoardConfigsList", order = 0)]
    public class ZipBoardConfigsList : ScriptableObject
    {
        [field: SerializeField] public ZipBoardConfig[] Configs { get; private set; }
        [field: SerializeField] public ZipLevelGeneratorSettings GeneratorSettings { get; private set; }
        

        public ZipBoardData GetLevelDataByIndex(int index)
        {
            if (Configs != null && Configs.Length > 0 && index < Configs.Length)
            {
                var config = Configs[index];
                return new ZipBoardData(config.Size, config.CheckpointPositions, config.Walls,config.OrientedTimeToFinish);
            }
            
            if (GeneratorSettings != null)
            {
                int seed = GeneratorSettings.Seed == -1 
                    ? System.Environment.TickCount + index 
                    : GeneratorSettings.Seed + index;
                
                ZipLevelGenerator generator = new ZipLevelGenerator(seed, GeneratorSettings.GenerationSettings);
                LevelData levelData = generator.GenerateLevelData();
                return new ZipBoardData(levelData.Size, levelData.CheckpointPositions, levelData.Walls,levelData.OrientedTimeToFinish);
            }
            
            Debug.LogError("No configs available and no generator settings set!");
            return null;
        }

        public ZipBoardData GetRandomGeneratedLevelData()
        {
            if (GeneratorSettings != null)
            {
                int seed = System.Environment.TickCount;
                
                ZipLevelGenerator generator = new ZipLevelGenerator(seed, GeneratorSettings.GenerationSettings);
                LevelData levelData = generator.GenerateLevelData();
                return new ZipBoardData(levelData.Size, levelData.CheckpointPositions, levelData.Walls,levelData.OrientedTimeToFinish);
            }
            Debug.LogError("No configs available and no generator settings set!");
            return null;
        }
        
    }
}