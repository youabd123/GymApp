namespace GymApp.Models;

/// <summary>
/// A one-line overview of a workout session. Derived from the session on demand —
/// never persisted, so the on-disk JSON shape is unaffected.
/// </summary>
public record SessionSummary(int ExerciseCount, int SetCount, decimal TotalVolume);
