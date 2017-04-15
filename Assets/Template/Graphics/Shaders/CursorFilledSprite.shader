// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:3000,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1873,x:33624,y:32716,varname:node_1873,prsc:2|emission-6-OUT,alpha-9766-OUT;n:type:ShaderForge.SFN_Color,id:5983,x:32487,y:32804,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_VertexColor,id:5376,x:32487,y:33011,varname:node_5376,prsc:2;n:type:ShaderForge.SFN_Multiply,id:603,x:32901,y:32931,cmnt:A,varname:node_603,prsc:2|A-5983-A,B-5376-A,C-9667-A;n:type:ShaderForge.SFN_TexCoord,id:1331,x:31808,y:33886,varname:node_1331,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_RemapRange,id:675,x:32065,y:33620,varname:node_675,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-1331-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:3591,x:31973,y:33374,varname:node_3591,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-675-OUT;n:type:ShaderForge.SFN_ArcTan2,id:782,x:32264,y:33323,varname:node_782,prsc:2,attp:2|A-3591-G,B-3591-R;n:type:ShaderForge.SFN_Slider,id:1375,x:31915,y:33221,ptovrint:False,ptlb:Progress,ptin:_Progress,varname:_Progress,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.726441,max:1;n:type:ShaderForge.SFN_Subtract,id:3349,x:32592,y:33196,varname:node_3349,prsc:2|A-8937-OUT,B-1375-OUT;n:type:ShaderForge.SFN_OneMinus,id:8937,x:32430,y:33286,varname:node_8937,prsc:2|IN-782-OUT;n:type:ShaderForge.SFN_Ceil,id:6058,x:32781,y:33172,varname:node_6058,prsc:2|IN-3349-OUT;n:type:ShaderForge.SFN_OneMinus,id:221,x:32992,y:33211,varname:node_221,prsc:2|IN-6058-OUT;n:type:ShaderForge.SFN_Tex2d,id:9667,x:32498,y:32547,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:True,tagnsco:False,tagnrm:False,tex:4ece8725a817d5f459d98efde394f47c,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:6,x:33066,y:32696,varname:node_6,prsc:2|A-9667-RGB,B-5983-RGB,C-5376-RGB;n:type:ShaderForge.SFN_Tex2d,id:2645,x:32894,y:33413,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:_Mask,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:49ed7dcbcfa509541b6de2aa14fe3f9d,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:943,x:33255,y:33255,varname:node_943,prsc:2|A-221-OUT,B-2645-R;n:type:ShaderForge.SFN_Multiply,id:9766,x:33225,y:32992,varname:node_9766,prsc:2|A-603-OUT,B-943-OUT;proporder:5983-1375-9667-2645;pass:END;sub:END;*/

Shader "Shader Forge/FilledSprite" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Progress ("Progress", Range(0, 1)) = 0.726441
        [PerRendererData]_MainTex ("MainTex", 2D) = "white" {}
        _Mask ("Mask", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent+3000"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float _Progress;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = (_MainTex_var.rgb*_Color.rgb*i.vertexColor.rgb);
                float3 finalColor = emissive;
                float2 node_3591 = (i.uv0*2.0+-1.0).rg;
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                return fixed4(finalColor,((_Color.a*i.vertexColor.a*_MainTex_var.a)*((1.0 - ceil(((1.0 - ((atan2(node_3591.g,node_3591.r)/6.28318530718)+0.5))-_Progress)))*_Mask_var.r)));
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
