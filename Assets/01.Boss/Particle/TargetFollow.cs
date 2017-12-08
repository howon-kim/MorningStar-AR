using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TargetFollow : MonoBehaviour
{
    private Transform target;

    public void FollowTarget(Transform target)
    {
        /*
        var tweener = transform.DOMove(target.position, 0f);
        tweener.OnUpdate(() =>
        {
            tweener.ChangeEndValue(target.position, true);
        });
        tweener.Play();
        */
        this.target = target;
    }

    public void Update()
    {
        if (target == null)
        {
            Destroy(this);
            return;
        }

        transform.position = target.position;
    }
}