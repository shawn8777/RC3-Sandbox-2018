﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SpatialSlur;

namespace RC3
{
    /// <summary>
    /// Rule for Conway's game of life
    /// </summary>
    public class MyCA : ICARule2D
    {

        //setup some possible instruction sets
        private GOLInstructionSet _instSetMO1 = new GOLInstructionSet(2, 3, 3, 3);
        private GOLInstructionSet _instSetMO2 = new GOLInstructionSet(3, 4, 3, 4);
        private GOLInstructionSet _instSetMO3 = new GOLInstructionSet(2, 5, 3, 3);

        //analysis manager - provides global model data and data analysis
        private AnalysisManager _analysisManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offsets"></param>
        public MyCA(AnalysisManager analysismanager)
        {
            _analysisManager = analysismanager;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public int NextAt(int i, int j, int[,] current)
        {
            //get cell age

            //get local neighborhood data
            int sumMO = GetNeighborSum(i, j, current, Neighborhoods.MooreR1);
            int sumVN1 = GetNeighborSum(i, j, current, Neighborhoods.VonNeumannR1);

            //get layer data

            //get stack data

            //get current state
            int state = current[i, j];

            //choose an instruction set
            GOLInstructionSet instructionSet = _instSetMO1;
            int output = 0;
            
            //if current state is "alive"
            if (state == 1)
            {
                if (sumMO < instructionSet.getInstruction(0))
                {
                    output = 0;
                }

                if (sumMO >= instructionSet.getInstruction(0) && sumMO <= instructionSet.getInstruction(1))
                {
                    output = 1;
                }

                if (sumMO > instructionSet.getInstruction(1))
                {
                    output = 0;
                }
            }

            //if current state is "dead"
            if (state == 0)
            {
                if (sumMO >= instructionSet.getInstruction(2) && sumMO <= instructionSet.getInstruction(3))
                {
                    output = 1;
                }
                else
                {
                    output = 0;
                }
            }

            return output;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="j0"></param>
        /// <returns></returns>
        private int GetNeighborSum(int i0, int j0, int[,] current, IndexPair[] neighborhood)
        {
            int m = current.GetLength(0);
            int n = current.GetLength(1);
            int sum = 0;

            foreach (IndexPair offset in neighborhood)
            {
                int i1 = Wrap(i0 + offset.I, m);
                int j1 = Wrap(j0 + offset.J, n);

                if (current[i1, j1] > 0)
                    sum++;
            }

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static int Wrap(int i, int n)
        {
            i %= n;
            return (i < 0) ? i + n : i;
        }
    }
}