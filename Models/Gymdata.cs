namespace GymApp.Models;

public class GymData
{
    /// <summary>
    /// Version of the on-disk data shape. Bumped when the model changes so that older
    /// files can be recognised and migrated instead of silently losing fields.
    /// </summary>
    public int SchemaVersion { get; set; } = 1;

    public List<Exercise> Exercises { get; set; } = new();
    public List<WorkoutSession> Sessions { get; set; } = new();
}