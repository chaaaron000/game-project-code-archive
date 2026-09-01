using System.IO;
using UnityEngine;

namespace Nostal.Settings
{
    public abstract class SettingsSO : ScriptableObject
    {
        public virtual void Load()
        {
            string path = GetSavePath();
            
            if (!File.Exists(path))
            {
                Save();
                return;
            }

            string json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, this);
        }

        protected void Save()
        {
            string path = GetSavePath();
            string json = JsonUtility.ToJson(this, true);
            File.WriteAllText(path, json);
        }

        private string GetSavePath()
        {
            return Path.Combine(Application.persistentDataPath, $"{name}.json");
        }
    }
}