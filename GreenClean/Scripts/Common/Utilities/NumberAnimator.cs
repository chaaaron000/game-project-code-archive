namespace QQbry
{
    using UnityEngine;
    using TMPro;
    using System.Globalization;

    public class NumberAnimator : MonoBehaviour
    {
        //[SerializeField] private ScoreCounter scoreCounter;             // ScoreCounter 클래스를 연결
        [SerializeField] private NumberDisplay numberDisplay;           // numberDisplay 클래스를 연결
        [SerializeField] private DigitUtility digitUtility;

        private bool[] isAnimating;         // 애니메이션이 실행중인지를 확인하는 변수입니다.
        private float[] currentTimes;       // 시간을 초 단위로 실행하기 위한 변수입니다.
        private float duration = 0.2f;      // 애니메이션이 재생되는 시간입니다.

        public void PlayAnimation(int index)                                            // 애니메이션을 시작하도록 신호를 주는 메서드입니다.
        {
            if (index >= isAnimating.Length) return;                                    // 메서드가 계산한 자릿수가 내가 입력한 자릿수를 넘어가는 오류를 내면 멈춥니다.
            
            currentTimes[index] = 0f;                                                   // 시간을 먼저 0으로 초기화합니다.
            isAnimating[index] = true;                                                  // 애니메이션이 실행중이라는 신호를 줘야 애니메이션을 시작합니다.
            numberDisplay.DigitText[index].transform.localScale = Vector3.one * 0.8f;   // NumberDisply 클래스에서 받은 자릿수의 크기를 0.8배로 줄입니다.
        }

        private void Start()                                                // 게임을 시작할 때 실행합니다.
        {
            isAnimating = new bool[numberDisplay.DigitText.Length];         // isAnimating을 초기화합니다.
            currentTimes = new float[numberDisplay.DigitText.Length];       // currentTimes를 초기화합니다.
        }

        private void Update()                                                               // 프레임 당 실행합니다.
        {
            for (int i = 0; i < numberDisplay.DigitText.Length; i++)                        // 내가 입력한 자릿수 개수만큼 반복합니다.
            {
                if (isAnimating[i])                                                         // i번째 배열의 숫자의 애니메이션을 실행해야 한다면
                {
                    currentTimes[i] += Time.deltaTime;                                      // 현재 시간을 초 단위로 바꿉니다.
                    float t = currentTimes[i] / duration;                                   // t 변수는 현재 시간을 애니메이션 재생 시간으로 나눈 값입니다.
                    float scale = 0.8f + 0.2f * t;                                          // 자릿수의 숫자는 0.8배에서 초당 0.2배씩 커지도록 숫자를 준비합니다.
                    numberDisplay.DigitText[i].transform.localScale = Vector3.one * scale;  // 자릿수의 숫자를 준비한 숫자만큼 키웁니다.

                    if (t >= 1f)                                                            // t는 최대 1이어야 하는데, 그보다 크거나 같은 오류가 날 때 예외처리를 합니다.
                    {
                        isAnimating[i] = false;                                             // 애니메이션을 중지시킵니다.
                        numberDisplay.DigitText[i].transform.localScale = Vector3.one;      // 자릿수의 숫자를 원래 크기로 복귀시킵니다.
                    }
                }
            }
        }
    }
}