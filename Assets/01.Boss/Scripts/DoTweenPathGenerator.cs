using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DoTweenPathGenerator
{
    public static TweenerCore<Vector3, Path, PathOptions> DOTweenPathFollow(this Transform transform)
    {
        var duration = 25f;
        var count = 20;
        var nearChance = 40f;

        var spawnBounds = GameObject.FindGameObjectWithTag("Spawn Bounds").GetComponent<Collider>();
        var target = GameObject.FindGameObjectWithTag("Player").transform;

        return DOTweenPathFollow(transform, spawnBounds, count, duration, nearChance, target);
    }

    private static TweenerCore<Vector3, Path, PathOptions> DOTweenPathFollow(Transform transform, Collider spawnBounds, int count, float duration, float nearChance, Transform target)
    {
        var pathList = new List<Vector3>();
        pathList.Add(transform.position);

        for (int i = 0; i < count; ++i)
            pathList.Add(GetRandomPointInCollider(spawnBounds, nearChance));

        var pathTweener = transform.DOPath(pathList.ToArray(), duration, PathType.CatmullRom, PathMode.Full3D, 10, Color.blue)
            .SetOptions(true)
            .SetLoops(-1, LoopType.Restart)
            //.SetEase(Ease.InOutCubic)
            .SetEase(Ease.Linear);
        //.SetLookAt(0f, null, Vector3.up);

        return pathTweener;
    }

    private static Vector3 GetRandomPointInCollider(Collider spawnBounds, float nearChance)
    {
        var limit = 2000f;
        var pos = Vector3.zero;

        if (Random.Range(0f, 100f) <= nearChance)
            pos = spawnBounds.transform.position + new Vector3(Random.Range(-limit, limit), Random.Range(-limit, limit), Random.Range(-limit, limit));
        else
            pos = spawnBounds.transform.position + new Vector3(Random.Range(-limit, limit), Random.Range(-limit, limit), Random.Range(0, limit));

        var closestPoint = spawnBounds.ClosestPoint(pos);
        return closestPoint;
    }
}