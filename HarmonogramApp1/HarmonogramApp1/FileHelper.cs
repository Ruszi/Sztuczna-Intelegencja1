using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HarmonogramApp1
{
    public static class FileHelper
    {
      

        // Zapis preferencji pracownika (ilu współpracowników chce na zmianie)
        public static void SaveCoworkerPreferences(string filePath, Employee emp, int days, int shifts)
        {
            using StreamWriter sw = new StreamWriter(filePath);

            // Nagłówki
            for (int d = 0; d < days; d++)
            {
                for (int s = 0; s < shifts; s++)
                {
                    sw.Write($"D{d + 1}Z{s + 1}\t");
                }
            }
            sw.WriteLine();

            // Życzenia pracownika co do liczby współpracowników
            sw.Write("IP\t");
            for (int d = 0; d < days; d++)
            {
                for (int s = 0; s < shifts; s++)
                {
                    if (emp.PreferredCoworkers != null && emp.PreferredCoworkers.TryGetValue((d, s), out int count))
                    {
                        sw.Write($"{count}\t");
                    }
                    else
                    {
                        sw.Write("\t");
                    }
                }
            }

            sw.WriteLine();
        }

        // Zapis preferencji pracodawcy (ilu pracowników chce na każdej zmianie)
        public static void SaveEmployerPreferences(string filePath, EmployerRequirements reqs, int days, int shifts)
        {
            using StreamWriter sw = new StreamWriter(filePath);

            // Nagłówki
            for (int d = 0; d < days; d++)
            {
                for (int s = 0; s < shifts; s++)
                {
                    sw.Write($"D{d + 1}Z{s + 1}\t");
                }
            }
            sw.WriteLine();

            // Preferencje pracodawcy
            sw.Write("ER\t");
            for (int d = 0; d < days; d++)
            {
                for (int s = 0; s < shifts; s++)
                {
                    int required = reqs.GetRequired(d, s);
                    sw.Write($"{required}\t");
                }
            }

            sw.WriteLine();
        }

        public static void SaveShiftAvailabilityMatrix(string filePath, List<Employee> employees, int days, int shifts)
        {
            using StreamWriter sw = new StreamWriter(filePath);

            // Nagłówki
            for (int d = 0; d < days; d++)
            {
                for (int s = 0; s < shifts; s++)
                {
                    sw.Write($"D{d + 1}Z{s + 1}\t");
                }
            }
            sw.WriteLine();

            // Wiersze dla każdego pracownika
            for (int i = 0; i < employees.Count; i++)
            {
                var emp = employees[i];
                sw.Write($"p{i + 1}\t");

                for (int d = 0; d < days; d++)
                {
                    for (int s = 0; s < shifts; s++)
                    {
                        bool allowed = !emp.UnpreferredDays.Contains(d) && !emp.UnpreferredShifts.Contains(s);
                        sw.Write((allowed ? "1" : "0") + "\t");
                    }
                }

                sw.WriteLine();
            }
        }


        

    }
}