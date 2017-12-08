using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAttack : MonoBehaviour
{
    public GameObject dragonBullet, dragonBulletfury;

    // 체력 따로 만들어놓고 체력 반값되면 광폭화 패턴도 만들기
    public bool isFury;

    public bool gameOver;

    public Creat_Manager createManager;
    private Camera cam;

    public int furyCount;
    private RoundManager manager;
    private float waitTime;
    private GameObject[] g, h;
    private HeadMoveCore moveCore;
    private bool attack;

    private void Start()
    {
        cam = Camera.main;
        StartCoroutine(Attack());
        isFury = false;

        furyCount = Creat_Manager.staticCount;
        moveCore = GetComponent<HeadMoveCore>();
        manager = GameObject.FindGameObjectWithTag("SpawnPoint").GetComponent<RoundManager>();
    }

    private void Update()
    {
        g = GameObject.FindGameObjectsWithTag("Enemy");
        h = GameObject.FindGameObjectsWithTag("Head");

        int enemyCount = g.Length + h.Length;

        if (enemyCount < (furyCount / 2) && isFury == false)
        {
            isFury = true;
            StartCoroutine(DragonAttackFury()); // 광폭화
        }

        attack = moveCore.isDragonAttack;
        gameOver = manager.isGameOver;
    }

    // 통상 공격
    private IEnumerator Attack()
    {
        for (; ; )
        {
            if (gameOver)
            {
                yield break;
            }
            Vector3 orgPos = transform.position;
            Quaternion camDirectionRotation = Quaternion.LookRotation(cam.transform.forward);
            waitTime = Random.Range(0.5f, 2f);

            if (attack)
            {
                Instantiate(dragonBullet, orgPos, camDirectionRotation);
            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    // 광폭화 패턴
    private IEnumerator DragonAttackFury()
    {
        for (; ; )
        {
            if (gameOver)
            {
                yield break;
            }
            waitTime = 10f;
            for (int i = 0; i <= 5; i++)
            {
                if (attack)
                {
                    Vector3 orgPos = transform.position;
                    Quaternion camDirectionRotation = Quaternion.LookRotation(cam.transform.forward);
                    Instantiate(dragonBulletfury, orgPos, camDirectionRotation);
                }
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(waitTime);
        }
    }
}