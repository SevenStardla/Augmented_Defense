# Augmented Defense

> "증강"을 선택하며 방어 체계를 성장시키는 2D 웨이브 디펜스 게임  
> A 2D wave-defense game where each augment changes your defensive build.

현재 Unity로 개발 중인 플레이 가능한 프로토타입입니다.  
OpenAI Game Builders Seoul 온라인 챌린지 제출을 목표로 개발하고 있습니다.

## 게임 소개

플레이어는 직접 전장을 이동하고 공격하면서 타워를 배치해 코어를 방어합니다.

각 웨이브를 완료하면 세 개의 증강 중 하나를 선택할 수 있습니다.  
선택한 증강에 따라 타워, 디펜더, 경제 또는 코어 능력이 강화됩니다.

```text
타워 배치
→ 웨이브 방어
→ 증강 선택
→ 빌드 강화
→ 다음 웨이브
```

## 현재 구현된 기능

- 경로를 따라 이동하는 적 웨이브
- 일반, 러너, 탱커 적과 마지막 웨이브 보스
- 코어 체력과 게임오버
- 골드를 사용한 타워 배치
- 타워 자동 공격
- 플레이어 이동과 직접 공격
- 웨이브 종료 후 증강 3개 선택
- 증강 중첩과 실제 능력치 적용
- 1배속, 2배속, 3배속 전환
- 승리, 패배 및 재시작
- 공격, 피격, 배치 범위 등의 기본 시각 피드백
- 브라우저에서 실행 가능한 WebGL 빌드

## 현재 증강 효과

- 타워 피해 증가
- 타워 공격속도 증가
- 타워 사거리 증가
- 디펜더 피해 증가
- 디펜더 공격속도 증가
- 디펜더 공격 범위 증가
- 적 처치 골드 증가
- 코어 체력 회복

## 조작법

| 입력 | 기능 |
|---|---|
| `WASD` | 디펜더 이동 |
| `Space` | 범위 안의 가장 가까운 적 공격 |
| 마우스 왼쪽 버튼 | 타워 배치 |
| `Start Wave` | 다음 웨이브 시작 |
| `Speed` | 1배속, 2배속, 3배속 전환 |
| 증강 카드 클릭 | 증강 선택 |


## 실행 방법

### 필요 환경

- Unity 6000.4.8f1
- Windows 개발 환경

### 로컬 실행

1. 저장소를 복제합니다.

    ```bash
    git clone https://github.com/SevenStardla/Augmented_Defense.git
    ```

2. Unity Hub에서 저장소 폴더를 엽니다.

3. Assets/Scenes/Main.unity 씬을 엽니다.

4. Unity Editor의 Play 버튼을 누릅니다.

DemoBootstrap이 실행 중 필요한 게임 오브젝트와 UI를 자동으로 구성합니다.

## 개발 상태

현재 단계는 핵심 플레이 루프를 검증하는 프로토타입입니다.

기본 웨이브 전투

타워 배치와 경제

증강 선택과 실제 능력치 적용

승리·패배·재시작

전체 플레이 루프 반복 검증

적 종류와 웨이브 구성 확장

증강 종류 확장

사운드와 전투 연출 개선

WebGL 빌드 및 공개 플레이 링크

외부 플레이테스트와 밸런스 조정
## 주요 코드 구조

```text
Assets/Scripts
├─ Core       게임 상태, 코어 체력, 경제
├─ Wave       웨이브 데이터와 진행
├─ Enemy      적 생성, 이동, 체력
├─ Tower      타워 배치, 공격, 표시
├─ Player     디펜더 이동과 공격
├─ Augment    증강 데이터, 선택, 능력치
├─ UI         게임 UI와 증강 선택 화면
└─ Demo       실행 중 데모 구성을 생성하는 Bootstrap
```

## 개발 문서

- [제출 실행 계획](SUBMISSION_EXECUTION_PLAN.md)
- [제출 자료 초안](SUBMISSION_MATERIALS.md)
- [날짜별 개발 기록](DevelopmentLogs/README.md)
- [개발 가이드](Augmented_Defense_Development_Guide.md)
- [스크립트 구성 안내](Assets/Scripts/README.md)

## Codex 활용

이 프로젝트는 Codex와 함께 다음 작업을 진행하고 있습니다.

기존 코드와 게임 흐름 분석
증강 시스템 구현 및 리팩터링
컴파일 오류와 상태 전환 문제 점검
제출 일정과 테스트 체크리스트 구성
기능을 구현한 뒤 Unity PlayMode에서 개발자가 직접 동작을 확인하고, 학습한 내용과 검증 결과를 날짜별 개발 기록으로 관리합니다.
## 기술

Unity 6
C#
Unity UI
2D Physics
## 향후 목표

브라우저에서 바로 실행할 수 있는 짧고 완결된 WebGL 데모를 만드는 것이 현재 목표입니다.
