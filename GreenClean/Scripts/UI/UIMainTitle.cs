using UnityEngine;
using UnityEngine.UI;

public class UIMainTitle : MonoBehaviour
{
    [Header("화면 전환 영상 프리팹")]
    [SerializeField]
    private GameObject videoTransitionPrefab;

    [SerializeField]
    private Button gameStartBtn;

    [SerializeField]
    private Button settingsBtn;

    [SerializeField]
    private Button quiteBtn;

    private void Awake()
    {
        //gameStartBtn.onClick.AddListener(() =>
        //    GameSceneManager.Instance.ChangeScene(SceneType.GAME)
        //);
        //settingsBtn.onClick.AddListener(() =>
        //    GameSceneManager.Instance.ChangeScene(SceneType.SETTINGS)
        //);
        //quiteBtn.onClick.AddListener(() => GameSceneManager.Instance.QuitGame());
        gameStartBtn.onClick.AddListener(() =>
        {
            // 클릭 효과음이 있다면 재생 (없어도 무방)
            GameSceneManager.Instance.PlayClickSound();

            if (videoTransitionPrefab != null)
            {
                // 영상 프리팹 소환! (소환되면 영상이 알아서 재생되고 뒤에서 씬을 넘겨줍니다)
                Instantiate(videoTransitionPrefab);
            }
            else
            {
                // 혹시 인스펙터에 프리팹 넣는 걸 깜빡했을 때를 대비한 안전장치 (그냥 씬 넘김)
                GameSceneManager.Instance.ChangeScene(SceneType.GAME);
            }
        });
        settingsBtn.onClick.AddListener(() =>
            GameSceneManager.Instance.ChangeScene(SceneType.SETTINGS)
        );
        quiteBtn.onClick.AddListener(() => GameSceneManager.Instance.QuitGame());

    }
}
