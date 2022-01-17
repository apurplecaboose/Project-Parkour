using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RaindropFX;

namespace RaindropFX {
    public class RaindropGenerator {
        #region parameters
        // system properties
        public bool fadeout_fadein_switch = false;
        public bool fastMode = false;
        public float fadeSpeed = 0.02f;

        public Texture2D raindropTex_alpha;
        public Texture2D cullMask_grayscale;
        public Vector2Int raindropTexSize;
        public Texture2D calcRainTex;
        public Vector2Int calcTexSize;

        public bool pixelization = false;
        public float pixResolution = 1;

        public bool forceRainTextureSize = true;
        public bool assForceToPixelSize = true;
        public bool assDropSizeToPixelSize = true;

        public float downSampling = 0.5f;
        public float calcTimeStep = 0.1f;
        public int refreshRate = 1;
        public bool generateTrail = true;
        public int maxStaticRaindropNumber = 5000;
        public int maxDynamicRaindropNumber = 10;
        public Vector2 raindropSizeRange = new Vector2(0.1f, 0.25f);
        public float maxRandomDynamicForce = 3.5f;
        public Color tintColor = Color.white;
        public int tintWeight = 1;

        // physical properties
        public bool useWind = false;
        public bool radialWind = false;
        public float windTurbulence = 0.1f;
        public float windTurbScale = 1.0f;
        public Vector2 wind = new Vector2(0.0f, 0.0f);
        public Vector2 gravity = new Vector2(0.0f, -9.8f);
        public float friction = 0.8f;

        // force lut
        public bool useForceLUT = false;
        public Texture2D forceLUT = null;
        public Texture2D signLUT = null;

        // Fog properties
        public bool useFog = false;
        public Color fogColor = Color.white;
        public float fogIntensity = 0.5f;
        public int fogIteration = 2;

        // wipe effect properties
        public bool wipeEffect = false;
        public float foggingSpeed = 1.0f;

        // screen blend properties
        public float distortion = 0.6f;
        public int edgeSoftness = 1;
        public float _inBlack = 55.0f;
        public float _inWhite = 180.0f;
        public float _outWhite = 160.0f;
        public float _outBlack = 5.0f;

        // foreground blur properties
        public bool dropletBlur = false;
        public float _focalize = 1.0f;
        public int blurIteration = 1;

        public Vector2 fadeSpeedRange = new Vector2(0.25f, 0.5f);
        public Vector2 shrinkSpeed = new Vector2(0.01f, 0.02f);
        public Vector2 loseWeightRange = new Vector2(0.01f, 0.02f);
        public float killSize = 0.05f;
        
        public Material blend_material;
        public Material blur_material;
        public Material dropblur_material;
        public Material level_material;
        public Material normal_material;
        public Material pixel_material;
        public Material wipe_material;
        public Material time_material;

        public List<Raindrop_STD> staticRaindrops;
        public List<Raindrop_STD> dynamicRaindrops;
        public CachePool_STD cachePool;

        private bool[][] rainTexColorFlags;
        private int staticUpdateCounter = 0;
        private int genClock = 0;
        private float ratio = 0.5f;

        // wipe effect
        private RenderTexture tempWip = null;
        private bool cnt = false;
        #endregion

        private float GetAssRatio() {
            return (assDropSizeToPixelSize ? ratio : 1.0f);
        }


        //--------------------------------------------------------------------------
        // *** Call this to re-init if you have changed the size of rain texture ***
        //--------------------------------------------------------------------------
        public void ReInit(Vector2Int _calcTexSize) {
            calcTexSize = _calcTexSize;
            //Debug.Log("Current screen rain texture size:" + calcTexSize);
            generateBaseMap();
            staticRaindrops.Clear();
            dynamicRaindrops.Clear();
        }

        //----------------------------------------------------------------
        // *** Don't forget to initialize the solver before using it ***
        //----------------------------------------------------------------
        public void Init(Texture2D raindropTex_alpha, Vector2Int calcRainTextureSize) {
            //GameObject cachePoolHolder = GameObject.Find("RaindropFX_CachePoolHolder");
            //if (cachePoolHolder == null) cachePoolHolder = new GameObject();
            //cachePoolHolder.name = "RaindropFX_CachePoolHolder";
            cachePool = Camera.main.GetComponent<CachePool_STD>();
            if (!cachePool) cachePool = Camera.main.gameObject.AddComponent<CachePool_STD>();
            else cachePool.Init();

            if (forceRainTextureSize) {
                calcTexSize = new Vector2Int(calcRainTextureSize.x, calcRainTextureSize.y);
            } else {
                calcTexSize = RaindropFX_Tools.GetViewSize();
            }

            this.raindropTex_alpha = raindropTex_alpha;
            raindropTexSize = new Vector2Int(raindropTex_alpha.width, raindropTex_alpha.height);
            generateBaseMap();
            staticRaindrops = new List<Raindrop_STD>();
            dynamicRaindrops = new List<Raindrop_STD>();
            PreparRainTexFlags();

            if (blend_material == null) {
                blend_material = new Material(Shader.Find("Hidden/Custom/ScreenBlendEffect"));
            }
            if (blur_material == null) {
                blur_material = new Material(Shader.Find("Hidden/Custom/GaussianBlur"));
            }
            if (level_material == null) {
                level_material = new Material(Shader.Find("Hidden/Custom/ColorLevel"));
            }
            if (dropblur_material == null) {
                dropblur_material = new Material(Shader.Find("Hidden/Custom/DropletBlur"));
            }
            if (normal_material == null) {
                normal_material = new Material(Shader.Find("Hidden/Custom/HeightToNormal"));
            }
            if (pixel_material == null) {
                pixel_material = new Material(Shader.Find("Hidden/Custom/PixelationShader"));
            }
            if (wipe_material == null) {
                wipe_material = new Material(Shader.Find("Hidden/Custom/Wiper"));
            }
            if (time_material == null) {
                time_material = new Material(Shader.Find("Hidden/Custom/TimeDelta"));
            }
        }

        //--------------------------------------------------------------------------
        // *** Don't forget to update properties before each calculation steps ***
        //--------------------------------------------------------------------------
        public void UpdateProps(
            ref bool fadeout_fadein_switch, ref bool fastMode, ref float fadeSpeed, ref bool forceRainTextureSize,
            ref Vector2Int forceSize, ref float calcTimeStep, ref int refreshRate, ref bool generateTrail, 
            ref int maxStaticRaindropNumber, ref int maxDynamicRaindropNumber, ref Vector2 raindropSizeRange, 
            ref bool useWind, ref float windTurbulence, ref float windTurbScale, ref Vector2 wind, ref Vector2 gravity, 
            ref float friction, ref float distortion, ref bool useFog, ref float fogIntensity, ref int fogIteration, 
            ref int edgeSoftness, ref float _inBlack, ref float _inWhite, ref float _outWhite, ref float _outBlack, 
            ref bool dropletBlur, ref float _focalize, ref int blurIteration, ref Color tintColor, ref int tintWeight, 
            ref Color fogColor, ref bool radialWind, ref Texture cullMask, ref bool pixelization, ref float pixResolution, 
            ref float downSampling, ref float foggingSpeed
        ) {
            if (forceRainTextureSize) {
                if (calcTexSize != forceSize) ReInit(forceSize);
            } else {
                var downSize = RaindropFX_Tools.GetDownSize(downSampling);
                if (calcTexSize != downSize) ReInit(downSize);
            }
            ratio = (float)calcTexSize.x / 800.0f;
            killSize = 0.05f * GetAssRatio();

            this.fadeout_fadein_switch = fadeout_fadein_switch;
            this.fastMode = fastMode;
            this.fadeSpeed = fadeSpeed;
            this.forceRainTextureSize = forceRainTextureSize;
            this.calcTimeStep = calcTimeStep * (assForceToPixelSize ? ratio : 1.0f);
            this.refreshRate = refreshRate;
            this.generateTrail = generateTrail;
            this.maxStaticRaindropNumber = maxStaticRaindropNumber;
            this.maxDynamicRaindropNumber = maxDynamicRaindropNumber;
            this.raindropSizeRange = raindropSizeRange;
            this.useWind = useWind;
            this.windTurbulence = windTurbulence;
            this.windTurbScale = windTurbScale;
            this.wind = wind;
            this.gravity = gravity;
            this.friction = friction;
            this.distortion = distortion;
            this.useFog = useFog;
            this.fogColor = fogColor;
            this.fogIntensity = fogIntensity;
            this.fogIteration = fogIteration;
            this.edgeSoftness = edgeSoftness;
            this._inBlack = _inBlack;
            this._inWhite = _inWhite;
            this._outBlack = _outBlack;
            this._outWhite = _outWhite;
            this.dropletBlur = dropletBlur;
            this._focalize = _focalize;
            this.blurIteration = blurIteration;
            this.tintColor = tintColor;
            this.tintWeight = tintWeight;
            this.radialWind = radialWind;
            if (cullMask != null) this.cullMask_grayscale = (Texture2D)cullMask;
            else this.cullMask_grayscale = null;
            this.pixelization = pixelization;
            this.pixResolution = pixResolution;
            this.foggingSpeed = foggingSpeed;
        }


        //----------------------------------------------------------------
        // Draw raindrop to the texture that is being calculated
        // Don't forget to call ApplyTexPainting(); after that
        //----------------------------------------------------------------
        public void PaintToCanvas(Raindrop_STD raindrop) {
            int widthOfRain = (int)raindrop.realSize.x;
            int heightOfRain = (int)raindrop.realSize.y;
            int left_up_X = (int)(raindrop.position.x - widthOfRain / 2);
            int left_up_Y = (int)(raindrop.position.y - heightOfRain / 2);

            for (int i = left_up_X, r = 0; i < left_up_X + widthOfRain; i++, r++) {
                for (int j = left_up_Y, c = 0; j < left_up_Y + heightOfRain; j++, c++) {
                    if (i < calcTexSize.x && i >= 0 && j >= 0 && j < calcTexSize.y) {
                        Color newColor = getColorAtPos(r, c, raindrop.size);
                        if (newColor == Color.white) calcRainTex.SetPixel(i, j, newColor);
                    }
                }
            }
        }

        //---------------------------------------------------------------------
        // Remove raindrop from the texture that is being calculated
        // Don't forget to call ApplyTexPainting(); after that
        //---------------------------------------------------------------------
        public void EraseFromCanvas(Raindrop_STD raindrop) {
            int widthOfRain = (int)raindrop.realSize.x;
            int heightOfRain = (int)raindrop.realSize.y;
            int left_up_X = (int)(raindrop.position.x - widthOfRain / 2);
            int left_up_Y = (int)(raindrop.position.y - heightOfRain / 2);

            for (int i = left_up_X; i < left_up_X + widthOfRain; i++) {
                for (int j = left_up_Y; j < left_up_Y + heightOfRain; j++) {
                    if (i < calcTexSize.x && i >= 0 && j >= 0 && j < calcTexSize.y) {
                        calcRainTex.SetPixel(i, j, Color.black);
                    }
                }
            }
        }

        //---------------------------------------------------------------------
        // The percentage of water droplet shrinking
        //---------------------------------------------------------------------
        public bool Shrink(Raindrop_STD raindrop, float shrinkAmount) {
            if (raindrop.size < killSize) return true;
            EraseFromCanvas(raindrop);
            raindrop.size *= (1.0f - shrinkAmount * GetAssRatio());
            raindrop.UpdateWeight();
            PaintToCanvas(raindrop);
            return false;
        }

        //---------------------------------------------------------------------
        // The percentage of water droplet fads
        //---------------------------------------------------------------------
        public bool Fade(Raindrop_STD raindrop, float fadeAmount) {
            int widthOfRain = (int)raindrop.realSize.x;
            int heightOfRain = (int)raindrop.realSize.y;
            int left_up_X = (int)(raindrop.position.x - widthOfRain / 2);
            int left_up_Y = (int)(raindrop.position.y - heightOfRain / 2);

            int cnt = 0;
            int judger = (int)(widthOfRain * heightOfRain * (fastMode ? 0.7f : 0.9f));
            for (int i = left_up_X; i < left_up_X + widthOfRain; i++) {
                for (int j = left_up_Y; j < left_up_Y + heightOfRain; j++) {
                    if (i < calcRainTex.width && i >= 0 && j >= 0 && j < calcRainTex.height) {
                        Color newColor = calcRainTex.GetPixel(i, j);
                        if (newColor.r > 0) {
                            if (fastMode && fadeout_fadein_switch && newColor.r < 0.6f) return true;
                            else if (newColor.r < fadeAmount) return true;
                            newColor.b = newColor.g = newColor.r = newColor.r - fadeAmount;
                            calcRainTex.SetPixel(i, j, newColor);
                        } else cnt++;
                    } else cnt++;
                    if (fadeout_fadein_switch && cnt >= judger) return true;
                }
            }

            if (fadeout_fadein_switch) {
                raindrop.lifeTime--;
                if (raindrop.lifeTime <= 0) return true;
            }

            return false;
        }

        public void ApplyTexPainting() {
            calcRainTex.Apply();
        }

        public void generateBaseMap() {
            calcRainTex = new Texture2D(calcTexSize.x, calcTexSize.y);
            for (int x = 0; x < calcTexSize.x; x++) {
                for (int y = 0; y < calcTexSize.y; y++) {
                    calcRainTex.SetPixel(x, y, Color.black);
                }
            }
            ApplyTexPainting();
        }

        //---------------------------------------------------------------------------------
        // Call this method to get the screen rain texture (height map) in each time step
        //---------------------------------------------------------------------------------
        public void CalcRainTex() {
            if (genClock < refreshRate) {
                genClock++;
            } else {
                genClock = 0;
                if (fadeout_fadein_switch) {
                    int lenS = staticRaindrops.Count;
                    int lenD = dynamicRaindrops.Count;
                    float fadePst = UnityEngine.Random.Range(fadeSpeedRange.x, fadeSpeedRange.y) * fadeSpeed;

                    // kill dynamic raindrops
                    if (fastMode) for (int i = lenD - 1; i >= 0; i--) Kill(i, false);
                    else UpdateDyanmicRaindrops();

                    // kill static raindrops
                    int cnt = 0;
                    for (int i = lenS - 1; i >= 0; i--) {
                        if (staticRaindrops[i].size <= killSize * (fastMode ? 3.0f : 1.0f)) {
                            Kill(i, true);
                            continue;
                        }
                        if (Fade(staticRaindrops[i], fadePst)) Kill(i, true);
                        if (fastMode && cnt++ >= 600) break;
                    }
                    if (lenS == 0 && lenD == 0) RaindropFX_Tools.PrintLog("all droplets killed.");
                    else RaindropFX_Tools.PrintLog("surplus: Static: " + lenS + " Dynamic: " + lenD);
                } else {
                    UpdateStaticRaindrops();
                    UpdateDyanmicRaindrops();
                    int genNum = UnityEngine.Random.Range(0, 10);
                    if (genNum >= 5) {
                        if (genNum >= 8) {
                            if (dynamicRaindrops.Count < maxDynamicRaindropNumber)
                                GenDynamicRaindrop(
                                    new Vector2(
                                        UnityEngine.Random.Range(0, calcTexSize.x),
                                        UnityEngine.Random.Range(0, calcTexSize.y)
                                    ),
                                    UnityEngine.Random.Range(
                                        raindropSizeRange.x, 
                                        raindropSizeRange.y
                                    )
                                );
                        } else GenRandomStaticRaindrops();
                    }
                }
                ApplyTexPainting();
            }
        }

        //--------------------------------------------------------------
        // Call this method after 'CalcRainTex();' to get screen rain
        // normal map and screen fog mask
        //--------------------------------------------------------------
        public void GenerateTextures(ref RenderTexture resultTex, ref RenderTexture fogMask, 
            ref RenderTexture wipeMask, ref RenderTexture wipeDelta) {
            var temp = RenderTexture.GetTemporary(calcTexSize.x, calcTexSize.y, 0);
            var temp2 = RenderTexture.GetTemporary(calcTexSize.x, calcTexSize.y, 0);
            var temp3 = RenderTexture.GetTemporary(calcTexSize.x, calcTexSize.y, 0);
            //var temp4 = RenderTexture.GetTemporary(calcTexSize.x, calcTexSize.y, 0);

            if (resultTex != null) RenderTexture.ReleaseTemporary(resultTex);
            if (fogMask != null) RenderTexture.ReleaseTemporary(fogMask);

            // apply wipe effect
            if (wipeMask == null) wipeMask = new RenderTexture(calcTexSize.x, calcTexSize.y, 0);
            if (tempWip == null) tempWip = new RenderTexture(calcTexSize.x, calcTexSize.y, 0);
            if (wipeEffect && wipeDelta != null) {
                var tempD = RenderTexture.GetTemporary(calcTexSize.x, calcTexSize.y, 0);
                if (cnt) Graphics.Blit(wipeMask, tempD);
                else RaindropFX_Tools.Blur(wipeMask, tempD, 1, blur_material);
                cnt = !cnt; SetTimeMat(ref wipeDelta);
                Graphics.Blit(tempD, wipeMask, time_material);

                // wipe rain tex
                wipe_material.SetTexture("_WipeTex", wipeMask);
                Graphics.Blit(calcRainTex, tempWip, wipe_material);
                RaindropFX_Tools.RenderTextureToTexture2D(ref tempWip, ref calcRainTex);

                RenderTexture.ReleaseTemporary(tempD);
            }

            // apply blur effect to calcRainTex
            RaindropFX_Tools.Blur(calcRainTex, temp, edgeSoftness, blur_material);

            // apply color level to calcRainTex
            SetLevelMat();
            Graphics.Blit(temp, temp2, level_material);
            fogMask = temp;

            // pixelize the raindrop
            //pixel_material.SetFloat("_PixelSize", pixResolution);
            //Graphics.Blit(temp2, temp4, pixel_material);

            // convert height map to normal map
            SetNormalMat(ref temp2);
            Graphics.Blit(temp2, temp3, normal_material);

            // output final result
            resultTex = temp3;

            //RenderTexture.ReleaseTemporary(temp);
            RenderTexture.ReleaseTemporary(temp2);
            //RenderTexture.ReleaseTemporary(temp3);
            //RenderTexture.ReleaseTemporary(temp4);
        }

        public void SetTimeMat(ref RenderTexture wipeDelta) {
            time_material.SetFloat("_ratio", (float)calcTexSize.x / calcTexSize.y);
            time_material.SetFloat("_speed", foggingSpeed * 0.988f);
            time_material.SetTexture("_WipeTex", wipeDelta);
        }
        public void SetLevelMat() {
            level_material.SetFloat("_inBlack", _inBlack);
            level_material.SetFloat("_inWhite", _inWhite);
            level_material.SetFloat("_outWhite", _outWhite);
            level_material.SetFloat("_outBlack", _outBlack);
        }
        public void SetNormalMat(ref RenderTexture temp) {
            normal_material.SetVector("_MainTex_TexelSize", new Vector4(calcTexSize.x, calcTexSize.y, 0, 0));
            normal_material.SetTexture("_HeightMap", temp);
        }
        public void SetScreenBlendMat(
            ref RenderTexture tempHeightMap, 
            ref RenderTexture tempWetMap, 
            ref RenderTexture wipeMap
        ) {
            blend_material.SetInt("_IsUseFog", useFog ? 1 : 0);
            blend_material.SetFloat("_Distortion", distortion);
            blend_material.SetFloat("_FogIntensity", fogIntensity);
            blend_material.SetFloat("_FogIteration", fogIteration);
            blend_material.SetTexture("_HeightMap", tempHeightMap);
            blend_material.SetTexture("_WetMap", tempWetMap);
            blend_material.SetTexture("_WipeMap", wipeMap);
            blend_material.SetTexture("_CullMask", cullMask_grayscale);
            blend_material.SetColor("_TintColor", tintColor);
            blend_material.SetColor("_FogTint", fogColor);
            blend_material.SetInt("_TintWeight", tintWeight);
            blend_material.SetVector("_MainTex_TexelSize", new Vector4(calcTexSize.x, calcTexSize.y, 0, 0));
        }
        public void SetDropblurMat(
            ref RenderTexture tempBlurredMainTex, 
            ref RenderTexture tempMask,
            ref Texture staticMask
        ) {
            dropblur_material.SetTexture("_BlurredMainTex", tempBlurredMainTex);
            dropblur_material.SetTexture("_MaskMap", tempMask);
            dropblur_material.SetTexture("_StaticMask", staticMask);
            dropblur_material.SetFloat("_focalize", _focalize);
            //dropblur_material.SetFloat("_smooth", depthSmooth);
        }

        //--------------------------
        // Kill raindrop by object
        //--------------------------
        public void Kill(Raindrop_STD p, bool isStatic) {
            if (isStatic) {
                EraseFromCanvas(p);
                cachePool.Recycle(p);
                staticRaindrops.Remove(p);
            } else {
                EraseFromCanvas(p);
                cachePool.Recycle(p);
                dynamicRaindrops.Remove(p);
            }
        }

        //----------------------
        // Kill raindrop by id
        //----------------------
        public void Kill(int id, bool isStatic) {
            if (isStatic) {
                if (id >= staticRaindrops.Count) return;
                EraseFromCanvas(staticRaindrops[id]);
                cachePool.Recycle(staticRaindrops[id]);
                staticRaindrops.RemoveAt(id);
            } else {
                if (id >= dynamicRaindrops.Count) return;
                EraseFromCanvas(dynamicRaindrops[id]);
                cachePool.Recycle(dynamicRaindrops[id]);
                dynamicRaindrops.RemoveAt(id);
            }
        }

        public Vector2 SamplingLUT(Vector2 uv) {
            var data = forceLUT.GetPixelBilinear(uv.x, uv.y);
            //var sign = signLUT.GetPixelBilinear(uv.x, uv.y);

            //if (sign.r < 0.5) data.r *= -1;
            //if (sign.g > 0.5) data.g *= -1;

            if (data.b < 0.5) data.r *= -1;
            if (data.a > 0.5) data.g *= -1;

            return new Vector2(data.r, data.g);
        }

        //-----------------------------------------------------------------------------------------------
        // Update five static waterdrops each time, you can increase the number of updates
        //-----------------------------------------------------------------------------------------------
        public void UpdateStaticRaindrops() {
            int updateNumber = UnityEngine.Random.Range(0, 5) + 1;
            while (updateNumber > 0) {
                if (staticRaindrops.Count == 0) return;
                updateNumber--;
                int randomID = UnityEngine.Random.Range(0, staticRaindrops.Count);
                bool flag = Shrink(
                    staticRaindrops[randomID], 
                    UnityEngine.Random.Range(shrinkSpeed.x, shrinkSpeed.y)
                );
                if (flag) Kill(randomID, true);
                if (staticUpdateCounter >= staticRaindrops.Count) staticUpdateCounter = 0;
                if (staticUpdateCounter < staticRaindrops.Count) 
                    flag = Fade(
                        staticRaindrops[staticUpdateCounter], 
                        UnityEngine.Random.Range(fadeSpeedRange.x, fadeSpeedRange.y)
                    );
                if (flag) Kill(staticUpdateCounter, true);
                staticUpdateCounter++;
            }
        }

        //-----------------------------------------------------------------------------------------------
        // Update one dynamic waterdrop each time, you can increase the number of updates
        //-----------------------------------------------------------------------------------------------
        public void UpdateDyanmicRaindrops() {
            if (dynamicRaindrops.Count == 0) return;
            int len = dynamicRaindrops.Count;
            Vector2 oldPos = Vector2.zero;
            for (int i = len - 1; i >= 0; i--) {
                EraseFromCanvas(dynamicRaindrops[i]);
                dynamicRaindrops[i].UpdateProp(raindropTexSize, friction);

                if (generateTrail) {
                    oldPos = dynamicRaindrops[i].position;
                    float lose = UnityEngine.Random.Range(loseWeightRange.x, loseWeightRange.y);
                    dynamicRaindrops[i].LoseWeight(lose);
                    GenTrailDrop(oldPos, dynamicRaindrops[i].size * lose * 50.0f);
                    if (dynamicRaindrops[i].size < killSize) {
                        Kill(i, false); continue;
                    }
                }

                Vector2 newlyAddedForce = Vector2.zero;
                if (useForceLUT) {
                    // read force from force LUT
                    if (forceLUT != null)
                        newlyAddedForce = SamplingLUT(dynamicRaindrops[i].position / calcTexSize);
                } else {
                    newlyAddedForce += gravity;
                    if (useWind) {
                        if (radialWind) {
                            Vector2 wDir = dynamicRaindrops[i].position -
                                            new Vector2(calcTexSize.x, calcTexSize.y) / 2;
                            if (wDir.magnitude == 0) wDir = Vector2.down;
                            newlyAddedForce += wDir.normalized * wind.magnitude;
                        } else newlyAddedForce += wind;

                        if (windTurbulence > 0) {
                            float turbulenceForce = RaindropFX_Tools.PerlinNoiseSampler(dynamicRaindrops[i].position,
                                                                                        windTurbScale);
                            Vector2 RTDir = RaindropFX_Tools.RotateAround(dynamicRaindrops[i].velocity, Vector2.zero, 90.0f);
                            newlyAddedForce += RTDir * turbulenceForce * windTurbulence;
                        }
                    }
                }
                dynamicRaindrops[i].UpdateForce(newlyAddedForce);
                int randomForceChecker = UnityEngine.Random.Range(0, 10);
                if (randomForceChecker < 3 && !radialWind) dynamicRaindrops[i].ApplyRandomForce(maxRandomDynamicForce);
                dynamicRaindrops[i].ApplyFriction();
                dynamicRaindrops[i].CalcNewPos(calcTimeStep);
                if (dynamicRaindrops[i].position.x >= 0 && 
                    dynamicRaindrops[i].position.x < calcTexSize.x && 
                    dynamicRaindrops[i].position.y >= 0 && 
                    dynamicRaindrops[i].position.y < calcTexSize.y) {
                    dynamicRaindrops[i].UpdateVelocity(calcTimeStep);
                    PaintToCanvas(dynamicRaindrops[i]);
                } else Kill(i, false);
            }
        }

        //-----------------------------------------------------------------------------
        // Dynamic water droplets produce a tail when they slide
        //-----------------------------------------------------------------------------
        public void GenTrailDrop(Vector2 centerPos, float size) {
            if (maxStaticRaindropNumber == 0) return;
            if (staticRaindrops.Count >= maxStaticRaindropNumber) Kill(0, true);
            Raindrop_STD newRaindrop = cachePool.GetRaindrop();
            newRaindrop.Reuse(
                true, centerPos, size, 
                (int)((fastMode ? 0.8f : 1.0f) / fadeSpeedRange.x / fadeSpeed), 
                raindropTexSize, friction
            );
            PaintToCanvas(newRaindrop);
            staticRaindrops.Add(newRaindrop);
        }

        //------------------------------------
        // Generate new static raindrops at
        // random location of the screen
        //------------------------------------
        public void GenRandomStaticRaindrops() {
            int createNumber = UnityEngine.Random.Range(0, 5) + 1;
            while (createNumber > 0) {
                createNumber--;
                if (staticRaindrops.Count >= maxStaticRaindropNumber) return;
                Raindrop_STD newRaindrop = cachePool.GetRaindrop();
                newRaindrop.Reuse(
                    true, new Vector2(
                        UnityEngine.Random.Range(0, calcTexSize.x),
                        UnityEngine.Random.Range(0, calcTexSize.y)
                    ),
                    UnityEngine.Random.Range(raindropSizeRange.x, raindropSizeRange.y) * 0.5f * GetAssRatio(),
                    (int)((fastMode ? 0.8f : 1.0f) / fadeSpeedRange.x / fadeSpeed),
                    raindropTexSize, friction
                );
                staticRaindrops.Add(newRaindrop);
            }
        }

        //------------------------------------
        // Generate new dynamic raindrops
        //------------------------------------
        public void GenDynamicRaindrop(Vector2 centerPos, float size) {
            if (dynamicRaindrops.Count >= maxDynamicRaindropNumber) return;
            Raindrop_STD newRaindrop = cachePool.GetRaindrop();
            newRaindrop.Reuse(
                false, centerPos,
                size * GetAssRatio(), -1, 
                raindropTexSize, friction
            );
            dynamicRaindrops.Add(newRaindrop);
        }

        //--------------------------------------
        // Scale the drop to the specified size
        //--------------------------------------
        public Color getColorAtPos(int x, int y, float scaleFactor) {
            float scale = 1.0f / scaleFactor;
            int xx = (int)(scale * x);
            int yy = (int)(scale * y);
            return rainTexColorFlags[xx][yy] ? Color.white : Color.black;
        }

        //-----------------------------------------
        // Prepare bool groups for droplet scaling
        //-----------------------------------------
        public void PreparRainTexFlags() {
            int widthOfRain = raindropTex_alpha.width;
            int heightOfRain = raindropTex_alpha.height;

            rainTexColorFlags = new bool[widthOfRain][];
            for (int i = 0; i < widthOfRain; i++) {
                rainTexColorFlags[i] = new bool[heightOfRain];
                for (int j = 0; j < heightOfRain; j++) {
                    rainTexColorFlags[i][j] = (raindropTex_alpha.GetPixel(i, j).a > 0) ? true : false;
                }
            }
        }

    }

}