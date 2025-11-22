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
using UnityEngine.Events;
using Zenject;

namespace Project.Scripts.Gameplay.Zip.Controller
{
    public class ZipGameMediator : MonoBehaviour
    {
        [SerializeField] private ZipBoardView _board;
        [SerializeField] private ZipBoardEffects _boardEffects;
        [SerializeField] private GameObject _hud;

        private IStaticDataService _staticDataService;
        private IWindowService _windowService;
        private IGameProgressService _gameProgressService;

        private ZipBoardConfigsList _boardConfigsList;
        private IGameStateMachine _gameStateMachine;

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
            _hud.SetActive(false);

            _boardConfigsList = _staticDataService.GetZipBoardConfigList();
            OpenCurrentLevel();
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
            _hud.SetActive(true);
            _board.CreateBoard(_boardConfigsList.GetLevelDataByIndex(_gameProgressService.GetZipLevelIndex()));
            _boardEffects.PlayBoardAppearAnimation(Vector3.zero, Vector3.one);
        }

        private void OpenHomeScreen()
        {
            _gameStateMachine.Enter<LoadingHomeScreenState>();
        }

        private void OnLevelFinished(float timeUsed)
        {
            StartCoroutine(LevelFinishing(timeUsed));
        }

        private IEnumerator LevelFinishing(float timeUsed)
        {
            yield return _boardEffects.PlayFinishAnimation();
            _boardEffects.PlayBoardAppearAnimation(Vector3.one, Vector3.zero);
            _hud.SetActive(false);
            var winMenu = _windowService.Open<WinMenu>(WindowId.WinPanel);
            winMenu.SetData(timeUsed);

            winMenu.SetMainMenuButtons(OpenHomeScreen);
            winMenu.SetNextLevelButtonAction(OpenNextLevel);
            winMenu.SetRestartButtonAction(OpenCurrentLevel);
        }
    }
}