using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSettings
{
    // 비디오 세팅
    public int resolutionWidth = 1920;   // 가로 해상도
    public int resolutionHeight = 1080;  // 세로 해상도
    public int displayMode = 3;          // 화면 모드
    public int refreshRate = 60;         // 선호 주사율
    public float brightness = 0.5f;      // 밝기
    
    // 오디오 세팅
    public float masterVolume = 0.5f;  // 마스터 볼륨
    public float bgmVolume = 0.5f;
    public float sfxVolume = 0.5f;
    public string voiceChatOutputDevice = "Default System Device";  // 보이스챗 출력 장치
    public float voiceChatVolume = 0f;  // 보이스 채팅 출력 볼륨
    public string microphoneDevice = "Default System Device";  // 마이크 출력 장치
    public float microphoneVolume = 0f;
    
    // 컨트롤 세팅
    public float mouseSensitivity = 10f;  // 마우스 감도

    public void ShowSettings() {
        Debug.Log($"Video -> {resolutionWidth} x {resolutionHeight} @ {refreshRate} Hz, displayMode: {displayMode}, Brightness : {brightness}");
        Debug.Log($"Audio -> masterVolume: {masterVolume}, OutputDevice: {voiceChatOutputDevice}, OutputVolume: {voiceChatVolume}, InputDevice: {microphoneDevice}, InputVolume : {microphoneVolume}");
        Debug.Log($"Control -> MouseSensitivity : {mouseSensitivity}");
    }
}
