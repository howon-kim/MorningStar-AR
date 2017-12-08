using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAiController : MonoBehaviour {
    
    public GameObject enemyBullet;  // 적 총알
    public GameObject spawnPoint;   // 적 소환장소인데 아직 필요없을거같음
    public Transform enemyTransform;
    public Rigidbody enemyRgbody;

    float posLimitY = 15f;
    float spawnLimitPos = 10f;
    float speed = 0.1f;

    float changeTime, duringMove;

    void Start ()
    {
        enemyTransform = gameObject.GetComponent<Transform>();
        enemyRgbody = gameObject.GetComponent<Rigidbody>();
        spawnPoint = GameObject.FindWithTag("SpawnPoint");

        StartCoroutine(EnemyRandomMove());
	}
	
	void Update ()
    {
        if (enemyTransform.position.x > spawnLimitPos)
        {
            enemyTransform.Translate(new Vector3(-1f, 0f, 0f));
        }
        else if (enemyTransform.position.x < -spawnLimitPos)
        {
            enemyTransform.Translate(new Vector3(1f, 0f, 0f));
        }

        if (enemyTransform.position.z > spawnLimitPos)
        {
            enemyTransform.Translate(new Vector3(0f, 0f, 1f));
        }
        else if (enemyTransform.position.z < -spawnLimitPos)
        {
            enemyTransform.Translate(new Vector3(0f, 0f, -1f));
        }
	}

    IEnumerator EnemyRandomMove()
    {
        for (; ; )
        {
            // 수시로 바뀌는 운동량 변화시간
            changeTime = 0.5f;
            duringMove = Random.Range(1f, 1.5f);

            float xRandom = Random.Range(-1f, 1f);
            float yRandom = Random.Range(0.01f, 0.1f);
            float zRandom = Random.Range(-1f, 1f);

            for (float i = 0f; i < duringMove; i = i + 0.1f)
            {
                if (enemyTransform.position.y > posLimitY) // 일정량만큼 y 값 아래로 떨어지기 시작
                {
                    enemyTransform.Translate(xRandom, yRandom, zRandom);
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                else if (enemyTransform.position.y <= posLimitY) // y 값으로 너무 떨어졌다 싶으면 더 이상 y값으로 낙하 안 함
                {
                    enemyTransform.Translate(xRandom, 0f, zRandom);
                    yield return new WaitForSeconds(Time.deltaTime);
                }
            }
            yield return new WaitForSeconds(changeTime);
        }
    }
}
