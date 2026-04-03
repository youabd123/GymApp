using System.Text.Json;
using GymApp.Models;

namespace GymApp.Services;

public class StorageService
{
    private readonly string _filePath = "Data/gymdata.json";

    public GymData LoadData()
    {
        if (!File.Exists(_filePath))
        {
            return new GymData();
        }

        var json = File.ReadAllText(_filePath);

        if (string.IsNullOrWhiteSpace(json) || json == "{}")
        {
            return new GymData();
        }

        return JsonSerializer.Deserialize<GymData>(json) ?? new GymData();
    }

    public void SaveData(GymData data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        File.WriteAllText(_filePath, json);
    }
}