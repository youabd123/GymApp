using GymApp.Input;
using GymApp.Models;
using GymApp.Services;

namespace GymApp;

public class GymApplication
{
    private readonly ProgressionService _progressionService;
    private readonly StorageService _storage;
    private readonly GymData _data;
    private readonly WorkoutService _workoutService;
    private readonly ConsoleReader _reader;

    public GymApplication()
    {
        _storage = new StorageService();
        _data = _storage.LoadData();
        _workoutService = new WorkoutService();
        _progressionService = new ProgressionService();
        _reader = new ConsoleReader();
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
                    ViewExercises();
                    break;

                case "5":
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
        Console.WriteLine("4. View Exercises");
        Console.WriteLine("5. Exit");
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

            if (!InputParser.TryParseMenuIndex(input, _data.Exercises.Count, out int index))
            {
                Console.WriteLine("Invalid choice.");
                Pause();
                continue;
            }

            var chosenExercise = _data.Exercises[index - 1];
            ShowExerciseProgress(chosenExercise.Id);

            // Picking the same exercise twice in one session tops up the existing entry
            // instead of creating a second one for the same ExerciseId.
            var entry = session.Entries.FirstOrDefault(e => e.ExerciseId == chosenExercise.Id);
            bool isNewEntry = entry is null;

            entry ??= new ExerciseEntry
            {
                ExerciseId = chosenExercise.Id,
                ExerciseName = chosenExercise.Name
            };

            while (true)
            {
                Console.WriteLine($"\n--- {chosenExercise.Name} ---");
                Console.WriteLine($"Sets logged this session: {entry.Sets.Count}");
                Console.WriteLine("1. Log set");
                Console.WriteLine("2. Back to exercise menu");
                Console.Write("Choose: ");
                string setChoice = Console.ReadLine() ?? "";

                if (setChoice == "2") break;

                if (setChoice != "1")
                {
                    Console.WriteLine("Invalid choice.");
                    continue;
                }

                int? reps = _reader.ReadReps("Reps (blank to cancel): ");
                if (reps is null)
                {
                    Console.WriteLine("Set cancelled.");
                    continue;
                }

                decimal? weight = _reader.ReadWeight("Weight in kg (blank to cancel): ");
                if (weight is null)
                {
                    Console.WriteLine("Set cancelled.");
                    continue;
                }

                entry.Sets.Add(new SetEntry { Reps = reps.Value, Weight = weight.Value });
                Console.WriteLine($"  Set logged: {reps} reps @ {weight} kg");
            }

            if (isNewEntry && entry.Sets.Count > 0)
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

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Name cannot be empty.");
            Pause();
            return;
        }

        bool exerciseExists = _data.Exercises.Any(exercise =>
            exercise.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (exerciseExists)
        {
            Console.WriteLine($"Exercise '{name}' already exists.");
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

        if (_data.Sessions.Count == 0)
        {
            Console.WriteLine("No workout history found.");
            Pause();
            return;
        }

        var sessions = _data.Sessions
            .OrderByDescending(session => session.Date)
            .ToList();

        for (int i = 0; i < sessions.Count; i++)
        {
            var session = sessions[i];

            Console.WriteLine($"\nWorkout {i + 1} - {session.Date:yyyy-MM-dd HH:mm}");

            if (session.Entries.Count == 0)
            {
                Console.WriteLine(" No exercises logged.");
                continue;
            }

            foreach (var entry in session.Entries)
            {
                Console.WriteLine($" {entry.ExerciseName}");

                foreach (var set in entry.Sets)
                {
                    Console.WriteLine($"   - {set.Reps} reps @ {set.Weight} kg");
                }
            }
        }

        Pause();
    }
        

    private void ViewExercises() 
    {
        Console.Clear();
        Console.WriteLine("==== EXERCISES ====");

        if (_data.Exercises.Count == 0)
        {
            Console.WriteLine("No exercises found.");
            Pause();
            return;
        }

        for (int i = 0; i < _data.Exercises.Count; i++)
        {
            var exercise = _data.Exercises[i];

            int usageCount = _data.Sessions.Count(session =>
                session.Entries.Any(entry => entry.ExerciseId == exercise.Id));

            Console.WriteLine($"{i + 1}. {exercise.Name} (Used {usageCount} times)");
        }

        Pause();
    }
    private void ShowExerciseProgress(Guid exerciseId)
    {
        var latestEntry = _progressionService.GetLatestEntryForExercise(_data, exerciseId);
        var bestSet = _progressionService.GetBestSetForExercise(_data, exerciseId);
        var totalSets = _progressionService.GetTotalSetsForExercise(_data, exerciseId);
        var totalVolume = _progressionService.GetTotalVolumeForExercise(_data, exerciseId);

        Console.WriteLine();

        if (latestEntry == null)
        {
            Console.WriteLine("No previous results for this exercise.");
            return;
        }

        Console.WriteLine("Previous results:");

        Console.WriteLine("Latest session:");
        foreach (var set in latestEntry.Sets)
        {
            Console.WriteLine($"- {set.Reps} reps @ {set.Weight} kg");
        }

        if (bestSet != null)
        {
            Console.WriteLine($"Best set: {bestSet.Reps} reps @ {bestSet.Weight} kg");
        }

        Console.WriteLine($"Total sets: {totalSets}");
        Console.WriteLine($"Total volume: {totalVolume} kg");
    }

    private void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
