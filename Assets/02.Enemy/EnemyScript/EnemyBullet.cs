using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{  // 총알이 플레이어 쪽으로 바라보도록 하기
    public float bulletSpeed;
    public int bulletHitSize;
    public int bulletDamage;

    private float currentTime;

    private int score;
    private GameObject player;
    //public RoundManager manager;
    private Camera cam;

    private PlayerHealth hpManager;

    private void Start()
    {
        //bulletHitSize = 5; // 총알 효과 사이즈
        score = 10; // 총알 파괴시 점수
        //bulletSpeed = 5; // 총알 속도
        currentTime = 0.0f;

        //bulletDamage = 15;

        bulletSpeed = Random.Range(5f, 10f);
        //cam = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player");

        hpManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        /*
        // 한 웨이브의 모든 적을 클리어하였거나 게임이 끝나면
        if (manager.isGameOver == true || manager.m_state == RoundManager.State.WAIT)
        {
            Death();    // 모든 총알 오브젝트 파괴
        }
        */
    }

    private void FixedUpdate()
    {
        transform.LookAt(player.GetComponent<Transform>());
        transform.Translate(Vector3.forward * Time.deltaTime * bulletSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        FXVShield shield = collision.collider.gameObject.GetComponent<FXVShield>();
        ShieldHealth shieldHealth = collision.collider.gameObject.GetComponent<ShieldHealth>();
        PlayerHealth playerHealth = collision.collider.gameObject.GetComponent<PlayerHealth>();
        iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactHeavy);


        if (shield)
        {
            var bulletPosition = collision.transform.position;
            var closetPoint = GetComponent<Collider>().ClosestPoint(bulletPosition);
            shield.OnHit(collision.contacts[0].point, bulletHitSize);
            shieldHealth.ShieldGetDamage(bulletDamage);
        }
        else if (playerHealth)
        {
            playerHealth.PlayerGetDamage(bulletDamage);
        }
    }
}