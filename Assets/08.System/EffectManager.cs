// MADE BY HOWON KIM
// IT MAINTAINS THE PARTICLE EFFECT
// MODIFIED ON NOV 28TH, 2017

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ParticleEffect
{
    Appear,
    Hit,
    Death,
    BossHit
};

public class EffectManager : MonoBehaviour
{
    public ParticleEffect sample;

    // 배열, 딕셔너리 DB (Prefab) - 프리팹들을 파일패스에 저장 \/
    /* PARTICLE SYSTEM LIST */
    public List<GameObject> effectList = new List<GameObject>();

    //public static Effects LaunchEffect { get{ return _LaunchEffect(Vector3 hitPoint, ParticleEffect particleEffect); }}
    private static EffectManager _instance;

    public static EffectManager instance
    {
        get
        {
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    /* LAUNCH EFFECT FUNCTION */

    public IEnumerator LaunchEffect(Vector3 hitPoint, ParticleEffect particleEffect)
    {
        var go = Instantiate(effectList[(int)particleEffect]);
        go.transform.position = hitPoint;
        go.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(GetComponent<ParticleSystem>().main.duration);
    }

    /** PREVIOUS PARTICLE AUTO DESTROY METHOD
    IEnumerator PlayAndDestroy(GameObject effect){
        yield return new WaitForSeconds(3);
        Destroy(effect);
    }
    **/
}