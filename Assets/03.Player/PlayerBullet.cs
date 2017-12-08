using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damage = 10;
    public int bulletScore = 20;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var eh = collision.gameObject.GetComponent<EnemyHealth>();
            if (eh != null)
                eh.TakeDamage(damage, collision.contacts[0].point);

            var bossHitMgr = collision.gameObject.GetComponent<HitManager>();
            if (bossHitMgr != null)
                bossHitMgr.CollisionProcess(damage);
        }

        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            var bulletHealth = collision.gameObject.GetComponent<BulletHealth>();
            if (bulletHealth != null)
            {
                ScoreManager.instance.AddTechnicalScore(bulletScore);
                bulletHealth.TakeDamage(damage, collision.contacts[0].point);
            }
        }

        if (collision.gameObject.CompareTag("Head"))
        {
            var bossHeadHitMgr = collision.gameObject.GetComponent<HitManager>();
            if (bossHeadHitMgr != null)
                bossHeadHitMgr.CollisionProcess(damage);
        }
    }
}