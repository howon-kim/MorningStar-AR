using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BulletHealth : MonoBehaviour
{
    public int initialHealth = 20;
    public int currentHealth;
    public int score = 0;

    public RoundManager manager;

    //public GameObject deathEffectGo;

    private bool isDead;
    public float bulletLifeTime = 25.0f; // 총알 사라지는 시간
    private float currentTime;


    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("SpawnPoint").GetComponent<RoundManager>();
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= bulletLifeTime)
            Death();
        
        // 한 웨이브의 모든 적을 클리어하였거나 게임이 끝나면
        if (manager.isGameOver == true || manager.m_state == RoundManager.State.WAIT)
        {
            Death();    // 모든 총알 오브젝트 파괴
        }
    }

    private void Awake()
    {
        currentHealth = initialHealth;
        Debug.Log("Hello");
    }

    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        if (currentHealth > 0)
        {   // 적의 체력이 0 보다 높을 때
            currentHealth -= amount;
            // StartCoroutine(EffectManager.instance.LaunchEffect(this.transform.position, ParticleEffect.Hit));
        }
        if (currentHealth <= 0)
        { // 적의 체력이 0 이하일 때
            ScoreManager.instance.AddTechnicalScore(score);
            Debug.Log("총알 파괴");
            Death();
        }   
    }

    private void Death()
    {
        isDead = true;
        StartCoroutine(EffectManager.instance.LaunchEffect(gameObject.transform.position, ParticleEffect.Death));
        Destroy(gameObject);
    }
}