using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float horizontalSpeed;   // 수평이동 코드
    public float verticalSpeed; // 수직이동 코드
    public float amplitude = 20f; //
    public float speedRate = 1f;

    public GameObject enemyBullet;
    public GameObject spawnGate;
    public GameObject modelFolder;

    // 가로 세로 방향은 동급이 되면 대각선으로만 이동하게 됨

    public float enemy2_yDiff = 0f;

    private Vector3 tempPosition;
    private GameObject player;
    private Camera cam;

    private float waitTime;

    private RoundManager manager;

    private bool gameOver;
    private bool isSpawnOver;

    private void Start()
    {
        tempPosition = transform.position;  // 초기값은 이 오브젝트의 현재 위치로 설정
        //EffectManager.instance.LaunchEffect(transform.position, ParticleEffect.Appear);

        horizontalSpeed = Random.Range(0.5f, 2f) * speedRate;
        verticalSpeed = Random.Range(0.5f, 2f) * speedRate;
        player = GameObject.FindGameObjectWithTag("Player");

        cam = Camera.main;
        StartCoroutine(EnemyAttack());
        amplitude = Random.Range(10f, 25f);
        manager = GameObject.FindGameObjectWithTag("SpawnPoint").GetComponent<RoundManager>();
    }

    private void FixedUpdate()
    {
        tempPosition.z = Mathf.Sin(Time.realtimeSinceStartup * verticalSpeed) * amplitude;  // Sin각도를 라디안 값으로 반환. +, - 값으로 스무스하게이동
        tempPosition.x = Mathf.Sin(Time.realtimeSinceStartup * horizontalSpeed) * amplitude;
        transform.position = new Vector3(tempPosition.x, tempPosition.y + 20f - enemy2_yDiff, tempPosition.z + 40f);  // 회전 기준 좌표 (x : 0 / y : 0 / z : 20)
        Vector3 gateVector = transform.position;

        if (isSpawnOver == false)
        {
            var go = Instantiate(spawnGate, gateVector, transform.rotation);
            go.name = "Stargate";
            isSpawnOver = true;
            modelFolder.SetActive(true);
        }

        Quaternion targetRot = Quaternion.LookRotation(tempPosition);
        Quaternion frameRot = Quaternion.RotateTowards(transform.rotation, targetRot, 100f * Time.deltaTime);

        transform.rotation = frameRot;
    }

    public void InitOnPos(GameObject obj)
    {
        Instantiate(obj, tempPosition, transform.rotation);
    }

    private void Update()
    {
        //플레이어를 바라봄
        transform.LookAt(player.transform);
        /*
        Quaternion playerDirectionRotation;
        //= Quaternion.LookRotation(cam.transform.forward);   // 쿼터니온의 LookRotation (카메라가 보는 각도) 가져오기
        playerDirectionRotation = Quaternion.RotateTowards(transform.rotation, cam.transform.rotation, amplitude);
        transform.rotation = playerDirectionRotation;*/

        //Quaternion camDirectionRotation = Quaternion.LookRotation(cam.transform.forward);   // 쿼터니온의 LookRotation (카메라가 보는 각도) 가져오기
        //transform.rotation = camDirectionRotation;

        gameOver = manager.isGameOver;  // 게임오버 상태인지 지속적인 갱신 필요
    }

    private IEnumerator EnemyAttack()
    {
        for (; ; )
        {
            if (gameOver == true) // 만약 게임오버라면
            {
                break;  // 적 공격 코루틴 종료
            }

            waitTime = Random.Range(1f, 2f);

            yield return new WaitForSeconds(1f);
            // 트랜스폼, 로테이션
            Vector3 orgPos = transform.position;
            Quaternion camDirectionRotation = Quaternion.LookRotation(cam.transform.forward);
            Instantiate(enemyBullet, orgPos, camDirectionRotation);

            yield return new WaitForSeconds(waitTime);
        }
    }

    public void DivideSpeed()
    {
    }
}