namespace GymApp.Models;

public class ExerciseEntry
{
    public Guid ExerciseId { get; set; }
    public string ExerciseName { get; set; } = "";
    public List<SetEntry> Sets { get; set; } = new();
}