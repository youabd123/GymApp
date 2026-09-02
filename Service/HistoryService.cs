using GymApp.Models;

namespace GymApp.Services;

public class HistoryService
{
    public IReadOnlyList<WorkoutSession> GetSessionsNewestFirst(GymData data)
    {
        return data.Sessions
            .OrderByDescending(session => session.Date)
            .ToList();
    }

    /// <summary>
    /// Counts only entries that actually have sets, so an exercise that was opened
    /// but never logged does not inflate the exercise count.
    /// </summary>
    public SessionSummary GetSummary(WorkoutSession session)
    {
        var entriesWithSets = session.Entries
            .Where(entry => entry.Sets.Count > 0)
            .ToList();

        var sets = entriesWithSets.SelectMany(entry => entry.Sets).ToList();

        return new SessionSummary(
            ExerciseCount: entriesWithSets.Count,
            SetCount: sets.Count,
            TotalVolume: sets.Sum(set => set.Weight * set.Reps));
    }

    /// <summary>
    /// Volume for a single exercise within one session — the same reps × weight
    /// formula <see cref="ProgressionService.GetTotalVolumeForExercise"/> uses across all sessions.
    /// </summary>
    public decimal GetEntryVolume(ExerciseEntry entry)
    {
        return entry.Sets.Sum(set => set.Weight * set.Reps);
    }
}
