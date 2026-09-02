using GymApp.Models;
using GymApp.Services;

namespace GymApp.Tests;

public class HistoryServiceTests
{
    private readonly HistoryService _service = new();

    [Fact]
    public void GetSessionsNewestFirst_OrdersByDateDescending()
    {
        var bench = TestData.Exercise("Bench Press");

        var oldest = TestData.Session(new DateTime(2026, 1, 1), TestData.Entry(bench, TestData.Set(8, 60m)));
        var middle = TestData.Session(new DateTime(2026, 2, 1), TestData.Entry(bench, TestData.Set(8, 65m)));
        var newest = TestData.Session(new DateTime(2026, 3, 1), TestData.Entry(bench, TestData.Set(8, 70m)));

        var data = TestData.Data([bench], oldest, newest, middle);

        var sessions = _service.GetSessionsNewestFirst(data);

        Assert.Equal([newest.Id, middle.Id, oldest.Id], sessions.Select(session => session.Id));
    }

    [Fact]
    public void GetSessionsNewestFirst_ReturnsEmptyListForEmptyHistory()
    {
        var data = TestData.Data([]);

        Assert.Empty(_service.GetSessionsNewestFirst(data));
    }

    [Fact]
    public void GetSummary_CountsExercisesAndSetsAcrossEntries()
    {
        var bench = TestData.Exercise("Bench Press");
        var squat = TestData.Exercise("Squat");

        var session = TestData.Session(
            new DateTime(2026, 3, 1),
            TestData.Entry(bench, TestData.Set(10, 60m), TestData.Set(8, 70m)),
            TestData.Entry(squat, TestData.Set(5, 100m)));

        var summary = _service.GetSummary(session);

        Assert.Equal(2, summary.ExerciseCount);
        Assert.Equal(3, summary.SetCount);
        // 10*60 + 8*70 + 5*100 = 600 + 560 + 500
        Assert.Equal(1660m, summary.TotalVolume);
    }

    [Fact]
    public void GetSummary_IgnoresEntriesWithoutSets()
    {
        var bench = TestData.Exercise("Bench Press");
        var squat = TestData.Exercise("Squat");

        var session = TestData.Session(
            new DateTime(2026, 3, 1),
            TestData.Entry(bench, TestData.Set(10, 60m)),
            TestData.Entry(squat));

        var summary = _service.GetSummary(session);

        Assert.Equal(1, summary.ExerciseCount);
        Assert.Equal(1, summary.SetCount);
        Assert.Equal(600m, summary.TotalVolume);
    }

    [Fact]
    public void GetSummary_ReturnsZeroesForSessionWithoutEntries()
    {
        var session = TestData.Session(new DateTime(2026, 3, 1));

        var summary = _service.GetSummary(session);

        Assert.Equal(0, summary.ExerciseCount);
        Assert.Equal(0, summary.SetCount);
        Assert.Equal(0m, summary.TotalVolume);
    }

    [Fact]
    public void GetSummary_CountsBodyweightSetsWithoutAddingVolume()
    {
        var pullUp = TestData.Exercise("Pull Up");

        var session = TestData.Session(
            new DateTime(2026, 3, 1),
            TestData.Entry(pullUp, TestData.Set(12, 0m), TestData.Set(10, 0m)));

        var summary = _service.GetSummary(session);

        Assert.Equal(1, summary.ExerciseCount);
        Assert.Equal(2, summary.SetCount);
        Assert.Equal(0m, summary.TotalVolume);
    }

    [Fact]
    public void GetEntryVolume_MultipliesRepsByWeight()
    {
        var bench = TestData.Exercise("Bench Press");
        var entry = TestData.Entry(bench, TestData.Set(10, 60m), TestData.Set(8, 82.5m));

        // 10*60 + 8*82.5 = 600 + 660
        Assert.Equal(1260m, _service.GetEntryVolume(entry));
    }

    [Fact]
    public void GetEntryVolume_ReturnsZeroForEntryWithoutSets()
    {
        var entry = TestData.Entry(TestData.Exercise());

        Assert.Equal(0m, _service.GetEntryVolume(entry));
    }
}
