using UnityEngine;
using System.Collections;

public class RealisticRainBlur
{
    private Material mtrl;

    public int blurDownSampleNum = 2;
    public float blurSpreadSize = 3.0f;
    public int blurIterations = 3;
  
    public RealisticRainBlur()
    {
        mtrl = new Material(Shader.Find("Hidden/RealisticRainBlur"));
    }

    public void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        float widthMod = 1.0f / (1.0f * (1 << blurDownSampleNum));
        mtrl.SetFloat("_DownSampleValue", blurSpreadSize * widthMod);
        sourceTexture.filterMode = FilterMode.Bilinear;
        int renderWidth = sourceTexture.width >> blurDownSampleNum;
        int renderHeight = sourceTexture.height >> blurDownSampleNum;

        RenderTexture renderBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);
        renderBuffer.filterMode = FilterMode.Bilinear;
        Graphics.Blit(sourceTexture, renderBuffer, mtrl, 0);

        for (int i = 0; i < blurIterations; i++)
        {
            float iterationOffs = (i * 1.0f);
            mtrl.SetFloat("_DownSampleValue", blurSpreadSize * widthMod + iterationOffs);

            RenderTexture tempBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);
            Graphics.Blit(renderBuffer, tempBuffer, mtrl, 1);
            RenderTexture.ReleaseTemporary(renderBuffer);
            renderBuffer = tempBuffer;

            tempBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);
            Graphics.Blit(renderBuffer, tempBuffer, mtrl, 2);

            RenderTexture.ReleaseTemporary(renderBuffer);
            renderBuffer = tempBuffer;
        }

        Graphics.Blit(renderBuffer, destTexture);
        RenderTexture.ReleaseTemporary(renderBuffer);
    }

    public void Destroy()
    {
        if(mtrl != null)
        {
            Material.DestroyImmediate(mtrl);
            mtrl = null;
        }
    }
}