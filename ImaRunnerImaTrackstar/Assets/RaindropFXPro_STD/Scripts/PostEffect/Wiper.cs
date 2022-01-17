﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;

namespace RaindropFX {
    [RequireComponent(typeof(Camera))]
    public class Wiper : MonoBehaviour {
        #region public params
        public PostProcessVolume postVolumn;
        public int cullLayer = 30;
        public List<GameObject> wipers;
        #endregion

        #region private params
        private RenderTexture wipeTexture;
        private Camera cam;
        private Dictionary<GameObject, int> originalRenderLayers;
        private RaindropFX_STD ppv;
        #endregion

        void Start() {
            cam = this.GetComponent<Camera>();
            originalRenderLayers = new Dictionary<GameObject, int>();
            wipeTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0);
            postVolumn.sharedProfile.TryGetSettings<RaindropFX_STD>(out ppv);
        }

        public void GetWiped(RaindropFX_STDRenderer target) {
            StartCoroutine(GetWipedI(target));
        }

        private IEnumerator GetWipedI(RaindropFX_STDRenderer target) {
            yield return (new WaitForEndOfFrame());
            RaindropFX_Tools.RenderTextureToTexture2D(ref target.tempWip, ref target.solver.calcRainTex);
        }

        private void Update() {
            bool state = postVolumn.enabled;
            var cullMask = cam.cullingMask;
            var clearFlag = cam.clearFlags;
            var bgcol = cam.backgroundColor;

            postVolumn.enabled = false;
            CollectRenderLayers();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.cullingMask = 1 << cullLayer;
            cam.targetTexture = wipeTexture;
            cam.backgroundColor = new Color(0, 0, 0, 0);
            cam.Render();
            cam.backgroundColor = bgcol;
            cam.targetTexture = null;
            cam.cullingMask = cullMask;
            cam.clearFlags = clearFlag;
            RestoreRenderLayers();
            postVolumn.enabled = state;
            
            ppv.wiper.value = wipeTexture;
        }

        void CollectRenderLayers() {
            originalRenderLayers.Clear();
            foreach (GameObject r in wipers) {
                if (!originalRenderLayers.ContainsKey(r)) {
                    originalRenderLayers.Add(r, r.gameObject.layer);
                    r.gameObject.layer = cullLayer;
                }
            }
        }

        void RestoreRenderLayers() {
            foreach (GameObject r in wipers) {
                int originalLayer = 0;
                originalRenderLayers.TryGetValue(r, out originalLayer);
                r.gameObject.layer = originalLayer;
            }
        }
    }
}