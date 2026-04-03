using GymApp.Models;

namespace GymApp.Services;

public class WorkoutService
{
    public WorkoutSession CreateSession()
    {
        return new WorkoutSession
        {
            Date = DateTime.Now
        };
    }

    public void SaveSession(GymData data, WorkoutSession session)
    {
        if (session.Entries.Count > 0)
        {
            data.Sessions.Add(session);
        }
    }
}
