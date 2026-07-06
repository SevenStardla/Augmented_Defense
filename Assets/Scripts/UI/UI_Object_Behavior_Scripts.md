# UI Object Behavior Scripts

이 문서는 현재 데모 UI를 실제 화면에서 움직이게 만들 때 사용할 행동 패턴과 성격 스크립트입니다. 기준 구현은 `DemoBootstrap.CreateUi()`와 `UIManager`입니다.

## 전체 UI 연출 원칙

- UI는 전투 상황을 방해하지 않고, 상태 변화가 생긴 순간에만 짧게 반응한다.
- 숫자 UI는 "값이 바뀌었다"는 사실을 0.15~0.35초 안에 보여주고 원래 자리로 돌아온다.
- 버튼은 클릭 가능 상태를 명확히 보여주되, 전투 화면 위에서 과하게 튀지 않는다.
- 게임 오버처럼 중요한 상태는 화면 중앙을 장악하고 입력 흐름을 멈춘다.

## Canvas

- 현재 역할: 모든 데모 UI의 루트. `ScreenSpaceOverlay`로 화면 위에 고정된다.
- 성격: 관찰자. 직접 움직이지 않고, 자식 UI들이 상태에 맞춰 반응하게 받쳐준다.
- 행동 패턴:
  - 게임 시작 시 즉시 표시.
  - 해상도 변화에는 `CanvasScaler`가 대응한다.
  - 페이드 인/아웃은 개별 자식 오브젝트에서 처리한다.
- 구현 메모:
  - 전체 Canvas를 흔들거나 확대하면 전투 시야가 불안정해지므로 피한다.

## Title

- 표시 문구: `Augmented Defense - Play Test`
- 현재 위치: 좌상단, `anchoredPosition (18, -14)`
- 성격: 고정된 테스트 배너. 게임 분위기를 알려주지만 플레이 중에는 조용해야 한다.
- 행동 패턴:
  - 게임 시작 후 0.25초 동안 알파 0에서 1로 페이드 인.
  - 이후에는 정지.
  - 게임 오버 시 0.2초 동안 70% 밝기로 낮춰 배경 정보처럼 보이게 한다.
- 트리거:
  - `BuildPhase` 진입: 정상 밝기.
  - `GameOver` 진입: 낮은 밝기.

## Core Text

- 표시 형식: `Core current/max`
- 연결 데이터: `CoreHealth.HealthChanged`
- 성격: 방어 대상의 생명 신호. 피해를 받을 때만 강하게 반응한다.
- 행동 패턴:
  - 체력이 감소하면 0.12초 동안 붉은색으로 점멸.
  - 동시에 크기를 1.12배 키운 뒤 0.18초에 원래 크기로 복귀.
  - 체력이 30% 이하가 되면 약한 붉은 펄스를 0.8초 간격으로 반복.
  - 체력이 0이 되면 흔들림 0.25초 후 `Game Over Panel`에 시선을 넘긴다.
- 트리거:
  - `HealthChanged`: 숫자 갱신, 감소량 강조.
  - `GameOver`: 반복 펄스 중단, 어두운 색으로 고정.
- 예시 대사/톤:
  - 안정: "아직 버틴다."
  - 위험: "방어선이 흔들린다."
  - 파괴: "핵심 방어 실패."

## Gold Text

- 표시 형식: `Gold value`
- 연결 데이터: `EconomyManager.GoldChanged`
- 성격: 보상과 소비를 즉시 알려주는 자원 표시기.
- 행동 패턴:
  - 골드 증가 시 노란색으로 0.2초 플래시, 숫자가 살짝 위로 떠오르는 느낌.
  - 골드 감소 시 0.15초 동안 작게 눌렸다가 복귀.
  - 타워 설치 비용이 부족한 상태에서 클릭하면 0.2초 좌우 흔들림.
- 트리거:
  - `GoldChanged`: 증가/감소 방향에 따라 다른 반응.
  - `TowerPlacement.TrySpend` 실패 시: 부족 반응.
- 예시 대사/톤:
  - 획득: "전리품 확보."
  - 소비: "방어 자산 투입."
  - 부족: "자원이 모자라다."

## Wave Text

- 표시 형식: `Wave number`
- 연결 데이터: `WaveManager.CurrentWaveNumber`
- 현재 동작: `UIManager.Update()`에서 매 프레임 갱신.
- 성격: 전투 리듬을 알려주는 라운드 안내자.
- 행동 패턴:
  - 새 웨이브 시작 시 0.25초 동안 크기 1.2배 확대 후 복귀.
  - 웨이브 진행 중에는 정지.
  - 웨이브 종료 후 다음 단계로 넘어갈 때 0.15초 밝게 점멸.
  - 마지막 웨이브 이후 `Clear` 상태가 되면 `Wave Clear` 또는 `All Waves Cleared`로 바꾸는 확장 가능.
- 트리거:
  - `Start Wave Button` 클릭 후 `WavePhase`: 웨이브 시작 강조.
  - `AugmentPhase`: 다음 선택 단계 예고.
  - `Clear`: 클리어 표시.
- 예시 대사/톤:
  - 시작: "다음 공세가 온다."
  - 종료: "잠깐의 정비 시간."

## Hint Text

- 표시 문구: `WASD move   Space shoot   Left click place tower`
- 현재 위치: 좌하단, `anchoredPosition (18, 18)`
- 성격: 조용한 조작 안내판.
- 행동 패턴:
  - 게임 시작 후 3초 동안 정상 표시.
  - 플레이어가 이동, 사격, 타워 설치를 각각 한 번 이상 수행하면 알파를 45%로 낮춘다.
  - 게임 오버 시 숨기거나 20% 밝기로 낮춘다.
- 트리거:
  - 첫 이동 입력: `WASD` 안내 약화.
  - 첫 사격 입력: `Space` 안내 약화.
  - 첫 타워 설치: `Left click` 안내 약화.
- 구현 메모:
  - 현재는 단일 텍스트라 부분별 약화가 어렵다. 나중에 세 개의 텍스트로 분리하면 더 자연스럽다.

## Start Wave Button

- 표시 문구: `Start Wave`
- 연결 동작: `waveManager.StartNextWave`
- 현재 위치: 우상단, `anchoredPosition (-118, -24)`
- 성격: 전투 개시 스위치. 준비가 끝나면 누르라고 재촉한다.
- 행동 패턴:
  - `BuildPhase` 또는 `AugmentPhase`에서는 클릭 가능.
  - 클릭 가능할 때 1.2초 간격으로 아주 약한 밝기 펄스.
  - 클릭하면 0.08초 눌림 스케일 0.94, 이후 원래 크기로 복귀.
  - `WavePhase`에서는 비활성 색상으로 낮추고 펄스 중단.
  - 모든 웨이브 종료 후에는 숨기거나 `Cleared` 상태로 고정.
- 트리거:
  - `BuildPhase`: 활성.
  - `WavePhase`: 비활성.
  - `AugmentPhase`: 활성.
  - `Clear` 또는 `GameOver`: 비활성.
- 예시 대사/톤:
  - 대기: "준비되면 시작."
  - 진행 중: "공세 처리 중."

## Restart Button

- 표시 문구: `Restart`
- 연결 동작: `RestartDemo`
- 현재 위치: 우상단, `anchoredPosition (-118, -68)`
- 성격: 언제든 다시 시작할 수 있는 안전장치.
- 행동 패턴:
  - 평상시에는 낮은 강조도 유지.
  - 마우스 오버 시 밝기 증가.
  - 클릭 시 0.1초 눌림 후 씬 재시작.
  - `GameOver`에서는 `Game Over Panel`보다 낮은 우선순위지만 명확히 보이게 유지.
- 트리거:
  - 항상 활성.
  - `GameOver`: 색상 대비를 살짝 올려 재시작 유도.

## Game Over Panel

- 포함 객체: `Game Over Text`
- 표시 조건: `GameManager.State == GameState.GameOver`
- 현재 동작: `UIManager.HandleStateChanged()`에서 활성/비활성 전환.
- 성격: 전투 종료 선언. 다른 UI보다 우선한다.
- 행동 패턴:
  - 기본 상태는 비활성.
  - 게임 오버 진입 시 알파 0에서 0.72까지 0.25초 페이드 인.
  - 동시에 스케일 0.9에서 1.0으로 커지며 중앙에 고정.
  - 표시 중에는 배경 전투가 멈춘 느낌을 유지한다. 현재 `GameManager.GameOver()`가 `Time.timeScale = 0`으로 처리한다.
  - 재시작 전까지 사라지지 않는다.
- 트리거:
  - `GameOver`: 표시.
  - 씬 재시작: 초기 비활성.
- 예시 대사/톤:
  - "방어선 붕괴."
  - "재시작해서 배치를 바꿔라."

## Game Over Text

- 표시 문구: `GAME OVER`
- 성격: 최종 판정 문구. 짧고 무겁게 보여준다.
- 행동 패턴:
  - 패널 등장보다 0.05초 늦게 표시.
  - 첫 등장 시 1.08배에서 1.0배로 내려앉는다.
  - 1초 뒤 아주 느린 밝기 호흡 효과를 준다.
- 트리거:
  - `Game Over Panel` 활성화 직후.

## UI Manager

- 현재 역할: UI 텍스트와 패널을 게임 상태에 연결하는 중재자.
- 성격: 무대 뒤 컨트롤러. 직접 보이지 않지만 모든 UI 반응의 기준점이다.
- 행동 패턴:
  - `CoreHealth.HealthChanged`를 구독해 `Core Text`를 갱신한다.
  - `EconomyManager.GoldChanged`를 구독해 `Gold Text`를 갱신한다.
  - `GameManager.StateChanged`를 구독해 `Game Over Panel`을 켜고 끈다.
  - `Update()`에서 `Wave Text`를 현재 웨이브 번호로 유지한다.
- 확장 메모:
  - 애니메이션을 붙일 때는 `UIManager`가 단순히 텍스트만 바꾸는 대신, 각 UI 전용 뷰 컴포넌트에 `PlayChanged()`, `PlayWarning()`, `SetInteractableState()` 같은 명령을 보내는 구조가 좋다.

## 우선 구현 순서

1. `Core Text`, `Gold Text`, `Wave Text`에 값 변경 반응을 추가한다.
2. `Start Wave Button`에 활성/비활성 상태와 클릭 눌림 연출을 추가한다.
3. `Game Over Panel`에 페이드/스케일 등장 연출을 추가한다.
4. `Hint Text`를 입력 완료 상태에 따라 점점 약하게 만든다.
5. 필요하면 각 UI를 별도 컴포넌트로 분리해 `UIManager`가 연출을 호출하도록 정리한다.

# World Object Behavior Scripts

이 섹션은 UI와 함께 화면에서 움직이는 월드 객체의 행동 패턴을 정리한 것입니다. 기준 구현은 `DemoBootstrap`, `Tower`, `TowerAttack`, `TowerPlacement`, `Enemy`, `EnemyMovement`, `EnemySpawner`, `DefenderController`, `CoreHealth`입니다.

## 전체 객체 연출 원칙

- 월드 객체는 색상과 크기 변화가 UI보다 먼저 읽혀야 한다. 플레이어는 전투 상황을 먼저 보고, UI는 그 결과를 확인하는 흐름이 좋다.
- 공격, 피격, 사망, 설치, 코어 피해는 각각 UI에 같은 신호를 보내야 한다.
- 현재 MVP는 단색 사각형/원형 스프라이트 중심이므로, 작은 스케일 변화와 플래시만으로도 행동이 잘 보인다.
- 오브젝트별 연출 스크립트는 게임 규칙 스크립트와 분리하는 편이 좋다. 예: `TowerAttack`은 데미지만 처리하고, `TowerViewAnimator`가 발사 반응을 처리한다.

## Player Defender

- 현재 생성 위치: `(-4.5, -2.5, 0)`
- 현재 색상/크기: 녹색, `0.55 x 0.55`
- 연결 스크립트: `DefenderController`
- 성격: 직접 조작하는 기동 방어자. 빠르고 반응성이 좋아야 한다.
- 현재 행동:
  - `WASD`로 이동.
  - `Space`를 누르고 있으면 사거리 안 가장 가까운 적을 공격.
  - 공격 쿨다운은 데모 기준 `0.28`초.
- 행동 패턴:
  - 이동 입력이 있을 때 이동 방향으로 3~6도 기울기.
  - 정지하면 0.12초 안에 정면 상태로 복귀.
  - 공격 성공 시 몸체가 0.06초 동안 1.08배 커졌다가 복귀.
  - 공격 대상이 없는데 `Space`를 누르면 짧은 헛공격 펄스 또는 사거리 링 표시.
  - 피격 대상이 생기면 플레이어에서 적 방향으로 짧은 라인/빔을 0.08초 표시.
- UI 연결:
  - 첫 이동 입력 후 `Hint Text`의 `WASD` 안내 약화.
  - 첫 공격 후 `Hint Text`의 `Space` 안내 약화.
- 생성 후보 스크립트:
  - `DefenderViewAnimator`: 이동 기울기, 공격 펄스, 사거리 링 표시.
  - `DefenderAttackVfx`: 공격 라인/빔 생성과 제거.

## Starting Tower / Placed Tower

- 현재 시작 타워 위치: `(-1.5, 0.7, 0)`
- 현재 색상/크기: 노란색, `0.6 x 0.6`
- 연결 스크립트: `Tower`, `TowerAttack`
- 성격: 고정 방어 장치. 침착하지만 공격 순간은 날카롭게 보여야 한다.
- 현재 행동:
  - 사거리 안 적을 검색한다.
  - 가장 가까운 살아있는 적을 공격한다.
  - 데모 기준 데미지 `11`, 공격 간격 `0.55`, 사거리 `2.8`.
- 행동 패턴:
  - 적이 사거리에 들어오면 머리 또는 몸체가 적 방향을 바라본다.
  - 발사 시 0.08초 반동: 적 반대 방향으로 살짝 밀렸다가 복귀.
  - 발사 시 색상이 노란색에서 흰색으로 짧게 플래시.
  - 쿨다운 중에는 미세한 장전 펄스를 줄 수 있다.
  - 대상이 없을 때는 완전히 정지해 방어 포탑처럼 보이게 한다.
- UI 연결:
  - 타워가 적을 처치하면 `Gold Text`가 증가 반응.
  - 타워 설치로 골드가 줄면 `Gold Text`가 소비 반응.
- 생성 후보 스크립트:
  - `TowerViewAnimator`: 발사 반동, 회전, 플래시.
  - `TowerRangePreview`: 선택/배치 중 사거리 표시.
  - `TowerTargetLine`: 공격 순간 적까지 짧은 라인 표시.

## Tower Placement Ghost

- 현재 구현 상태: 아직 고스트 오브젝트는 없음. `TowerPlacement`가 마우스 위치에 즉시 타워를 생성한다.
- 성격: 설치 가능 여부를 알려주는 배치 예고 표시.
- 행동 패턴:
  - 마우스 위치를 따라 반투명 타워 실루엣 표시.
  - 설치 가능하면 녹색/노란색, 불가능하면 붉은색.
  - 설치 가능 위치에서는 사거리 원을 함께 표시.
  - 설치 실패 시 고스트가 0.15초 흔들리고 붉게 점멸.
  - 설치 성공 시 0.15초 스케일 업 후 실제 타워로 고정.
- UI 연결:
  - 골드 부족으로 설치 실패하면 `Gold Text` 부족 반응.
  - 첫 설치 성공 후 `Hint Text`의 `Left click` 안내 약화.
- 생성 후보 스크립트:
  - `TowerPlacementPreview`: 마우스 추적, 가능/불가능 색상, 사거리 원.
  - `PlacementFailureFeedback`: 실패 흔들림과 경고 플래시.

## Enemy

- 현재 생성 위치: 경로 첫 번째 웨이포인트.
- 현재 색상/크기: 붉은색, `0.45 x 0.45`
- 연결 스크립트: `Enemy`, `EnemyMovement`
- 성격: 단순하지만 끈질긴 침입자. 코어까지 계속 전진한다.
- 현재 행동:
  - 웨이포인트 배열을 따라 이동.
  - 코어에 도착하면 코어에 피해를 주고 사라진다.
  - 데미지를 받아 체력이 0 이하가 되면 사망하고 골드를 지급한다.
  - 데모 기준 체력 `35`, 이동 속도 `1.1`, 코어 피해 `10`, 보상 `7`.
- 행동 패턴:
  - 이동 중에는 진행 방향으로 살짝 늘어난 형태.
  - 피해를 받으면 0.08초 흰색 또는 밝은 붉은색 플래시.
  - 체력이 낮아질수록 흔들림 또는 깜빡임을 약하게 추가.
  - 사망 시 0.12초 축소와 페이드 후 제거.
  - 코어 도달 사망은 일반 처치와 구분해 안쪽으로 빨려 들어가는 느낌으로 처리.
- UI 연결:
  - 처치 사망: `Gold Text` 증가 반응.
  - 코어 도달: `Core Text` 피해 반응, 체력 0이면 `Game Over Panel` 표시.
- 생성 후보 스크립트:
  - `EnemyViewAnimator`: 이동 방향 스쿼시, 피격 플래시, 사망 축소.
  - `EnemyHealthBar`: 적 머리 위 작은 체력바.
  - `EnemyReachCoreVfx`: 코어 도달 시 충돌/흡수 효과.

## Enemy Spawner

- 현재 연결 스크립트: `EnemySpawner`
- 성격: 웨이브의 박자를 만드는 숨은 장치.
- 현재 행동:
  - 웨이브 수만큼 적을 생성한다.
  - 데모 기준 생성 간격은 `0.65`초.
  - 살아있는 적 수를 `AliveCount`로 관리한다.
- 행동 패턴:
  - 웨이브 시작 직전 경로 시작점에 0.4초 경고 표시.
  - 적 생성 순간 작은 붉은 플래시 또는 링 확장.
  - 마지막 적 생성 후 경고 표시 제거.
- UI 연결:
  - 웨이브 시작 시 `Wave Text` 강조.
  - 모든 적 제거 후 `Wave Text` 점멸, `Start Wave Button` 재활성.
- 생성 후보 스크립트:
  - `SpawnPointWarningView`: 생성 지점 경고 표시.
  - `WaveSpawnVfx`: 적 생성 순간 링/플래시.

## Enemy Path / Waypoints

- 현재 경로:
  - `(-7, 2.7, 0)`
  - `(-3.5, 2.7, 0)`
  - `(-3.5, -0.8, 0)`
  - `(1.2, -0.8, 0)`
  - `(1.2, 2.1, 0)`
  - `(5.9, 2.1, 0)`
- 현재 표시: 노란색 `LineRenderer`.
- 성격: 침입 경로를 알려주는 위험선.
- 행동 패턴:
  - 평상시에는 낮은 밝기의 선.
  - 웨이브 시작 시 0.3초 동안 경로 방향으로 빛이 흐르는 느낌.
  - 적이 코어에 가까워질수록 마지막 구간을 조금 더 밝게 표시.
  - 게임 오버 시 선 색상을 어둡게 낮춘다.
- UI 연결:
  - `Start Wave Button` 클릭과 함께 경로 강조.
  - `GameOver` 진입 시 경로 강조 중단.
- 생성 후보 스크립트:
  - `PathPulseView`: 웨이브 시작 경로 펄스.
  - `PathDangerView`: 코어 근처 위험 구간 강조.

## Core

- 현재 위치: 마지막 웨이포인트 근처 `(5.9, 2.1, 0)`
- 현재 색상/크기: 하늘색, `0.9 x 0.9`
- 연결 스크립트: `CoreHealth`
- 성격: 반드시 지켜야 하는 중심 목표. 평소에는 안정적이고, 피해 시 강하게 반응한다.
- 현재 행동:
  - 체력 `100`으로 시작.
  - 적이 도착하면 피해를 받는다.
  - 체력이 0이 되면 `GameManager.GameOver()` 호출.
- 행동 패턴:
  - 평상시에는 아주 느린 호흡 스케일 `1.0 -> 1.03 -> 1.0`.
  - 피해를 받으면 0.15초 붉은 플래시와 0.2초 흔들림.
  - 체력 30% 이하에서는 하늘색과 붉은색 사이를 느리게 점멸.
  - 체력 0이 되면 빛이 꺼지듯 어두워지고 `Game Over Panel` 등장.
- UI 연결:
  - 피해: `Core Text` 붉은 점멸.
  - 체력 0: `Game Over Panel` 표시, `Title` 밝기 감소, `Hint Text` 약화.
- 생성 후보 스크립트:
  - `CoreViewAnimator`: 호흡, 피격 흔들림, 위험 점멸, 파괴 연출.
  - `CoreDamageVfx`: 적 도달 시 충격파.

## Wave Object Flow

1. `Start Wave Button` 클릭.
2. `WaveManager.StartNextWave()`가 `WavePhase`로 전환.
3. `Wave Text`가 웨이브 시작 반응.
4. `EnemySpawner`가 경로 시작점에 적을 순차 생성.
5. `Enemy`가 경로를 따라 이동.
6. `Tower`와 `Player Defender`가 가장 가까운 적을 공격.
7. 적 처치 시 `Gold Text`가 증가 반응.
8. 적이 코어에 도달하면 `Core`와 `Core Text`가 피해 반응.
9. 모든 적이 사라지면 다음 웨이브 준비 또는 클리어 상태로 이동.
10. 코어 체력이 0이면 `Game Over Panel`이 중앙에 등장하고 전투가 멈춘다.

## 추천 생성 스크립트 목록

- `DefenderViewAnimator`: 플레이어 이동/공격 시각 반응.
- `DefenderAttackVfx`: 플레이어 공격 라인 또는 빔.
- `TowerViewAnimator`: 타워 회전, 발사 반동, 플래시.
- `TowerRangePreview`: 배치/선택 중 사거리 표시.
- `TowerPlacementPreview`: 마우스 위치 설치 고스트.
- `PlacementFailureFeedback`: 설치 실패 흔들림, 붉은 플래시.
- `EnemyViewAnimator`: 피격, 이동, 사망 연출.
- `EnemyHealthBar`: 적 체력바.
- `SpawnPointWarningView`: 웨이브 시작 전 생성 지점 경고.
- `WaveSpawnVfx`: 적 생성 순간 효과.
- `PathPulseView`: 경로 펄스.
- `CoreViewAnimator`: 코어 호흡, 피격, 파괴 연출.
- `CoreDamageVfx`: 코어 충격파.

## 객체와 UI 통합 우선 구현 순서

1. `EnemyViewAnimator`, `TowerViewAnimator`, `CoreViewAnimator`를 먼저 만든다.
2. `TowerPlacementPreview`를 추가해 설치 전부터 플레이어가 결과를 예측하게 한다.
3. `Gold Text`, `Core Text`, `Wave Text` 반응을 월드 이벤트와 연결한다.
4. `EnemyHealthBar`와 `TowerRangePreview`를 추가해 전투 정보를 보강한다.
5. `SpawnPointWarningView`와 `PathPulseView`로 웨이브 시작 연출을 완성한다.
