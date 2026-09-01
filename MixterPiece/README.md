# MixterPiece

AI 활용 게임 개발 대회 출품을 목적으로 만든 논리 퍼즐 게임입니다. 팀 프로젝트의 C# 스크립트 **59개 전체**를 맥락 보존용으로 모았으며, 코드 자체뿐 아니라 AI를 게임 개발 과정에 어떻게 연결했는지를 포트폴리오의 주요 축으로 사용합니다.

> `Scripts/`에 존재하는 모든 파일을 제가 작성한 것은 아닙니다. 특히 퍼즐 핵심 알고리즘과 Command/Undo 일부는 팀원 구현이므로 아래에서 구분합니다.

## Portfolio focus — AI-assisted development workflow

이 프로젝트의 AI 활용은 단순 코드 자동완성보다 다음 흐름에 가깝습니다.

```text
기획/개발 자료
    ↓
LLM Wiki로 프로젝트 지식 정리
    ↓
정제된 Wiki만 Google Drive Source로 제공
    ↓
AI 기획 브레인스토밍 / 구현 계획
    ↓
Coding Agent
    ↓
Unity 프로젝트 수정
    ↓
unity-cli
    ↓
Editor Refresh → Compile → Console 확인 → 수정 → 재검증
```

### Project knowledge를 제한해서 제공

전체 대화나 자료를 매번 그대로 넣는 대신, 프로젝트의 규칙·구조·기획 내용을 LLM Wiki 형태로 정리했습니다. 기획 브레인스토밍에서는 이 **정제된 Wiki 내용만 Google Drive Source로 제공**해 AI가 현재 프로젝트의 규칙과 맥락을 기준으로 답하도록 사용했습니다.

### 개발 규칙을 프로젝트 지침으로 고정

`AI-Workflow/AGENTS.md`에는 AI가 작업할 때 따라야 하는 프로젝트 규칙을 남겼습니다.

예:

- Wiki와 문서의 위치/사용 규칙
- Unity 프로젝트 수정 방식
- C# 변경 후 Unity compile 및 Console 확인
- 로그 작성 규칙
- public API 문서 주석 및 중요 로직 주석 규칙

매 작업마다 같은 프롬프트를 반복하기보다 **프로젝트 수준의 개발 규칙으로 고정**한 사례입니다.

### Unity에서 결과를 다시 검증

AI가 C#을 수정한 뒤 `unity-cli`를 이용해 Unity Editor refresh와 compile을 실행하고 Console error/warning을 확인하도록 작업 흐름을 구성했습니다.

포트폴리오에서는 이를 **AI가 코드를 작성했다**보다,

> AI가 작업할 컨텍스트와 규칙을 설계하고, 결과를 실제 엔진에서 검증하는 루프까지 개발 과정에 포함했다

는 점으로 설명합니다.

## 직접 변경이 확인된 코드 사례

### Grid wall data / movement checks

커밋 `00f94ec7e20ba9ca3c5517368d6adca6258f4cc8`

> Add grid wall data, movement checks, and wall rendering

GitHub 작성자 `chaaaron000` 확인.

- `GridState`에 벽 좌표 데이터와 이동 가능 여부 검사를 추가
- 격자 방향을 좌표 offset으로 변환하는 `GridDirection` / utility 추가
- 벽 렌더링에 필요한 데이터 흐름 연결

관련 현재 코드:

- `Scripts/Grid/GridState.cs`
- `Scripts/Grid/GridDirection.cs`
- `Scripts/Grid/GridView.cs`

`GridState` 자체의 최초 구현은 팀 코드이므로 **파일 전체가 아니라 이 커밋에서 추가·수정한 벽 관련 기능을 개인 기여로 설명합니다.**

### Settings UI

커밋 `1074c5f327f49b10629d73cc8e8661912b637bb1`

> 세팅 UI 기능 구현

GitHub 작성자 `chaaaron000` 확인.

게임 설정 UI와 접근성 표시 관련 흐름을 구현·연결했습니다.

### AudioMixer / persistent volume settings

커밋 `05e31644d1cb55db30a80f2851df4b322d6014d2`

> AudioMixer 추가

GitHub 작성자 `chaaaron000` 확인.

- Master / BGM / SFX 볼륨을 AudioMixer에 연결
- `PlayerPrefs`에 사용자 볼륨 설정 저장
- 초기 실행 시 기본값과 저장된 설정을 구분해 적용
- `SoundLibrary`, `SoundManager`, `GameSettingsService` 연동

대표 코드:

- `Scripts/SoundManager.cs`
- `Scripts/SoundLibrary.cs`
- `Scripts/GameSettingsService.cs`

## 팀원 구현으로 확인된 주요 코드

다음은 프로젝트 이해를 위해 아카이브에 포함하지만 개인 구현으로 주장하지 않습니다.

- `Scripts/Paint/PaintSpreadCalculator.cs` — 벽 기반 물감 확산 BFS의 주요 구현 commit 작성자 `AripyKSU`
- `Scripts/Command/PaintBucketUseCommand.cs` 및 초기 Command/Undo 흐름 — 주요 초기 구현 commit 작성자 `AripyKSU`
- Paint Bucket queue/reservation 관련 주요 commit — 팀원 구현
- 색약 보정 material 관련 주요 commit — 팀원 구현

따라서 포트폴리오에서 퍼즐 BFS나 Undo를 설명할 경우 **팀 전체 구조를 설명하는 맥락**으로만 사용하고, 개인 기술 사례로는 위에서 commit이 확인된 작업을 우선합니다.

## Portfolio story

MixterPiece는 단순히 “AI를 많이 사용한 프로젝트”가 아니라,

**프로젝트 지식 정리 → 제한된 컨텍스트 제공 → AI 기획/구현 → Unity에서 검증**

까지 작업 흐름을 설계한 프로젝트로 설명합니다. 게임 코드 측에서는 실제로 직접 수정한 Grid wall, 설정 UI, AudioMixer 사례를 함께 보여줘 **AI 사용과 엔진/코드 판단을 분리해서 증명**하는 것이 목표입니다.
