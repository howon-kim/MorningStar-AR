﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class FXVShieldHitMaterialEditor : ShaderGUI 
{
    public enum ACTIVATION_TYPE_OPTIONS
    {
        FXV_TEXTURE = 0,
        FXV_UV = 1
    }

    bool firstTimeApply = true;

    MaterialEditor materialEditor;

    MaterialProperty _ColorProperty = null;

    MaterialProperty _RippleTexProperty = null;
    MaterialProperty _RippleScaleProperty = null;
    MaterialProperty _RippleDistortionProperty = null;

    MaterialProperty _PatternTexProperty = null;
    MaterialProperty _PatternScaleProperty = null;

    MaterialProperty _HitAttenuationProperty = null;
    MaterialProperty _HitPowerProperty = null;

    public void FindProperties(MaterialProperty[] props)
    {
        _ColorProperty = FindProperty("_Color", props);

        _RippleTexProperty = FindProperty("_RippleTex", props);
        _RippleScaleProperty = FindProperty("_RippleScale", props);
        _RippleDistortionProperty = FindProperty("_RippleDistortion", props);

        _PatternTexProperty = FindProperty("_PatternTex", props);
        _PatternScaleProperty = FindProperty("_PatternScale", props);

        _HitAttenuationProperty = FindProperty("_HitAttenuation", props);
        _HitPowerProperty = FindProperty("_HitPower", props);
    }

    public override void OnGUI(MaterialEditor editor, MaterialProperty[] props)
    {
        FindProperties(props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly

        materialEditor = editor;
        Material material = materialEditor.target as Material;

        // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching some existing
        // material to a standard shader.
        // Do this before any GUI code has been issued to prevent layout issues in subsequent GUILayout statements (case 780071)
        if (firstTimeApply)
        {
            //MaterialChanged(material, m_WorkflowMode);
            firstTimeApply = false;
        }

        ShaderPropertiesGUI(material);
    }


    public void ShaderPropertiesGUI(Material targetMat)
    {
        string[] keyWords = targetMat.shaderKeywords;

        bool usePatternTexture = keyWords.Contains("USE_PATTERN_TEXTURE");
        bool useDistortionForPatternTexture = keyWords.Contains("USE_DISTORTION_FOR_PATTERN_TEXTURE");

        EditorGUI.BeginChangeCheck();

        GUILayout.Label("Color", EditorStyles.boldLabel);

        materialEditor.ColorProperty(_ColorProperty, "Color");
        materialEditor.ShaderProperty(_HitAttenuationProperty, "Hit Attenuation");
        materialEditor.ShaderProperty(_HitPowerProperty, "Hit Power");

        GUILayout.Label("Pattern Texture", EditorStyles.boldLabel);

        usePatternTexture = EditorGUILayout.Toggle("Enabled", usePatternTexture);

        if (usePatternTexture)
        {
            materialEditor.TexturePropertySingleLine(new GUIContent("Texture", ""), _PatternTexProperty);
            materialEditor.ShaderProperty(_PatternScaleProperty, "Pattern Scale");

            GUILayout.Label("Distortion Ripple Texture", EditorStyles.boldLabel);

            useDistortionForPatternTexture = EditorGUILayout.Toggle("Enabled", useDistortionForPatternTexture);

            if (useDistortionForPatternTexture)
            {
                materialEditor.TexturePropertySingleLine(new GUIContent("Texture", ""), _RippleTexProperty);
                materialEditor.ShaderProperty(_RippleScaleProperty, "Ripple Scale");
                materialEditor.ShaderProperty(_RippleDistortionProperty, "Ripple Distortion");
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            List<string> keywords = new List<string>();

            if (usePatternTexture)
                keywords.Add("USE_PATTERN_TEXTURE");

            if (useDistortionForPatternTexture)
                keywords.Add("USE_DISTORTION_FOR_PATTERN_TEXTURE");

            targetMat.shaderKeywords = keywords.ToArray();
            EditorUtility.SetDirty(targetMat);
        }
    }
}
