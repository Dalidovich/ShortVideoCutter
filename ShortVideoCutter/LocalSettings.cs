using System.Text.Json;

namespace ShortVideoCutter;

public class LocalSettings()
{
    public string TextData { get; set; } = string.Empty;
    public string SaveDirectory { get; set; } = string.Empty;
    public string ExportDir { get; set; } = string.Empty;

    private const string _fileName = "settings.json";

    private string _settingsPath => Path.Combine(Directory.GetCurrentDirectory(), _fileName);

    private void _Save()
    {
        var settings = JsonSerializer.Serialize(this);
        File.WriteAllText(_settingsPath, settings);
    }

    public static LocalSettings Load()
    {
        var localSettings = new LocalSettings();
        if (!File.Exists(localSettings._settingsPath))
        {
            localSettings._Save();
            return null;
        }

        var settingsRaw = File.ReadAllText(localSettings._settingsPath);
        var settings = JsonSerializer.Deserialize<LocalSettings>(settingsRaw);
        localSettings.TextData = settings.TextData.ToString();
        localSettings.SaveDirectory = settings.SaveDirectory.ToString();
        localSettings.ExportDir = settings.ExportDir .ToString();

        Console.WriteLine(localSettings.ToString());
        if (!string.IsNullOrEmpty(localSettings.TextData) && 
            !string.IsNullOrEmpty(localSettings.SaveDirectory) && 
            !string.IsNullOrEmpty(localSettings.ExportDir))
        {
            return localSettings;
        }

        return null;
    }

    public override string ToString()
    {
        return $"TextData:{TextData}\tSaveDirectory:{SaveDirectory}\tExportDir:{ExportDir}";
    }
}