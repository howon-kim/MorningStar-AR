using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayEnabled : MonoBehaviour
{
    public float delay = 1f;
    public float scaleDuration = 0.5f;
    public MeshRenderer target;
    public AnimationCurve curve;

    public IEnumerator Start()
    {
        var originalScale = transform.localScale;
        var currentScale = Vector3.zero;
        transform.localScale = Vector3.zero;

        target.enabled = false;

        yield return new WaitForSeconds(delay);

        target.enabled = true;

        var beginTime = Time.time;
        var currentTime = 0f;

        for (; ; )
        {
            currentTime = (Time.time - beginTime) / scaleDuration;
            currentScale = Vector3.Lerp(Vector3.zero, originalScale, curve.Evaluate(currentTime));
            transform.localScale = currentScale;
            if ((currentScale - originalScale).magnitude <= 0f)
                break;

            yield return null;
        }

        transform.localScale = originalScale;
    }
}