using System;
using System.Collections;
using Project.Scripts.Gameplay.Windows.WindowTypes;
using Project.Scripts.Gameplay.Zip.Configs;
using Project.Scripts.Gameplay.Zip.View;
using UnityEngine;

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


        private int _currentBoardIndex;
        private void Start()
        {
            _mainMenuWindow.SetNextLevelButtonAction(OpenCurrentLevel);
            _winMenu.SetNextLevelButtonAction(OpenCurrentLevel);
            _winMenu.SetMainMenuButtons(OpenMainMenu);
            
            _board.BoardFinished += OnLevelFinished;
            
            
            _mainMenuWindow.gameObject.SetActive(true);
            _winMenu.gameObject.SetActive(false);
            _hud.SetActive(false);
        }

        private void OpenCurrentLevel()
        {
            _mainMenuWindow.gameObject.SetActive(false);
            _winMenu.gameObject.SetActive(false);
            _hud.SetActive(true);
            _board.CreateBoard(_zipBoardConfigsList.GetLevelDataByIndex(_currentBoardIndex));
        }

        private void OnLevelFinished(float timeUsed)
        {
            StartCoroutine(LevelFinishing(timeUsed));
   
        }

        private IEnumerator LevelFinishing(float timeUsed)
        {
            yield return _boardEffects.PlayFinishAnimation();
            _currentBoardIndex++;
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