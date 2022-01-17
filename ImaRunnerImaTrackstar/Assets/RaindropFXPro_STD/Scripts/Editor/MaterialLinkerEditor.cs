using System.Collections;
using UnityEngine;
using UnityEditor;
using RaindropFX;

[CustomEditor(typeof(MaterialLinker))]
public class MaterialLinkerEditor : Editor {
    bool mat_foldout = true;
    bool base_foldout = true;
    bool phys_foldout = true;
    bool drop_foldout = true;
    bool lut_foldout = true;
    bool wipe_foldout = true;

    private void Prop(string name) {
        SerializedProperty prop = serializedObject.FindProperty(name);
        EditorGUILayout.PropertyField(prop);
    }

    public override void OnInspectorGUI() {
        MaterialLinker mainComp = (MaterialLinker)target;
        serializedObject.Update();
        //base.OnInspectorGUI();

        mat_foldout = EditorGUILayout.Foldout(mat_foldout, "[Material Settings]", true);
        if (mat_foldout) {
            Prop("targetMat");
            Prop("normalTex_PropName");
            Prop("fogMask_PropName");
            Prop("wipeMask_PropName");
        } EditorGUILayout.Space();

        base_foldout = EditorGUILayout.Foldout(base_foldout, "[Basic Settings]", true);
        if (base_foldout) {
            Prop("fadeout_fadein_switch");
            Prop("fastMode");
            Prop("calcRainTextureSize");
            Prop("_raindropTex_alpha");
            Prop("fadeSpeed");
            Prop("refreshRate");
            Prop("maxStaticRaindropNumber");
            Prop("maxDynamicRaindropNumber");
        } EditorGUILayout.Space();

        phys_foldout = EditorGUILayout.Foldout(phys_foldout, "[Physics Settings]", true);
        if (phys_foldout) {
            Prop("calcTimeStep");
            Prop("useWind");
            Prop("radialWind");
            Prop("windTurbulence");
            Prop("windTurbScale");
            Prop("wind");
            Prop("gravity");
            Prop("friction");
        } EditorGUILayout.Space();

        drop_foldout = EditorGUILayout.Foldout(drop_foldout, "[Droplet Settings]", true);
        if (drop_foldout) {
            Prop("generateTrail");
            Prop("raindropSizeRange");
            //Prop("tintColor");
            Prop("distortion");
            Prop("edgeSoftness");
            Prop("_inBlack");
            Prop("_inWhite");
            Prop("_outWhite");
            Prop("_outBlack");
        } EditorGUILayout.Space();

        lut_foldout = EditorGUILayout.Foldout(lut_foldout, "[Force LUT Settings]", true);
        if (lut_foldout) {
            Prop("useForceLUT");
            Prop("forceLUT");
        } EditorGUILayout.Space();

        wipe_foldout = EditorGUILayout.Foldout(wipe_foldout, "[Wipe Settings]", true);
        if (wipe_foldout) {
            Prop("wipeEffect");
            Prop("wipeDelta");
            Prop("foggingSpeed");
        } EditorGUILayout.Space();
        
        serializedObject.ApplyModifiedProperties();
    }

}
