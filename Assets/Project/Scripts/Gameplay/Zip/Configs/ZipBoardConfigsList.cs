using Project.Scripts.Gameplay.Zip.LevelGeneration;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.Configs
{
    [CreateAssetMenu(fileName = "ZipBoardConfigsList", menuName = "Configs/Zip/ZipBoardConfigsList", order = 0)]
    public class ZipBoardConfigsList : ScriptableObject
    {
        [field: SerializeField] public ZipBoardConfig[] Configs { get; private set; }
        [field: SerializeField] public ZipLevelGeneratorSettings GeneratorSettings { get; private set; }
        
        /// <summary>
        /// Gets level data by index. If index exceeds available configs and GeneratorSettings is set,
        /// generates a new level using the generator settings.
        /// </summary>
        public ZipBoardData GetLevelDataByIndex(int index)
        {
            // If we have pre-made configs and index is within range, use them
            if (Configs != null && Configs.Length > 0 && index < Configs.Length)
            {
                var config = Configs[index];
                return new ZipBoardData(config.Size, config.CheckpointPositions, config.Walls);
            }
            
            // If we have generator settings, generate a new level
            if (GeneratorSettings != null)
            {
                // Use index as seed offset for variety when generating
                int seed = GeneratorSettings.Seed == -1 
                    ? System.Environment.TickCount + index 
                    : GeneratorSettings.Seed + index;
                
                var generator = new ZipLevelGenerator(seed, GeneratorSettings.GenerationSettings);
                var levelData = generator.GenerateLevelData();
                return new ZipBoardData(levelData.Size, levelData.CheckpointPositions, levelData.Walls);
            }
            
            // Fallback: if no configs and no generator, return null or throw
            Debug.LogError("No configs available and no generator settings set!");
            return null;
        }
        
        /// <summary>
        /// Legacy method for backward compatibility
        /// </summary>
        [System.Obsolete("Use GetLevelDataByIndex instead")]
        public ZipBoardConfig GetConfigByIndex(int index) => Configs[index % Configs.Length];
    }
}