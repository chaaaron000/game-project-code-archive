# GreenClean

2주 팀 프로젝트의 C# 스크립트 **63개 전체**를 맥락 보존용으로 모았습니다. 아래 기여도 설명이 우선이며, `Scripts/`에 존재하는 모든 파일을 개인 구현으로 주장하지 않습니다.

## Portfolio focus

- 여러 게임 시스템이 공유하는 진행 상태를 Blackboard로 관리
- 반응 조건을 `CheckBlackboardValue`, `And`, `Or`, `Not`, `AlwaysTrue`, `IsValueChanged` 노드로 조합
- Blackboard Schema를 이용해 Key별 타입을 정의하고 런타임 값 설정을 검증
- 조건과 마스코트 반응 데이터를 JSON으로 저장
- Schema를 읽어 유효한 Key와 타입에 맞는 필드만 노출하는 Unity Custom Inspector 구현
- 초기 카드 사용량 조건 이후 콤보 3/5/7/10 요구를 기존 조건 노드 조합으로 확장

## 직접 설계·구현이 확인된 부분

### Blackboard + Mascot Reaction system

커밋 `be89d826a2176585899d2ba530fb091a0a3ffc5f`

> 블랙보드 구현 및 마스코트 리액선 테이블 관리 시스템 구현

GitHub 작성자 `chaaaron000` 확인.

대표 코드:

- `Scripts/Blackboard/GameProgressBlackboard.cs`
- `Scripts/Blackboard/BlackboardCondition.cs`
- `Scripts/Blackboard/BlackboardSchema.cs`
- `Scripts/Blackboard/BlackboardDefaults.cs`
- `Scripts/Blackboard/Editor/BlackboardSchemaEditor.cs`
- `Scripts/Blackboard/Editor/BlackboardDefaultsEditor.cs`
- `Scripts/MascotReaction/MascotReactionTable.cs`
- `Scripts/MascotReaction/MascotReactionTableEditor.cs`
- `Scripts/MascotReaction/Editor/MascotReactionTableEditorInspector.cs`

### 게임 로직 연동

커밋 `8085a0e842bcee4ed64089f6e4e8aa7175f6bbc4`

> 리액션 기능 구현

GitHub 작성자 `chaaaron000` 확인.

- `CardManager`에 카드 사용 이벤트를 추가
- `GameManager`에서 카드 사용량을 Blackboard에 기록
- Blackboard 값 변경을 마스코트 반응 평가와 연결

`CardManager.cs`, `GameManager.cs`는 팀의 다른 로직도 포함한 공동 맥락 파일이므로 **내가 변경한 부분만 개인 기여로 설명합니다.**

### 실제 확장 사례

커밋 `352a618e167226fb235b46a291876c955b2757cf`

> 콤보 리액션 조건 추가 및 버그 수정

GitHub 작성자 `chaaaron000` 확인.

- `COMBO_COUNT`를 Blackboard Key로 추가
- `IsValueChanged` 조건 노드 추가
- `IsValueChanged(COMBO_COUNT) AND COMBO_COUNT == N` 조합으로 3/5/7/10 콤보 반응을 데이터에 추가

초기에 만든 구조가 이후 요구사항에서도 실제로 재사용됐다는 점을 포트폴리오에서 보여줄 수 있습니다.

## 팀원 구현으로 확인된 부분

범용 JSON DataManager / SaveData 계열은 커밋 `aaced3d4ed0501a81a6e5b8dc37af20e87acd950`의 작성자가 `kjmh1234`로 확인되어 개인 구현으로 주장하지 않습니다.

예:

- `Scripts/Utility/DataManager.cs`
- `Scripts/Utility/GameSaveData.cs`

이 파일들은 전체 프로젝트 맥락 보존을 위해 아카이브에 남겨둡니다.

## Origin of the idea

이전에 중단한 게임 모작 과정에서 실제 게임의 **조건 노드 데이터 구조**를 개인적으로 뜯어본 적이 있습니다. 해당 모작은 완성 프로젝트가 아니므로 포트폴리오 항목에서는 제외하고, 당시 배운 **조건을 조합 가능한 데이터 노드로 표현하는 방식**을 GreenClean의 마스코트 반응 문제에 맞게 응용했다는 학습 배경만 사용합니다.

## Portfolio story

이 프로젝트에서 강조할 핵심은 마스코트 대사를 몇 개 구현한 것이 아니라,

**게임 상태 → Blackboard → 조합형 Condition → Reaction Data → Editor Tool**

로 책임을 분리하고, 콘텐츠 조건이 늘어나도 게임 코드에 `if`를 계속 추가하지 않도록 구조와 편집 도구를 함께 만든 경험입니다.
