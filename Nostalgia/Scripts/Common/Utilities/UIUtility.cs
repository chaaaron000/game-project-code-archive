using TMPro;

public static class UIUtility
{
    /// <summary>
    /// TMP_Dropdown 원하는 데이터로 값을 변경하는 메소드
    /// </summary>
    /// <param name="dropdown">값을 변경하고 싶은 TMP_Dropdown</param>
    /// <param name="target">타겟 데이터</param>
    public static bool SetDropdownValueToTarget(TMP_Dropdown dropdown, string target)
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            var dropdownData = dropdown.options[i].text;
            if (Equals(target, dropdownData))
            {
                dropdown.value = i;
                return true;
            }
        }

        // 찾는 데이터가 없는 경우
        dropdown.value = 0;
        return false;
    }
}
