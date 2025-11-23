using System;
using System.Collections;
using Project.Scripts.Gameplay.StaticData;
using Project.Scripts.Gameplay.Windows;
using Project.Scripts.Gameplay.Windows.WindowTypes;
using Project.Scripts.Gameplay.Zip.Configs;
using Project.Scripts.Gameplay.Zip.View;
using Project.Scripts.Infrastructure.Progress;
using Project.Scripts.Infrastructure.States.GameStates;
using Project.Scripts.Infrastructure.States.StateMachine;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Gameplay.Zip.Controller
{
    public class ZipGameMediator : MonoBehaviour
    {
        [SerializeField] private ZipBoardView _board;
        [SerializeField] private ZipBoardEffects _boardEffects;
        [SerializeField] private TimerView _timer;

        private IStaticDataService _staticDataService;
        private IWindowService _windowService;
        private IGameProgressService _gameProgressService;

        private ZipBoardConfigsList _boardConfigsList;
        private IGameStateMachine _gameStateMachine;

        private float _timeUsed;
        private bool _active;
        private ZipBoardData _currentBoardData;

        public float TimeUsed => _timeUsed;

        [Inject]
        private void Construct(IWindowService windowService, IStaticDataService staticDataService,
            IGameProgressService gameProgressService, IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
            _windowService = windowService;
            _staticDataService = staticDataService;
            _gameProgressService = gameProgressService;
        }


        private void Start()
        {
            _board.BoardFinished += OnLevelFinished;
            _timer.gameObject.SetActive(false);

            _boardConfigsList = _staticDataService.GetZipBoardConfigList();
            OpenCurrentLevel();
        }

        private void Update()
        {
            if (!_active) return;
            _timeUsed += Time.deltaTime;
            _timer.UpdateTimer(_timeUsed);
        }


        private void OnDestroy()
        {
            _windowService.CloseAll();
        }


        private void OpenNextLevel()
        {
            _gameProgressService.SetZipLevelIndex(_gameProgressService.GetZipLevelIndex() + 1);
            OpenCurrentLevel();
        }

        private void OpenCurrentLevel()
        {
            _windowService.Hide(WindowId.WinPanel);
            _timer.gameObject.SetActive(true);
            _currentBoardData = _boardConfigsList.GetLevelDataByIndex(_gameProgressService.GetZipLevelIndex());
            _board.CreateBoard(_currentBoardData);
            _boardEffects.PlayBoardAppearAnimation(Vector3.zero, Vector3.one);
            _timeUsed = 0;
            _active = true;
        }

        private void OpenHomeScreen()
        {
            _gameStateMachine.Enter<LoadingHomeScreenState>();
        }

        private async void OnLevelFinished()
        {
            _active = false;
            await _boardEffects.PlayFinishAnimation();
            _boardEffects.PlayBoardAppearAnimation(Vector3.one, Vector3.zero);
            _timer.gameObject.SetActive(false);
            var winMenu = _windowService.Open<WinMenu>(WindowId.WinPanel);
            winMenu.SetData(_timeUsed, _currentBoardData.OrientedTimeToFinish * 3);

            winMenu.Init(OpenHomeScreen, OpenNextLevel, OpenCurrentLevel);
        }
    }
}