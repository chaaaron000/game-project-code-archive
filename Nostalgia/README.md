# Nostalgia

장기 팀 프로젝트에서 Photon Fusion 기반 멀티플레이 구조와 네트워크 기능을 다루고, 기존 몹 행동 코드를 Fusion FSM 구조로 옮긴 경험을 정리합니다.

## Portfolio focus

- Photon Fusion 기반 세션 / 로비 / Shared Mode 멀티플레이 개발 경험
- Vivox positional voice chat을 게임 세션과 연결
- 기존 몹의 Coroutine/상태 로직을 `Fusion.Addons.FSM` 기반 상태 구조로 리팩터링
- 장기 프로젝트에서 기존 코드에 기능을 추가하면서 소유권과 변경 범위를 구분한 경험

## Authorship finding — important

### 기존 Expressionless 알고리즘

초기 `Expressionless` 몹 알고리즘을 추가한 커밋:

- `339eb57783803693ae2cf30e2756030917093367` — `Expressionless 알고리즘 구현`
- GitHub 작성자: **AripyKSU**

따라서 초기 몹 알고리즘 자체는 개인 구현으로 주장하지 않습니다.

### Fusion FSM 리팩터링

후속 커밋:

- `fc7134720ce37d8ac652dd4e9aba5fe79d5346f0` — `FSM 테스팅 중`
- GitHub 작성자: **chaaaron000**

이 커밋에서 `ExpressionlessAI`와 Alert / Chase / Attack / Idle 상태 클래스를 추가하고 기존 몹 행동을 `Fusion.Addons.FSM` 기반 상태 구조로 옮겼습니다. 이 아카이브에는 해당 **리팩터링 코드만 CORE로 포함**합니다.

> Photon의 `Fusion.Addons.FSM` 구현 소스 자체도 같은 커밋에 들어왔지만 제3자 코드이므로 이 저장소에는 복사하지 않습니다.

## Vivox / network integration

확인된 개인 작성 커밋 예시:

- `f1ceda3bc615179178f782ca39364142957764a8` — 게임 세션 이름으로 Vivox positional channel 참가 연동
- `a38e79858af69ddbf164280209e1cf73b78772e2` — Vivox 입출력 장치 변경 및 설정 UI 연동

두 커밋 모두 GitHub 작성자가 `chaaaron000`으로 확인됐습니다.

다만 `NetworkManager`, `VivoxManager` 같은 장기 유지 파일은 팀원 수정과 샘플/SDK 기반 코드가 섞일 가능성이 있으므로 현재 파일 전체를 개인 코드처럼 CORE에 복사하지 않습니다. 포트폴리오에서는 **직접 추가한 기능과 판단 범위만 설명**합니다.

## Recommended portfolio story

이 프로젝트의 기술 사례는 `몹 AI를 처음부터 만들었다`가 아니라 다음 흐름이 정확합니다.

1. 기존 프로젝트에 이미 동작하던 몹 행동 로직이 존재
2. 네트워크 프로젝트가 커지면서 상태와 전이를 명시적으로 관리할 필요가 생김
3. Photon Fusion의 FSM Addon을 검토하고 기존 행동을 상태 단위로 분리
4. `ExpressionlessAI`가 상태 머신을 조립하고 Alert / Chase / Attack 상태가 각 책임을 담당하도록 리팩터링

네트워크 기술 선택과 Vivox 연동은 별도의 사례로 설명합니다.

> 이 폴더는 실행 가능한 Unity 프로젝트가 아니라 포트폴리오용 코드 아카이브입니다. Photon Fusion / Vivox SDK 및 제3자 코드의 원본은 포함하지 않습니다.
