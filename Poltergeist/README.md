# Poltergeist

첫 Unity 팀 프로젝트의 C# 스크립트를 보관한 폴더입니다. 현재 31개 C# 파일을 기능과 책임 기준으로 다시 분류해 두었으며, 최신 기술력을 보여주는 대표작보다는 **처음 게임을 완성하고 Git 협업을 경험한 출발점**으로 설명하는 프로젝트입니다.

> `Scripts` 폴더의 모든 파일을 제가 작성한 것은 아닙니다. 오래된 팀 프로젝트라 작성자를 확실히 구분하기 어려운 코드는 제 개인 기여로 설명하지 않습니다. 이 저장소의 폴더 구조는 포트폴리오 열람을 위해 다시 정리한 것이므로 원본 Unity 프로젝트의 경로와 다를 수 있습니다.

## 코드 폴더 구조

- `Scripts/Gameplay/InfiniteLoop` — 반복 복도와 공포 연출
- `Scripts/Gameplay/Interaction` — 문과 오브젝트 상호작용
- `Scripts/Gameplay/Player` — 플레이어 조작과 사용 입력
- `Scripts/Gameplay/Inventory` — 인벤토리와 아이템
- `Scripts/GameFlow` — 장면 전환과 게임 시작
- `Scripts/Audio` — 오디오 제어
- `Scripts/UI` — 화면 표시
- `Scripts/Legacy/Misc` — 역할이 불분명하거나 현재 포트폴리오에서 강조하지 않는 과거 코드

## 이 프로젝트에서 맡은 주요 작업

- 반복 복도 구조와 공포 연출 구현
- 문 상태와 상호작용 로직 구현
- 후반 반복 구간과 추격 연출 작업
- Git과 GitHub를 팀 개발에 처음 사용하고 협업 방식에 적응

## 직접 작성이 확인된 사례

### 반복 복도 제작

커밋 `88ab451e2ee7f3903438500bdf2a8788efc9dddd`

GitHub 작성자 `chaaaron000`으로 확인했습니다.

반복 복도 구조를 만들고 플레이어 이동 관련 설정을 조정한 작업입니다.

현재 관련 코드 예시:

- `Scripts/Gameplay/InfiniteLoop/GhostAnimation.cs`
- `Scripts/Gameplay/InfiniteLoop/LoopFiveJumpScare.cs`
- `Scripts/Gameplay/InfiniteLoop/LoopFourSofa.cs`
- `Scripts/Gameplay/InfiniteLoop/LoopSevenChasing.cs`

각 파일은 이후 팀원 수정이 섞였을 가능성이 있으므로 현재 파일 전체보다 개별 커밋으로 확인되는 구현 범위를 우선해 설명합니다.

### 문 상호작용

커밋 `162293de8be0f86cbf80887b8ce4a1960c5ae53e`

GitHub 작성자 `chaaaron000`으로 확인했습니다.

- `Scripts/Gameplay/Interaction/DoorOpenClose.cs` 추가
- 열 수 없는 문, 열쇠 문, 퀴즈 문, 지하실 문 등의 종류별 동작 분기
- 플레이어의 레이캐스트 상호작용과 문 동작 연결

후속 커밋 `aeebfded08143b6420b958563e929efc5257a5f2`의 `DoorOpenClose.cs` 수정도 `chaaaron000` 작성으로 확인됩니다.

### 반복 구간 7번째 원형 제작

커밋 `c07178cd1e809d0b74fc187c173de442944b0b84`

GitHub 작성자 `chaaaron000`으로 확인했습니다.

후반 반복 구간과 연출을 구성한 작업 이력으로 사용할 수 있습니다.

## 팀 코드와 작성자 확인이 필요한 코드

인벤토리, 오디오, 화면 구성, 기타 상호작용 스크립트 등은 전체 프로젝트의 맥락을 보존하기 위해 함께 두었습니다. 별도의 커밋 확인 없이 제 개인 구현으로 표시하지 않습니다.

## 제외한 제3자 코드

원본 프로젝트에는 에셋과 튜토리얼에서 가져온 코드도 함께 들어 있었습니다. 코드 아카이브를 만들면서 다음 경로의 제3자 코드는 제거했습니다.

- `AtmosphericHouse/Scripts`
- `Downloaded_Assets/TutorialInfo`

명백한 제3자 코드는 포트폴리오 코드 아카이브에 남기지 않는 것을 원칙으로 합니다.

## 이 경험에서 보여주고 싶은 점

이 프로젝트의 코드를 현재 기준으로 과장해서 보여주기보다, **처음에는 필요한 기능을 직접 붙이며 게임 한 편을 완성하고 Git 협업을 배웠고, 이후 프로젝트에서는 기존 코드를 구조화하고 데이터와 개발 도구까지 고민하게 됐다**는 성장 과정의 시작점으로 사용합니다.
