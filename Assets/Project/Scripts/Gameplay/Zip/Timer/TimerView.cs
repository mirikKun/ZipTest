using System;
using Project.Scripts.Gameplay.Zip.View;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.Timer
{
    public class TimerView:MonoBehaviour
    {
        [SerializeField] private ZipBoardView _zipBoardView;
        [SerializeField] private TextMeshProUGUI _timer;

        private void Update()
        {
            int minutes = Mathf.FloorToInt(_zipBoardView.TimeUsed / 60f);
            int seconds = Mathf.FloorToInt(_zipBoardView.TimeUsed % 60f);
            _timer.text = $"{minutes:00}:{seconds:00}";
        }
    }
}