Shader "MyShader/ToonDif"
{
    Properties//定义内容
    {
        _MainTex ("Texture", 2D) = "white" {}//纹理
        //_Color ("Color", Color) = (1,1,1,1)//颜色
        _ShadowRamp("ShadowRamp",2D) = "white"{}//对纹理采样，阶梯化漫反射值
        //浮点_ShadowOffset,控制阴影的效果,使兰伯特光照修改成可以用_ShadowOffset控制效果的类半兰伯特光照
        _ShadowOffset("ShadowOffset",Range(0,1))=0.4
    }
    SubShader
    {
        Tags { 
            "Lightmode"="ForwardBase"
            "RenderType"="Opaque" }
        LOD 100

        Pass//工序，先后处理
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile_fog
            #pragma multi_compile_fwdbase;

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata//输入
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;//法向量
            };

            struct v2f//输入
            {
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;//法向量
                float4 pos : SV_POSITION;
                UNITY_FOG_COORDS(1)
                SHADOW_COORDS(2)//阴影
            };

            sampler2D _MainTex;//纹理
            sampler2D _ShadowRamp;//阴影纹理
            fixed _ShadowOffset;
            float4 _MainTex_ST;
            //float4 _Color;//颜色

            v2f vert (appdata v)//输出，v2f是vertex to fragment，vertex给fragment传递的信息
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = normalize(mul(v.normal,(float3x3)unity_WorldToObject));//传递法向量
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o);//阴影  

                //o.normal = v.normal;//传递法向量

                return o;
            }

            fixed4 frag(v2f i) : SV_Target//输出，fixed4是RGBA，fixed是定点数
            {
                fixed shadow = SHADOW_ATTENUATION(i);

                fixed4 ambient = UNITY_LIGHTMODEL_AMBIENT;
                fixed3 wLight = normalize(UnityWorldSpaceLightDir(0).xyz);
                fixed a = saturate(dot(i.normal,wLight)* shadow * _ShadowOffset + _ShadowOffset);

                a = tex2D(_ShadowRamp,fixed2(a,0));//计算得到的漫反射值作为un坐标的u，也就是x，对这张纹理进行采样，阶梯化漫反射值
                fixed4 col = tex2D(_MainTex,i.uv)* a * _LightColor0;//输出颜色=贴图
                UNITY_APPLY_FOG(i.fogCoord, col);

                //fixed4 col = _Color;//输出颜色=_Color
                //反射光强度等于_worldSpaceLightPos0和i.normal的点乘，dot表示两个向量的点乘,_worldSpaceLightPos0是世界坐标系下的光源坐标，
                //float intensity = dot(_WorldSpaceLightPos0,i.normal);
                //intensity = smoothstep(0,0.01, intensity);
                //col *= intensity;//color乘以光线强度

                return col + ambient;
            }
            ENDCG
        }
    }
    Fallback"Diffuse"
}
