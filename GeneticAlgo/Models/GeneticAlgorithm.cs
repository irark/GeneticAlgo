using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgo.Models
{
    public class GeneticAlgorithm
    {
        private readonly int _roomWidth; // ширина приміщення
        private readonly int _roomLength; // довжина приміщення
        private readonly int _numOutlets; // кількість розеток
        private readonly int _minDistance; // кількість розеток
        private readonly List<Outlet> _outlets; // список розеток
        private readonly int _maxIterations = 1000; // максимальна кількість ітерацій
        private readonly int _populationSize = 50; // розмір популяції
        private readonly double _crossoverRate = 0.8; // шанс на виконання оператора кросовера
        private readonly double _mutationRate = 0.1; // шанс на виконання оператора мутації
        private readonly Random _random = new(); // генератор випадкових чисел

        public OutletSolution BestSolution { get; private set; } // найкраще рішення

        public GeneticAlgorithm(int roomWidth, int roomLength, int numOutlets, int minDistance)
        {
            _roomWidth = roomWidth;
            _roomLength = roomLength;
            _numOutlets = numOutlets;
            _minDistance = minDistance;
            _outlets = new List<Outlet>();

            for (var i = 0; i < numOutlets; i++)
            {
                var newOutlet = GenerateRandomOutlet();
                while (!CheckDistance(newOutlet))
                {
                    newOutlet = GenerateRandomOutlet();
                }
                _outlets.Add(newOutlet);
            }
        }

        private bool CheckDistance(Outlet newOutlet)
        {
            foreach (var outlet in _outlets)
            {
                var d = Math.Sqrt(Math.Pow(outlet.X - newOutlet.X, 2) +
                                  Math.Pow(outlet.Y - newOutlet.Y, 2));
                if (d < _minDistance)
                    return false;
            }

            return true;
        }

        private Outlet GenerateRandomOutlet()
        {
            var randValue = new List<Outlet>()
            {
                new(_random.Next(_roomWidth), 0),
                new(_random.Next(_roomWidth), _roomLength - 1),
                new(0, _random.Next(_roomLength)),
                new(_roomWidth - 1, _random.Next(_roomLength))
            };
            return randValue[_random.Next(4)];
        }
        public void Run()
        {
            // Generate init population
            List<OutletSolution> population = new();

            for (var i = 0; i < _populationSize; i++)
                population.Add(new OutletSolution(_outlets, _roomWidth, _roomLength, _minDistance));

            // Main loop
            for (var i = 0; i < _maxIterations; i++)
            {
                // Calculating fitness function
                foreach (OutletSolution solution in population)
                    solution.CalculateFitness();

                // Sorting by Fitness
                population = population.OrderBy(solution => solution.Fitness).ToList();

                // Saving best result
                BestSolution = population[0];

                // Creating new population
                List<OutletSolution> newPopulation = new();

                // Adding best individuals from old population to new population
                for (var j = 0; j < _populationSize / 2; j++)
                    newPopulation.Add(population[j]);
                

                //Applying crossover
                for (var j = 0; j < _populationSize / 4; j++)
                {
                    OutletSolution parent1 = SelectParent(population);
                    OutletSolution parent2 = SelectParent(population);

                    if (_random.NextDouble() < _crossoverRate)
                    {
                        Crossover(parent1, parent2, out OutletSolution child1, out OutletSolution child2);
                        newPopulation.Add(child1);
                        newPopulation.Add(child2);
                    }
                    else
                    {
                        newPopulation.Add(parent1);
                        newPopulation.Add(parent2);
                    }
                }

                // Applying mutation
                for (var j = 0; j < _populationSize / 2; j++)
                {
                    OutletSolution parent = SelectParent(population);

                    if (_random.NextDouble() < _mutationRate)
                    {
                        OutletSolution child = Mutate(parent);
                        newPopulation.Add(child);
                    }
                    else
                    {
                        newPopulation.Add(parent);
                    }
                }

                // Replaced old population by new
                population = newPopulation;
            }
        }
        private OutletSolution SelectParent(List<OutletSolution> population)
        {
            var totalFitness = population.Sum(solution => solution.Fitness);
            var randomFitness = _random.NextDouble() * totalFitness;

            foreach (OutletSolution solution in population)
            {
                randomFitness -= solution.Fitness;
                if (randomFitness <= 0)
                    return solution;
            }
            return null;
        }
        private void Crossover(OutletSolution parent1, OutletSolution parent2, out OutletSolution child1, out OutletSolution child2)
        {
            var crossoverPoint = _random.Next(_numOutlets);
            child1 = new OutletSolution(_outlets, _roomWidth, _roomLength, _minDistance);
            child2 = new OutletSolution(_outlets, _roomWidth, _roomLength, _minDistance);

            for (var i = 0; i < crossoverPoint; i++)
            {
                child1.SetOutlet(i, parent1.GetOutlet(i));
                child2.SetOutlet(i, parent2.GetOutlet(i));
            }

            for (var i = crossoverPoint; i < _numOutlets; i++)
            {
                child1.SetOutlet(i, parent2.GetOutlet(i));
                child2.SetOutlet(i, parent1.GetOutlet(i));
            }
        }
        
        private OutletSolution Mutate(OutletSolution parent)
        {
            OutletSolution child = new(_outlets, _roomWidth, _roomLength, _minDistance);
            var indexToMutate = _random.Next(_numOutlets);
            var newOutlet = GenerateRandomOutlet();
            while (!CheckDistance(newOutlet))
            {
                newOutlet = GenerateRandomOutlet();
            }
            child.SetOutlet(indexToMutate, newOutlet);

            for (var i = 0; i < _numOutlets; i++)
            {
                if (i != indexToMutate)
                    child.SetOutlet(i, parent.GetOutlet(i));
            }
            return child;
        }
    }
}