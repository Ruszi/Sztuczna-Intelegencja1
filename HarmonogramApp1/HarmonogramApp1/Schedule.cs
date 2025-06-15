using HarmonogramApp1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonogramApp1
{
    // chromosom 
    public class Schedule
    {
        public int Days { get; }
        public int Shifts { get; }
        public int Employees { get; }

        public int[,,] Data { get; set; }

        public Schedule(int days, int shifts, int employees)
        {
            Days = days;
            Shifts = shifts;
            Employees = employees;
            Data = new int[days, shifts, employees];
        }

        // Losowe wypełnienie harmonogramu (prosty generator)
        public void Randomize(Random rng, List<Employee> employees)
        {
            for (int d = 0; d < Days; d++)
            {
                for (int s = 0; s < Shifts; s++)
                {
                    int required = 2; // można dodać jako parametr później

                    // Tylko ci, którzy nie mają zakazu na ten dzień i zmianę
                    var candidates = employees
                        .Where(e => !e.UnpreferredDays.Contains(d) && !e.UnpreferredShifts.Contains(s))
                        .OrderBy(x => rng.Next()) // losowe przemieszanie
                        .Take(required)
                        .ToList();

                    foreach (var emp in candidates)
                    {
                        Data[d, s, emp.Id] = 1;
                    }

                    // NIE dobieramy nikogo z zakazami nawet jeśli brakuje ludzi
                }
            }
        }



        public void Print()
        {
            Console.WriteLine("HARMONOGRAM (1 - pracuje, 0 - wolne)\n");

            int columnWidth = 6;

            // Nagłówki kolumn
            Console.Write(new string(' ', 7)); // Miejsce na etykietę P1, P2...
            for (int d = 0; d < Days; d++)
            {
                int shiftCount = (d >= 5) ? 2 : Shifts;
                for (int s = 0; s < shiftCount; s++)
                {
                    string header = $"D{d + 1}Z{s + 1}";
                    Console.Write(header.PadRight(columnWidth));
                }
            }
            Console.WriteLine();

            // Wiersze z danymi
            for (int e = 0; e < Employees; e++)
            {
                Console.Write($"P{e + 1}".PadRight(7));
                for (int d = 0; d < Days; d++)
                {
                    int shiftCount = (d >= 5) ? 2 : Shifts;
                    for (int s = 0; s < shiftCount; s++)
                    {
                        Console.Write($"{Data[d, s, e]}".PadRight(columnWidth));
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }



    }

}
