using System.Text.RegularExpressions;

public static class StringExtensions
{
    /// <summary>
    /// 문자열을 스네이크 표기법으로 변경하는 확장메소드입니다.
    /// </summary>
    /// <param name="str">변경할 대상 문자열</param>
    /// <returns>스네이크 표기법으로 변경된 문자열이 반환됩니다. null이거나 공백이면 입력값을 그대로 반환합니다.</returns>
    public static string ToSnakeCase(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }
        
        string result = Regex.Replace(str, "([a-z0-9])([A-Z])", "$1_$2");
        result = Regex.Replace(result, "([A-Z]+)([A-Z][a-z])", "$1_$2");
        return result.ToLowerInvariant();
    }
}
