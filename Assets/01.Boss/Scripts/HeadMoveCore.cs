using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMoveCore : MonoBehaviour
{
    public float playDelay = 2f;
    public float lookatDelay = 0.5f;
    public float lookatPathDuration = 3f;
    public float lookatPlayerDuration = 1f;
    public string playerTag = "Player";

    //인선이 변수 공격이 가능한지 안한지 판단
    public bool isDragonAttack = false;

    public TweenerCore<Vector3, Path, PathOptions> pathTweener;

    private Transform target;
    private Tweener lookatTweener = null;
    private Vector3 forward = Vector3.zero;
    private Vector3 pathPoint = Vector3.zero;

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(playDelay);
        Debug.Log("Launch Lookat Follow");
        target = GameObject.FindGameObjectWithTag(playerTag).transform;

        for (; ; )
        {
            yield return StartCoroutine(LookatPlayer());
            yield return StartCoroutine(LookatPath());
        }
    }

    private IEnumerator LookatPath()
    {
        Debug.Log("Lookat Path");

        if (lookatTweener != null)
            lookatTweener.Kill();

        isDragonAttack = false;

        pathPoint = Vector3.zero;
        forward = (pathPoint - transform.position).normalized;

        lookatTweener = transform.DOLookAt(transform.position + forward, lookatDelay);
        lookatTweener.OnUpdate(() =>
        {
            pathPoint = pathTweener.PathGetPoint(0f);
            forward = (pathPoint - transform.position).normalized;
            lookatTweener.ChangeEndValue(transform.position + forward, true);
        });

        yield return new WaitForSeconds(lookatPathDuration);
    }

    private IEnumerator LookatPlayer()
    {
        Debug.Log("Lookat Player");

        isDragonAttack = true;

        if (lookatTweener != null)
            lookatTweener.Kill();

        forward = (target.position - transform.position).normalized;

        lookatTweener = transform.DOLookAt(transform.position + forward, lookatDelay);
        lookatTweener.OnUpdate(() =>
        {
            forward = (target.position - transform.position).normalized;
            lookatTweener.ChangeEndValue(transform.position + forward, true);
        });

        yield return new WaitForSeconds(lookatPlayerDuration);
    }
}