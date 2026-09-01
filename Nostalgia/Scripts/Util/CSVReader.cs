using System.Collections.Generic; // List 사용
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    public TextAsset csvFile;

    public string[] GetThirdColumnData()
    {
        if (csvFile == null)
        {
            Debug.LogError("CSV 파일이 없습니다!");
            return new string[0];
        }

        List<List<string>> parsedData = SimpleCSVParser.Parse(csvFile.text);
        List<string> thirdColumn = new List<string>();

        foreach (List<string> row in parsedData)
        {
            if (row.Count >= 3)
            {
                if (row.Count > 3)
                {
                    // 3열 이후 데이터 합침
                    string merged = string.Join(",", row.GetRange(2, row.Count - 2));
                    thirdColumn.Add(merged.Trim());
                }
                else
                {
                    thirdColumn.Add(row[2].Trim());
                }
            }
        }

        return thirdColumn.ToArray();
    }
}