namespace QQbry
{
    using UnityEngine;

    public class DigitUtility : MonoBehaviour
    {
        //[SerializeField] private ScoreCounter scoreCounter;

        public bool IsDigitChanged(int digit)                           // 각 자릿수가 바뀌었는지 확인하는 메서드
        {
            int beforeDigit = GameManager.Instance.beforeTotalScore / digit % 10;    // ScoreCounter 클래스에 있는 beforeScore의 자릿수를 빼옵니다.
            int afterDigit = GameManager.Instance.afterTotalScore / digit % 10;      // 같은 클래스에 있는 afterScore의 자릿수를 빼옵니다.

            return beforeDigit != afterDigit;                           // 자릿수가 다르면 메서드에서 true를 반환합니다.
        }

    }
}