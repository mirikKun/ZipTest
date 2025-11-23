using System.Collections;
using Cysharp.Threading.Tasks;
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
    public class ZipEndlessGameMediator : MonoBehaviour
    {
        [SerializeField] private ZipBoardView _board;
        [SerializeField] private ZipBoardEffects _boardEffects;
        [SerializeField] private TimerView _timer;

        [SerializeField] private float _startTimeGiven = 20;
        [Range(0, 1)] [SerializeField] private float _durationMultiplicatorOverboard = 0.9f;

        private IStaticDataService _staticDataService;
        private IWindowService _windowService;
        private IGameProgressService _gameProgressService;

        private ZipBoardConfigsList _boardConfigsList;
        private IGameStateMachine _gameStateMachine;

        private float _remainingTime;
        private float _timeUsed;
        private int _levelsPassed;
        private bool _active;
        private ZipBoardData _currentZipBoardData;

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
            _timer.gameObject.SetActive(true);

            _boardConfigsList = _staticDataService.GetZipBoardConfigList();
            _active = true;
            _remainingTime = _startTimeGiven;
            OpenNextLevel();
        }

        private void Update()
        {
            if (!_active) return;
            _remainingTime -= Time.deltaTime;
            _timeUsed += Time.deltaTime;
            _timer.UpdateTimer(_remainingTime);

            if (_remainingTime <= 0)
            {
                OnTimeRunOut();
                _timer.UpdateTimer(0);
            }
        }


        private void OnDestroy()
        {
            _windowService.CloseAll();
        }


        private void OpenNextLevel()
        {
            _windowService.Hide(WindowId.LosePanel);
            _currentZipBoardData = _boardConfigsList.GetRandomGeneratedLevelData();
            _board.CreateBoard(_currentZipBoardData, false);
            _boardEffects.PlayBoardAppearAnimation(Vector3.zero, Vector3.one);
        }

        private async void OnLevelFinished()
        {
            _levelsPassed++;
            float additionalTime = _currentZipBoardData.OrientedTimeToFinish *
                                   (Mathf.Pow(_durationMultiplicatorOverboard, _levelsPassed));
            _remainingTime += additionalTime;
            _timer.PlayNewTimeEffect(additionalTime);
            await _boardEffects.PlayFinishAnimation();
            _boardEffects.PlayBoardAppearAnimation(Vector3.one, Vector3.zero);
            ZipCellView[,] cellsToDestroy = _board.Cells;
            await UniTask.WaitForSeconds(_boardEffects.TotalBoardAppearAnimationDuration / 2);
            OpenNextLevel();
            await UniTask.WaitForSeconds(_boardEffects.TotalBoardAppearAnimationDuration / 2);
            ClearBoard(cellsToDestroy);
        }

        private void ClearBoard(ZipCellView[,] cellsToDestroy)
        {
            if (cellsToDestroy != null)
            {
                for (int x = 0; x < cellsToDestroy.GetLength(0); x++)
                {
                    for (int y = 0; y < cellsToDestroy.GetLength(1); y++)
                    {
                        if (cellsToDestroy[x, y] != null)
                        {
                            cellsToDestroy[x, y].Destroy();
                        }
                    }
                }
            }
        }

        private void OnTimeRunOut()
        {
            _active = false;
            _board.SetActive(false);
            var winMenu = _windowService.Open<LoseMenu>(WindowId.LosePanel);
            _boardEffects.PlayBoardAppearAnimation(Vector3.one, Vector3.zero);

            winMenu.SetData(_timeUsed, _levelsPassed);
            winMenu.Init(OpenHomeScreen, OpenNextLevel);
        }

        private void OpenHomeScreen()
        {
            _gameStateMachine.Enter<LoadingHomeScreenState>();
        }
    }
}