using HarmonogramApp1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonogramApp1
{
    public class FitnessEvaluator
    {
        // Wagi dla różnych komponentów oceny
        private const int WeightForPerfectEmployeeCount = 20;
        private const int PenaltyForTooFewEmployees = -100;
        private const int PenaltyForTooManyEmployees = -50;
        private const int PenaltyForMultipleShiftsPerDay = -100;
        private const int RewardForPreferredShift = 5;
        private const int RewardForPreferredDay = 5;
        private const int PenaltyForUnpreferredShift = -50;
        private const int PenaltyForUnpreferredDay = -50;
        private const int PenaltyForForbiddenAssignment = -100;




        private readonly List<Employee> _employees;
      

        public FitnessEvaluator(List<Employee> employees,
                              bool useNeuralNetwork = false,
                              string pythonPath = null,
                              string scriptPath = null,
                              double neuralNetworkWeight = 0.3)
        {
            _employees = employees ?? throw new ArgumentNullException(nameof(employees));
           
        }

        public int Evaluate(Schedule schedule, EmployerRequirements requirements)
        {
           
            return EvaluateTraditional(schedule, requirements);
        }

        public int EvaluateV2(Schedule schedule, EmployerRequirements requirements, double alpha = 0.5)
        {
            if (alpha < 0 || alpha > 1)
                throw new ArgumentException("Alpha must be between 0 and 1");

            int differenceSum = 0;
            int unwantedSituations = 0;

            for (int d = 0; d < schedule.Days; d++)
            {
                for (int s = 0; s < schedule.Shifts; s++)
                {
                    int actualCount = 0;
                    for (int e = 0; e < schedule.Employees; e++)
                    {
                        if (schedule.Data[d, s, e] == 1)
                        {
                            actualCount++;

                            // Sprawdź niepożądane sytuacje
                            var emp = _employees[e];
                            if (emp.UnpreferredDays.Contains(d) || emp.UnpreferredShifts.Contains(s))
                            {
                                unwantedSituations++;
                            }
                        }
                    }

                    int required = requirements.GetRequired(d, s);
                    differenceSum += Math.Abs(required - actualCount);
                }
            }

            double fitness = alpha * differenceSum + (1 - alpha) * unwantedSituations;
            int maxPenalty = schedule.Days * schedule.Shifts * schedule.Employees;
            return maxPenalty - (int)fitness;
        }

        private int EvaluateTraditional(Schedule schedule, EmployerRequirements requirements)
        {
            int score = 0;

            // Ocena liczby pracowników na zmianie
            score += EvaluateEmployeeCounts(schedule, requirements);

            // Ocena preferencji pracowników
            score += EvaluateEmployeePreferences(schedule, requirements);

            // Ocena zasad dotyczących zmian
            score += EvaluateShiftRules(schedule);

            return score;
        }

        

        private int EvaluateEmployeeCounts(Schedule schedule, EmployerRequirements requirements)
        {
            int score = 0;

            for (int d = 0; d < schedule.Days; d++)
            {
                for (int s = 0; s < schedule.Shifts; s++)
                {
                    int count = 0;
                    for (int e = 0; e < schedule.Employees; e++)
                    {
                        if (schedule.Data[d, s, e] == 1)
                            count++;
                    }

                    int required = requirements.GetRequired(d, s);

                    if (count < required)
                        score += PenaltyForTooFewEmployees;
                    else if (count > required)
                        score += PenaltyForTooManyEmployees;
                    else
                        score += WeightForPerfectEmployeeCount;
                }
            }

            return score;
        }

        private int EvaluateEmployeePreferences(Schedule schedule, EmployerRequirements requirements)
        {
            int score = 0;

            foreach (var emp in _employees)
            {
                for (int d = 0; d < schedule.Days; d++)
                {
                    for (int s = 0; s < schedule.Shifts; s++)
                    {
                        if (schedule.Data[d, s, emp.Id] == 1)
                        {
                            // Nagrody za preferencje
                            if (emp.PreferredShifts.Contains(s))
                                score += RewardForPreferredShift;
                            if (emp.PreferredDays.Contains(d))
                                score += RewardForPreferredDay;

                            // Kary za niechciane przypisania
                            if (emp.UnpreferredShifts.Contains(s))
                                score += PenaltyForUnpreferredShift;
                            if (emp.UnpreferredDays.Contains(d))
                                score += PenaltyForUnpreferredDay;

                            // Dodatkowa kara, jeśli szef wymaga pracownika na zmianie, a on nie chce tam pracować
                            int required = requirements.GetRequired(d, s);
                            if (required > 0 && (emp.UnpreferredDays.Contains(d) || emp.UnpreferredShifts.Contains(s)))
                            {
                                score += PenaltyForForbiddenAssignment;
                            }
                        }
                    }
                }
            }

            return score;
        }

        private int EvaluateShiftRules(Schedule schedule)
        {
            int score = 0;

            for (int d = 0; d < schedule.Days; d++)
            {
                for (int e = 0; e < schedule.Employees; e++)
                {
                    int shiftsWorked = 0;
                    for (int s = 0; s < schedule.Shifts; s++)
                    {
                        if (schedule.Data[d, s, e] == 1)
                            shiftsWorked++;
                    }

                    if (shiftsWorked > 1)
                        score += PenaltyForMultipleShiftsPerDay;
                }
            }

            return score;
        }
    }



}
