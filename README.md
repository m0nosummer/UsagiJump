<!-- 여기에 게임 시작 화면 스크린샷 추가 -->

## 프로젝트 개요

**Usagi Jump**는 Unity 엔진으로 개발한 2D 횡스크롤 점프 게임입니다. 플레이어는 토끼 캐릭터를 조작하여 끊임없이 생성되는 플랫폼을 건너며 최고 점수를 달성하는 것이 목표입니다. 게임의 핵심은 **색상 매칭 시스템**으로, 캐릭터의 색상과 플랫폼의 색상이 일치해야만 착지할 수 있습니다.

### 게임 방식
- 화면을 길게 터치하여 점프 충전 (충전 게이지를 표시하지 않기에 난이도 증가)
- Green/Orange 두 가지 상태 중 하나를 유지
- 당근 아이템을 획득하여 색상 전환
- 캐릭터 색상과 플랫폼 색상이 일치해야 착지 성공
- 색상이 불일치하면 게임오버

### 기술 스택
- **Engine**: Unity 2022.3
- **Language**: C#
- **SDK**: Google Play Games Services
- **UI**: TextMeshPro, Unity UI
---

## 핵심 기능 구현

### 1. 터치 기반 차징 점프 시스템
**파일**: `JumpController.cs`

플레이어의 터치 입력을 3단계(Began → Stationary/Moved → Ended)로 처리하여 점프력을 조절합니다.

```csharp
if (touch.phase == TouchPhase.Began)
{
    _jumpChargeTime = 0f;
    // 점프 준비 스프라이트로 변경
}
else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
{
    // 최대 2초까지 충전
    _jumpChargeTime = Mathf.Min(_jumpChargeTime + Time.deltaTime, maxChargeTime);
}
else if (touch.phase == TouchPhase.Ended)
{
    // 충전 시간에 비례한 점프력 계산
    float jumpForce = Mathf.Lerp(jumpForceMin, jumpForceMax, _jumpChargeTime / maxChargeTime);
    Vector2 jumpVector = new Vector2(horizontalJumpSpeed, jumpForce);
    _rb.AddForce(jumpVector, ForceMode2D.Impulse);
}
```

**주요 특징**:
- `Mathf.Lerp`를 활용한 점프력 보간 (5~10 범위)
- 수평 속도와 수직 점프력을 결합한 포물선 운동
- `IsGrounded` 플래그로 공중 점프 방지
- UI 터치 방지를 위한 `EventSystem.IsPointerOverGameObject()` 체크

### 2. 색상 전환 메커니즘
**파일**: `PlayerController.cs`, `GameManager.cs`

게임의 핵심 로직으로, 캐릭터의 색상 상태와 플랫폼 색상을 비교하여 게임 진행을 결정합니다.

```csharp
void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Green"))
    {
        if (gameManager.isGreen == false)  // 색상 불일치
        {
            inGameUI.ShowGameOver();
            return;
        }
        // 착지 성공
        _rb.linearVelocity = Vector2.zero;
        IsGrounded = true;
        playerSpriteRenderer.sprite = gameManager.greenIdleSprite;
    }
}

void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.gameObject.CompareTag("Carrot"))
    {
        // 당근 획득 시 색상 토글
        gameManager.isGreen = !gameManager.isGreen;
        // 스프라이트 즉시 변경
        playerSpriteRenderer.sprite = gameManager.isGreen 
            ? gameManager.greenJumpingSprite 
            : gameManager.orangeJumpingSprite;
    }
}
```

**상태 관리**:
- Boolean 플래그 `isGreen`으로 명확한 상태 관리
- 6가지의 직접 그린 스프라이트 활용 (Green/Orange × Idle/JumpPreparation/Jumping)
- 실시간 스프라이트 교체로 유저가 상태 확인에 어려움이 없도록 함

### 3. 프로시저럴 맵 생성 시스템
**파일**: `MapGenerator.cs`

플레이어의 진행에 따라 무한히 맵을 생성하고, 뷰포트를 벗어난 맵은 자동으로 삭제하여 메모리를 관리합니다.

```csharp
void Update()
{
    // 플레이어가 특정 지점을 넘으면 새 세그먼트 생성
    if (playerTransform.position.x > _spawnX - (segmentLength * 3) - segmentLength + 1.0f)
    {
        SpawnSegment();
        DeleteSegment();
    }
    
    // 실시간 점수 계산
    gameManager.score = (int)(playerTransform.position.x - _startPos.x) * 10 
                        + (gameManager.carrotCnt * 100);
}

private void SpawnSegment()
{
    // Green과 Orange 플랫폼을 교차로 생성
    GameObject segment = Instantiate(mapSegments[_nextSquare]);
    _nextSquare = (_nextSquare + 1) % 2;
    
    // 플랫폼 길이 랜덤화 (0.5~1.5배)
    Vector3 tempScale = segment.transform.localScale;
    tempScale.x = Random.Range(0.5f, 1.5f);
    segment.transform.localScale = tempScale;
    
    // 정확한 위치 계산 (플랫폼 양 끝을 기준으로)
    _spawnX += tempScale.x * 0.5f;
    segment.transform.position = Vector3.right * _spawnX;
    _spawnX += segment.transform.localScale.x * 0.5f;
    
    // 10% 확률로 당근 생성
    if (Random.value < carrotSpawnChance)
    {
        SpawnCarrotNearSegment(segment);
    }
}
```

**최적화 기법**:
- 활성 세그먼트를 리스트로 관리하여 메모리 효율 극대화
- 화면 밖 오브젝트 자동 삭제
- 부모-자식 관계 활용 (당근을 플랫폼의 자식으로 설정해서 랜덤 맵 -> 랜덤 당근 생성으로 이어지게)

### 4. 점수 및 리더보드 시스템
**파일**: `GameManager.cs`, `GoogleManager.cs`

```csharp
// 점수 계산
public void Update()
{
    scoreText.text = score.ToString("0") + "m";
}

// 게임 종료 시 최고 점수 갱신 및 리더보드 업로드
public void UpdateScore()
{
    if (score > PlayerPrefs.GetInt("HighScore", 0))
    {
        PlayerPrefs.SetInt("HighScore", score);
        SubmitToLeaderboard(score);
    }
}

// Google Play Games Services 연동
private void SubmitToLeaderboard(int highScore)
{
    Social.ReportScore(highScore, GPGSIds.leaderboard_top_rankings, (bool success) => {});
}
```

**점수 구성**:
- 이동 거리: `(playerX - startX) × 10`
- 당근 보너스: `carrotCnt × 100`
- (당근을 무시하지 않고 플레이어에게 고득점을 위한 리스크 있는 플레이를 유도)
- PlayerPrefs로 로컬에 최고 점수 저장
- Google Play Games Services를 통한 글로벌 리더보드

### 5. 게임 상태 관리
**파일**: `InGameUI.cs`

```csharp
public void PauseGame()
{
    pausePanel.SetActive(true);
    Time.timeScale = 0f;  // 게임 시간 정지
    jumpController.EnableInput(false);  // 입력 비활성화
}

public void ResumeGame()
{
    Time.timeScale = 1f;
    pausePanel.SetActive(false);
    StartCoroutine(ResumeGameAfterDelay(0.1f));  // 딜레이 후 입력 재활성화
}

private IEnumerator ResumeGameAfterDelay(float delay)
{
    yield return new WaitForSecondsRealtime(delay);  // 실시간 기준으로 대기
    jumpController.EnableInput(true);
}
```

**일시정지 처리**:
- `Time.timeScale` 조정으로 게임 시간 제어
- 입력 시스템 별도 비활성화
- Coroutine을 활용한 재개 시 입력 누적 방지

---

## 기술적 도전과 해결

### 문제 1: UI 터치와 게임플레이 터치 충돌
**증상**: 일시정지 버튼이나 설정 버튼을 누를 때 동시에 점프가 발동되는 문제

**원인**: Unity의 터치 입력이 UI와 게임 오브젝트를 구분하지 않고 모두 감지

**해결책**:
```csharp
if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) 
{ 
    return; 
}
```
EventSystem을 활용하여 UI 영역 터치를 필터링

### 문제 2: 게임 재개 시 의도치 않은 점프 발생
**증상**: 일시정지 해제 직후 캐릭터가 자동으로 점프하는 현상

**원인**: `Time.timeScale = 0`에서 `1`로 전환되는 순간, 이전에 누적된 터치 입력이 한꺼번에 처리됨

**해결책**:
```csharp
private IEnumerator ResumeGameAfterDelay(float delay)
{
    yield return new WaitForSecondsRealtime(delay);
    Time.timeScale = 1f;
    jumpController.EnableInput(true);
}
```
실시간 기준 Coroutine으로 0.1초 딜레이를 추가하여 입력 버퍼 초기화

### 문제 3: 맵 세그먼트 위치 계산 오류
**증상**: 플랫폼 크기가 랜덤이다 보니 플랫폼 간 간격이 일정하지 않거나 겹치는 문제

**원인**: 중심점 기준으로 위치를 계산하여 스케일 변화를 반영하지 못함

**해결책**:
```csharp
_spawnX += tempScale.x * 0.5f;  // 이전 플랫폼의 오른쪽 끝
segment.transform.position = Vector3.right * _spawnX;
_spawnX += segment.transform.localScale.x * 0.5f;  // 현재 플랫폼의 오른쪽 끝
```
플랫폼의 양 끝(edge)을 기준으로 위치를 계산하여 정확한 배치 구현

### 문제 4: 튜토리얼 중복 표시 방지
**증상**: 게임을 재시작할 때마다 튜토리얼이 다시 표시됨

**해결책**:
```csharp
if (PlayerPrefs.GetInt("Test6HasSeenTutorialPanel", 0) == 0)
{
    jumpController.EnableInput(false);
    Time.timeScale = 0f;
    tutorialPanel.SetActive(true);
}
```
PlayerPrefs를 활용하여 튜토리얼 표시 여부를 영구 저장

---

## 프로젝트 구조

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs          # 게임 상태, 점수, 스프라이트 관리
│   │   ├── Singleton.cs            # 싱글톤 패턴 베이스 클래스
│   │   └── SoundManager.cs         # 오디오 재생 관리
│   ├── Player/
│   │   ├── PlayerController.cs     # 충돌 감지 및 색상 매칭 로직
│   │   └── JumpController.cs       # 터치 입력 및 점프 물리
│   ├── Map/
│   │   ├── MapGenerator.cs         # 맵 생성
│   │   ├── ParallaxBackground.cs   # 패럴랙스 배경 효과
│   │   └── CameraController.cs     # 카메라 플레이어 추적
│   ├── UI/
│   │   ├── MainMenuUI.cs           # 메인 메뉴 인터페이스
│   │   ├── InGameUI.cs             # 인게임 UI 및 일시정지
│   │   └── SettingMenu.cs          # 설정 메뉴 (사운드, 개인정보)
│   └── Services/
│       └── GoogleManager.cs        # Google Play Games 연동
```

---

## 사용된 디자인 패턴

### Singleton Pattern
```csharp
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance { get { ... } }
}
```
GameManager, SoundManager 등 전역 접근이 필요한 클래스에 적용

### Object Pooling 개념
활성 맵 세그먼트를 리스트로 관리하고 재사용하여 Instantiate/Destroy 호출 최소화

### State Pattern (간소화 버전)
Boolean 플래그(`isGreen`)를 활용한 단순하지만 효과적인 상태 관리

---

## 스크린샷

<!-- 여기에 실제 스크린샷 추가 -->
| 메인 메뉴 | 게임 플레이 |
|:---:|:---:|:---:|
| ![1](https://github.com/user-attachments/assets/66bbde40-91d1-45b5-b4d2-d1ff364f964a) | ![Video Project](https://github.com/user-attachments/assets/c4b3a364-a505-4154-8515-f8e58fea8b0d) |

---

## 배운 점

### 1. 모바일 입력 처리
- Unity의 터치 입력 시스템에 대한 이해
- EventSystem을 활용한 UI와 게임플레이 입력 분리 기법
- Time.timeScale 조작 시 발생하는 입력 버퍼 문제와 해결 방법

### 2. 프로시저럴 생성 알고리즘
- 제한된 리소스로 무한한 콘텐츠를 생성하는 방법
- 메모리 효율을 고려한 오브젝트 생명주기 관리
- 랜덤성과 공정성의 균형 (플랫폼 크기 제한)

### 3. 외부 서비스 연동
- Google Play Games Services와 리더보드 시스템 이용해보기
- 이 부분은 에러가 많아 확실히 해결하지는 못했음

### 4. 게임 최적화
- Object Pooling을 통한 메모리 및 성능 최적화
- Audio Mixer를 활용한 효율적인 사운드 관리
- Singleton 패턴의 적절한 활용과 남용 방지

### 5. 게임 플레이
- 어떻게 해야 플레이어가 게임에 대한 흥미를 느끼고 다시 도전하게 할 지 고민
- 당근과 같은 오브젝트를 강제할 수 있는 방법을 고안
- 플레이어가 맵이 랜덤으로 제작되고 있다는 것을 느끼게 할 수 있는 방법 고안
---
## 추가 사항
- 효과음 소스, 글꼴, 배경은 무료 라이센스 사용
- 나머지 캐릭터 스프라이트 및 UI 스프라이트는 전부 직접 제작
- This project is for portfolio purposes only.
