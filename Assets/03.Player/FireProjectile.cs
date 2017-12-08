using UnityEngine;
using System.Collections;

public class FireProjectile : MonoBehaviour
{
    private RaycastHit hit;
    public GameObject[] projectiles;
    public Transform muzzle;

    [HideInInspector]
    public int currentProjectile = 0; // Type of gun

    public float speed; // Gun speed
    private int shootableMask; // Masking enemy
    public int gunDamage; // Gun Damage

    // ** HTC VIVE SETTING ** //
    public GameObject steamController;

    private SteamVR_TrackedController controller;
    private SteamVR_Controller.Device device;
    private SteamVR_TrackedObject trackedObj;

    // ** BULLET SETTING ** //
    public float bulletInterval;

    private float currentInterval;
    private float bulletDestroy;

    private void Awake()
    {
        shootableMask = LayerMask.GetMask("Shootable");
        speed = 4000;
        gunDamage = 25;
        currentInterval = 0f;
        bulletDestroy = 10f;

        controller = GetComponent<SteamVR_TrackedController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<SteamVR_TrackedController>();
        }

        trackedObj = GetComponent<SteamVR_TrackedObject>();

        // For Click Purpose
        //controller.TriggerClicked += new ClickedEventHandler(Fire);
    }

    private void Update()
    {
        currentInterval -= Time.deltaTime; // Bullet Interval

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            nextEffect();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            nextEffect();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            previousEffect();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            previousEffect();
        }

        if ((Input.GetKey(KeyCode.Mouse0) || controller.triggerPressed) && currentInterval <= 0)
        {
            currentInterval = bulletInterval;
            Shoot();
        }
        // ** 레이 업데이트 ** //
        //Debug.DrawRay(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Camera.main.ScreenPointToRay(Input.mousePosition).direction * 100, Color.yellow);
    }

    public void Shoot()
    {
        GameObject projectile = Instantiate(projectiles[currentProjectile], muzzle.transform.position, muzzle.transform.rotation) as GameObject;
        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * speed * 5f);
        projectile.GetComponent<ProjectileScript>().impactNormal = hit.normal;

        if (steamController != null &&
            steamController.activeInHierarchy)
            SteamVR_Controller.Input((int)trackedObj.index).TriggerHapticPulse(3500);
        /*
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, shootableMask))
        {
            GameObject hitobj = hit.collider.gameObject;

            // ** EnemyHealth Script ** //
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            EnemyBullet enemyBullet = hit.collider.GetComponent<EnemyBullet>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(gunDamage, hit.point);
            }

            if (enemyBullet != null)
            {
                enemyBullet.TakeDamage();
            }
        }*/
        // yield return new WaitForSeconds(bulletDestroy);
        Destroy(projectile, bulletDestroy);
    }

    public void nextEffect()
    {
        if (currentProjectile < projectiles.Length - 1)
            currentProjectile++;
        else
            currentProjectile = 0;
    }

    public void previousEffect()
    {
        if (currentProjectile > 0)
            currentProjectile--;
        else
            currentProjectile = projectiles.Length - 1;
    }

    public void AdjustSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}