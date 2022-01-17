// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Custom/TestShader" {
    SubShader{
        Pass{
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            //通过结构体定义顶点着色器输入
            struct a2v{
                //POSTION语义：使用模型空间顶点坐标填充vertex变量
                float4 vertex : POSITION; 
                //NORMAL语义：使用模型空间法向量填充normal变量
                float3 normal : NORMAL; 
                //TEXCOORD0语义：使用模型第一套纹理坐标填充texcoord变量
                float4 texcoord : TEXCOORD0; 
            };

            struct v2f{
                //SV_POSTION语义：pos存储了裁剪空间中顶点位置信息
                float4 pos : SV_POSITION; 
                //COLOR0语义：color存储颜色信息
                float3 color : COLOR0;
            };


            //SV_POSITION语义：顶点着色器的输出作为裁剪空间顶点坐标
            v2f vert(a2v v){
                v2f o;
                //o.pos = UnityObjectToClipPos(v.vertex);
                o.pos = v.texcoord.xyww;
				o.pos.y *= _ProjectionParams.x;
				o.pos.x -= 0.5; o.pos.x *= 2;
				o.pos.y += 0.5; o.pos.y *= 2;

                //将法向量从[-1, 1]映射至[0, 1]，通过color传递给像素（片元）着色器
                o.color = v.normal * 0.5 + fixed3(0.5, 0.5, 0.5);
                return o;
            }

            //SV_Target语义：像素着色器的输出存储到一个渲染目标(RenderTarget)当中
            fixed4 frag(v2f i) : SV_Target{         
                return fixed4(i.color, 1.0);
            }
            ENDCG
        }
    }
}