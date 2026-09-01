# GreenClean

2주 팀 프로젝트에서 구현한 Blackboard 기반 상태 공유와 데이터 주도 마스코트 반응 시스템을 선별해 정리합니다.

## Portfolio focus

- 여러 게임 시스템에서 공유해야 하는 진행 상태를 `BlackboardKey`와 타입 스키마로 관리
- 반응 조건을 `CheckBlackboardValue`, `And`, `Or`, `Not`, `AlwaysTrue`, `IsValueChanged` 노드로 조합
- `SerializeReference` 기반 다형 조건 데이터를 JSON으로 저장
- Blackboard Schema를 읽어 유효한 Key와 타입에 맞는 입력 필드만 노출하는 Custom Inspector 구현
- 초기 카드 사용량 조건 이후 콤보 3/5/7/10 조건을 기존 구조를 활용해 확장

## Origin of the idea

이전에 중단한 게임 모작 과정에서 실제 게임의 조건 노드 데이터를 개인적으로 분석한 경험이 있었습니다. 해당 모작 자체는 포트폴리오 프로젝트로 포함하지 않고, 그때 익힌 **조건을 조합 가능한 데이터 노드로 표현하는 방식**을 GreenClean의 문제에 맞게 응용한 경험만 설명합니다.

## Authorship

### CORE — 직접 설계 / 구현

- `Blackboard/BlackboardCondition.cs`
- `Blackboard/GameProgressBlackboard.cs`
- `Editor/MascotReactionTableEditorInspector.cs`
- `MascotReaction/MascotReactionTableEditor.cs`
- `MascotReaction/MascotReactionTable.cs`
- Blackboard Schema / Defaults 및 값 타입 보조 구조

주요 근거 커밋:

- `be89d826a2176585899d2ba530fb091a0a3ffc5f` — 블랙보드 구현 및 마스코트 리액션 테이블 관리 시스템 구현
- `8085a0e842bcee4ed64089f6e4e8aa7175f6bbc4` — 리액션 기능 구현 및 게임 이벤트 연동
- `352a618e167226fb235b46a291876c955b2757cf` — 콤보 리액션 조건 추가 및 버그 수정

위 커밋들의 GitHub 작성자는 `chaaaron000`으로 확인했습니다.

### MODIFIED — 기존 팀 코드와 연동

`CardManager`의 카드 사용 이벤트와 `GameManager`의 카드 사용량/콤보 값을 Blackboard에 기록하도록 연결했습니다. 해당 파일들은 팀 프로젝트의 다른 로직도 함께 포함하므로, 이 아카이브에서는 전체 파일을 내 코드처럼 복사하지 않고 기여 범위만 문서화합니다.

### 제외한 팀원 작업

프로젝트의 범용 JSON DataManager / SaveData 시스템은 다른 팀원이 주도한 작업이므로 이 프로젝트의 개인 기여로 포함하지 않습니다.

## Why this is portfolio-worthy

이 시스템의 핵심은 특정 마스코트 대사 몇 개를 구현한 것이 아니라, **게임 상태와 콘텐츠 반응 규칙을 분리하고 새로운 조건을 기존 노드 조합으로 추가할 수 있게 만든 것**입니다. 또한 런타임 구조와 함께 Unity Editor 도구까지 만들어 콘텐츠 수정 과정에서 코드 변경을 줄이는 방향으로 확장했습니다.

> 이 폴더는 실행 가능한 Unity 프로젝트가 아니라 포트폴리오용 코드 아카이브입니다. 외부 의존성과 보조 클래스 일부는 의도적으로 생략될 수 있습니다.
