using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class FXVShieldMaterialEditor : ShaderGUI 
{
    public enum ACTIVATION_TYPE_OPTIONS
    {
        FXV_TEXTURE = 0,
        FXV_TEXTURE_UV = 1,
        FXV_UV = 2,
        FXV_CUSTOM_TEX = 3
    }

    bool firstTimeApply = true;

    MaterialEditor materialEditor;

    MaterialProperty _ColorProperty = null;
    MaterialProperty _ColorRimMinProperty = null;
    MaterialProperty _ColorRimMaxProperty = null;

    MaterialProperty _MainTexProperty = null;
    MaterialProperty _TextureColorProperty = null;
    MaterialProperty _TextureScaleProperty = null;
    MaterialProperty _TexturePowerProperty = null;
    MaterialProperty _TextureRimMinProperty = null;
    MaterialProperty _TextureRimMaxProperty = null;
    MaterialProperty _TextureScrollXProperty = null;
    MaterialProperty _TextureScrollYProperty = null;

    MaterialProperty _DistortTexProperty = null;
    MaterialProperty _DistortionFactorProperty = null;

    MaterialProperty _PatternTexProperty = null;
    MaterialProperty _PatternColorProperty = null;
    MaterialProperty _PatternScaleProperty = null;
    MaterialProperty _PatternPowerProperty = null;
    MaterialProperty _PatternRimMinProperty = null;
    MaterialProperty _PatternRimMaxProperty = null;

    MaterialProperty _OverlapRimProperty = null;

    MaterialProperty _DirectionVisibilityProperty = null;

    MaterialProperty _ActivationTexProperty = null;
    MaterialProperty _ActivationRimProperty = null;

    public void FindProperties(MaterialProperty[] props)
    {
        _ColorProperty = FindProperty("_Color", props);
        _ColorRimMinProperty = FindProperty("_ColorRimMin", props);
        _ColorRimMaxProperty = FindProperty("_ColorRimMax", props);

        _MainTexProperty = FindProperty("_MainTex", props);
        _TextureColorProperty = FindProperty("_TextureColor", props);
        _TextureScaleProperty = FindProperty("_TextureScale", props);
        _TexturePowerProperty = FindProperty("_TexturePower", props);
        _TextureRimMinProperty = FindProperty("_TextureRimMin", props);
        _TextureRimMaxProperty = FindProperty("_TextureRimMax", props);
        _TextureScrollXProperty = FindProperty("_TextureScrollX", props);
        _TextureScrollYProperty = FindProperty("_TextureScrollY", props);

        _DistortTexProperty = FindProperty("_DistortTex", props);
        _DistortionFactorProperty = FindProperty("_DistortionFactor", props);

        _PatternTexProperty = FindProperty("_PatternTex", props);
        _PatternColorProperty = FindProperty("_PatternColor", props);
        _PatternScaleProperty = FindProperty("_PatternScale", props);
        _PatternPowerProperty = FindProperty("_PatternPower", props);
        _PatternRimMinProperty = FindProperty("_PatternRimMin", props);
        _PatternRimMaxProperty = FindProperty("_PatternRimMax", props);

        _OverlapRimProperty = FindProperty("_OverlapRim", props);

        _DirectionVisibilityProperty = FindProperty("_DirectionVisibility", props);

        _ActivationTexProperty = FindProperty("_ActivationTex", props);
        _ActivationRimProperty = FindProperty("_ActivationRim", props);
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
        bool useMainTexture = keyWords.Contains("USE_MAIN_TEXTURE");
        bool useDistortionForMainTexture = keyWords.Contains("USE_DISTORTION_FOR_MAIN_TEXTURE");
        //bool useDepthOverlapRim = keyWords.Contains("USE_DEPTH_OVERLAP_RIM");
        bool useColorRim = keyWords.Contains("USE_COLOR_RIM");
        bool useDirectionVisibility = keyWords.Contains("USE_DIRECTION_VISIBILITY");

        bool activationEnabled = keyWords.Contains("ACTIVATION_EFFECT_ON");

        ACTIVATION_TYPE_OPTIONS activationType = ACTIVATION_TYPE_OPTIONS.FXV_TEXTURE;

        if (keyWords.Contains("ACTIVATION_TYPE_TEXTURE"))
            activationType = ACTIVATION_TYPE_OPTIONS.FXV_TEXTURE;
        if (keyWords.Contains("ACTIVATION_TYPE_TEX_UV"))
            activationType = ACTIVATION_TYPE_OPTIONS.FXV_TEXTURE_UV;
        if (keyWords.Contains("ACTIVATION_TYPE_UV"))
            activationType = ACTIVATION_TYPE_OPTIONS.FXV_UV;
        if (keyWords.Contains("ACTIVATION_TYPE_CUSTOM_TEX"))
            activationType = ACTIVATION_TYPE_OPTIONS.FXV_CUSTOM_TEX;

        EditorGUI.BeginChangeCheck();

        GUILayout.Label("Color Rim", EditorStyles.boldLabel);

        useColorRim = EditorGUILayout.Toggle("Enabled", useColorRim);

        if (useColorRim)
        {
            materialEditor.ColorProperty(_ColorProperty, "Color");
            materialEditor.ShaderProperty(_ColorRimMinProperty, "Color Rim Min");
            materialEditor.ShaderProperty(_ColorRimMaxProperty, "Color Rim Max");
        }

        GUILayout.Label("Texture Rim", EditorStyles.boldLabel);

        useMainTexture = EditorGUILayout.Toggle("Enabled", useMainTexture);

        if (useMainTexture)
        {
            materialEditor.TexturePropertySingleLine(new GUIContent("Texture", ""), _MainTexProperty);
            materialEditor.ColorProperty(_TextureColorProperty, "Color");
            materialEditor.ShaderProperty(_TextureScaleProperty, "Texture Scale");
            materialEditor.ShaderProperty(_TexturePowerProperty, "Texture Power");
            materialEditor.ShaderProperty(_TextureRimMinProperty, "Texture Rim Min");
            materialEditor.ShaderProperty(_TextureRimMaxProperty, "Texture Rim Max");
            materialEditor.ShaderProperty(_TextureScrollXProperty, "Texture Scroll Speed X");
            materialEditor.ShaderProperty(_TextureScrollYProperty, "Texture Scroll Speed Y");


            GUILayout.Label("Distortion Texture", EditorStyles.boldLabel);

            useDistortionForMainTexture = EditorGUILayout.Toggle("Enabled", useDistortionForMainTexture);

            if (useDistortionForMainTexture)
            {
                materialEditor.TexturePropertySingleLine(new GUIContent("Texture", ""), _DistortTexProperty);
                materialEditor.ShaderProperty(_DistortionFactorProperty, "Distortion Factor");
            }
        }

        GUILayout.Label("Pattern Texture", EditorStyles.boldLabel);

        usePatternTexture = EditorGUILayout.Toggle("Enabled", usePatternTexture);

        if (usePatternTexture)
        {
            materialEditor.TexturePropertySingleLine(new GUIContent("Texture", ""), _PatternTexProperty);
            materialEditor.ColorProperty(_PatternColorProperty, "Color");
            materialEditor.ShaderProperty(_PatternScaleProperty, "Pattern Scale");
            materialEditor.ShaderProperty(_PatternPowerProperty, "Pattern Power");
            materialEditor.ShaderProperty(_PatternRimMinProperty, "Pattern Rim Min");
            materialEditor.ShaderProperty(_PatternRimMaxProperty, "Pattern Rim Max");
            //materialEditor.ShaderProperty(_TextureScrollXProperty, "Texture Scroll Speed X");
            // materialEditor.ShaderProperty(_TextureScrollYProperty, "Texture Scroll Speed Y");
        }

        GUILayout.Label("Depth Test Overlap Rim", EditorStyles.boldLabel);

       // useDepthOverlapRim = EditorGUILayout.Toggle("Enabled", useDepthOverlapRim);

       // if (useDepthOverlapRim)
       // {
            materialEditor.ShaderProperty(_OverlapRimProperty, "Rim Size");
       // }

        GUILayout.Label("Direction Based Visibility", EditorStyles.boldLabel);

        useDirectionVisibility = EditorGUILayout.Toggle("Enabled", useDirectionVisibility);

        if (useDirectionVisibility)
        {
            materialEditor.ShaderProperty(_DirectionVisibilityProperty, "Visibility Factor");
        }

        GUILayout.Label("Activation Effect", EditorStyles.boldLabel);

        activationEnabled = EditorGUILayout.Toggle("Activation Effect Enabled", activationEnabled);

        if (activationEnabled)
        {
            activationType = (ACTIVATION_TYPE_OPTIONS)EditorGUILayout.EnumPopup("Activation Type:", activationType);

            materialEditor.ShaderProperty(_ActivationRimProperty, "Activation Rim Size");
            if (activationType == ACTIVATION_TYPE_OPTIONS.FXV_CUSTOM_TEX)
                materialEditor.TexturePropertySingleLine(new GUIContent("Texture", ""), _ActivationTexProperty);
        }

        if (EditorGUI.EndChangeCheck())
        {
            List<string> keywords = new List<string> { activationEnabled ? "ACTIVATION_EFFECT_ON" : "ACTIVATION_EFFECT_OFF" };

            if (usePatternTexture)
                keywords.Add("USE_PATTERN_TEXTURE");

            if (useMainTexture)
                keywords.Add("USE_MAIN_TEXTURE");

            if (useDistortionForMainTexture)
                keywords.Add("USE_DISTORTION_FOR_MAIN_TEXTURE");

           // if (useDepthOverlapRim)
           //     keywords.Add("USE_DEPTH_OVERLAP_RIM");

            if (useColorRim)
                keywords.Add("USE_COLOR_RIM");

            if (useDirectionVisibility)
                keywords.Add("USE_DIRECTION_VISIBILITY");

            if (activationEnabled)
            {
                if (activationType == ACTIVATION_TYPE_OPTIONS.FXV_TEXTURE)
                    keywords.Add("ACTIVATION_TYPE_TEXTURE");

                if (activationType == ACTIVATION_TYPE_OPTIONS.FXV_UV)
                    keywords.Add("ACTIVATION_TYPE_UV");

                if (activationType == ACTIVATION_TYPE_OPTIONS.FXV_TEXTURE_UV)
                    keywords.Add("ACTIVATION_TYPE_TEX_UV");

                if (activationType == ACTIVATION_TYPE_OPTIONS.FXV_CUSTOM_TEX)
                    keywords.Add("ACTIVATION_TYPE_CUSTOM_TEX");
            }

            targetMat.shaderKeywords = keywords.ToArray();
            EditorUtility.SetDirty(targetMat);
        }
    }
}
