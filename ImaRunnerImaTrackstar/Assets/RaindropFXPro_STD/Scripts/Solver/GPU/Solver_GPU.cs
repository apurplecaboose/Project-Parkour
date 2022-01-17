using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RaindropFX;

namespace RaindropFX {
    [ExecuteAlways]
    public class Solver_GPU : MonoBehaviour {

        #region public params
        public float downSampling = 1.0f;

        public int maxStaticDropNum = 2000;
        public int maxDynamicDropNum = 10;
        public int staticSpawnRate = 1;
        public int dynamicSpawnRate = 5;

        public float fadeSpeed = 1.0f;
        public Vector2 dropSizeRange = new Vector2(1.0f, 5.0f);
        #endregion

        #region private params
        public RenderTexture rainTexture = null;

        private float ratio = 1.0f;
        private Material paint_material = null;
        #endregion

        private void SafetyCheck() {
            var scrSize = RaindropFX_Tools.GetViewSize();
            ratio = scrSize.x / 800.0f;

            if (rainTexture == null) {
                rainTexture = new RenderTexture(scrSize.x, scrSize.y, 16);
                rainTexture.name = "rainTexture";
            }
            if (paint_material == null) {
                paint_material = new Material(Shader.Find("Shader/RaindropFX/GPU/Painter"));
            }
        }

        private void OnPostRender() {
            SafetyCheck();

            //paint_material.SetVectorArray()
        }
    }
}