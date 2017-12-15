using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int totalHealth = 100;
    public int currentHealth;
    public int score = 300;

    public int divideCount = 1;

    // public ParticleSystem deathEffect;
    public GameObject enemy2;

    public RoundManager manager;
    private EnemyMovement movement;

    private bool isDead;

    private void Awake()
    {
        currentHealth = totalHealth;
        //StartCoroutine(EffectManager.instance.LaunchEffect(this.transform.position, ParticleEffect.Appear));
        manager = GameObject.FindGameObjectWithTag("SpawnPoint").GetComponent<RoundManager>();
        // EnemyMovement = GetComponent<movement>();
    }

    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        /** 게임 오브젝트 파괴되기 때문에 이 스크립트 비활성화 **
        if(isDead)
            return;
        **/
        Debug.Log("Enemy : I am hurt");
        if (currentHealth > 0)
        {   // 적의 체력이 0 보다 높을 때
            currentHealth -= amount;
            StartCoroutine(EffectManager.instance.LaunchEffect(this.transform.position, ParticleEffect.Hit));
        }
        if (currentHealth <= 0)
        { // 적의 체력이 0 이하일 때
            currentHealth = 0;
            ScoreManager.instance.AddKillScore(score);
            Death();
        }
    }

    private void Death()
    {
        /* Enemy 2 Clone Disabled */
        isDead = true;
        /* Enemy 2 Clone Disabled */

        if (isDead == false)
        {
            isDead = true;
            StartCoroutine(EffectManager.instance.LaunchEffect(this.transform.position, ParticleEffect.Death));


            // First Clone
            var firstClone = Instantiate(enemy2, this.transform.position, Quaternion.identity);
            firstClone.GetComponent<EnemyHealth>().isDead = true;

            // Second Clone
            var secondClone = Instantiate(enemy2, this.transform.position, Quaternion.identity);
            secondClone.GetComponent<EnemyHealth>().isDead = true;

            // Destroy Mother Object
            Destroy(gameObject);

            // SCALE *2 DOWN
            // SCORE *2 DOWN
            // HEATLH *2 DOWN

        }
        else
        {
            StartCoroutine(EffectManager.instance.LaunchEffect(this.transform.position, ParticleEffect.Death));
            Destroy(gameObject); // Applying coroutine just for giving the time to play particle EffectManager.
        }
    }
}