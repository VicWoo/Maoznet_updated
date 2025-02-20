﻿using System;
using System.Collections.Generic;
using System.Text;
using Network.Matrices;
using System.Windows.Forms;

namespace Network.Blocks
{
    public sealed class BlockCalculations
    {
        private BlockCalculations() { }

        public static double AverageBlockSize(BlockCollection cc)
        {
            int sum = 0;
            for (int i = 0; i < cc.Count; ++i)
                sum += cc[i].Size;

            return (double)sum / (double)cc.Count;
        }

        public static double AverageBlockMembers(BlockCollection cc)
        {
            double sum = 0;
            for (int i = 0; i < cc.BlockOverlap.Rows; i++)
                sum += cc.BlockOverlap[i, i];

            return (double)sum / (double)cc.BlockOverlap.Rows;
        }

        public static double CMOI(BlockCollection cc)
        {
            Matrix O = cc.BlockOverlap;
            double total_sum = 0;
            for (int i = 0; i < O.Rows; i++)
                for (int j = i + 1; j < O.Rows; j++)
                {
                    if (O[j, j] != 0)
                        total_sum += (double)(O[i, j]) / O[j, j];
                }

            double coi = (double)(O.Rows * O.Rows - O.Rows);
            if (coi != 0)
                coi = (total_sum * 2.0) / coi;
            return coi;
        }

        public static Pair<double, double> COC(BlockCollection cc)
        {
            double simple_sum = 0, complex_sum = 0;

            Matrix CBCO = cc.BlockByBlockOverlap;
            Vector v = CBCO as Vector;

            if (CBCO.Cols == 1)
                return new Pair<double, double>(0.0, 0.0);

            double denominator = (double)(((double)CBCO.Cols * ((double)CBCO.Cols - 1)) / 2.0);


            if (v != null)
            {
                Vector w = new Vector(v.Size);
                w.Clear();
                for (int i = 0; i < v.Size; ++i)
                {
                    for (int j = i + 1; j < v.Size; ++j)
                        w[j] += cc.GetBlockByBlockOverlap(i, j);
                }

                for (int j = 1; j < v.Size; ++j)
                {
                    double sum = w[j];
                    simple_sum += (sum / v[j]);
                    if (v[j] > 1)
                        complex_sum += (sum / (v[j] - 1));
                }
            }
            else
            {
                for (int j = 1; j < CBCO.Rows; ++j)
                {
                    double sum = 0.0;

                    for (int i = 0; i < j; ++i)
                        sum += CBCO[i, j];

                    simple_sum += sum / CBCO[j, j];
                    if (CBCO[j, j] > 1)
                        complex_sum += sum / (CBCO[j, j] - 1);
                }

            }

            return new Pair<double, double>(simple_sum / denominator, complex_sum / denominator);
        }

        public static double NPOL(BlockCollection cc, Matrix data)
        {
            int total_sum = 0;

            for (int i = 0; i < cc.Count; i++)
            {
                total_sum += cc[i].Size * (data.Rows - cc[i].Size);
            }

            double first_term = 4.0 / (double)(cc.Count * data.Rows * data.Rows);

            return first_term * total_sum;
        }
    }
}
