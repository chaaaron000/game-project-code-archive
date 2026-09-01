using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class JsonUtilityTestEditor
{
    private const string MenuPath = "Tools/Tests/Json Utility/Run Editor Test";

    [MenuItem(MenuPath)]
    private static void Run()
    {
        string testJsonPath = GetJsonPath(typeof(JsonUtilityEditorTestData));
        string missingJsonPath = GetJsonPath(typeof(JsonUtilityEditorMissingData));

        try
        {
            DeleteTestFile(testJsonPath);
            DeleteTestFile(missingJsonPath);

            TestTryLoadReturnsFalseWhenFileDoesNotExist();
            TestSaveCreatesJsonFile(testJsonPath);
            TestTryLoadRestoresSavedData();

            Debug.Log($"[JsonUtilityTestEditor] All tests passed. Test file: {testJsonPath}");
        }
        catch (Exception exception)
        {
            Debug.LogError($"[JsonUtilityTestEditor] Test failed: {exception.Message}");
            Debug.LogException(exception);
        }
        finally
        {
            DeleteTestFile(testJsonPath);
            DeleteTestFile(missingJsonPath);
        }
    }

    private static void TestTryLoadReturnsFalseWhenFileDoesNotExist()
    {
        bool loaded = global::JsonUtility.TryLoad<JsonUtilityEditorMissingData>(out JsonUtilityEditorMissingData data);

        AssertFalse(loaded, "TryLoad should return false when the json file does not exist.");
        AssertTrue(data == null, "TryLoad should set data to null when the json file does not exist.");
    }

    private static void TestSaveCreatesJsonFile(string testJsonPath)
    {
        var expected = CreateTestData();

        global::JsonUtility.Save(expected);

        AssertTrue(File.Exists(testJsonPath), $"Save should create a json file. Path: {testJsonPath}");

        string json = File.ReadAllText(testJsonPath);
        AssertTrue(json.Contains("\"name\": \"Green Clean\""), "Saved json should contain the string field.");
        AssertTrue(json.Contains("\"score\": 42"), "Saved json should contain the int field.");
        AssertTrue(json.Contains("\"optionalNote\": null"), "Saved json should include null fields.");
    }

    private static void TestTryLoadRestoresSavedData()
    {
        bool loaded = global::JsonUtility.TryLoad<JsonUtilityEditorTestData>(out JsonUtilityEditorTestData actual);

        AssertTrue(loaded, "TryLoad should return true after Save.");
        AssertTrue(actual != null, "TryLoad should deserialize data after Save.");
        AssertTrue(actual.name == "Green Clean", "Loaded string field should match.");
        AssertTrue(actual.score == 42, "Loaded int field should match.");
        AssertTrue(actual.optionalNote == null, "Loaded null field should match.");
        AssertTrue(actual.nested != null, "Loaded nested data should not be null.");
        AssertTrue(actual.nested.enabled, "Loaded nested bool field should match.");
        AssertTrue(Mathf.Approximately(actual.nested.ratio, 0.75f), "Loaded nested float field should match.");
    }

    private static JsonUtilityEditorTestData CreateTestData()
    {
        return new JsonUtilityEditorTestData
        {
            name = "Green Clean",
            score = 42,
            optionalNote = null,
            nested = new JsonUtilityEditorNestedData
            {
                enabled = true,
                ratio = 0.75f,
            },
        };
    }

    private static string GetJsonPath(Type type)
    {
        return Path.Combine(Application.persistentDataPath, "json_data", $"{type.Name.ToSnakeCase()}.json");
    }

    private static void DeleteTestFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static void AssertTrue(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static void AssertFalse(bool condition, string message)
    {
        AssertTrue(!condition, message);
    }

    [Serializable]
    private sealed class JsonUtilityEditorTestData
    {
        public string name;
        public int score;
        public string optionalNote;
        public JsonUtilityEditorNestedData nested;
    }

    [Serializable]
    private sealed class JsonUtilityEditorNestedData
    {
        public bool enabled;
        public float ratio;
    }

    [Serializable]
    private sealed class JsonUtilityEditorMissingData
    {
        public string value;
    }
}
