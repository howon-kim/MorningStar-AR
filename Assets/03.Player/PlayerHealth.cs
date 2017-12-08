using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float initialHealth = 100f;
    public float currentHealth;

    public ParticleSystem playerHitParticle;
    public Slider playerBar;

    public RoundManager manager;

    private void Awake()
    {
        currentHealth = initialHealth;
        playerBar.maxValue = initialHealth;
        playerBar.value = initialHealth;

        manager = GameObject.Find("RoundManager").GetComponent<RoundManager>();
    }

    private void Update()
    {
        if (manager.isGameOver == false && currentHealth <= 0)
        {
            // 게임오버 프로세스 진행
            manager.isGameOver = true;
            manager.Defeat();
        }
    }

    public void PlayerGetDamage(float damage)
    {
        currentHealth -= damage;
        playerBar.value = currentHealth;
        Debug.Log("현재의 플레이어 잔여 체력 : " + currentHealth);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Bullet")
        {
            EffectManager.instance.LaunchEffect(col.transform.position, ParticleEffect.Hit);
            playerHitParticle.Play();
            Destroy(col.gameObject);
        }
    }
}