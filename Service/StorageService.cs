using System.Text.Json;
using GymApp.Models;

namespace GymApp.Services;

public class StorageService
{
    private const string LegacyRelativePath = "Data/gymdata.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _filePath;

    public StorageService(string? filePath = null)
    {
        _filePath = filePath ?? DefaultFilePath();
    }

    public string FilePath => _filePath;

    /// <summary>
    /// %LocalAppData%\GymApp\gymdata.json — outside both the repository and the build
    /// output, so training data survives "dotnet clean" and a Debug/Release switch.
    /// </summary>
    private static string DefaultFilePath() => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "GymApp",
        "gymdata.json");

    public GymData LoadData()
    {
        if (!File.Exists(_filePath))
        {
            MigrateLegacyFileIfPresent();
        }

        if (!File.Exists(_filePath))
        {
            return new GymData();
        }

        string json;

        try
        {
            json = File.ReadAllText(_filePath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Console.WriteLine($"Could not read '{_filePath}': {ex.Message}");
            Console.WriteLine("Starting with empty data. The existing file has not been changed.");
            return new GymData();
        }

        if (string.IsNullOrWhiteSpace(json) || json.Trim() == "{}")
        {
            return new GymData();
        }

        try
        {
            return JsonSerializer.Deserialize<GymData>(json) ?? new GymData();
        }
        catch (JsonException ex)
        {
            QuarantineCorruptFile(ex);
            return new GymData();
        }
    }

    public void SaveData(GymData data)
    {
        var json = JsonSerializer.Serialize(data, SerializerOptions);

        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Write to a temporary file first, then swap it in, so an interrupted run
        // cannot leave a half-written data file behind.
        var tempPath = _filePath + ".tmp";
        File.WriteAllText(tempPath, json);
        File.Move(tempPath, _filePath, overwrite: true);
    }

    /// <summary>
    /// Earlier versions stored data next to the executable or next to the project file,
    /// depending on how the app was started. Copy the first one found to the new location
    /// so no existing history is lost.
    /// </summary>
    private void MigrateLegacyFileIfPresent()
    {
        string[] legacyPaths =
        [
            Path.Combine(AppContext.BaseDirectory, LegacyRelativePath),
            Path.GetFullPath(LegacyRelativePath)
        ];

        foreach (var legacyPath in legacyPaths)
        {
            if (!File.Exists(legacyPath))
            {
                continue;
            }

            // The repository ships an empty placeholder — nothing worth migrating.
            var contents = TryReadAllText(legacyPath);
            if (string.IsNullOrWhiteSpace(contents) || contents.Trim() == "{}")
            {
                continue;
            }

            try
            {
                var directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.Copy(legacyPath, _filePath);
                Console.WriteLine($"Moved your training data from '{legacyPath}' to '{_filePath}'.");
                return;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                Console.WriteLine($"Could not move '{legacyPath}' to '{_filePath}': {ex.Message}");
            }
        }
    }

    private static string? TryReadAllText(string path)
    {
        try
        {
            return File.ReadAllText(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    private void QuarantineCorruptFile(JsonException ex)
    {
        var backupPath = Path.Combine(
            Path.GetDirectoryName(_filePath) ?? ".",
            $"gymdata.corrupt-{DateTime.Now:yyyyMMdd-HHmmss}.json");

        Console.WriteLine($"'{_filePath}' could not be read as valid JSON: {ex.Message}");

        try
        {
            File.Move(_filePath, backupPath, overwrite: true);
            Console.WriteLine($"The damaged file has been kept as '{backupPath}'.");
        }
        catch (Exception moveEx) when (moveEx is IOException or UnauthorizedAccessException)
        {
            Console.WriteLine($"The damaged file could not be moved aside: {moveEx.Message}");
        }

        Console.WriteLine("Starting with empty data.");
    }
}
