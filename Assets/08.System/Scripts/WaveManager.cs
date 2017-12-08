using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{

    [Header("Script Component")]
    public RoundManager roundManager;
    public EnemyMovement enemyMovement;

    public RoundManager.Difficulty m_difficulty;
    public RoundManager.State m_state;

    [Header("Wave Process")]
    public int waveCount;
    public int waveAdd;
    public int waveTime;
    public int waveClearTime;

    [Header("Wavetext & Anim")]
    public Text waveText;
    public Animator waveTextAnim;

    // 소환시간
    private WaitForSeconds spawnTime;

    public bool gameOver, roundStart;

    public GameObject enemy1;

    private void Start()
    {
        DifficultySet();
        waveClearTime = 0;
    }

    private void DifficultySet()
    {
        switch (m_difficulty)
        {
            case RoundManager.Difficulty.EZ:
                waveCount = 3;
                spawnTime = new WaitForSeconds(2f);
                waveTime = 10;
                waveAdd = 2;
                break;

            case RoundManager.Difficulty.NM:
                waveCount = 6;
                waveTime = 12;
                waveAdd = 3;
                spawnTime = new WaitForSeconds(1.8f);
                break;

            case RoundManager.Difficulty.HD:
                waveCount = 8;
                waveTime = 16;
                waveAdd = 4;
                spawnTime = new WaitForSeconds(1.6f);
                break;

            case RoundManager.Difficulty.SHD:
                waveCount = 10;
                waveTime = 20;
                waveAdd = 5;
                spawnTime = new WaitForSeconds(1.5f);
                break;
        }
    }

    void Update()
    {
        //m_difficulty = roundManager.m_difficulty;
        m_state = roundManager.m_state;
        gameOver = roundManager.isGameOver;
        roundStart = roundManager.isRoundStart;
    }

    // 게임시작 시 웨이브
    void BossScene()
    {

    }
    public IEnumerator Wave()
    {
        m_state = RoundManager.State.PLAY;
        waveText.text = "ROUND " + (waveClearTime + 1);
        waveTextAnim.SetTrigger("WaveStart");   // 웨이브 시작 애니메이션 재생
        Debug.Log("웨이브 시작");
        yield return new WaitForSeconds(3f);

        // 만약 현재 웨이브가 보스 웨이브라면 보스 웨이브 전용 애니메이션 재생
        if ((waveClearTime + 1) == waveCount)
        {
            AudioManager.instance.StopAudio();
            //roundManager.gamingAudio.Stop();
            AudioManager.instance.PlayAudio(BackgroundMusic.Alert);
            //roundManager.alertAudio.Play();
            waveText.text = "BOSS APPEAR!!";
            waveTextAnim.SetTrigger("BossWave");
            yield return new WaitForSeconds(4f);

            AudioManager.instance.PlayAudio(BackgroundMusic.Boss);
            //roundManager.bossAudio.Play();
        }

        if (gameOver == false && roundStart == true && m_state == RoundManager.State.PLAY)   // 웨이브 시작중
        {
            // 기본 waveTime에 난이도 별 waveAdd 추가
            // ex ) EZ 난이도 1 웨이브면 10번 소환 + EZ기준 2번 소환
            for (int i = 1; i <= waveTime + waveAdd; i++)
            {
                if (m_state == RoundManager.State.OVER || gameOver == true)
                {
                    break;
                }
                //Vector3 pos = new Vector3(Random.Range(-5.0f, 5.0f), 20f, Random.Range(-5.0f, 5.0f));
                //Quaternion camDirectionRotation = Quaternion.LookRotation(cam.transform.forward);

                //Instantiate(enemy1, pos, camDirectionRotation);
                //Instantiate(dragon);
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
        for (;;)
        {
            if (gameOver == true) // 검색하기 전에 게임오버 상태인지 판별하기
            {
                yield break;    // 코루틴 종료
            }

            GameObject[] g = null;
            g = GameObject.FindGameObjectsWithTag("Enemy");

            Debug.Log("적 오브젝트 찾는중");

            if (g.Length > 0)  // 적 남아있어?
            {
                Debug.Log("아직 적이 남아 있습니다.");
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
        m_state = RoundManager.State.WAIT;


        // 만약 해당 웨이브가 최종 웨이브였다면 Win() 메소드 실행
        if (waveClearTime == (waveCount - 1))
        {
            roundManager.Win();
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
}