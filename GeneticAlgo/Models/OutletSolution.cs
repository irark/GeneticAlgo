using System;
using System.Collections.Generic;

namespace GeneticAlgo.Models
{
    public class OutletSolution
    {
        public List<Outlet> Outlets;
        private int _roomWidth;
        private int _roomLength;
        private int _minDistance;
        private double _fitness;

        public OutletSolution(List<Outlet> outlets, int roomWidth, int roomLength, int minDistance)
        {
            Outlets = outlets;
            _roomWidth = roomWidth;
            _roomLength = roomLength;
            _minDistance = minDistance;
            CalculateFitness();
        }

        public Outlet GetOutlet(int index)
        {
            return Outlets[index];
        }

        public void SetOutlet(int index, Outlet outlet)
        {
            Outlets[index] = outlet;
            CalculateFitness();
        }

        public double Fitness => _fitness;

        public void CalculateFitness()
        {
            double distance = 0;
            double penalty = 0;

            for (var i = 0; i < Outlets.Count; i++)
            {
                for (var j = i + 1; j < Outlets.Count; j++)
                {
                    var d = Math.Sqrt(Math.Pow(Outlets[j].X - Outlets[i].X, 2) +
                                      Math.Pow(Outlets[j].Y - Outlets[i].Y, 2));
                    distance += d;
                    if (d < _minDistance)
                    {
                        penalty += 1.0 / (d * d);
                    }
                }
            }

            _fitness = 1 / (distance + penalty);
        }
    }
}