using Project.Scripts.Gameplay.Levels;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class LevelInitializer : MonoBehaviour, IInitializable
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Transform _startPoint;
        private ILevelDataProvider _levelDataProvider;

        [Inject]
        private void Construct( ILevelDataProvider levelDataProvider)
        {
            _levelDataProvider = levelDataProvider;
        }

        public void Initialize()
        {
            _levelDataProvider.SetStartPoint(_startPoint.position);
        }
    }
}