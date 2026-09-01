using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class JsonUtility
{
    private static readonly string SaveDirectory = Path.Combine(Application.persistentDataPath, "json_data");

    private static readonly JsonSerializerSettings JsonConvertSettings = new()
    {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Include,
    };

    /// <summary>
    /// %userprofile%\AppData\LocalLow\<companyname>\<productname> 경로에 json을 저장합니다.
    /// </summary>
    /// <param name="data">저장할 데이터 클래스입니다.</param>
    /// <typeparam name="T"></typeparam>
    public static void Save<T>(T data) where T : class
    {
        if (!Directory.Exists(SaveDirectory))
        {
            Directory.CreateDirectory(SaveDirectory);
        }

        string path = JsonPath(typeof(T));
        string json = JsonConvert.SerializeObject(data, JsonConvertSettings);
        File.WriteAllText(path, json);
    }

    /// <summary>
    /// %userprofile%\AppData\LocalLow\<companyname>\<productname> 에서 로드를 시도합니다.
    /// </summary>
    /// <param name="data">로드한 결과 클래스입니다. 기본값은 null입니다.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>로드에 성곻하면 true, 실패하면 false를 반환합니다.</returns>
    public static bool TryLoad<T>(out T data) where T : class
    {
        data = null;
        string jsonPath = JsonPath(typeof(T));

        if (!File.Exists(jsonPath))
        {
            DebugConsole.LogWarning($"[JsonUtility] json 파일이 존재하지 않습니다: {jsonPath}");
            return false;
        }

        string json = File.ReadAllText(jsonPath);
        data = JsonConvert.DeserializeObject<T>(json, JsonConvertSettings);
        return data != null;
    }

#if UNITY_EDITOR
    public static void DeleteAllJsonFiles()
    {
        if (!Directory.Exists(SaveDirectory))
        {
            return;
        }

        Directory.Delete(SaveDirectory, true);
    }
#endif

    private static string JsonPath(System.Type type)
    {
        return Path.Combine(SaveDirectory, $"{type.Name.ToSnakeCase()}.json");
    }
}