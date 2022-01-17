using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using RaindropFX;

namespace RaindropFX {
    [Serializable]
    [PostProcess(typeof(RaindropFX_GPURenderer), PostProcessEvent.AfterStack, "RaindropFX/RaindropFX_GPU")]
    public sealed class RaindropFX_GPU : PostProcessEffectSettings {
        [Tooltip("Waterdrops will fade in/out automatically if you disable/enable this.")]
        public BoolParameter fadeout_fadein_switch = new BoolParameter { value = false };

        [Tooltip("Noise texture is used to specify where raindrops should be generated.")]
        public TextureParameter _noiseTex = new TextureParameter { value = null };

        [Tooltip("Frame interval of texture rendering.")]
        public IntParameter refreshRate = new IntParameter { value = 10 };

        [Range(0f, 16f), Tooltip("Spawn rate of static raindrops.")]
        public FloatParameter StaticSpawnRate = new FloatParameter { value = 5.0f };

        [Range(0f, 3.6f), Tooltip("Spawn rate of dynamic raindrops.")]
        public FloatParameter DynamicSpawnRate = new FloatParameter { value = 1.0f };

        [Range(0, 15), Tooltip("Fusion small droplets.")]
        public IntParameter fusion = new IntParameter { value = 2 };

        [Tooltip("Gravity adjustment.")]
        public Vector2Parameter gravity = new Vector2Parameter { value = new Vector2(0, -9.8f) };

        [Tooltip("Wind power adjustment.")]
        public Vector2Parameter wind = new Vector2Parameter { value = Vector2.zero };

        [Range(0f, 10f), Tooltip("Adjust scale of wind turbulence.")]
        public FloatParameter windTurbulence = new FloatParameter { value = 1.0f };

        [Range(0, 10), Tooltip("Screen blend effect intensity.")]
        public FloatParameter distortion = new FloatParameter { value = 0.6f };

        [Range(0f, 255f), Tooltip("Color level parameter.")]
        public FloatParameter _inBlack = new FloatParameter { value = 55.0f };

        [Range(0f, 255f), Tooltip("Color level parameter.")]
        public FloatParameter _inWhite = new FloatParameter { value = 180.0f };

        [Range(0f, 255f), Tooltip("Color level parameter.")]
        public FloatParameter _outWhite = new FloatParameter { value = 160.0f };

        [Range(0f, 255f), Tooltip("Color level parameter.")]
        public FloatParameter _outBlack = new FloatParameter { value = 5.0f };

        [Tooltip("Debug raindrop texture.")]
        public BoolParameter debug = new BoolParameter { value = false };
    }

    public class RaindropFX_GPURenderer : PostProcessEffectRenderer<RaindropFX_GPU> {

        #region parameters
        Material SIM_Mat;
        Material BLEND_Mat;
        Material BLUR_Mat;

        int clock = 0;
        bool swapBuffer = true;
        private RenderTexture[] _simBuffer = null;
        private RenderTexture result = null;
        public PostProcessRenderContext context;

        bool initFlag = false;
        #endregion
        
        //---------------------------------
        // Calculate raindrops animation
        //---------------------------------
        public override void Render(PostProcessRenderContext context) {
            this.context = context;
            if (settings._noiseTex.value != null) {
                Constraints();

                if (clock++ >= settings.refreshRate.value) {
                    clock = 0;
                    SIM_Mat.SetTexture("_NoiseTex", settings._noiseTex.value);
                    SIM_Mat.SetFloat("_staDens", settings.StaticSpawnRate.value);
                    SIM_Mat.SetFloat("_dynDens", settings.DynamicSpawnRate.value);
                    SIM_Mat.SetFloat("_TurbScale", settings.windTurbulence.value);
                    SIM_Mat.SetInt("_fadeout", settings.fadeout_fadein_switch.value ? 0 : 1);
                    SIM_Mat.SetVector("_Force", new Vector4(
                        settings.wind.value.x + settings.gravity.value.x,
                        settings.wind.value.y + settings.gravity.value.y
                    ));
                    context.command.Blit(_simBuffer[swapBuffer ? 1 : 0], _simBuffer[swapBuffer ? 0 : 1], SIM_Mat);
                    swapBuffer = !swapBuffer;
                    if (settings.debug.value) {
                        context.command.Blit(_simBuffer[swapBuffer ? 0 : 1], context.destination);
                        return;
                    }
                }

                // apply blur effect to solver.calcRainTex
                //var result = RenderTexture.GetTemporary(
                //    _simBuffer[0].width, _simBuffer[0].height, 0
                //);
                RaindropFX_Tools.Blur(
                    _simBuffer[swapBuffer ? 0 : 1], result,
                    settings.fusion.value, BLUR_Mat
                );

                // pixelize the raindrop
                BLEND_Mat.SetFloat("_PixelSize", -1);

                // convert height map to normal map and create screen blend effect
                SetBlendMat(ref result, ref result, ref result);

                // output final result
                context.command.Blit(context.source, context.destination, BLEND_Mat);

                //RenderTexture.ReleaseTemporary(result);
            } else {
                Debug.Log("noise tex is null!");
                context.command.Blit(context.source, context.destination);
            }
        }

        private void SetBlendMat(ref RenderTexture heightMap, ref RenderTexture wetMap, ref RenderTexture wipeMap) {
            //BLEND_Mat.SetColor("_TintColor", tintColor.value);
            //BLEND_Mat.SetColor("_FogTint", fogTint.value);
            //BLEND_Mat.SetInt("_IsUseFog", useFog.value ? 1 : 0);
            //BLEND_Mat.SetInt("_IsUseWipe", wipeEffect.value ? 1 : 0);
            BLEND_Mat.SetFloat("_Distortion", settings.distortion.value);
            //BLEND_Mat.SetFloat("_FogIntensity", fogIntensity.value);
            //BLEND_Mat.SetFloat("_FogIteration", fogIteration.value);
            BLEND_Mat.SetTexture("_HeightMap", heightMap);
            //BLEND_Mat.SetTexture("_WetMap", wetMap);
            //BLEND_Mat.SetTexture("_WipeMap", wipeMap);
            //BLEND_Mat.SetTexture("_CullMask", _rainMask_grayscale.value);
            //BLEND_Mat.SetTexture("_MainTex", background);

            BLEND_Mat.SetInt("_IsUseFog", 0);
            BLEND_Mat.SetInt("_IsUseWipe", 0);

            BLEND_Mat.SetFloat("_inBlack", settings._inBlack.value);
            BLEND_Mat.SetFloat("_inWhite", settings._inWhite.value);
            BLEND_Mat.SetFloat("_outWhite", settings._outWhite.value);
            BLEND_Mat.SetFloat("_outBlack", settings._outBlack.value);
        }

        //---------------------------------
        // Initialize raindrop solver
        //---------------------------------
        private void InitSys() {
            RaindropFX_Tools.debugLog = false;

            _simBuffer = new RenderTexture[2];
            _simBuffer[0] = new RenderTexture(Screen.width, Screen.height, 0);
            _simBuffer[1] = new RenderTexture(Screen.width, Screen.height, 0);
            result = new RenderTexture(Screen.width, Screen.height, 0);

            if (Shader.Find("Custom/RaindropFX/GPUCore") != null)
                SIM_Mat = new Material(Shader.Find("Custom/RaindropFX/GPUCore"));
            if (Shader.Find("Custom/RaindropFX/ScreenBlendEffect_GPU") != null)
                BLEND_Mat = new Material(Shader.Find("Custom/RaindropFX/ScreenBlendEffect_GPU"));
            if (Shader.Find("Hidden/Custom/GaussianBlur_GPU") != null)
                BLUR_Mat = new Material(Shader.Find("Hidden/Custom/GaussianBlur_GPU"));
        }

        //---------------------------------
        // Parameter security detection
        //---------------------------------
        private void Constraints() {
            if (_simBuffer == null) initFlag = true;
            if (initFlag) { InitSys(); initFlag = false; }
        }

    }
}