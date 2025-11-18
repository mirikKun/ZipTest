using System;
using Project.Scripts.Gameplay.Zip.Board;
using Project.Scripts.Gameplay.Zip.Configs;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.View
{
    public class ZipBoardView:MonoBehaviour
    {
        [SerializeField] private ZipBoardConfig _config;
        [SerializeField] private Grid _grid;
        
        [Header("Prefabs")]
        [SerializeField] private ZipCheckPointView _checkPointPrefab;
        [SerializeField] private ZipCellView _cellPrefab;
        [SerializeField] private ZipLineView _linePrefab;

        private ZipBoard _board;
        private void Start()
        {
            CreateBoard();
        }

        private void CreateBoard()
        {
            throw new NotImplementedException();
        }
    }
}