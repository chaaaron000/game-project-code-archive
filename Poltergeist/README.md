# Poltergeist

첫 Unity 팀 프로젝트의 C# 스크립트를 아카이브했습니다. 현재 **31개 C# 파일**을 보관하며, 이 프로젝트는 최신 기술력을 보여주는 대표작보다는 **처음 게임을 완성하고 Git 협업을 경험한 출발점**으로 사용하는 것이 적합합니다.

> `Scripts/`의 모든 파일이 개인 구현은 아닙니다. 오래된 팀 프로젝트라 작성자 구분이 불명확한 코드는 개인 기여로 주장하지 않습니다.

## Portfolio focus

- 첫 Unity 팀 프로젝트 완성 경험
- 반복 복도/공포 연출과 상호작용 스크립팅
- 문 상태와 상호작용 로직 구현
- Git/GitHub를 팀 개발에 사용하고 협업 방식에 적응한 경험
- 이후 프로젝트와 비교해 개발 방식이 어떻게 달라졌는지 보여주는 성장 기준점

## 직접 작성이 확인된 사례

### Infinite Loop 제작

커밋 `88ab451e2ee7f3903438500bdf2a8788efc9dddd`

> Create Infinite Loop and Modify Player gravity

GitHub 작성자 `chaaaron000` 확인.

반복 복도 구조를 만들고 플레이어 이동 관련 설정을 조정한 작업입니다.

현재 관련 코드 예:

- `Scripts/Dongguk_Project/Scripts/InfiniteLoop/GhostAnimation.cs`
- `Scripts/Dongguk_Project/Scripts/InfiniteLoop/LoopFiveJumpScare.cs`
- `Scripts/Dongguk_Project/Scripts/InfiniteLoop/LoopFourSofa.cs`
- `Scripts/Dongguk_Project/Scripts/InfiniteLoop/LoopSevenChasing.cs`

각 파일은 이후 팀 수정 가능성이 있으므로 최종 파일 전체보다 **개별 commit으로 확인되는 구현 범위**를 우선해 설명합니다.

### Door interaction

커밋 `162293de8be0f86cbf80887b8ce4a1960c5ae53e`

> Add DoorOpenClose Script and set Door type

GitHub 작성자 `chaaaron000` 확인.

- `Scripts/Dongguk_Project/Scripts/DoorOpenClose.cs` 추가
- 열 수 없는 문 / Key Door / Quiz Door / Basement Door 등의 타입 분기
- 플레이어 raycast interaction과 문 동작 연결

후속 `aeebfded08143b6420b958563e929efc5257a5f2` (`Update DoorOpenClose.cs`)도 `chaaaron000` 작성으로 확인됩니다.

### Loop 7 prototype

커밋 `c07178cd1e809d0b74fc187c173de442944b0b84`

> Create Loop 7 Prototype

GitHub 작성자 `chaaaron000` 확인.

후반 반복 구간과 연출을 구성한 작업 이력으로 사용할 수 있습니다.

## 팀 코드 / 미확인 코드

Inventory, Audio, UI, 기타 상호작용 스크립트 등은 전체 프로젝트 맥락을 보존하기 위해 함께 두었습니다. 별도 commit 검증 없이 개인 구현으로 표시하지 않습니다.

## 제외한 제3자 코드

원본 프로젝트에는 에셋·튜토리얼 코드도 함께 들어 있었기 때문에 아카이브 생성 시 다음 경로를 제거했습니다.

- `AtmosphericHouse/Scripts`
- `Downloaded_Assets/TutorialInfo`

명백한 제3자 코드는 포트폴리오 코드 아카이브에 남기지 않는 것을 원칙으로 합니다.

## Portfolio story

이 프로젝트를 기술적으로 과장하기보다,

> 처음에는 기능을 직접 붙이며 게임 한 편을 완성하고 Git 협업을 배웠고, 이후 Nostalgia와 GreenClean에서 기존 코드를 구조화하고 데이터·도구 수준까지 고민하게 됐다

는 성장 서사의 시작점으로 사용하는 것이 적합합니다.
