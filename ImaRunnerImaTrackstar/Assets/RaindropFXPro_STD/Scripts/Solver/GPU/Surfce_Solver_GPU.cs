using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RaindropFX;

public class Surfce_Solver_GPU : MonoBehaviour {

    #region public params
    public bool fadeout_fadein_switch = false;
    public int refreshRate = 1;
    public int fusion = 1;

    public Vector2Int rainTexSize = new Vector2Int(800, 450);

    public Texture _noiseTex = null;
    [HideInInspector]
    public RenderTexture _baseTex;

    public bool debug = false;
    #endregion

    #region private params
    private RenderTexture[] _simBuffer = null;
    private Material SIM_Mat = null;
    private Material BLUR_Mat = null;
    private Renderer meshRender = null;
    private ForceLUTGenerator LUTGenerator = null;

    private int clock = 0;
    private bool swapBuffer = true;
    #endregion

    private void Update() {
        Render();
    }

    public void Render() {
        if (_noiseTex != null) {
            SafetyCheck();

            if (clock++ >= refreshRate) {
                clock = 0;
                
                SIM_Mat.SetTexture("_ForceLUT", LUTGenerator.forceLUT);
                //SIM_Mat.SetTexture("_SignLUT", lutComp.GetSignLUT());
                SIM_Mat.SetTexture("_NoiseTex", _noiseTex);
                SIM_Mat.SetFloat("_TurbScale", -1.0f);
                SIM_Mat.SetVector("_Force", Vector4.zero);
                SIM_Mat.SetInt("_fadeout", fadeout_fadein_switch ? 0 : 1);
                SIM_Mat.SetVector("_MainTex_TexelSize", new Vector4(
                    rainTexSize.x, rainTexSize.y, 0, 0
                ));

                // Simulation
                Graphics.Blit(
                    _simBuffer[swapBuffer ? 1 : 0], 
                    _simBuffer[swapBuffer ? 0 : 1], 
                    SIM_Mat
                );
                
                // Apply blur effect to raw raindrop texture
                RaindropFX_Tools.Blur(
                    _simBuffer[swapBuffer ? 0 : 1],
                    _simBuffer[2], fusion, BLUR_Mat
                );

                meshRender.sharedMaterial.SetTexture("_HeightMap", _simBuffer[2]);
                meshRender.sharedMaterial.SetTexture("_FogMaskMap", _simBuffer[2]);
                meshRender.sharedMaterial.SetInt("_debug", debug ? 1 : 0);
                meshRender.sharedMaterial.SetVector("_MainTex_TexelSize", new Vector4(
                    rainTexSize.x, rainTexSize.y, 0, 0
                ));
                swapBuffer = !swapBuffer;
            }

        }
    }

    private void SafetyCheck() {
        if (LUTGenerator == null)
            LUTGenerator = this.GetComponent<ForceLUTGenerator>();
        if (meshRender == null)
            meshRender = this.GetComponent<MeshRenderer>();
        if (SIM_Mat == null)
            SIM_Mat = new Material(Shader.Find("Custom/RaindropFX/GPUCore"));
        if (BLUR_Mat == null)
            BLUR_Mat = new Material(Shader.Find("Hidden/Custom/GaussianBlur_GPU"));
        if (_simBuffer == null) {
            _simBuffer = new RenderTexture[3];
            for (int i = 0; i < 3; i++)
                _simBuffer[i] = new RenderTexture(rainTexSize.x, rainTexSize.y, 0);
        }
    }
}
