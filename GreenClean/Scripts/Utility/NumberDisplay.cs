namespace QQbry
{
    using UnityEngine;
    using TMPro;

    public class NumberDisplay : MonoBehaviour
    {
        [SerializeField] private NumberAnimator numberAnimator;
        [SerializeField] private DigitUtility digitUtility;
        //[SerializeField] private ScoreCounter scoreCounter;

        [SerializeField] private TMP_Text digitPrefab;              // 자릿수 프리팹을 하나 만듭니다. 이 프리팹은 아래의 메서드로 여러 개를 복제시켜 사용합니다.
        [SerializeField] private Transform digitParent;             // 프리팹의 부모 계층을 하나 만듭니다. 부모 계층에 클래스를 연결합니다.
        [SerializeField] private int digitHowMany = 3;              // 자릿수를 에디터에서도 바꿀 수 있도록 칸을 만듭니다. 점수는 6자리, 정화 타일 개수는 2자리가 좋을 것 같습니다.

        private TMP_Text[] digitText;                           // 0을 표시할 배열입니다.
        public TMP_Text[] DigitText => digitText;               // digitText를 읽기 위한 구문입니다.

        private void Awake()        // 게임이 시작될 때 제어합니다.
        {
            SetDigit();             // SetDigit()을 실행합니다. 이 메서드는 아래에서 설명합니다.
        }

        private void SetDigit()                                                 // 게임이 시작될 때, 점수가 표시되는 텍스트를 만듭니다.
        {
            digitText = new TMP_Text[digitHowMany];                             // 0을 표시할 자릿수의 개수는 digitHowMany입니다.
            for (int i = 0; i < digitHowMany; i++)                              // 각 자리에 처리할 구문입니다.
            {
                TMP_Text newDigitText = Instantiate(digitPrefab, digitParent);     // for 구문에서만 쓸 digitText 텍스트를 선언합니다. digitPrefab을 여러 개로 만들 것이고, 제어는 digitParent에서 합니다.
                newDigitText.text = "0";                                           // 자릿수에 초기에 들어갈 숫자는 0 입니다.
                this.digitText[i] = newDigitText;                                  // 만들어진 여러 개의 digitText에 0을 넣습니다.
            }
        }

        public void ChangeDigit()                                       // 각 자릿수의 숫자를 바꾸는 메서드
        {
            Debug.Log("ChangeDigit 진입");
            int digit = 1;                                              // 1의 자릿수로 초기화합니다.

            for (int i = 0; i < digitHowMany; i++)                      // digitHowMany로 정한 자릿수 개수만큼 반복합니다.
            {                    
                int num = GameManager.Instance.afterTotalScore / digit % 10;     // num 변수는 계산 이후 점수의 자릿수입니다.
                digitText[i].text = num.ToString();                 // digitText 배열의 i번째 칸에 자릿수를 넣습니다.

                if (digitUtility.IsDigitChanged(digit))                 // DigitUtility의 메서드 중 자릿수가 바뀌었는지 확인하는 메서드를 호출합니다.
                {
                    numberAnimator.PlayAnimation(i);                    // 그리고 애니메이션을 재생합니다.
                }
                digit *= 10;                                            // 자릿수를 하나 올립니다.
            }
        }
    }
}
