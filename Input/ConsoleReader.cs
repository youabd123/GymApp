namespace GymApp.Input;

/// <summary>
/// Thin console wrapper over <see cref="InputParser"/>. Re-prompts on invalid input
/// instead of silently substituting a default value. A blank line cancels and returns null.
/// </summary>
public class ConsoleReader
{
    public string? ReadText(string prompt)
    {
        Console.Write(prompt);
        var input = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(input) ? null : input;
    }

    public int? ReadReps(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (InputParser.TryParseReps(input, out var reps))
            {
                return reps;
            }

            Console.WriteLine(
                $"Invalid reps. Enter a whole number between {InputParser.MinReps} and {InputParser.MaxReps}, " +
                "or leave blank to cancel.");
        }
    }

    /// <param name="blankIsZero">
    /// When true a blank line means 0 kg (bodyweight) rather than cancelling.
    /// </param>
    public decimal? ReadWeight(string prompt, bool blankIsZero = false)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                return blankIsZero ? 0m : null;
            }

            if (InputParser.TryParseWeight(input, out var weight))
            {
                return weight;
            }

            Console.WriteLine(
                $"Invalid weight. Enter a number between {InputParser.MinWeight} and {InputParser.MaxWeight} " +
                "(e.g. 82.5 or 82,5), or leave blank to cancel.");
        }
    }

    public bool Confirm(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (y/n): ");
            var input = Console.ReadLine()?.Trim().ToLowerInvariant();

            switch (input)
            {
                case "y" or "yes" or "j" or "ja":
                    return true;
                case "n" or "no" or "nej" or "":
                case null:
                    return false;
                default:
                    Console.WriteLine("Please answer y or n.");
                    break;
            }
        }
    }
}
