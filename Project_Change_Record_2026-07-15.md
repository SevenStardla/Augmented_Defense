# Augmented Defense 프로젝트 변경 기록

작성일: 2026-07-15

기준 문서: `Augmented_Defense_Development_Guide.md`

## 작업 기준

- 씬(`Assets/Scenes/Main.unity`)과 `ProjectSettings`에는 기존 미확인 변경이 있어 이번 작업에서 수정하지 않았다.
- 가이드의 1순위 안정화 항목인 웨이브 종료 조건, 생존 수 관리, 이벤트 기반 UI 갱신, 종료 상태 방어를 우선 반영했다.
- 기존 ScriptableObject 필드는 유지해서 현재 데모 구성과 Inspector 데이터가 깨지지 않도록 했다.

## 수정한 파일

### `Assets/Scripts/Wave/WaveData.cs`

- `WaveEntry` 구조체를 추가했다.
  - `EnemyData enemyData`
  - `int count`
  - `float spawnInterval`
- `WaveData.entries` 배열을 추가했다.
- `HasEntries` 속성을 추가했다.
- 기존 `enemyCount`, `enemyData`는 유지했다.
- 결과적으로 기존 단일 적 웨이브 방식과 새 복합 웨이브 방식을 모두 지원한다.

### `Assets/Scripts/Enemy/EnemySpawner.cs`

- `IsSpawning` 속성을 추가해 현재 스폰 코루틴 진행 여부를 외부에서 확인할 수 있게 했다.
- `EnemyRemoved` 이벤트를 추가해 적 제거 흐름을 다른 시스템이 구독할 수 있게 했다.
- `SpawnWave(WaveData wave)`를 추가했다.
  - `WaveData.entries`가 있으면 엔트리별 적 종류, 수량, 스폰 간격을 사용한다.
  - 엔트리가 없으면 기존 `enemyCount`, `enemyData`, 기본 `spawnInterval`을 사용한다.
- `SpawnBatch` 내부 메서드로 스폰 반복 로직을 통합했다.
- 스폰 위치 계산을 `GetSpawnPosition()`으로 분리했다.
- 생존 수 감소는 `Enemy.Died` 단일 경로만 사용하도록 정리했다.
  - 코어 도달 시에도 `Enemy.Die(false)`가 `Died`를 발생시키므로 중복 감소를 피한다.

### `Assets/Scripts/Wave/WaveManager.cs`

- 기존 `spawning` bool 대신 `Coroutine runningWave`로 웨이브 실행 상태를 추적한다.
- `TotalWaves`, `IsWaveRunning` 속성을 추가했다.
- `CurrentWaveNumber`가 마지막 웨이브 이후에도 총 웨이브 수를 넘지 않도록 보정했다.
- `WaveChanged(int currentWave, int totalWaves)` 이벤트를 추가했다.
- 웨이브 시작과 종료 시 UI가 이벤트로 갱신될 수 있게 `WaveChanged`를 호출한다.
- 웨이브 종료 조건을 `spawner.IsSpawning == false && spawner.AliveCount == 0`으로 명시했다.
- `EnemySpawner.SpawnWave(wave)`를 사용하도록 변경했다.

### `Assets/Scripts/Core/GameManager.cs`

- `StartGame()`에서 `Time.timeScale = 1f`를 복구하도록 했다.
- `GameOver()`가 이미 `GameOver` 또는 `Clear` 상태이면 다시 처리하지 않도록 막았다.
- `Clear()`가 이미 `Clear` 또는 `GameOver` 상태이면 다시 처리하지 않도록 막았다.
- 결과적으로 코어 체력 0, 마지막 웨이브 종료 같은 이벤트가 반복 호출되어도 종료 상태 전환이 한 번만 일어난다.

### `Assets/Scripts/Core/EconomyManager.cs`

- `CanAfford`가 음수 비용을 유효 비용으로 판단하지 않도록 수정했다.
- `TrySpend`에 0 이하 비용 방어를 추가했다.
  - 0 비용은 성공으로 처리한다.
  - 음수 비용은 실패로 처리해서 골드가 증가하는 역효과를 막았다.

### `Assets/Scripts/UI/UIManager.cs`

- 매 프레임 `Update()`에서 웨이브 텍스트를 갱신하던 방식을 제거했다.
- `WaveManager.WaveChanged`를 구독해 이벤트 기반으로 웨이브 UI를 갱신한다.
- `RefreshWaveText()`를 추가해 현재 웨이브와 총 웨이브 수를 `Wave current/total` 형식으로 표시한다.
- 기존 체력, 골드, 상태 변경 이벤트 구독/해제 흐름에 웨이브 이벤트도 포함했다.

### `Assets/Scripts/UI/UI_Object_Behavior_Scripts.md`

- `Wave Text` 설명을 현재 코드에 맞게 갱신했다.
- 표시 형식을 `Wave current/total`로 수정했다.
- 연결 데이터를 `WaveManager.WaveChanged`, `CurrentWaveNumber`, `TotalWaves`로 수정했다.
- `UIManager.Update()` 기반 갱신 설명을 제거하고 `RefreshWaveText()` 이벤트 기반 갱신 설명으로 바꿨다.
- `UI Manager` 행동 패턴에서 웨이브 텍스트 갱신 방식도 이벤트 구독 방식으로 수정했다.

### `Assets/Scripts/Tower/TowerAttack.cs`

- 현재 게임 상태가 `WavePhase`가 아니면 타워 공격 로직을 실행하지 않도록 했다.
- Build, Augment, GameOver, Clear 상태에서 타워가 적을 공격하는 상황을 막았다.

### `Assets/Scripts/Player/DefenderController.cs`

- `GameOver` 또는 `Clear` 상태에서는 이동과 공격 입력을 무시하도록 했다.
- 공격은 `WavePhase`에서만 처리하도록 제한했다.
- Build/Augment 상태에서 공격 입력으로 전투 결과가 바뀌는 상황을 막았다.

### `Assets/Scripts/Augment/AugmentData.cs`

- `id` 필드를 추가했다.
- `maxStacks` 필드를 추가했다.
- `Key` 속성을 추가했다.
  - `id`가 비어 있으면 에셋 이름을 fallback key로 사용한다.
- 표시 이름 변경이 중첩 판정에 영향을 주지 않도록 준비했다.

### `Assets/Scripts/Augment/AugmentManager.cs`

- 증강별 중첩 수를 저장하는 `stackCounts` 딕셔너리를 추가했다.
- `RollOffers()`가 `CanSelect` 기준으로 후보를 필터링하도록 변경했다.
- `SelectAugment()`가 중복 선택 가능 여부와 최대 중첩 수를 검사하도록 변경했다.
- `GetStackCount(AugmentData augment)`를 추가했다.
- `canStack == false`이면 최대 1회만 선택된다.
- `canStack == true`이면 `maxStacks`까지 선택된다.

## 검토했지만 수정하지 않은 파일/영역

### `Assets/Scenes/Main.unity`

- 작업 시작 전부터 변경된 상태였다.
- 사용자 또는 Unity Editor가 만든 변경일 수 있어 이번 작업에서 수정하지 않았다.

### `ProjectSettings/EditorSettings.asset`

- 작업 시작 전부터 변경된 상태였다.
- 요청 없이 ProjectSettings를 덮어쓰지 않기 위해 수정하지 않았다.

### `ProjectSettings/SceneTemplateSettings.json`

- 작업 시작 전부터 추적되지 않은 새 파일이었다.
- 이번 코드 안정화 작업과 직접 관련이 없어 수정하지 않았다.

### `Augmented_Defense_Development_Guide.md`

- 기준 문서로 읽기만 했다.
- 원본 인코딩이 깨져 보였지만 의미를 추정할 수 있는 구조와 우선순위를 기준으로 작업했다.

### `Assets/Scripts/Demo/DemoBootstrap.cs`

- 현재 데모 자동 구성은 기존 MVP 실행 목적과 맞아 유지했다.
- 새 `WaveData.entries`는 선택 기능으로 추가했기 때문에 기존 데모 웨이브 생성 코드를 바꿀 필요가 없었다.

### `Assets/Scripts/Tower/TowerPlacement.cs`

- 배치 가능 상태 확인, 위치 검증, `EconomyManager.TrySpend` 호출 순서가 가이드와 맞아 유지했다.

### `Assets/Scripts/Enemy/Enemy.cs`

- 사망 중복 방지와 보상 지급 경로가 이미 `IsDead`로 보호되고 있어 유지했다.
- 생존 수 감소 중복 문제는 `EnemySpawner`의 구독 경로에서 해결했다.

### `Assets/Scripts/Enemy/EnemyMovement.cs`

- 코어 도달 후 `Enemy.NotifyReachedCore()`가 즉시 사망 상태로 전환하므로 중복 코어 피해 위험은 낮다고 판단했다.
- 속도 배율/상태 이상 시스템은 아직 도입하지 않았다.

## 검증 결과

- `rg`로 변경된 API 참조를 확인했다.
- `SpawnWave`, `IsSpawning`, `WaveChanged`, `maxStacks` 참조가 의도한 파일에만 존재하는 것을 확인했다.
- 로컬 PATH에서 Unity 실행 파일을 찾지 못해 Unity Editor 컴파일/PlayMode 검증은 실행하지 못했다.
- 테스트 프레임워크 패키지와 게임 코드 asmdef가 없어 자동 테스트는 추가하지 않았다.

## 남은 권장 작업

- Unity Editor에서 컴파일 오류 확인.
- PlayMode에서 다음 항목 수동 확인:
  - 첫 웨이브 시작 후 모든 스폰 완료 전 웨이브가 끝나지 않는지 확인.
  - 적 처치와 코어 도달 양쪽에서 `AliveCount`가 한 번만 감소하는지 확인.
  - 마지막 웨이브 이후 `Clear`가 한 번만 발생하는지 확인.
  - `GameOver`와 `Clear` 상태에서 이동, 공격, 배치 입력이 막히는지 확인.
  - 웨이브 텍스트가 `Wave 현재/전체` 형식으로 갱신되는지 확인.


수정한 핵심 파일

  - Assets/Scripts/Wave/WaveData.cs: WaveEntry 추가, 복합 웨이브 데이터 지원.
  - Assets/Scripts/Enemy/EnemySpawner.cs: IsSpawning, SpawnWave, EnemyRemoved 추가. 웨이브 종료 조건이 스폰 완료와 생존 수를 함께 보도록 정리.
  - Assets/Scripts/Wave/WaveManager.cs: IsSpawning == false && AliveCount == 0 기준으로 웨이브 종료. WaveChanged 이벤트 추가.
  - Assets/Scripts/UI/UIManager.cs: 매 프레임 웨이브 텍스트 갱신 제거, 이벤트 기반 갱신으로 변경.
  - Assets/Scripts/Core/GameManager.cs: GameOver/Clear 중복 진입 방지, 게임 시작 시 timeScale 복구.
  - Assets/Scripts/Core/EconomyManager.cs: 음수 비용으로 골드가 늘어나는 방어 구멍 차단.
  - Assets/Scripts/Tower/TowerAttack.cs: WavePhase에서만 타워 공격.
  - Assets/Scripts/Player/DefenderController.cs: 종료 상태 입력 차단, 공격은 WavePhase에서만 처리.
  - Assets/Scripts/Augment/AugmentData.cs, Assets/Scripts/Augment/AugmentManager.cs: 증강 id, maxStacks, 중첩 수 관리 추가.
  - Assets/Scripts/UI/UI_Object_Behavior_Scripts.md: UI 문서의 웨이브 표시 설명을 새 이벤트 기반 구조에 맞게 갱신.

  수정하지 않은 기존 변경
  작업 시작 전부터 Assets/Scenes/Main.unity, ProjectSettings/EditorSettings.asset, ProjectSettings/SceneTemplateSettings.json, Augmented_Defense_Development_Guide.md가 변경/추적되지 않은
  상태였습니다. 사용자 또는 Unity Editor 변경일 수 있어 건드리지 않았습니다.

  검증은 rg 참조 확인과 git diff --check까지 완료했습니다.
  ## 검증 결과

- `rg`를 통해 변경된 API 참조를 확인했다.
- `git diff --check`를 통해 공백 및 기본적인 diff 오류를 확인했다.
- Unity UI 패키지를 설치한 뒤 컴파일 오류가 사라진 것을 확인했다.
- Unity Editor에서 PlayMode 실행을 완료했다.
- `Start Wave` 버튼을 누르면 웨이브가 정상적으로 시작되는 것을 확인했다.
- 적들이 지정된 노란색 경로를 따라 이동하는 것을 확인했다.
- 타워가 적을 공격하고 정상적으로 처치하는 것을 확인했다.
- 적 처치 시 `Gold`가 정상적으로 증가하는 것을 확인했다.
- 웨이브 종료 후 `Wave` 숫자가 정상적으로 증가하는 것을 확인했다.
- `Restart` 기능은 아직 미구현 상태이며, 실행 시 빈 씬으로 전환되는 것을 확인했다.