using HarmonogramApp1;



internal class Program
{
    private static void Main(string[] args)
    {
        int days = 7;
        int shifts = 3;
        int employeesCount = 5;
        var rng = new Random();


        // Pracownicy + ich preferencje
        var employees = new List<Employee>();
        for (int i = 0; i < employeesCount; i++)
        {
            var emp = new Employee
            {
                Id = i,
                Name = $"Pracownik {i + 1}",
                PreferredShifts = new List<int> { 0 },
                PreferredDays = new List<int> { 0, 1, 2, 3 }
            };

            if (i == 0) // Pracownik 1 ma silne preferencje
            {
                emp.UnpreferredShifts = new List<int> { 2 };  // Nie chce nocy
                emp.UnpreferredDays = new List<int> {6,7}; // Nie chce weekendów
            }

            employees.Add(emp);
        }

        
        //  var schedule = new Schedule(days, shifts, employeesCount);
        // schedule.Randomize(rng);
        //  schedule.Print();

        //var evaluator = new FitnessEvaluator(employees);
        // Losowy harmonogram
        var randomSchedule = new Schedule(days, shifts, employeesCount);
        randomSchedule.Randomize(rng, employees);

        randomSchedule.Print();
        var requirements = new EmployerRequirements();

        // Przykładowe ustawienie: 3 pracowników w poniedziałek na zmianie 1, reszta po 2
        // requirements.RequiredEmployeesPerShift[(0, 0)] = 3;


        requirements.GenerateRandomRequirements(days, shifts, minEmployees: 1, maxEmployees: 3);
        // Zapis preferencji pracodawcy
        FileHelper.SaveEmployerPreferences("preferencje_pracodawcy.txt", requirements, days, shifts);

        // Zapis preferencji pracownika
        FileHelper.SaveShiftAvailabilityMatrix("dostepnosc_pracownikow.txt", employees, days, shifts);


        // Inicjalizacja evaluatora
        var evaluator = new FitnessEvaluator(employees);

        int randomFitness = evaluator.Evaluate(randomSchedule, requirements);
        Console.WriteLine($"Fitness losowego: {randomFitness}");

        // Genetyczny harmonogram

        // Test starej funkcji fitness
        var gaV1 = new GeneticAlgorithm(
            populationSize: 100,
            generations: 1000,
            mutationRate: 0.01,
            days: days,
            shifts: shifts,
            employees: employeesCount,
            employeeList: employees,
            requirements: requirements,
            evaluator: evaluator,
            useV2Fitness: false
        );
        var resultV1 = gaV1.Run();
        int fitnessV1 = evaluator.Evaluate(resultV1, requirements);
        Console.WriteLine($"Stara funkcja fitness: {fitnessV1}");
        resultV1.Print();

        // uzycie nowej funkcji 

        var gaV2 = new GeneticAlgorithm(
      populationSize: 100,
      generations: 1000,
      mutationRate: 0.01,
      days: days,
      shifts: shifts,
      employees: employeesCount,
      employeeList: employees,
      requirements: requirements,
      evaluator: evaluator,
      useV2Fitness: true // Użycie nowej funkcji fitness bo jest na true jeśli na false to będzie uzywać starej funkcji 
  );
        var resultV2 = gaV2.Run();
        int fitnessV2 = evaluator.EvaluateV2(resultV2, requirements);
        Console.WriteLine($"Nowa funkcja fitness: {fitnessV2}");
        resultV2.Print();

        // Wybór najlepszego harmonogramu
        // Oceń oba harmonogramy TĄ SAMĄ metodą fitness
        int finalFitnessV1 = evaluator.Evaluate(resultV1, requirements);
        int finalFitnessV2 = evaluator.Evaluate(resultV2, requirements);

        // Wybierz lepszy harmonogram i oblicz jego fitness
        var bestSchedule = finalFitnessV1 > finalFitnessV2 ? resultV1 : resultV2;
        int bestFitness = finalFitnessV1 > finalFitnessV2 ? finalFitnessV1 : finalFitnessV2;

        Console.WriteLine("\nNajlepszy harmonogram (oceniony starą metodą):");
        bestSchedule.Print();
        Console.WriteLine($"Jego fitness: {bestFitness}");

       /* // Dla porównania - ocena nową metodą
        Console.WriteLine("\nOcena alternatywna (nową metodą):");
        Console.WriteLine($"Harmonogram V1: {evaluator.EvaluateV2(resultV1, requirements)}");
        Console.WriteLine($"Harmonogram V2: {evaluator.EvaluateV2(resultV2, requirements)}");
       */
        /*var bestSchedule = gaV2.Run();
        bestSchedule.Print();
        //  int fitness = evaluator.Evaluate(schedule);

        int finalFitness = evaluator.Evaluate(bestSchedule, requirements);
        //  Console.WriteLine($"Fitness tego harmonogramu: {fitness}");
        Console.WriteLine($"Najlepszy fitness: {finalFitness}");*/


    }
}