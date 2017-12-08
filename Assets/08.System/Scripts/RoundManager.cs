using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    [Header("Steam Controller")]
    public SteamVR_TrackedController ctrRight;

    public SteamVR_TrackedController ctrLeft;

    [Header("Wave Default Setting")]
    public GameObject enemy1;

    public GameObject bossPoint;

    [Header("Text")]
    public Text scoreText;

    public Text roundStartText;
    public Text resultText;
    public Text finishText;
    public Text waveText;

    [Header("HP Bar")]
    public Slider HP_bar;

    public GameObject hp_Slider;

    [Header("Text Animation")]
    public Animator roundStartTextAnim;

    public Animator finishTextAnim;
    public Animator waveTextAnim;

    [Header("State Bool")]
    public bool isRoundStart;

    public bool isGameOver;
    public bool isWaveOver;

    [Header("Total Score")]
    public int score;

    [Header("Wave Process")]
    public int waveCount;

    public int waveAdd;
    public int waveTime;
    public int waveClearTime;

    [Header("Result Text")]
    public string isResult;

    [Header("Script Component")]
    public EnemyMovement enemyMovement;

    // 상태 및 난이도 판별용 열거형
    public enum Difficulty { EZ, NM, HD, SHD }

    public enum State { READY, PLAY, WAIT, OVER }

    public State m_state;
    public Difficulty m_difficulty;

    private Color alphaZero = new Color(255, 255, 255, 0);
    private Color alphaMax = new Color(255, 255, 255, 255);

    private Camera cam;

    public WaitForSeconds spawnTime;

    private void Start()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        isRoundStart = true;
        m_state = State.READY;
        m_difficulty = Difficulty.EZ;

        AudioManager.instance.StopAudio();
        StartCoroutine(RoundStart());

        cam = Camera.main;
        DifficultySetup();

        //AudioManager.instance.PlayAudio(BackgroundMusic.Lobby);
    }

    private void Update()
    {
        scoreText.text = ScoreManager.instance.DisplayScore();
        //"SCORE : " + score;

        // 게임 준비 상태일 때에 시작하기
        //if (isRoundStart == false && m_state == State.READY)
        //{
        //    if (ctrLeft.triggerPressed == true || ctrRight.triggerPressed == true || Input.GetKey(KeyCode.Space))
        //    {
        //        isRoundStart = true;
        //        AudioManager.instance.StopAudio();
        //        StartCoroutine(RoundStart());
        //    }
        //}

        // 게임오버 상태일 때 커맨드 조작
        if (isGameOver == true && m_state == State.OVER)
        {
            if (ctrLeft.triggerPressed == true || ctrRight.triggerPressed == true || Input.GetKey(KeyCode.Space))
            {
                // 재시작
                GameRestart();
            }
        }
    }

    // 라운드 시작 시 애니메이션 재생
    private IEnumerator RoundStart()
    {
        scoreText.color = alphaZero;
        //titleText.color = alphaZero;
        roundStartText.text = "READY";
        roundStartText.color = alphaMax;

        roundStartTextAnim.SetTrigger("Start");

        yield return new WaitForSeconds(2f);

        roundStartText.text = "START!!";

        yield return new WaitForSeconds(2f);

        roundStartText.color = alphaZero;
        scoreText.color = alphaMax;
        hp_Slider.SetActive(true);
        m_state = State.PLAY;
        StartCoroutine(Wave());
        AudioManager.instance.PlayAudio(BackgroundMusic.Normal);
    }

    // 게임 종료
    public void GameOver()
    {
        AudioManager.instance.PlayAudio(BackgroundMusic.Result);

        isGameOver = true;
        isRoundStart = false;

        m_state = State.OVER;
        resultText.color = alphaMax;
        //resultText.text = isResult + "\n\nYOUR SCORE : " + scoreText.text + "\n\nRETRY ?\n\n(PRESS 'FIRE' BUTTON)";
        resultText.text = ScoreManager.instance.SummaryScore();
    }

    // 게임 승리 및 애니메이션 재생
    public void Win()
    {
        AudioManager.instance.StopAudio();
        //bossAudio.Stop();
        AudioManager.instance.PlayAudio(BackgroundMusic.Win);

        isGameOver = true;
        isRoundStart = false;

        finishText.text = "VICTORY!!";
        isResult = "YOU WIN!!";
        finishTextAnim.SetTrigger("Victory");
        Invoke("GameOver", 5f);
        scoreText.color = alphaZero;
    }

    // 게임 패배 및 애니메이션 재생
    public void Defeat()
    {
        if ((waveClearTime + 1) == waveCount)
        {
            AudioManager.instance.StopAudio();
        }
        else
        {
            AudioManager.instance.StopAudio();
        }

        AudioManager.instance.PlayAudio(BackgroundMusic.Defeat);

        isGameOver = true;
        isRoundStart = false;
        finishText.text = "DEFEATED..";
        isResult = "YOU LOSE..";
        finishTextAnim.SetTrigger("Defeat");
        Invoke("GameOver", 5f);
        scoreText.color = alphaZero;
    }

    // 재시작
    private void GameRestart()
    {
        score = 0;  // 스코어 초기화
        resultText.enabled = false;

        // 씬 매니저로 씬을 새로 불러올까 아니면 그냥 재시작 개념? 추후 고려
        SceneManager.LoadScene(0);
    }

    // 게임시작 시 웨이브
    private IEnumerator Wave()
    {
        m_state = State.PLAY;
        waveText.text = "ROUND " + (waveClearTime + 1);
        waveTextAnim.SetTrigger("WaveStart");   // 웨이브 시작 애니메이션 재생
        Debug.Log("웨이브 시작");
        yield return new WaitForSeconds(3f);

        // 만약 현재 웨이브가 보스 웨이브라면 보스 웨이브 전용 애니메이션 재생
        if ((waveClearTime + 1) == waveCount)
        {
            Instantiate(bossPoint);
            AudioManager.instance.StopAudio();
            //roundManager.gamingAudio.Stop();
            AudioManager.instance.PlayAudio(BackgroundMusic.Alert);
            //roundManager.alertAudio.Play();
            waveText.text = "BOSS APPEAR!!";
            waveTextAnim.SetTrigger("BossWave");

            yield return new WaitForSeconds(4f);
            AudioManager.instance.PlayAudio(BackgroundMusic.Boss);
            //roundManager.bossAudio.Play();
            waveTime += waveAdd;
            StartCoroutine(IsWaveClear());
            yield break;
        }

        if (isGameOver == false && isRoundStart == true && m_state == State.PLAY)   // 웨이브 시작중
        {
            // 기본 waveTime에 난이도 별 waveAdd 추가
            // ex ) EZ 난이도 1 웨이브면 10번 소환 + EZ기준 2번 소환
            for (int i = 1; i <= waveTime + waveAdd; i++)
            {
                if (m_state == State.OVER || isGameOver == true)
                {
                    break;
                }
                enemyMovement.InitOnPos(enemy1);
                yield return spawnTime; // EZ : 2초 / NM : 1.8초 / HD : 1.6초 / SHD : 1.5초 간격으로 리스폰
            }
        }

        // 다음 웨이브 리스폰 횟수는 waveAdd만큼 증가시킨다.
        waveTime += waveAdd;

        // 해당 웨이브가 다 끝나면 몹 다 죽었는지 판별
        StartCoroutine(IsWaveClear());
        Debug.Log("웨이브가 끝났습니다.");
    }

    // 한 웨이브 적들 스폰 완료하였을때 적이 살아있는지 여부 판별
    private IEnumerator IsWaveClear()
    {
        // 5초마다 적 오브젝트 남았는지 검색
        for (; ; )
        {
            if (isGameOver == true) // 검색하기 전에 게임오버 상태인지 판별하기
            {
                yield break;    // 코루틴 종료
            }

            GameObject[] g, h = null;

            g = GameObject.FindGameObjectsWithTag("Enemy");
            h = GameObject.FindGameObjectsWithTag("Head");

            int enemyExist = g.Length + h.Length;

            Debug.Log("적 오브젝트 찾는중");

            if (enemyExist > 0)  // 적 남아있어?
            {
                Debug.Log("아직 적이 " + enemyExist + "마리 남아 있습니다.");
            }
            else // 안 남아 있음
            {
                StartCoroutine(WaveClear());
                Debug.Log("남아있는 적 오브젝트가 없습니다");
                yield break;
            }
            yield return new WaitForSeconds(3f);
        }
    }

    private IEnumerator WaveClear()
    {
        Debug.Log("웨이브를 성공적으로 마쳤습니다.");
        m_state = State.WAIT;

        // 만약 해당 웨이브가 최종 웨이브였다면 Win() 메소드 실행
        if (waveClearTime == (waveCount - 1))
        {
            Win();
        }
        else if (waveClearTime < waveCount) // 최종 웨이브가 아닐 경우 웨이브 클리어 애니메이션 재생
        {
            waveText.text = "CLEAR!!";
            waveTextAnim.SetTrigger("WaveClear");
            waveClearTime++; // 웨이브 클리어 횟수 1 증가
            yield return new WaitForSeconds(5f);
            StartCoroutine(Wave());
        }
    }

    // 난이도 설정 후 적용
    private void DifficultySetup()
    {
        switch (m_difficulty)
        {
            case Difficulty.EZ:
                waveCount = 3;
                spawnTime = new WaitForSeconds(2f);
                waveTime = 10;
                waveAdd = 2;
                break;

            case Difficulty.NM:
                waveCount = 6;
                waveTime = 12;
                waveAdd = 3;
                spawnTime = new WaitForSeconds(1.8f);
                break;

            case Difficulty.HD:
                waveCount = 8;
                waveTime = 16;
                waveAdd = 4;
                spawnTime = new WaitForSeconds(1.6f);
                break;

            case Difficulty.SHD:
                waveCount = 10;
                waveTime = 20;
                waveAdd = 5;
                spawnTime = new WaitForSeconds(1.5f);
                break;
        }
    }
}