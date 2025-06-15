using HarmonogramApp1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonogramApp1
{
    public class GeneticAlgorithm
    {
        private int _populationSize;
        private int _generations;
        private double _mutationRate;
        private Random _rng;
        private FitnessEvaluator _evaluator;
        private List<Employee> _employees;
        private EmployerRequirements _requirements;
        private bool _useV2Fitness;// żeby uzyć nową metode Evaluate 2 

        private int _days, _shifts, _employeeCount;
        private List<int> _fitnessHistory = new List<int>();


        public GeneticAlgorithm(int populationSize, int generations, double mutationRate,
                                int days, int shifts, int employees,
                                List<Employee> employeeList,
                                EmployerRequirements requirements,
                                FitnessEvaluator evaluator, bool useV2Fitness = false)
        {
            _populationSize = populationSize;
            _generations = generations;
            _mutationRate = mutationRate;
            _days = days;
            _shifts = shifts;
            _employeeCount = employees;
            _evaluator = evaluator;
            _employees = employeeList;
            _requirements = requirements;
            _rng = new Random();
            _useV2Fitness = useV2Fitness;
        }

        public Schedule Run()
        {
            List<Schedule> population = new();
            for (int i = 0; i < _populationSize; i++)
            {
                var s = new Schedule(_days, _shifts, _employeeCount);
                s.Randomize(_rng, _employees);
                population.Add(s);
            }

            Schedule best = null;
            int bestFitness = int.MinValue;

            for (int gen = 0; gen < _generations; gen++)
            {
                var scored = population
            .Select(s => new {
                Schedule = s,
                Fitness = _useV2Fitness
                    ? _evaluator.EvaluateV2(s, _requirements)
                    : _evaluator.Evaluate(s, _requirements)
            })
            .OrderByDescending(x => x.Fitness)
            .ToList();


                if (scored[0].Fitness > bestFitness)
                {
                    bestFitness = scored[0].Fitness;
                    best = Clone(scored[0].Schedule);
                   
                    Console.WriteLine($"[Gen {gen}] Best fitness: {bestFitness}");
                    _fitnessHistory.Add(bestFitness);

                }

                var parents = scored.Take(_populationSize / 2).Select(x => x.Schedule).ToList();// sprawdzić czemu 2 

                var newPopulation = new List<Schedule>();
                while (newPopulation.Count < _populationSize)
                {
                    var p1 = parents[_rng.Next(parents.Count)];
                    var p2 = parents[_rng.Next(parents.Count)];
                    var child = Crossover(p1, p2);
                    Mutate(child);
                    newPopulation.Add(child);
                }
               
                population = newPopulation;
            }

            return best;
        }

        private Schedule Crossover(Schedule parent1, Schedule parent2)
        {
            var child = new Schedule(_days, _shifts, _employeeCount);

            for (int d = 0; d < _days; d++)
            {
                for (int s = 0; s < _shifts; s++)
                {
                    for (int e = 0; e < _employeeCount; e++)
                    {
                        int gene = _rng.NextDouble() < 0.5
                            ? parent1.Data[d, s, e]
                            : parent2.Data[d, s, e];

                        var emp = _employees[e];
                        bool forbidden = emp.UnpreferredDays.Contains(d) || emp.UnpreferredShifts.Contains(s);

                        child.Data[d, s, e] = (gene == 1 && !forbidden) ? 1 : 0;
                    }
                }
            }

            return child;
        }

        private void Mutate(Schedule schedule)
        {
            for (int d = 0; d < _days; d++)
            {
                for (int s = 0; s < _shifts; s++)
                {
                    for (int e = 0; e < _employeeCount; e++)
                    {
                        if (_rng.NextDouble() < _mutationRate)
                        {
                            var emp = _employees[e];
                            bool forbidden = emp.UnpreferredDays.Contains(d) || emp.UnpreferredShifts.Contains(s);

                            if (schedule.Data[d, s, e] == 0 && !forbidden)
                            {
                                schedule.Data[d, s, e] = 1;
                            }
                            else if (schedule.Data[d, s, e] == 1)
                            {
                                schedule.Data[d, s, e] = 0;
                            }
                        }
                    }
                }
            }
        }

        private Schedule Clone(Schedule original)
        {
            var clone = new Schedule(_days, _shifts, _employeeCount);
            for (int d = 0; d < _days; d++)
                for (int s = 0; s < _shifts; s++)
                    for (int e = 0; e < _employeeCount; e++)
                        clone.Data[d, s, e] = original.Data[d, s, e];
            return clone;
        }
    }
}

