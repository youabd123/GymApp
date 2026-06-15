using GymApp.Models;

namespace GymApp.Services;

public class ProgressionService
{
    public ExerciseEntry? GetLatestEntryForExercise(GymData data, Guid exerciseId)
    {
        return data.Sessions
            .OrderByDescending(session => session.Date)
            .SelectMany(session => session.Entries)
            .FirstOrDefault(entry => entry.ExerciseId == exerciseId);
    }

    public SetEntry? GetBestSetForExercise(GymData data, Guid exerciseId)
    {
        return data.Sessions
            .SelectMany(session => session.Entries)
            .Where(entry => entry.ExerciseId == exerciseId)
            .SelectMany(entry => entry.Sets)
            .OrderByDescending(set => set.Weight)
            .ThenByDescending(set => set.Reps)
            .FirstOrDefault();
    }

    public decimal GetTotalVolumeForExercise(GymData data, Guid exerciseId)
    {
        return data.Sessions
            .SelectMany(session => session.Entries)
            .Where(entry => entry.ExerciseId == exerciseId)
            .SelectMany(entry => entry.Sets)
            .Sum(set => set.Weight * set.Reps);
    }

    public int GetTotalSetsForExercise(GymData data, Guid exerciseId)
    {
        return data.Sessions
            .SelectMany(session => session.Entries)
            .Where(entry => entry.ExerciseId == exerciseId)
            .SelectMany(entry => entry.Sets)
            .Count();
    }
}