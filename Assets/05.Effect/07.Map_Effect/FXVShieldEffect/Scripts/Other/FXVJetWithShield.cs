using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXVJetWithShield : MonoBehaviour
{
    public FXVShield shield;

    private void Start()
    {
        shield.SetShieldEffectDirection((transform.right + transform.up).normalized);
    }

    private void Update()
    {
    }
}