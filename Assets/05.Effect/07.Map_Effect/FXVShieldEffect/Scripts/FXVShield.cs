using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class FXVShield : MonoBehaviour
{
    public bool shieldActive = true;
    public float shieldActivationSpeed = 1.0f;
    private float shieldActivationRim = 0.2f;

    public float hitEffectDuration = 0.5f;

    public Light shieldLight;
    public Material hitMaterial;
    public Material activationMaterial;
    public Color hitColor;
    public bool autoHitPatternScale = true;

    private Color lightColor;

    private Material baseMaterial;
    private Material activationMaterialCopy;

    private Collider myCollider;
    private CommandBuffer cmdBuffer;
    private Renderer myRenderer;

    private float shieldActivationTime;
    private float shieldActivationDir;

    private int activationTimeProperty;
    private int shieldDirectionProperty;

    private float hitAttenuationBase = 1.0f;
    //private float rippleScaleBase = 1.0f;

    private void Awake()
    {
        myRenderer = GetComponent<Renderer>();
        activationTimeProperty = Shader.PropertyToID("_ActivationTime");
        shieldDirectionProperty = Shader.PropertyToID("_ShieldDirection");

        FXVShieldPostprocess shieldPostrocess = Camera.main.GetComponent<FXVShieldPostprocess>();
        if (shieldPostrocess)
            shieldPostrocess.AddShieldRenderer(myRenderer);

        shieldActivationDir = 0.0f;

        if (shieldLight)
            lightColor = shieldLight.color;

        myCollider = transform.GetComponent<Collider>();

        if (shieldActive)
        {
            shieldActivationTime = 1.0f;
            myCollider.enabled = true;
        }
        else
        {
            shieldActivationTime = 0.0f;
            myCollider.enabled = false;
        }

        if (shieldLight)
            shieldLight.color = Color.Lerp(Color.black, lightColor, shieldActivationTime);

        myRenderer.material.SetFloat(activationTimeProperty, shieldActivationTime);
        myRenderer.material.SetVector(shieldDirectionProperty, new Vector4(1.0f, 0.0f, 0.0f, 0.0f));

        baseMaterial = myRenderer.material;

        shieldActivationRim = activationMaterial.GetFloat("_ActivationRim");

        activationMaterialCopy = new Material(activationMaterial);

        if (hitMaterial)
        {
            hitAttenuationBase = hitMaterial.GetFloat("_HitAttenuation");
            //rippleScaleBase = hitMaterial.GetFloat("_RippleScale");
        }
    }

    private void OnDestroy()
    {
        if (Camera.main)
        {
            FXVShieldPostprocess shieldPostrocess = Camera.main.GetComponent<FXVShieldPostprocess>();
            if (shieldPostrocess)
                shieldPostrocess.RemoveShieldRenderer(myRenderer);
        }
    }

    private void Update()
    {
        if (shieldActivationDir > 0.0f)
        {
            shieldActivationTime += shieldActivationSpeed * Time.deltaTime;
            if (shieldActivationTime >= 1.0f)
            {
                shieldActivationTime = 1.0f;
                shieldActivationDir = 0.0f;
                myRenderer.material = baseMaterial;
            }

            if (shieldLight)
                shieldLight.color = Color.Lerp(Color.black, lightColor, shieldActivationTime);
        }
        else if (shieldActivationDir < 0.0f)
        {
            shieldActivationTime -= shieldActivationSpeed * Time.deltaTime;
            if (shieldActivationTime <= -shieldActivationRim)
            {
                shieldActivationTime = -shieldActivationRim;
                shieldActivationDir = 0.0f;
                myRenderer.enabled = false;
                myRenderer.material = baseMaterial;
            }

            if (shieldLight)
                shieldLight.color = Color.Lerp(Color.black, lightColor, shieldActivationTime);
        }

        myRenderer.material.SetFloat(activationTimeProperty, shieldActivationTime);
    }

    public bool GetIsShieldActive()
    {
        return (shieldActivationTime == 1.0f) || (shieldActivationDir == 1.0f);
    }

    public void SetShieldActive(bool active, bool animated = true)
    {
        if (animated)
        {
            GetComponent<MeshRenderer>().enabled = true;
            shieldActivationDir = (active) ? 1.0f : -1.0f;
            if (activationMaterialCopy)
            {
                activationMaterialCopy.CopyPropertiesFromMaterial(myRenderer.material);
                activationMaterialCopy.SetFloat("_ActivationRim", shieldActivationRim);
                activationMaterialCopy.SetTexture("_ActivationTex", activationMaterial.GetTexture("_ActivationTex"));
                activationMaterialCopy.shaderKeywords = activationMaterial.shaderKeywords;
                myRenderer.material = activationMaterialCopy;
            }

            if (active)
                myRenderer.enabled = true;
        }
        else
        {
            shieldActivationTime = (active) ? 1.0f : 0.0f;
            shieldActivationDir = 0.0f;
            myRenderer.enabled = active;
        }

        myCollider.enabled = active;
    }

    public void SetShieldEffectDirection(Vector3 dir)
    {
        myRenderer.material.SetVector(shieldDirectionProperty, new Vector4(dir.x, dir.y, dir.z, 0.0f));
    }

    /*public void OnHit(Vector2 hitUV, float hitScale)
    {
        GameObject hitObject = new GameObject("hitFX");
        hitObject.transform.parent = transform;
        hitObject.transform.position = transform.position;
        hitObject.transform.rotation = transform.rotation;
        hitObject.transform.localScale = Vector3.one;

        MeshRenderer mr = hitObject.AddComponent<MeshRenderer>();
        MeshFilter mf = hitObject.AddComponent<MeshFilter>();

        mf.mesh = gameObject.GetComponent<MeshFilter>().mesh;
        mr.material = new Material(hitMaterial);

        mr.material.SetFloat("_HitPosU", hitUV.x);
        mr.material.SetFloat("_HitPosV", hitUV.y);
        mr.material.SetFloat("_HitAttenuation", hitAttenuationBase/hitScale);
        //mr.material.SetFloat("_RippleScale", rippleScaleBase/hitScale);
        if (autoHitPatternScale)
        {
            if (myRenderer.material.HasProperty("_PatternScale"))
                mr.material.SetFloat("_PatternScale", myRenderer.material.GetFloat("_PatternScale"));
            else
                autoHitPatternScale = false;
        }
        mr.material.color = hitColor;

        FXVShieldHit hit = hitObject.AddComponent<FXVShieldHit>();
        hit.StartHitFX(hitEffectDuration);
    }*/

    public void OnHit(Vector3 hitPos, float hitScale)
    {
        GameObject hitObject = new GameObject("hitFX");
        hitObject.transform.parent = transform;
        hitObject.transform.position = transform.position;
        hitObject.transform.localScale = Vector3.one;

        Vector3 hitDiff = transform.position - hitPos;
        hitDiff.Normalize();

        hitObject.transform.rotation = Quaternion.LookRotation(hitDiff) * Quaternion.Euler(-1.75f, -27.0f, -4.5f);

        MeshRenderer mr = hitObject.AddComponent<MeshRenderer>();
        MeshFilter mf = hitObject.AddComponent<MeshFilter>();

        mf.mesh = gameObject.GetComponent<MeshFilter>().mesh;
        mr.material = new Material(hitMaterial);

        mr.material.SetFloat("_HitPosU", 0.5f);// hitUV.x);
        mr.material.SetFloat("_HitPosV", 0.15f);// hitUV.y);
        mr.material.SetFloat("_HitAttenuation", hitAttenuationBase / hitScale);
        //mr.material.SetFloat("_RippleScale", rippleScaleBase/hitScale);

        if (autoHitPatternScale)
        {
            if (myRenderer.material.HasProperty("_PatternScale"))
                mr.material.SetFloat("_PatternScale", myRenderer.material.GetFloat("_PatternScale"));
            else
                autoHitPatternScale = false;
        }
        mr.material.color = hitColor;

        FXVShieldHit hit = hitObject.AddComponent<FXVShieldHit>();
        hit.StartHitFX(hitEffectDuration);
    }

    /*
    public static Vector3 closetPoint = Vector3.zero;

    public void OnDrawGizmos()
    {
        if (closetPoint != Vector3.zero)
        {
            var oldColor = Gizmos.color;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(closetPoint, 0.5f);
            Gizmos.color = oldColor;
        }
    }
    */
}