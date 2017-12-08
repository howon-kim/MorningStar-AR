﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXVBullet : MonoBehaviour
{

    public float speed = 10.0f;
    public float lifetime = 2.0f;

	public float bulletHitSize = 1.0f;

    private Vector3 moveDir = Vector3.zero;
    private Ray ray;

    private float currentTime = 0.0f;

    void Start ()
    {

    }

	void Update ()
    {
        Vector3 newPos = transform.position + moveDir * speed * Time.deltaTime;

        ray.origin = transform.position;
        RaycastHit rhi;

        Vector3 offset = newPos - transform.position;

        currentTime += Time.deltaTime;

        transform.position = newPos;

        bool needDestroy = false;

        if (currentTime > lifetime)
        {
            needDestroy = true;
        }

        if (Physics.Raycast(ray, out rhi, offset.magnitude))
        {
            needDestroy = true;

            FXVShield shield = rhi.collider.gameObject.GetComponent<FXVShield>();

            if (shield)
		        shield.OnHit(rhi.point, bulletHitSize);
        }

        if (needDestroy)
        {
            DestroyObject(gameObject);
        }
    }

    public void Shoot(Vector3 dir)
    {
        moveDir = dir;
        currentTime = 0.0f;

        ray = new Ray(transform.position, moveDir);
    }
}
