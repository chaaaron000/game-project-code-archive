# Game Project Code Archive

게임 프로그래머 포트폴리오와 면접 준비를 위해 팀 프로젝트의 C# 스크립트를 프로젝트별로 모아둔 개인 작업용 아카이브입니다.

> **중요:** 이 저장소에 파일이 존재한다는 사실은 곧 해당 파일 전체를 제가 작성했다는 뜻이 아닙니다. 팀 프로젝트의 코드 맥락을 함께 보존하기 위해 프로젝트 스크립트를 전체적으로 가져왔으며, 각 프로젝트 README에서 Git commit 이력을 근거로 직접 작성한 부분, 기존 코드에 수정·연동한 부분, 팀원 코드 또는 소유권을 아직 확인하지 않은 부분을 구분합니다.

## Projects

| Project | C# files | Portfolio focus |
|---|---:|---|
| GreenClean | 63 | Blackboard, data-driven conditions, Unity Editor tooling |
| Nostalgia | 231 | Photon Fusion, multiplayer integration, FSM refactoring, Vivox |
| MixterPiece | 59 | AI-assisted game development workflow, Unity tooling, settings/audio |
| Poltergeist | 31 | First Unity team project, interaction/level scripting, collaboration history |

총 **384개 C# 스크립트**를 보관합니다. Unity `.meta`, Scene, Prefab, 아트 리소스는 포함하지 않습니다.

## Authorship policy

프로젝트 README에서는 다음 기준으로 기여를 설명합니다.

- **직접 설계·구현** — Git commit 작성자와 변경 내용을 통해 직접 구현이 확인된 부분
- **기존 코드 수정·연동** — 팀원이 만든 파일 또는 장기간 공동 수정된 파일에서 직접 추가·변경한 기능
- **팀 코드 / 맥락 보관** — 프로젝트 구조를 파악하기 위해 함께 보관하지만 개인 구현으로 주장하지 않는 코드
- **미확인** — commit 이력을 추가 조사하기 전까지 개인 구현으로 주장하지 않는 코드

현재 파일의 최종 작성자만 보고 소유권을 판단하지 않으며, 가능한 경우 기능이 처음 추가되거나 구조가 변경된 commit까지 확인합니다.

## Third-party code

명백한 제3자 SDK·에셋·튜토리얼 소스는 아카이브 대상에서 제외합니다. 예를 들어 Poltergeist의 `AtmosphericHouse` 및 `Downloaded_Assets/TutorialInfo` 코드는 가져오지 않았습니다.

이 저장소는 현재 **private 작업용**입니다. 추후 공개 포트폴리오로 전환하기 전에는 팀 코드와 제3자 코드의 공개 가능 여부를 다시 검토합니다.

## Excluded project

Visual Novel Dev는 Naninovel 유료 에셋과의 경계를 깔끔하게 유지하기 위해 이 코드 아카이브에서 제외했습니다. 포트폴리오에서는 Naninovel 자체 코드를 공개하지 않고, AI가 수정한 `.nani` 스크립트를 공식 Language Server로 진단하고 batch validation까지 연결한 개발 워크플로만 설명할 예정입니다.
