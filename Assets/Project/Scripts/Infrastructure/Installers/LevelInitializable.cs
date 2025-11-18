using Code.Gameplay.Cameras.Provider;
using Code.Gameplay.Levels;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class LevelInitializer : MonoBehaviour, IInitializable
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Transform _startPoint;
        private ICameraProvider _cameraProvider;
        private ILevelDataProvider _levelDataProvider;

        [Inject]
        private void Construct(ICameraProvider cameraProvider, ILevelDataProvider levelDataProvider)
        {
            _levelDataProvider = levelDataProvider;
            _cameraProvider = cameraProvider;
        }

        public void Initialize()
        {
            _levelDataProvider.SetStartPoint(_startPoint.position);
            _cameraProvider.SetMainCamera(_mainCamera);
        }
    }
}