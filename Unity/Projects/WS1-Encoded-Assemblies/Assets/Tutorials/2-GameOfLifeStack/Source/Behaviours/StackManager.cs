﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SpatialSlur;

namespace RC3
{
    /// <summary>
    /// 
    /// </summary>
    public class StackManager : MonoBehaviour
    {

        [SerializeField] private ModelInitializer _initializer;
        [SerializeField] private CellLayer _layerPrefab;
        [SerializeField] private Cell _cellPrefab;

        [SerializeField] private int _layerColumns = 10;
        [SerializeField] private int _layerRows = 10;
        [SerializeField] private int _layerCount = 10;

        private CellLayer[] _layers;
        private ModelState[] _history;
        private CAModel2D _model;
        private CARule2D _defaultRule = new Conway2D(Neighborhoods.MooreR1);
        private int _stepCount;
        private bool _pause = true;


        /// <summary>
        /// 
        /// </summary>
        public CAModel2D Model
        {
            get { return _model; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool Pause
        {
            get { return _pause; }
            set { _pause = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            _layers = new CellLayer[_layerCount];
            _history = new ModelState[_layerCount];

            // instantiate layers
            for(int i = 0; i < _layerCount; i++)
            {
                CellLayer copy = Instantiate(_layerPrefab, transform);
                copy.transform.localPosition = new Vector3(0.0f, i, 0.0f);

                // create cell layer
                copy.Initialize(_cellPrefab, _layerRows, _layerColumns);
                _layers[i] = copy;

                // create history
                _history[i] = new int[_layerRows, _layerColumns];
            }

            // instantiate rule and model
            _model = new CAModel2D(_defaultRule, _layerRows, _layerColumns);

            // initialize model
            _initializer.Initialize(_model.CurrentState);

            // center manager gameobject at the world origin
            transform.localPosition = new Vector3(_layerColumns, _layerCount, _layerRows) * -0.5f;
        }


        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            // re-initialize on key down
            if (Input.GetKeyDown(KeyCode.Space))
                _initializer.Initialize(_model.CurrentState);

            //
            CycleLayers();

            // advance model
            _model.Step();
            //_model.StepParallel();
            _stepCount++;

            // copy model state to bottom layer
            ModelState current = _model.CurrentState;
            Array.Copy(current, _history[0], current.Count);

            // update cells
            UpdateCells();
        }


        /// <summary>
        /// 
        /// </summary>
        private void CycleLayers()
        {
            int curr = _layerCount - 1;
            CellLayer layer0 = _layers[curr];
            ModelState histor0 = _history[curr];
            
            // move each layer up by one
            do
            {
                _layers[curr] = _layers[curr - 1];
                _history[curr] = _history[curr - 1];
            } while (--curr > 0);

            // move last layer back to the bottom
            _layers[0] = layer0;
            _history[0] = histor0;

            // update layer positions
            for (int j = 0; j < _layerCount; j++)
                _layers[j].transform.localPosition = new Vector3(0.0f, j, 0.0f);
        }


        /// <summary>
        /// Updates cells from the given layer based on the current state of the model
        /// </summary>
        /// <param name="layer"></param>
        private void UpdateCells()
        {
            Cell[,] cells = _layers[0].Cells;
            int[,] state = _model.CurrentState;

            for (int i = 0; i < _layerRows; i++)
            {
                for (int j = 0; j < _layerColumns; j++)
                    cells[i, j].SetState(state[i, j]);
            }
        }
    }
}
