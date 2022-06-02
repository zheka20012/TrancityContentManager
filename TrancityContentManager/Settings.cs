using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TrancityContentManager
{
    public static class Settings
    {
        private const string ConfigFilename = "settings.cfg";

        private class SerializedSettings
        {
            public string executablePath;
        }

        private static SerializedSettings settings;

        public static string ExecutablePath
        {
            get => settings.executablePath;
            set => settings.executablePath = value;
        }

        public static void Load()
        {
            if (File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "/" + ConfigFilename))
            {
                string text = File.ReadAllText(Path.GetDirectoryName(Application.ExecutablePath) + "/" + ConfigFilename);

                settings = JsonConvert.DeserializeObject<SerializedSettings>(text);
            }
            else
            {
                settings = new SerializedSettings();
            }

        }

        public static void Save()
        {
            string text = JsonConvert.SerializeObject(settings);

            File.WriteAllText(Path.GetDirectoryName(Application.ExecutablePath) + "/" + ConfigFilename, text);
        }
    }
}