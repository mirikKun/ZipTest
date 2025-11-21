using Project.Scripts.Gameplay.Zip.LevelGeneration;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.Configs
{
    [CreateAssetMenu(fileName = "ZipLevelGeneratorSettings", menuName = "Configs/Zip/ZipLevelGeneratorSettings", order = 1)]
    public class ZipLevelGeneratorSettings : ScriptableObject
    {
        [SerializeField] private ZipLevelGenerator.GenerationSettings _generationSettings = new ZipLevelGenerator.GenerationSettings();
        [SerializeField] private int _seed = -1; // -1 means random seed

        public ZipLevelGenerator.GenerationSettings GenerationSettings => _generationSettings;
        public int Seed => _seed;

        /// <summary>
        /// Generates a new level using the configured settings
        /// </summary>
        public ZipBoardData GenerateLevel()
        {
            ZipLevelGenerator generator = _seed == -1 
                ? new ZipLevelGenerator(_generationSettings)
                : new ZipLevelGenerator(_seed, _generationSettings);

            var levelData = generator.GenerateLevelData();
            return new ZipBoardData(levelData.Size, levelData.CheckpointPositions, levelData.Walls);
        }
    }
}

