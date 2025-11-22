using System;
using System.Collections;
using Project.Scripts.Gameplay.Windows.WindowTypes;
using Project.Scripts.Gameplay.Zip.Configs;
using Project.Scripts.Gameplay.Zip.View;
using Project.Scripts.Infrastructure.Progress;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure
{
    public class GameMediator:MonoBehaviour
    {
        [SerializeField] private ZipBoardConfigsList _zipBoardConfigsList;
        [SerializeField] private MainMenuWindow _mainMenuWindow;
        [SerializeField] private GameObject _hud;
        [SerializeField] private WinMenu _winMenu;
        [SerializeField] private ZipBoardView _board;
        [SerializeField] private ZipBoardEffects _boardEffects;


        private IGameProgressService _gameProgressService;

        [Inject]
        private void Construct(IGameProgressService gameProgressService)
        {
            _gameProgressService = gameProgressService;
        }
        private void Start()
        {
            _mainMenuWindow.SetNextLevelButtonAction(OpenCurrentLevel);
            _winMenu.SetNextLevelButtonAction(OpenNextLevel);
            _winMenu.SetMainMenuButtons(OpenMainMenu);
            _winMenu.SetRestartButtonAction(OpenCurrentLevel);
            
            _board.BoardFinished += OnLevelFinished;
            
            
            _mainMenuWindow.gameObject.SetActive(true);
            _winMenu.gameObject.SetActive(false);
            _hud.SetActive(false);
        }

        private void OpenNextLevel()
        {
            _gameProgressService.SetZipLevelIndex(_gameProgressService.GetZipLevelIndex()+1);
            OpenCurrentLevel();
        }
        private void OpenCurrentLevel()
        {
            _mainMenuWindow.gameObject.SetActive(false);
            _winMenu.gameObject.SetActive(false);
            _hud.SetActive(true);
            _board.CreateBoard(_zipBoardConfigsList.GetLevelDataByIndex(_gameProgressService.GetZipLevelIndex()));
            _boardEffects.PlayPlayBoardAppearAnimation(Vector3.zero,Vector3.one);
            
        }

        private void OnLevelFinished(float timeUsed)
        {
            StartCoroutine(LevelFinishing(timeUsed));
   
        }

        private IEnumerator LevelFinishing(float timeUsed)
        {
            yield return _boardEffects.PlayFinishAnimation();
            _boardEffects.PlayPlayBoardAppearAnimation(Vector3.one,Vector3.zero);
            _winMenu.gameObject.SetActive(true);
            _hud.SetActive(false);
            _winMenu.SetData(timeUsed);
        }
        private void OpenMainMenu()
        {
            _mainMenuWindow.gameObject.SetActive(true);
            _winMenu.gameObject.SetActive(false);
            _hud.SetActive(false);
        }

    
    }
}