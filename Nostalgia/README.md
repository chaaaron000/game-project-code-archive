# Nostalgia

장기 팀 프로젝트의 C# 스크립트 **231개 전체**를 맥락 보존용으로 모았습니다. Photon Fusion 기반 멀티플레이, Vivox 음성 채팅, 몹 상태 구조 리팩터링 등 다양한 작업이 섞여 있으므로 아래에서 확인된 개인 기여만 구분합니다.

> `Scripts/`에 존재하는 모든 파일을 제가 작성한 것은 아닙니다.

## Portfolio focus

- Photon Fusion 기반 세션 / 로비 / Shared Mode 멀티플레이 개발 경험
- Vivox positional voice chat을 게임 세션과 연결
- 기존 Coroutine 중심 몹 행동을 Fusion FSM 기반 상태 구조로 리팩터링
- 장기 팀 프로젝트에서 기존 코드와 제3자 SDK 위에 기능을 추가하고 유지한 경험

## 중요한 소유권 구분 — Expressionless

### 초기 몹 알고리즘은 팀원 구현

커밋 `339eb57783803693ae2cf30e2756030917093367`

> Expressionless 알고리즘 구현

GitHub 작성자: **AripyKSU**

따라서 `Scripts/Entity/Expressionless.cs`의 초기 몹 알고리즘 자체를 개인 구현으로 주장하지 않습니다.

### Fusion FSM 리팩터링은 직접 구현

커밋 `fc7134720ce37d8ac652dd4e9aba5fe79d5346f0`

> FSM 테스팅 중

GitHub 작성자: **chaaaron000**

이 커밋에서 기존 행동을 `Fusion.Addons.FSM` 구조로 옮기기 위해 다음 코드를 추가했습니다.

- `Scripts/Entity/Expressionless/ExpressionlessAI.cs`
- `Scripts/Entity/Expressionless/ExpressionlessAlertBehaviour.cs`
- `Scripts/Entity/Expressionless/ExpressionlessAttackBehaviour.cs`
- `Scripts/Entity/Expressionless/ExpressionlessChaseBehaviour.cs`
- `Scripts/Entity/Expressionless/ExpressionlessIdleBehaviour.cs`

포트폴리오에서는 이를 **“몹 AI를 처음부터 구현”**이라고 설명하지 않고,

> 기존 Coroutine/상태 로직을 분석하고, 네트워크 환경에서 상태와 전이를 더 명시적으로 관리하기 위해 Fusion FSM 구조로 리팩터링

한 경험으로 설명합니다.

Photon의 `Fusion.Addons.FSM` 구현 소스 자체는 제3자 코드이며 이 아카이브의 `Scripts/`에 포함하지 않았습니다.

## Vivox / network integration — 직접 변경 확인

### 세션과 positional channel 연결

커밋 `f1ceda3bc615179178f782ca39364142957764a8`

> Vivox 채널 참여 추가

GitHub 작성자 `chaaaron000` 확인.

- 게임 생성/참가 성공 시 Fusion session name을 Vivox positional channel name으로 사용
- Steam 사용자 이름을 Vivox 로그인 이름과 연결

관련 현재 파일:

- `Scripts/Network/NetworkManager.cs`
- `Scripts/Vivox/VivoxManager.cs`

### 음성 입출력 장치 설정

커밋 `a38e79858af69ddbf164280209e1cf73b78772e2`

> Vivox 입출력 장치 변경 기능 추가 및 메인 메뉴 세팅 버튼 이벤트 연결

GitHub 작성자 `chaaaron000` 확인.

Vivox 입력/출력 장치 선택과 설정 UI 연동을 추가했습니다.

## 공동 수정 파일에 대한 원칙

`NetworkManager.cs`, `VivoxManager.cs`, `GameManager.cs`처럼 프로젝트 전체 기간 동안 여러 사람이 수정한 파일은 **파일 전체를 개인 구현으로 표시하지 않습니다.** 포트폴리오에서는 commit으로 확인 가능한 기능 단위만 기여로 설명합니다.

그 외 231개 스크립트는 프로젝트 구조와 코드 맥락을 보존하기 위해 함께 두었으며, 필요한 경우 포트폴리오 제작 과정에서 파일별 commit 이력을 추가 확인합니다.

## Recommended portfolio story

Nostalgia는 다음 두 축으로 설명하는 것이 가장 정확합니다.

1. **멀티플레이 기술 선택과 통합** — Photon Fusion, 세션/로비, Vivox 등 실제 멀티플레이 프로젝트에서 SDK와 게임 흐름을 연결한 경험
2. **기존 코드 리팩터링** — 팀원이 만든 몹 행동을 이해한 뒤 Fusion FSM으로 상태 책임을 분리한 경험

즉 새 기능을 혼자 처음부터 만드는 능력보다, **규모가 커진 팀 프로젝트의 기존 코드를 읽고 구조를 바꾸며 외부 네트워크 기술과 연결한 경험**을 보여주는 프로젝트입니다.
