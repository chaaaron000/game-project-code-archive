using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : SingletonComponent<SoundManager>
{
    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource bgmSource;

    [SerializeField]
    private AudioSource sfxSource;

    [Header("Audio Mixer")] //오디오 믹서를 조종하기 위해 변수
    [SerializeField]
    private AudioMixer audioMixer;

    private Dictionary<string, AudioClip> audioCache = new Dictionary<string, AudioClip>();

    protected override void Awake()
    {
        base.Awake();

        //게임 시작 시 저장된 볼륨 값을 불러와 믹서에 미리 적용합니다.
        InitVolumeSettings();
        PlayBGM("TitleBGM");
        
    }

    //private void OnEnable()
    //{
    //    GameSceneManager.Instance.SceneLoadCompleted += PlayBGMByScene;
    //}

    //private void OnDisable()
    //{
    //    GameSceneManager.Instance.SceneLoadCompleted -= PlayBGMByScene;
    //}

    // 파일 이름을 통해 BGM 재생
    public void PlayBGM(string bgmName)
    {
        // Resources/Sounds/ 폴더에서 bgmName과 일치하는 오디오 클립 로드
        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + bgmName);

        if (clip != null)
        {
            if (bgmSource.clip == clip)
                return; // 이미 재생 중이면 무시
            bgmSource.clip = clip;
            bgmSource.Play();
        }
        else
        {
            DebugConsole.LogWarning(
                $"[SoundManager] BGM 파일을 찾을 수 없습니다: Sounds/{bgmName}"
            );
        }
    }

    // 파일 이름을 통해 SFX 재생
    public void PlaySFX(string sfxName)
    {
        // 1. 보관함에 해당 이름의 사운드가 없다면 Resources 폴더에서 찾아옵니다.
        if (!audioCache.ContainsKey(sfxName))
        {
            AudioClip clip = Resources.Load<AudioClip>("Sounds/" + sfxName);
            if (clip != null)
            {
                audioCache.Add(sfxName, clip); // 찾은 파일을 보관함에 저장
            }
            else
            {
                DebugConsole.LogWarning($"사운드를 찾을 수 없습니다: {sfxName}");
                return;
            }
        }

        // 2. 보관함에 있는 사운드를 즉시 재생합니다.
        sfxSource.PlayOneShot(audioCache[sfxName]);
    }

    private void InitVolumeSettings()
    {
        // DataManager에서 값 빼오기
        float master = DataManager.Instance.SaveData.masterVolume;
        float bgm = DataManager.Instance.SaveData.bgmVolume;
        float sfx = DataManager.Instance.SaveData.sfxVolume;

        audioMixer.SetFloat("MasterVol", Mathf.Log10(master) * 20);
        audioMixer.SetFloat("BGMVol", Mathf.Log10(bgm) * 20);
        audioMixer.SetFloat("SFXVol", Mathf.Log10(sfx) * 20);
    }

    private void OnEnable()
    {
        // 커스텀 이벤트 대신 유니티 공식 이벤트를 구독합니다.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 메모리 누수 방지를 위해 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 유니티 공식 씬 로드 콜백 (씬이 바뀌면 엔진이 자동으로 호출함)
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 현재 로드된 씬의 빌드 인덱스를 SceneType으로 변환하여 전달
        PlayBGMByScene(true, (SceneType)scene.buildIndex);
    }

    private void PlayBGMByScene(bool isLoadSuccess, SceneType loadedScene)
    {
        if (!isLoadSuccess) return;

        string bgmName = loadedScene switch
        {
            SceneType.GAME => "GameBGM",
            SceneType.TITLE => "TitleBGM",
            SceneType.INTRO => "TitleBGM",
            SceneType.SETTINGS => "TitleBGM",
            _ => "TitleBGM"
        };

        DebugConsole.Log($"[SoundManager] {loadedScene} 씬 감지. {bgmName} 재생 시도");
        PlayBGM(bgmName);
    }
}
