using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldHealth : MonoBehaviour
{
    public float initialShield = 100f;
    public float currentShield;
    public Slider shieldBar;

    private void Awake()
    {
        currentShield = initialShield;
        shieldBar.maxValue = initialShield;
        shieldBar.value = initialShield;
    }

    public void ShieldGetDamage(float damage)
    {
        currentShield -= damage;
        shieldBar.value = currentShield;
        Debug.Log("현재의 쉴드 잔여 체력 : " + currentShield);
    }

    private void Update()
    {
        if (currentShield <= 0)
        {
            gameObject.GetComponent<FXVShield>().SetShieldActive(false, true);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Bullet")
        {
            //EffectManager.instance.LaunchEffect(col.transform.position, ParticleEffect.DomeGotHit);
            currentShield -= col.gameObject.GetComponent<EnemyBullet>().bulletDamage;
            Destroy(col.gameObject); // 총알 파괴
        }
    }
}