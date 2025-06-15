using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonogramApp1

// Pracownik 
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }




        // Pozytywne preferencje
        public List<int> PreferredShifts { get; set; } = new List<int>();
        public List<int> PreferredDays { get; set; } = new List<int>();

        // Negatywne preferencje (niechciane zmiany / dni)
        public List<int> UnpreferredShifts { get; set; } = new List<int>();
        public List<int> UnpreferredDays { get; set; } = new List<int>();

        // Nowość: preferowana liczba współpracowników na konkretnych zmianach
        public Dictionary<(int day, int shift), int> PreferredCoworkers { get; set; } = new();
    }

}
