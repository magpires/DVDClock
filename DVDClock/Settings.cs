using System;
using System.IO;
using System.Text.Json;
using System.Drawing;

namespace DVDClock
{
    public class Settings
    {
        public int SpeedIndex { get; set; } = 2; // 0 = Muito lenta, 1 = Lenta, 2 = Normal, 3 = Rápida, 4 = Muito rápida
        public string ClockColorHex { get; set; } = "#FFFFFF";

        public static string SettingsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DVDClock", "settings.json");

        public static Settings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
                }
            }
            catch
            {
                // Ignore and return default
            }
            return new Settings();
        }

        public void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(SettingsPath);
                if (dir != null && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch
            {
                // Ignore on fail
            }
        }

        public Color GetClockColor()
        {
            try
            {
                return ColorTranslator.FromHtml(ClockColorHex);
            }
            catch
            {
                return Color.White;
            }
        }
        
        public int GetSpeedMultiplier()
        {
            return SpeedIndex switch
            {
                0 => 1,
                1 => 2,
                2 => 4,
                3 => 7,
                4 => 10,
                _ => 4
            };
        }
    }
}
