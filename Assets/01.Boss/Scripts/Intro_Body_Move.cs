using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro_Body_Move : MoveBase
{
    public Transform follow_Target;

    public float followSpeed = 50f;
    public float distance = 5f;

    public void Init(Transform target, float followSpeed, float distance)
    {
        this.follow_Target = target;
        this.followSpeed = followSpeed;
        this.distance = distance;
    }

    public void Awake()
    {
        Launch();
    }

    public void Launch()
    {
        if (follow_Target == null)
            return;

        StartCoroutine(Process());
    }

    private IEnumerator Process()
    {
        for (; ; )
        {
            if (follow_Target == null)
                yield break;
            transform.position = Vector3.Lerp(transform.position, follow_Target.position + follow_Target.forward * -distance, Time.deltaTime * followSpeed);
            transform.LookAt(follow_Target);
            yield return null;
        }
    }

    public override void ChangeFollowTarget(Transform newTarget)
    {
        follow_Target = newTarget;
        Launch();
    }
}