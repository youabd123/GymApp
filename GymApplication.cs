using GymApp.Models;
using GymApp.Services;
using System.Linq;

namespace GymApp;

public class GymApplication
{
    private readonly StorageService _storage;
    private readonly GymData _data;
    private readonly WorkoutService _workoutService;
    public GymApplication()
    {
        _storage = new StorageService();
        _data = _storage.LoadData();
        _workoutService = new WorkoutService();
    }

    public void Run()
    {
        bool isRunning = true;

        while (isRunning)
        {
            Console.Clear();
            ShowMenu();

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    StartWorkout();
                    break;

                case "2":
                    AddExercise();
                    break;

                case "3":
                    ViewHistory();
                    break;

                case "4":
                    isRunning = false;
                    Console.WriteLine("Closing GymApp...");
                    break;

                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    Pause();
                    break;
            }
        }
    }

    private void ShowMenu()
    {
        Console.WriteLine("==== GYM APP ====");
        Console.WriteLine("1. Start Workout");
        Console.WriteLine("2. Add Exercise");
        Console.WriteLine("3. View History");
        Console.WriteLine("4. Exit");
        Console.Write("Choose: ");
    }

    private void StartWorkout()
    {
        Console.Clear();
        Console.WriteLine("==== START WORKOUT ====");

        if (_data.Exercises.Count == 0)
        {
            Console.WriteLine("No exercises found. Add exercises first.");
            Pause();
            return;
        }

        var session = _workoutService.CreateSession();
        bool workoutRunning = true;

        while (workoutRunning)
        {
            Console.Clear();
            Console.WriteLine("==== WORKOUT IN PROGRESS ====");
            Console.WriteLine("Exercises:");

            for (int i = 0; i < _data.Exercises.Count; i++)
                Console.WriteLine($"{i + 1}. {_data.Exercises[i].Name}");

            Console.WriteLine("0. Finish workout");
            Console.Write("Choose exercise: ");
            string input = Console.ReadLine() ?? "";

            if (input == "0")
            {
                workoutRunning = false;
                continue;
            }

            if (!int.TryParse(input, out int index) || index < 1 || index > _data.Exercises.Count)
            {
                Console.WriteLine("Invalid choice.");
                Pause();
                continue;
            }

            var chosenExercise = _data.Exercises[index - 1];
            var entry = new ExerciseEntry
            {
                ExerciseId = chosenExercise.Id,
                ExerciseName = chosenExercise.Name
            };

            while (true)
            {
                Console.WriteLine($"\n--- {chosenExercise.Name} ---");
                Console.WriteLine("1. Log set");
                Console.WriteLine("2. Back to exercise menu");
                Console.Write("Choose: ");
                string setChoice = Console.ReadLine() ?? "";

                if (setChoice == "2") break;

                if (setChoice == "1")
                {
                    Console.Write("Reps: ");
                    if (!int.TryParse(Console.ReadLine(), out int reps) || reps <= 0)
                    {
                        Console.WriteLine("Invalid reps.");
                        continue;
                    }

                    Console.Write("Weight (kg): ");
                    if (!decimal.TryParse(Console.ReadLine(), out decimal weight)) weight = 0;

                    entry.Sets.Add(new SetEntry { Reps = reps, Weight = weight });
                    Console.WriteLine($"  Set logged: {reps} reps @ {weight} kg");
                }
            }

            if (entry.Sets.Count > 0)
                session.Entries.Add(entry);
        }

        if (session.Entries.Count > 0)
        {
            _workoutService.SaveSession(_data, session);
            _storage.SaveData(_data);
            Console.WriteLine("\nWorkout saved!");
        }
        else
        {
            Console.WriteLine("\nNo sets logged — workout not saved.");
        }

        Pause();
    }

    private void AddExercise()
    {
        Console.Clear();
        Console.WriteLine("==== ADD EXERCISE ====");
        Console.Write("Exercise name: ");
        string name = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Name cannot be empty.");
            Pause();
            return;
        }

        var exercise = new Exercise { Name = name };
        _data.Exercises.Add(exercise);
        _storage.SaveData(_data);

        Console.WriteLine($"Exercise '{name}' added!");
        Pause();
    }


    private void ViewHistory()
    {
        Console.Clear();
        Console.WriteLine("==== WORKOUT HISTORY ====");

        var sessions = _data.Sessions;

        if (sessions.Count == 0)
        {
            Console.WriteLine("No workout sessions found.");
            Pause();
            return;
        }

        foreach (var session in sessions.OrderByDescending(s => s.Date))
        {
            Console.WriteLine($"\n{session.Date:yyyy-MM-dd HH:mm}");
            foreach (var entry in session.Entries)
            {
                Console.WriteLine($"  {entry.ExerciseName}");
                foreach (var set in entry.Sets)
                    Console.WriteLine($"    {set.Reps} reps @ {set.Weight} kg");
            }
        }

        Pause();
    }
    private void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
