using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RaindropFX;

namespace RaindropFX {
    public class ForceLUTGenerator : MonoBehaviour {

        #region public params
        public bool autoLUTCamera = true;
        public Vector3 gravity = new Vector3(0, -9.8f, 0);
        public Vector3 velocity = new Vector3(0, 0, 0);
        public Vector3 wind = new Vector3(0, 0, 0);
        //public float turbulence = 0.0f;
        public Vector2Int lutSize = new Vector2Int(512, 512);
        //[HideInInspector]
        public Camera targetCam;
        //[HideInInspector]
        public Texture2D forceLUT = null;
        [HideInInspector]
        public Texture2D signLUT = null;

        public bool useNormalMap = false;
        public bool manualUpdate = false;

        public bool debug = false;
        public float debugLength = 0.2f;
        #endregion

        #region private params
        //private Vector3 oldG, oldW, oldV;

        private RenderTexture rawLUT = null;
        private Material forceMat = null;
        private Renderer renderComp = null;
        private bool lutNeedUpdate = false;
        #endregion

        #region functions
        private Camera CreateCamera(string name) {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(this.transform);
            obj.transform.localPosition = obj.transform.forward;
            obj.transform.LookAt(this.transform);

            Camera cam = obj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.cullingMask = 1 << this.gameObject.layer;
            cam.backgroundColor = Color.black;
            cam.targetTexture = rawLUT;
            //cam.enabled = false;

            return cam;
        }

        public void SetLUTSize(Vector2Int size) {
            rawLUT = new RenderTexture(size.x, size.y, 24, RenderTextureFormat.ARGBFloat);
            rawLUT.name = "rawLUT";
        }

        public void UpdateLUT(int sign = 0) {
            targetCam.enabled = true;

            // store camera state
            Material mt = renderComp.sharedMaterial;
            RenderTexture rt = targetCam.targetTexture;

            renderComp.material = forceMat;
            renderComp.material.SetTexture("_TurbTex", mt.GetTexture("_BumpMap"));
            renderComp.material.SetTextureScale("_TurbTex", mt.GetTextureScale("_BumpMap"));
            renderComp.material.SetTextureOffset("_TurbTex", mt.GetTextureOffset("_BumpMap"));

            renderComp.material.SetVector("_Gravity", gravity);
            renderComp.material.SetVector("_Wind", wind);
            renderComp.material.SetVector("_Velocity", velocity);
            renderComp.material.SetFloat("_Turbulence", 0.0f);
            renderComp.material.SetInt("_useNormalMap", useNormalMap ? 1 : 0);
            renderComp.material.SetFloat("_NegSign", sign);

            //targetCam.targetTexture = rawLUT;
            targetCam.Render();

            // restore camera state
            targetCam.targetTexture = rt;
            renderComp.sharedMaterial = mt;

            // copy data to readable LUT
            RenderTexture currentActiveRT = RenderTexture.active;
            RenderTexture.active = rawLUT;

            if (sign == 0 || sign == 2 || sign == 4) {
                if (forceLUT == null || forceLUT.width != rawLUT.width || forceLUT.height != rawLUT.height) {
                    forceLUT = new Texture2D(rawLUT.width, rawLUT.height, TextureFormat.RGBAFloat, false, true);
                    forceLUT.name = "forceLUT";
                }
                forceLUT.ReadPixels(new Rect(0, 0, rawLUT.width, rawLUT.height), 0, 0);
                forceLUT.Apply();
            } else if (sign == 1 || sign == 3) {
                if (signLUT == null || signLUT.width != rawLUT.width || signLUT.height != rawLUT.height) {
                    signLUT = new Texture2D(rawLUT.width, rawLUT.height, TextureFormat.RGBAFloat, false, true);
                    signLUT.name = "signLUT";
                }
                signLUT.ReadPixels(new Rect(0, 0, rawLUT.width, rawLUT.height), 0, 0);
                signLUT.Apply();
            }

            RenderTexture.active = currentActiveRT;

            targetCam.enabled = false;
        }

        public Vector2 SamplingLUT_UV(Vector2 uv) {
            var data = forceLUT.GetPixelBilinear(uv.x, uv.y);

            if (data.b < 0.5) data.r *= -1;
            if (data.a > 0.5) data.g *= -1;

            return new Vector2(data.r, data.g);
        }

        public Vector2 SamplingLUT_UV_SIGN(Vector2 uv) {
            var data = forceLUT.GetPixelBilinear(uv.x, uv.y);
            var sign = signLUT.GetPixelBilinear(uv.x, uv.y);

            if (sign.r < 0.5) data.r *= -1;
            if (sign.g > 0.5) data.g *= -1;

            //Debug.Log(data);
            return new Vector2(data.r, data.g);
        }

        public Vector3 SamplingLUT(Vector2 uv) {
            var data = forceLUT.GetPixelBilinear(uv.x, uv.y);
            var sign = signLUT.GetPixelBilinear(uv.x, uv.y);

            if (sign.r < 0.5) data.r *= -1;
            if (sign.g < 0.5) data.g *= -1;
            if (sign.b < 0.5) data.b *= -1;

            //Debug.Log(data);
            return new Vector3(data.r, data.g, data.b);
        }

        private void SaftyCheck() {
            if (rawLUT == null || rawLUT.width != lutSize.x || rawLUT.height != lutSize.y)
                SetLUTSize(lutSize);
            if (renderComp == null) renderComp = GetComponent<Renderer>();
            if (forceMat == null) forceMat = new Material(Shader.Find("Hidden/Custom/ForceLUTShader"));
            if (targetCam == null && autoLUTCamera) targetCam = CreateCamera("RFX_LUT_Cam");
        }

        private void OnDrawGizmos() {
            if (forceLUT == null) return;
            if (debug) {
                var mesh = GetComponent<MeshFilter>().sharedMesh;
                var vertices = mesh.vertices; int cnt = 0;
                foreach (var p in vertices) {
                    var wp = transform.TransformPoint(p);

                    // v3 version
                    //Gizmos.DrawLine(wp, wp + SamplingLUT(mesh.uv[cnt++]));

                    // v2 version
                    var tangent = mesh.tangents[cnt];
                    Vector3 tanv = (new Vector3(tangent.x, tangent.y, tangent.z));
                    var bitan = Vector3.Cross(tanv, mesh.normals[cnt]);
                    var uvSpaceForce = SamplingLUT_UV(mesh.uv[cnt++]);
                    //Debug.Log(uvSpaceForce);
                    var dir = tanv * uvSpaceForce.x + bitan * uvSpaceForce.y;
                    Gizmos.DrawLine(
                        wp, wp + transform.TransformDirection(dir) * debugLength
                    );
                }
            }
        }

        private void Update() {
            SaftyCheck();

            if (transform.hasChanged || lutNeedUpdate) {
                targetCam.cullingMask = 1 << this.gameObject.layer;
                UpdateLUT(4);//UpdateLUT(2); UpdateLUT(3);
                transform.hasChanged = false;
                lutNeedUpdate = false;
            }
        }

        private void OnValidate() {
            lutNeedUpdate = true;
            manualUpdate = false;
        }
        #endregion

    }
}