using GymApp.Input;

namespace GymApp.Tests;

public class InputParserTests
{
    [Theory]
    [InlineData("82.5")]
    [InlineData("82,5")]
    [InlineData(" 82,5 ")]
    public void TryParseWeight_AcceptsBothDecimalSeparators(string input)
    {
        Assert.True(InputParser.TryParseWeight(input, out var weight));
        Assert.Equal(82.5m, weight);
    }

    [Fact]
    public void TryParseWeight_AcceptsZeroForBodyweightExercises()
    {
        Assert.True(InputParser.TryParseWeight("0", out var weight));
        Assert.Equal(0m, weight);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("-5")]
    [InlineData("1001")]
    [InlineData("1,234.5")]
    public void TryParseWeight_RejectsInvalidInput(string? input)
    {
        Assert.False(InputParser.TryParseWeight(input, out var weight));
        Assert.Equal(0m, weight);
    }

    [Theory]
    [InlineData("8", 8)]
    [InlineData(" 12 ", 12)]
    [InlineData("1000", 1000)]
    public void TryParseReps_AcceptsWholeNumbersInRange(string input, int expected)
    {
        Assert.True(InputParser.TryParseReps(input, out var reps));
        Assert.Equal(expected, reps);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-5")]
    [InlineData("1001")]
    [InlineData("8.5")]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData(null)]
    public void TryParseReps_RejectsInvalidInput(string? input)
    {
        Assert.False(InputParser.TryParseReps(input, out var reps));
        Assert.Equal(0, reps);
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("3", 3)]
    public void TryParseMenuIndex_AcceptsValueWithinRange(string input, int expected)
    {
        Assert.True(InputParser.TryParseMenuIndex(input, itemCount: 3, out var index));
        Assert.Equal(expected, index);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("4")]
    [InlineData("x")]
    [InlineData("")]
    public void TryParseMenuIndex_RejectsValueOutsideRange(string input)
    {
        Assert.False(InputParser.TryParseMenuIndex(input, itemCount: 3, out var index));
        Assert.Equal(0, index);
    }
}
