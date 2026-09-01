# GreenClean

2주 팀 프로젝트에서 만든 코드를 원본 폴더 구조에 가깝게 보관하고, 그중 제가 직접 구현하거나 수정한 부분을 구분해 정리합니다.

> `Scripts` 폴더에는 팀 프로젝트의 C# 스크립트가 함께 들어 있습니다. 파일이 이 저장소에 있다는 이유만으로 파일 전체를 제가 작성한 것은 아닙니다.

## 제가 주도적으로 구현한 부분

### 블랙보드 기반 상태 공유

여러 게임 시스템에서 공유해야 하는 진행 상태를 `BlackboardKey`와 타입 스키마로 관리하도록 구성했습니다.

관련 코드 예시:

- `Scripts/Blackboard/GameProgressBlackboard.cs`
- `Scripts/Blackboard/BlackboardCondition.cs`
- `Scripts/Blackboard/BlackboardSchema.cs`
- `Scripts/Blackboard/BlackboardDefaults.cs`
- `Scripts/Blackboard/Editor/BlackboardSchemaEditor.cs`
- `Scripts/Blackboard/Editor/BlackboardDefaultsEditor.cs`

키별 자료형을 스키마에 정의하고 기본값과 실제 런타임 값을 분리했습니다. Unity 직렬화 제약 때문에 하나의 `object` 값을 저장하는 대신 지원 자료형별 값을 명시적으로 보관하는 구조를 사용했습니다.

### 데이터 기반 마스코트 반응 시스템

마스코트의 반응 조건을 게임 코드에 개별적으로 하드코딩하는 대신 조건 객체를 조합해 표현할 수 있도록 만들었습니다.

지원한 조건에는 다음과 같은 형태가 있습니다.

- 블랙보드 값 비교
- 여러 조건을 모두 만족하는 조건
- 여러 조건 중 하나를 만족하는 조건
- 조건 반전
- 항상 참인 조건
- 특정 값의 변경 감지

이 구조를 이용해 카드 사용량뿐 아니라 이후 콤보 3, 5, 7, 10회 반응도 기존 조건 조합을 재사용해 추가했습니다.

관련 코드 예시:

- `Scripts/MascotReaction/MascotReactionTable.cs`
- `Scripts/MascotReaction/MascotReactionTableEditor.cs`
- `Scripts/MascotReaction/Editor/MascotReactionTableEditorInspector.cs`
- `Scripts/Blackboard/BlackboardCondition.cs`

### Unity 편집기 도구

반응 데이터를 코드에서 직접 수정하지 않아도 되도록 전용 편집기 화면을 만들었습니다. 블랙보드 스키마를 읽어 등록된 키만 선택할 수 있게 하고, 선택한 키의 자료형에 맞는 입력 필드만 표시하도록 구성했습니다. 반응 추가·삭제, 펼치기·접기, JSON 불러오기·저장도 지원했습니다.

## 기존 팀 코드에 수정·연동한 부분

`CardManager`에서 카드 사용 이벤트를 발생시키고, `GameManager`에서 카드 사용량과 콤보 값을 블랙보드에 기록하도록 연결했습니다. 이 파일들은 다른 팀원의 게임 로직도 함께 포함하므로 파일 전체를 제 구현으로 설명하지 않습니다.

## 팀원 구현으로 구분하는 부분

프로젝트의 범용 `DataManager`, 저장 데이터 구조와 관련 기능은 다른 팀원이 주도한 작업입니다. 코드 보관을 위해 `Scripts/Utility` 아래에 포함되어 있지만 제 개인 기여로 설명하지 않습니다.

## 확인 가능한 커밋

- `be89d826a2176585899d2ba530fb091a0a3ffc5f` — 블랙보드 및 마스코트 반응 테이블 관리 시스템 구현
- `8085a0e842bcee4ed64089f6e4e8aa7175f6bbc4` — 반응 기능과 게임 이벤트 연동
- `352a618e167226fb235b46a291876c955b2757cf` — 콤보 반응 조건 추가 및 버그 수정

위 커밋의 GitHub 작성자는 `chaaaron000`으로 확인했습니다.

## 이 경험에서 보여주고 싶은 점

특정 대사를 몇 개 출력한 기능보다, **게임 상태와 콘텐츠 반응 규칙을 분리하고 새로운 조건을 기존 구조의 조합으로 확장할 수 있게 만든 과정**이 핵심입니다. 런타임 코드뿐 아니라 Unity 편집기 도구까지 함께 만들어 콘텐츠 수정 과정에서 코드 변경을 줄이는 방향으로 설계했습니다.

이전에 중단한 게임 모작에서 조건을 조합 가능한 데이터 노드로 표현하는 방식을 개인적으로 분석한 경험이 있었고, 그 아이디어를 이 프로젝트의 요구에 맞게 다시 설계해 적용했습니다. 해당 모작 프로젝트 자체는 이 저장소에 포함하지 않습니다.
