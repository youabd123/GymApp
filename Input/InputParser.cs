using System.Globalization;

namespace GymApp.Input;

/// <summary>
/// Pure parsing and validation of user input. Kept free of Console calls so it can be unit tested.
/// </summary>
public static class InputParser
{
    public const int MinReps = 1;
    public const int MaxReps = 1000;
    public const decimal MinWeight = 0m;
    public const decimal MaxWeight = 1000m;

    /// <summary>
    /// Accepts both "82.5" and "82,5" regardless of the machine's locale.
    /// Rejects blanks, non-numbers, negative weights and implausibly large weights.
    /// A weight of 0 is valid — it represents a bodyweight exercise.
    /// </summary>
    public static bool TryParseWeight(string? input, out decimal weight)
    {
        weight = 0m;

        var trimmed = input?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            return false;
        }

        // Both separators present is ambiguous ("1,234.5") — reject rather than guess.
        if (trimmed.Contains(',') && trimmed.Contains('.'))
        {
            return false;
        }

        var normalized = trimmed.Replace(',', '.');

        if (!decimal.TryParse(
                normalized,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out var parsed))
        {
            return false;
        }

        if (parsed < MinWeight || parsed > MaxWeight)
        {
            return false;
        }

        weight = parsed;
        return true;
    }

    /// <summary>
    /// Reps must be a whole number between <see cref="MinReps"/> and <see cref="MaxReps"/>.
    /// </summary>
    public static bool TryParseReps(string? input, out int reps)
    {
        reps = 0;

        var trimmed = input?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            return false;
        }

        if (!int.TryParse(
                trimmed,
                NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out var parsed))
        {
            return false;
        }

        if (parsed < MinReps || parsed > MaxReps)
        {
            return false;
        }

        reps = parsed;
        return true;
    }

    /// <summary>
    /// Parses a 1-based menu choice and checks it against the number of available items.
    /// </summary>
    public static bool TryParseMenuIndex(string? input, int itemCount, out int index)
    {
        index = 0;

        var trimmed = input?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            return false;
        }

        if (!int.TryParse(trimmed, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var parsed))
        {
            return false;
        }

        if (parsed < 1 || parsed > itemCount)
        {
            return false;
        }

        index = parsed;
        return true;
    }
}
