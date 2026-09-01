using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public static class SimpleCSVParser
{
    /// <summary>
    /// CSV 텍스트를 2차원 문자열 리스트로 파싱합니다.
    /// 셀 내부의 줄바꿈, 쉼표, 따옴표 등을 지원합니다.
    /// </summary>
    public static List<List<string>> Parse(string csvText)
    {
        List<List<string>> result = new List<List<string>>();
        List<string> currentRow = new List<string>();
        StringBuilder currentCell = new StringBuilder();

        bool insideQuote = false;
        int i = 0;

        while (i < csvText.Length)
        {
            char c = csvText[i];

            if (c == '"')
            {
                if (insideQuote && i + 1 < csvText.Length && csvText[i + 1] == '"')
                {
                    // "" → " (escaped quote)
                    currentCell.Append('"');
                    i++; // skip next quote
                }
                else
                {
                    // Toggle quote state
                    insideQuote = !insideQuote;
                }
            }
            else if (c == ',' && !insideQuote)
            {
                currentRow.Add(currentCell.ToString());
                currentCell.Clear();
            }
            else if ((c == '\n' || c == '\r') && !insideQuote)
            {
                if (c == '\r' && i + 1 < csvText.Length && csvText[i + 1] == '\n')
                    i++; // skip \n after \r

                currentRow.Add(currentCell.ToString());
                currentCell.Clear();
                result.Add(currentRow);
                currentRow = new List<string>();
            }
            else
            {
                currentCell.Append(c);
            }

            i++;
        }

        // 마지막 셀과 행 추가
        currentRow.Add(currentCell.ToString());
        result.Add(currentRow);

        return result;
    }
}

