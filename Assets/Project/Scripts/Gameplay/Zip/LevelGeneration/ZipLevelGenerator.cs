using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Gameplay.Zip.Configs;
using UnityEngine;


namespace Project.Scripts.Gameplay.Zip.LevelGeneration
{
    [Serializable]
    public class LevelData
    {
        public Vector2Int Size;
        public Vector2Int[] CheckpointPositions;
        public ZipCellWallsConfig[] Walls;
        public float OrientedTimeToFinish;
    }

    public class ZipLevelGenerator
    {
        [Serializable]
        public class GenerationSettings
        {
            [Header("Board Size")]
            public bool ForceSquareBoard = true; // If true, board is always square
            public Vector2Int MinSize = new Vector2Int(4, 4);
            public Vector2Int MaxSize = new Vector2Int(8, 8);
            
            [Header("Checkpoints")]
            public int MinCheckpoints = 2;
            public int MaxCheckpoints = 5;
            public bool UseRelativeCheckpointCount = true; // If true, checkpoint count depends on board size
            [Range(0.05f, 0.8f)] public float MinCheckpointDensity = 0.4f; // Minimum percentage of total cells
            [Range(0.05f, 0.8f)] public float MaxCheckpointDensity = 0.6f; // Maximum percentage of total cells
            
            [Header("Checkpoint Placement")]
            public bool RandomCheckpointPlacement = true; // If true, checkpoints are placed randomly (except first and last)
            
            [Header("Time Settings")]
            [Range(0.1f, 2.0f)] public float TimePerCell = 0.4f; // Time multiplier per cell for expected completion time
        }

        private GenerationSettings _settings;
        private System.Random _random;

        public ZipLevelGenerator(GenerationSettings settings = null)
        {
            _settings = settings ?? new GenerationSettings();
            _random = new System.Random();
        }

        public ZipLevelGenerator(int seed, GenerationSettings settings = null)
        {
            _settings = settings ?? new GenerationSettings();
            _random = new System.Random(seed);
        }

        /// <summary>
        /// Generates data for a new Zip level
        /// </summary>
        public LevelData GenerateLevelData()
        {
            Vector2Int size = GenerateBoardSize();
            
            // First generate a path that covers the entire board
            // Try multiple times with different starting positions
            List<Vector2Int> path = null;
            int maxAttempts = 5;
            
            for (int attempt = 0; attempt < maxAttempts && path == null; attempt++)
            {
                path = GenerateHamiltonianPath(size);
            }
            
            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("Failed to generate Hamiltonian path, using fallback");
                path = GenerateFallbackPath(size);
            }
            
            // Then place checkpoints along the path
            int checkpointCount = GenerateCheckpointCount(size);
            Vector2Int[] checkpointPositions = PlaceCheckpointsAlongPath(path, checkpointCount);
            
            // Generate expected time: total cells * time per cell
            int totalCells = size.x * size.y;
            float orientedTimeToFinish = totalCells * _settings.TimePerCell;
            
            return new LevelData
            {
                Size = size,
                CheckpointPositions = checkpointPositions,
                Walls = new ZipCellWallsConfig[0], // No walls for now
                OrientedTimeToFinish=orientedTimeToFinish
            };
        }
        
        /// <summary>
        /// Generates a new Zip level (for runtime use)
        /// Note: For Editor use GenerateLevelData and create ScriptableObject via SerializedObject
        /// </summary>
        public ZipBoardConfig GenerateLevel()
        {
            LevelData data = GenerateLevelData();
            ZipBoardConfig config = ScriptableObject.CreateInstance<ZipBoardConfig>();
            
            // Use reflection to set private fields via SerializeField
            SetPrivateField(config, "Size", data.Size);
            SetPrivateField(config, "CheckpointPositions", data.CheckpointPositions);
            SetPrivateField(config, "Walls", data.Walls);
            
            return config;
        }

        private Vector2Int GenerateBoardSize()
        {
            if (_settings.ForceSquareBoard)
            {
                int size = _random.Next(
                    Mathf.Max(_settings.MinSize.x, _settings.MinSize.y),
                    Mathf.Min(_settings.MaxSize.x, _settings.MaxSize.y) + 1);
                return new Vector2Int(size, size);
            }
            else
            {
                int width = _random.Next(_settings.MinSize.x, _settings.MaxSize.x + 1);
                int height = _random.Next(_settings.MinSize.y, _settings.MaxSize.y + 1);
                return new Vector2Int(width, height);
            }
        }

        private int GenerateCheckpointCount(Vector2Int size)
        {
            int totalCells = size.x * size.y;
            
            if (_settings.UseRelativeCheckpointCount)
            {
                // Choose random density in range from Min to Max
                float density = (float)_random.NextDouble() * 
                    (_settings.MaxCheckpointDensity - _settings.MinCheckpointDensity) + 
                    _settings.MinCheckpointDensity;
                
                int relativeCount = Mathf.RoundToInt(totalCells * density);
                int min = Mathf.Max(_settings.MinCheckpoints, 2);
                int max = Mathf.Min(_settings.MaxCheckpoints, totalCells - 1); // No more than cells minus 1
                return Mathf.Clamp(relativeCount, min, max);
            }
            else
            {
                int min = Mathf.Max(_settings.MinCheckpoints, 2);
                int max = Mathf.Min(_settings.MaxCheckpoints, totalCells - 1);
                return _random.Next(min, max + 1);
            }
        }

        /// <summary>
        /// Generates a Hamiltonian path (path through all cells exactly once)
        /// Uses Warnsdorff's rule for fast generation
        /// </summary>
        private List<Vector2Int> GenerateHamiltonianPath(Vector2Int size)
        {
            int totalCells = size.x * size.y;
            List<Vector2Int> path = new List<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            
            // Start from a random position
            Vector2Int current = new Vector2Int(
                _random.Next(0, size.x),
                _random.Next(0, size.y)
            );
            
            // Use Warnsdorff's rule: choose next cell with fewest unvisited neighbors
            for (int i = 0; i < totalCells; i++)
            {
                path.Add(current);
                visited.Add(current);
                
                if (i == totalCells - 1)
                    break; // All cells visited
                
                // Get unvisited neighbors with their "degree" (number of unvisited neighbors)
                List<(Vector2Int pos, int degree)> candidates = new List<(Vector2Int, int)>();
                List<Vector2Int> neighbors = GetUnvisitedNeighbors(current, size, visited);
                
                foreach (var neighbor in neighbors)
                {
                    int degree = GetUnvisitedNeighbors(neighbor, size, visited).Count;
                    candidates.Add((neighbor, degree));
                }
                
                if (candidates.Count == 0)
                {
                    // If no unvisited neighbors but not all cells visited - path is impossible
                    return null;
                }
                
                // Sort by degree (lower degree = better)
                // If multiple with same degree, choose randomly
                candidates = candidates.OrderBy(c => c.degree)
                    .ThenBy(_ => _random.Next())
                    .ToList();
                
                current = candidates[0].pos;
            }
            
            return path;
        }
        
        /// <summary>
        /// Gets unvisited neighboring cells
        /// </summary>
        private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int pos, Vector2Int size, HashSet<Vector2Int> visited)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            Vector2Int[] directions =
            {
                new Vector2Int(0, 1),  // Up
                new Vector2Int(0, -1), // Down
                new Vector2Int(1, 0),  // Right
                new Vector2Int(-1, 0)  // Left
            };
            
            foreach (var dir in directions)
            {
                Vector2Int neighbor = pos + dir;
                if (neighbor.x >= 0 && neighbor.x < size.x &&
                    neighbor.y >= 0 && neighbor.y < size.y &&
                    !visited.Contains(neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }
            
            return neighbors;
        }
        
        /// <summary>
        /// Fallback path generation method (snake pattern) if Hamiltonian path not found
        /// </summary>
        private List<Vector2Int> GenerateFallbackPath(Vector2Int size)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            
            for (int y = 0; y < size.y; y++)
            {
                if (y % 2 == 0)
                {
                    for (int x = 0; x < size.x; x++)
                    {
                        path.Add(new Vector2Int(x, y));
                    }
                }
                else
                {
                    for (int x = size.x - 1; x >= 0; x--)
                    {
                        path.Add(new Vector2Int(x, y));
                    }
                }
            }
            
            return path;
        }
        
        /// <summary>
        /// Places checkpoints along the path
        /// First checkpoint at path start, last checkpoint at path end
        /// </summary>
        private Vector2Int[] PlaceCheckpointsAlongPath(List<Vector2Int> path, int checkpointCount)
        {
            if (path == null || path.Count == 0)
            {
                Debug.LogError("Path is empty, cannot place checkpoints");
                return new[] { new Vector2Int(0, 0) };
            }
            
            if (checkpointCount < 2)
            {
                checkpointCount = 2;
            }
            
            if (checkpointCount > path.Count)
            {
                checkpointCount = path.Count;
            }
            
            List<Vector2Int> checkpointPositions = new List<Vector2Int>();
            
            // First checkpoint always at path start
            checkpointPositions.Add(path[0]);
            
            // If more than 2 checkpoints needed, place the rest
            if (checkpointCount > 2)
            {
                if (_settings.RandomCheckpointPlacement)
                {
                    // Random placement: random indices between first and last
                    int remainingCheckpoints = checkpointCount - 2; // Minus first and last
                    
                    // Available indices for placement (exclude first and last)
                    List<int> availableIndices = new List<int>();
                    for (int i = 1; i < path.Count - 1; i++)
                    {
                        availableIndices.Add(i);
                    }
                    
                    // Shuffle available indices
                    availableIndices = availableIndices.OrderBy(_ => _random.Next()).ToList();
                    
                    // Select random indices
                    for (int i = 0; i < remainingCheckpoints && i < availableIndices.Count; i++)
                    {
                        checkpointPositions.Add(path[availableIndices[i]]);
                    }
                }
                else
                {
                    // Even placement
                    float step = (float)(path.Count - 1) / (checkpointCount - 1);
                    
                    for (int i = 1; i < checkpointCount - 1; i++)
                    {
                        int index = Mathf.RoundToInt(i * step);
                        index = Mathf.Clamp(index, 1, path.Count - 2); // Don't include first and last
                        
                        // Make sure we don't duplicate positions
                        if (!checkpointPositions.Contains(path[index]))
                        {
                            checkpointPositions.Add(path[index]);
                        }
                        else
                        {
                            // Find nearest free position
                            for (int offset = 1; offset < path.Count; offset++)
                            {
                                int nextIndex = Mathf.Clamp(index + offset, 0, path.Count - 1);
                                if (!checkpointPositions.Contains(path[nextIndex]))
                                {
                                    checkpointPositions.Add(path[nextIndex]);
                                    break;
                                }
                                
                                int prevIndex = Mathf.Clamp(index - offset, 0, path.Count - 1);
                                if (!checkpointPositions.Contains(path[prevIndex]))
                                {
                                    checkpointPositions.Add(path[prevIndex]);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            
            // Last checkpoint always at path end
            Vector2Int lastCheckpoint = path[path.Count - 1];
            if (!checkpointPositions.Contains(lastCheckpoint))
            {
                checkpointPositions.Add(lastCheckpoint);
            }
            
            // Sort checkpoints by order in path
            checkpointPositions = checkpointPositions.OrderBy(cp => path.IndexOf(cp)).ToList();
            
            return checkpointPositions.ToArray();
        }


        private void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName, 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                field.SetValue(obj, value);
            }
            else
            {
                // Try via property with SerializeField
                var property = obj.GetType().GetProperty(fieldName);
                if (property != null && property.CanWrite)
                {
                    property.SetValue(obj, value);
                }
            }
        }
    }
}

