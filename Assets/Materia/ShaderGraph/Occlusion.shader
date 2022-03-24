Shader "Unlit/Occlusion"
{
	Properties
	{
		[NoScaleoffset] _MainTex("Texture", 2D) = "white" {}
		_maskColor("MaskColor",Color) = (0,1,0)\
	}

		SubShader
		{
			Tags { "RenderType" = "Opaque" "Queue" = "Geometry+100"  }

			Pass
			{
				Blend SrcAlpha One
				ZTest Greater
				ZWrite Off

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#pragma multi_compile_fog
				#include "UnityCG.cginc"

				struct MeshData
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float3 normal : NORMAL;
				};

				struct Interpolator
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
					float4 pos : TEXCOORD1;
					float3 normal : NORMAL;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _maskColor;

				Interpolator vert(MeshData v)
				{
					Interpolator o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.pos = v.vertex;
					o.normal = v.normal;
					return o;
				}

				fixed4 frag(Interpolator i) : SV_Target
				{
					fixed4 col = fixed4(0,0,0,0.1);
					float3 wpos = mul(unity_ObjectToWorld, i.pos).xyz;
					float3 viewDir = UnityWorldSpaceViewDir(wpos);
					viewDir = normalize(viewDir);

					float3 normal = UnityObjectToWorldNormal(i.normal);
					float flag = 1 - saturate(dot(viewDir,normal));
					float br = pow(flag, 2.0);
					col = lerp(col , _maskColor , br);
					return col;
				}
				ENDCG
			}

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _Alpha;

				struct MeshData
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct Interpolator
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				Interpolator vert(MeshData v)
				{
					Interpolator o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				float4 frag(Interpolator i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					return col;
				}
				ENDCG
			}
		}
}
