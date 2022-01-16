using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealisticRainDrop : MonoBehaviour
{
    [Range(1, 4)]
    public int rtQuality = 3;

    [Range(0,1)]
    public float fade = 1;

    [Range(0.2f, 4)]
    public float intensity = 1.75f;

    public bool blur = false;
    [Range(0, 6)]
    public int blurDownSampleNum = 2;
    [Range(0.0f, 20.0f)]
    public float blurSpreadSize = 3.0f;
    [Range(0, 8)]
    public int blurIterations = 3;

    private Material mtrl = null;

    private int srcTexPropId = 0;
    private int paramsPropId = 0;

    private RealisticRainBlur srb = null;

    private void Awake()
    {
        mtrl = new Material(Shader.Find("Hidden/RealisticRainDrop"));

        srcTexPropId = Shader.PropertyToID("_SrcTex");
        paramsPropId = Shader.PropertyToID("_Params");

        srb = new RealisticRainBlur();
    }

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if(mtrl == null || mtrl.shader == null || !mtrl.shader.isSupported)
        {
            enabled = false;
            return;
        }

        mtrl.SetTexture(srcTexPropId, src);
        mtrl.SetVector(paramsPropId, new Vector4(fade, intensity, 0, 0));
        int rtSize = 4 - rtQuality + 1;

        srb.blurDownSampleNum = blurDownSampleNum;
        srb.blurSpreadSize = blurSpreadSize;
        srb.blurIterations = blurIterations;

        RenderTexture srcBlurRT = null;
        RenderTexture rainDropSrcRT = RenderTexture.GetTemporary(src.width / rtSize, src.height / rtSize, 0, src.format);
        RenderTexture rainDropDestRT = RenderTexture.GetTemporary(rainDropSrcRT.width, rainDropSrcRT.height, 0, rainDropSrcRT.format);
        if (blur) srcBlurRT = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);
        if (blur) srb.OnRenderImage(src, srcBlurRT);
        Graphics.Blit(src, rainDropSrcRT);
        Graphics.Blit(rainDropSrcRT, rainDropDestRT, mtrl, 0);
        if (blur) mtrl.SetTexture(srcTexPropId, srcBlurRT);
        Graphics.Blit(rainDropDestRT, dest, mtrl, 1);
        RenderTexture.ReleaseTemporary(rainDropSrcRT);
        RenderTexture.ReleaseTemporary(rainDropDestRT);
        if (blur) RenderTexture.ReleaseTemporary(srcBlurRT);
    }

    private void OnDestroy()
    {
        if(mtrl != null)
        {
            DestroyImmediate(mtrl);
            mtrl = null;
        }

        if(srb != null)
        {
            srb.Destroy();
            srb = null;
        }
    }
}
