using GymApp.Models;

namespace GymApp.Tests;

/// <summary>
/// Small builders so the tests read as data rather than as object construction.
/// </summary>
internal static class TestData
{
    public static Exercise Exercise(string name = "Bench Press") => new() { Name = name };

    public static SetEntry Set(int reps, decimal weight) => new() { Reps = reps, Weight = weight };

    public static ExerciseEntry Entry(Exercise exercise, params SetEntry[] sets) => new()
    {
        ExerciseId = exercise.Id,
        ExerciseName = exercise.Name,
        Sets = sets.ToList()
    };

    public static WorkoutSession Session(DateTime date, params ExerciseEntry[] entries) => new()
    {
        Date = date,
        Entries = entries.ToList()
    };

    public static GymData Data(IEnumerable<Exercise> exercises, params WorkoutSession[] sessions) => new()
    {
        Exercises = exercises.ToList(),
        Sessions = sessions.ToList()
    };
}
