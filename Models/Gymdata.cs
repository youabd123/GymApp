namespace GymApp.Models;

public class GymData
{
    public List<Exercise> Exercises { get; set; } = new();
    public List<WorkoutSession> Sessions { get; set; } = new();
}