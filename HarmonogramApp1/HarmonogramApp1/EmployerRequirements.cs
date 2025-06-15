using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonogramApp1
{

    public class EmployerRequirements
    {
        // Klucz: (dzień, zmiana), wartość: ilu pracowników ma być na tej zmianie
        public Dictionary<(int day, int shift), int> RequiredEmployeesPerShift { get; set; } = new();

        public int GetRequired(int day, int shift)
        {
            return RequiredEmployeesPerShift.TryGetValue((day, shift), out var count) ? count : 2; // domyślnie 2
        }


        public void GenerateRandomRequirements(int days, int shifts, int minEmployees = 1, int maxEmployees = 3)
        {
            Random rng = new Random();

            for (int d = 0; d < days; d++)
            {
                for (int s = 0; s < shifts; s++)
                {
                    RequiredEmployeesPerShift[(d, s)] = rng.Next(minEmployees, maxEmployees + 1);
                }
            }
        }

    }


}
