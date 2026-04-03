using System;
using System.Collections.Generic;

namespace GymApp.Models;

public class WorkoutSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; } = DateTime.Now;

    public List<ExerciseEntry> Entries { get; set; } = new();
}
