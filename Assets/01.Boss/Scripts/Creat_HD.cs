using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Creat_HD : Creat_Manager
{
    public GameObject head;
    private Transform SpawnPoint;

    public Animator bossHoleLoop;

    public DragonAttack dragonAttack;

    public AudioSource roar;

    public float distance = 10f;

    public float delay;

    public bool isInvincible = true;

    public void Awake()
    {
        SpawnPoint = GameObject.FindGameObjectWithTag("Gate").GetComponent<Transform>();

        Spawn();
    }

    public void Spawn()
    {
        StartCoroutine(Creat_Head());
    }

    public IEnumerator Creat_Head()
    {
        yield return new WaitForSeconds(2f);

        Vector3 tempPosition = new Vector3(SpawnPoint.position.x, SpawnPoint.position.y, SpawnPoint.position.z + distance);
        GameObject hd_Obj = Instantiate(head, tempPosition, head.transform.rotation);
        roar.Play();

        dragonAttack = GameObject.FindGameObjectWithTag("Head").GetComponent<DragonAttack>();
        dragonAttack.enabled = false;

        hd_Obj.transform.parent = gameObject.transform;

        boss_Lenght.Add(hd_Obj);

        hd_Obj.transform.DOMove(hd_Obj.transform.position + hd_Obj.transform.forward * distance * 2, 1f);

        yield return new WaitForSeconds(2f);

        bossHoleLoop.SetTrigger("Exit");

        yield return new WaitUntil(() => bossHoleLoop.transform.localScale.x <= float.Epsilon);

        hd_Obj.GetComponent<Creat_BD>().Creat_Body();

        var pathTweener = hd_Obj.transform.DOTweenPathFollow();
        hd_Obj.GetComponent<HeadMoveCore>().pathTweener = pathTweener;

        delay = hd_Obj.GetComponent<Creat_BD>().spawnDelay;
        yield return new WaitForSeconds(delay * staticCount);

        dragonAttack.enabled = true;
        isInvincible = false;
    }
}