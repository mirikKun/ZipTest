using System.IO;
using System.Reflection;
using Project.Scripts.Gameplay.Zip.Configs;
using Project.Scripts.Gameplay.Zip.LevelGeneration;
using UnityEditor;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.LevelGeneration.Editor
{
    public class ZipLevelGeneratorEditor : EditorWindow
    {
        private ZipLevelGenerator.GenerationSettings _settings = new ZipLevelGenerator.GenerationSettings();
        private int _seed = -1; // -1 means random seed
        private string _outputPath = "Assets/Project/Configs/Generated";
        private Vector2 _scrollPosition;

        [MenuItem("Tools/Zip/Level Generator")]
        public static void ShowWindow()
        {
            GetWindow<ZipLevelGeneratorEditor>("Zip Level Generator");
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            EditorGUILayout.LabelField("Zip Level Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Board size settings
            EditorGUILayout.LabelField("Board Size Settings", EditorStyles.boldLabel);
            _settings.ForceSquareBoard = EditorGUILayout.Toggle("Force Square Board", _settings.ForceSquareBoard);
            
            if (_settings.ForceSquareBoard)
            {
                int minSquareSize = EditorGUILayout.IntField("Min Size", Mathf.Max(_settings.MinSize.x, _settings.MinSize.y));
                int maxSquareSize = EditorGUILayout.IntField("Max Size", Mathf.Min(_settings.MaxSize.x, _settings.MaxSize.y));
                _settings.MinSize = new Vector2Int(minSquareSize, minSquareSize);
                _settings.MaxSize = new Vector2Int(maxSquareSize, maxSquareSize);
            }
            else
            {
                _settings.MinSize = EditorGUILayout.Vector2IntField("Min Size", _settings.MinSize);
                _settings.MaxSize = EditorGUILayout.Vector2IntField("Max Size", _settings.MaxSize);
            }
            
            EditorGUILayout.Space();
            
            // Checkpoint settings
            EditorGUILayout.LabelField("Checkpoint Settings", EditorStyles.boldLabel);
            _settings.MinCheckpoints = EditorGUILayout.IntField("Min Checkpoints", _settings.MinCheckpoints);
            _settings.MaxCheckpoints = EditorGUILayout.IntField("Max Checkpoints", _settings.MaxCheckpoints);
            _settings.UseRelativeCheckpointCount = EditorGUILayout.Toggle("Use Relative Count", _settings.UseRelativeCheckpointCount);
            
            if (_settings.UseRelativeCheckpointCount)
            {
                _settings.MinCheckpointDensity = EditorGUILayout.Slider("Min Checkpoint Density", _settings.MinCheckpointDensity, 0.05f, 0.5f);
                _settings.MaxCheckpointDensity = EditorGUILayout.Slider("Max Checkpoint Density", _settings.MaxCheckpointDensity, 0.05f, 0.5f);
                
                // Ensure Min <= Max
                if (_settings.MinCheckpointDensity > _settings.MaxCheckpointDensity)
                {
                    _settings.MaxCheckpointDensity = _settings.MinCheckpointDensity;
                }
            }
            
            EditorGUILayout.Space();
            
            // Checkpoint placement settings
            EditorGUILayout.LabelField("Checkpoint Placement", EditorStyles.boldLabel);
            _settings.RandomCheckpointPlacement = EditorGUILayout.Toggle("Random Placement", _settings.RandomCheckpointPlacement);
            
            if (_settings.RandomCheckpointPlacement)
            {
                EditorGUILayout.HelpBox("Checkpoints will be placed randomly along the path (except first and last).", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Checkpoints will be placed evenly along the path.", MessageType.Info);
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Checkpoints will be placed along a path that covers the entire board. " +
                "First checkpoint is at the start of the path, last checkpoint is at the end.", MessageType.Info);
            
            EditorGUILayout.Space();
            
            // Time settings
            EditorGUILayout.LabelField("Time Settings", EditorStyles.boldLabel);
            _settings.TimePerCell = EditorGUILayout.Slider("Time Per Cell", _settings.TimePerCell, 0.1f, 2.0f);
  
            EditorGUILayout.Space();
            
            // Generation settings
            EditorGUILayout.LabelField("Generation Settings", EditorStyles.boldLabel);
            _seed = EditorGUILayout.IntField("Seed (-1 for random)", _seed);
            
            EditorGUILayout.Space();
            
            // Output settings
            EditorGUILayout.LabelField("Output Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            _outputPath = EditorGUILayout.TextField("Output Path", _outputPath);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string path = EditorUtility.SaveFolderPanel("Select Output Folder", _outputPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    // Convert absolute path to Unity relative path
                    if (path.StartsWith(Application.dataPath))
                    {
                        _outputPath = "Assets" + path.Substring(Application.dataPath.Length);
                    }
                    else
                    {
                        _outputPath = path;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Generation buttons
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Generate Single Level", GUILayout.Height(30)))
            {
                GenerateAndSaveLevel();
            }
            
            if (GUILayout.Button("Generate Multiple Levels", GUILayout.Height(30)))
            {
                ShowBatchGenerationDialog();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Preview last generated level
            if (GUILayout.Button("Preview Last Generated Level"))
            {
                ZipLevelGenerator generator = CreateGenerator();
                LevelData levelData = generator.GenerateLevelData();
                
                ZipBoardConfig preview = ScriptableObject.CreateInstance<ZipBoardConfig>();
                SetConfigValues(preview, levelData);
                
                Selection.activeObject = preview;
                EditorUtility.FocusProjectWindow();
            }
            
            EditorGUILayout.EndScrollView();
        }

        private void GenerateAndSaveLevel()
        {
            ZipLevelGenerator generator = CreateGenerator();
            LevelData levelData = generator.GenerateLevelData();
            
            // Create ScriptableObject
            ZipBoardConfig config = ScriptableObject.CreateInstance<ZipBoardConfig>();
            
            // Set values via SerializedObject for proper SerializeField handling
            SetConfigValues(config, levelData);
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }
            
            // Generate file name
            string fileName = $"ZipLevel_{levelData.Size.x}x{levelData.Size.y}_{levelData.CheckpointPositions.Length}CP_{System.DateTime.Now:yyyyMMdd_HHmmss}.asset";
            string fullPath = Path.Combine(_outputPath, fileName).Replace('\\', '/');
            
            // Save asset
            AssetDatabase.CreateAsset(config, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("Success", $"Level generated and saved to:\n{fullPath}", "OK");
            
            // Select created asset
            Selection.activeObject = config;
            EditorUtility.FocusProjectWindow();
        }

        private void ShowBatchGenerationDialog()
        {
            BatchGenerationWindow.ShowWindow(_settings, _outputPath, _seed);
        }

        private ZipLevelGenerator CreateGenerator()
        {
            if (_seed == -1)
            {
                return new ZipLevelGenerator(_settings);
            }
            else
            {
                return new ZipLevelGenerator(_seed, _settings);
            }
        }

        internal static void SetConfigValues(ZipBoardConfig config, LevelData levelData)
        {
            if (config == null)
            {
                Debug.LogError("Config is null in SetConfigValues");
                return;
            }
            
            if (levelData == null)
            {
                Debug.LogError("LevelData is null in SetConfigValues");
                return;
            }
            
            // Use SerializedObject to set values
            // For [field: SerializeField] need to use correct backing field name
            SerializedObject serializedObject = new SerializedObject(config);
            
            // Try to find property through different name variants
            SerializedProperty sizeProp = serializedObject.FindProperty("Size");
            if (sizeProp == null)
            {
                // Try to find via backing field
                sizeProp = serializedObject.FindProperty("<Size>k__BackingField");
            }
            
            if (sizeProp != null)
            {
                sizeProp.vector2IntValue = levelData.Size;
            }
            else
            {
                // If not found via SerializedObject, use reflection
                SetPropertyViaReflection(config, "Size", levelData.Size);
            }
            
            // Check that CheckpointPositions is not null
            if (levelData.CheckpointPositions == null)
            {
                Debug.LogError("CheckpointPositions is null in LevelData");
                levelData.CheckpointPositions = new Vector2Int[0];
            }
            
            SerializedProperty checkpointsProp = serializedObject.FindProperty("CheckpointPositions");
            if (checkpointsProp == null)
            {
                checkpointsProp = serializedObject.FindProperty("<CheckpointPositions>k__BackingField");
            }
            
            if (checkpointsProp != null)
            {
                checkpointsProp.arraySize = levelData.CheckpointPositions.Length;
                for (int i = 0; i < levelData.CheckpointPositions.Length; i++)
                {
                    checkpointsProp.GetArrayElementAtIndex(i).vector2IntValue = levelData.CheckpointPositions[i];
                }
            }
            else
            {
                SetPropertyViaReflection(config, "CheckpointPositions", levelData.CheckpointPositions);
            }
            
            SerializedProperty wallsProp = serializedObject.FindProperty("Walls");
            if (wallsProp == null)
            {
                wallsProp = serializedObject.FindProperty("<Walls>k__BackingField");
            }
            
            if (wallsProp != null)
            {
                wallsProp.arraySize = 0;
            }
            else
            {
                SetPropertyViaReflection(config, "Walls", new ZipCellWallsConfig[0]);
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        internal static void SetPropertyViaReflection(object obj, string propertyName, object value)
        {
            var property = obj.GetType().GetProperty(propertyName, 
                System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.Instance);
            
            if (property != null && property.CanWrite)
            {
                property.SetValue(obj, value);
            }
            else
            {
                // Try to find backing field
                var field = obj.GetType().GetField($"<{propertyName}>k__BackingField",
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                
                if (field != null)
                {
                    field.SetValue(obj, value);
                }
                else
                {
                    // Last attempt - find any field with similar name
                    var fields = obj.GetType().GetFields(
                        System.Reflection.BindingFlags.NonPublic | 
                        System.Reflection.BindingFlags.Instance);
                    
                    foreach (var f in fields)
                    {
                        if (f.Name.Contains(propertyName))
                        {
                            f.SetValue(obj, value);
                            break;
                        }
                    }
                }
            }
        }
    }

    public class BatchGenerationWindow : EditorWindow
    {
        private ZipLevelGenerator.GenerationSettings _settings;
        private string _outputPath;
        private int _seed;
        private int _count = 10;

        public static void ShowWindow(ZipLevelGenerator.GenerationSettings settings, string outputPath, int seed)
        {
            BatchGenerationWindow window = GetWindow<BatchGenerationWindow>("Batch Generate Levels");
            window._settings = settings;
            window._outputPath = outputPath;
            window._seed = seed;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Batch Level Generation", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            _count = EditorGUILayout.IntField("Number of Levels", _count);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Generate", GUILayout.Height(30)))
            {
                GenerateBatch();
                Close();
            }
        }

        private void GenerateBatch()
        {
            // Check that settings are initialized
            if (_settings == null)
            {
                EditorUtility.DisplayDialog("Error", "Settings are not initialized. Please close and reopen the batch generation window.", "OK");
                return;
            }
            
            if (string.IsNullOrEmpty(_outputPath))
            {
                EditorUtility.DisplayDialog("Error", "Output path is not set.", "OK");
                return;
            }
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }
            
            ZipLevelGenerator generator;
            if (_seed == -1)
            {
                generator = new ZipLevelGenerator(_settings);
            }
            else
            {
                generator = new ZipLevelGenerator(_seed, _settings);
            }
            
            for (int i = 0; i < _count; i++)
            {
                try
                {
                    LevelData levelData = generator.GenerateLevelData();
                    
                    // Check that data is valid
                    if (levelData == null || levelData.CheckpointPositions == null)
                    {
                        Debug.LogError($"Failed to generate level data for level {i + 1}");
                        continue;
                    }
                    
                    // Create ScriptableObject
                    ZipBoardConfig config = ScriptableObject.CreateInstance<ZipBoardConfig>();
                    
                    // Set values
                    ZipLevelGeneratorEditor.SetConfigValues(config, levelData);
                    
                    string fileName = $"ZipLevel_{levelData.Size.x}x{levelData.Size.y}_{levelData.CheckpointPositions.Length}CP_{i:D4}.asset";
                    string fullPath = Path.Combine(_outputPath, fileName).Replace('\\', '/');
                    
                    AssetDatabase.CreateAsset(config, fullPath);
                    
                    EditorUtility.DisplayProgressBar("Generating Levels", $"Generating level {i + 1}/{_count}", (float)(i + 1) / _count);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error generating level {i + 1}: {e.Message}\n{e.StackTrace}");
                }
            }
            
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("Success", $"Generated {_count} levels in:\n{_outputPath}", "OK");
        }
    }
}

