// Splatmap shader for mesh terrain (not for unity terrain)
Shader "UnityLibrary/Terrain/Splatmask4Textures" {
	Properties{
		_MainTex1("Texture1", 2D) = "white" {}
		_MainTex1Normal("Normal1", 2D) = "bump" {}
		_MainTex1Specular("Specular1", 2D) = "white" {}
		_MainTex1Smooth("Smooth1", 2D) = "white" {}
		_MainTex1AO("AO1", 2D) = "white" {}
		_MainTex2("Texture2", 2D) = "white" {}
		_MainTex2Normal("Normal2", 2D) = "bump" {}
		_MainTex2Specular("Specular2", 2D) = "white" {}
		_MainTex2Smooth("Smooth1", 2D) = "white" {}
		_MainTex2AO("AO2", 2D) = "white" {}
		_MainTex3("Texture3", 2D) = "white" {}
		_MainTex3Normal("Normal3", 2D) = "bump" {}
		_MainTex4("Texture4", 2D) = "white" {}
		_MainTex4Normal("Normal4", 2D) = "bump" {}
		_Mask("SplatMask (RGBA)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_SpecularColor("SpecularColor", Color) = (1,1,1,1)
		_MainTex2LayerMask("LayerMask1", 2D) = "black"{}
		_MainTex2LayerTex("LayerTex1", 2D) = "black"{}
		_MainTex2LayerNormal("LayerNormal1", 2D) = "black"{}
		_MainTex2LayerSmooth("LayerSmooth1", 2D) = "black"{}
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf StandardSpecular fullforwardshadows
			#pragma target 3.0

			sampler2D _MainTex1;
			sampler2D _MainTex2;
			//sampler2D _MainTex3;
			//sampler2D _MainTex4;
			sampler2D _MainTex1Normal;
			sampler2D _MainTex2Normal;
			//sampler2D _MainTex3Normal;
			//sampler2D _MainTex4Normal;
			sampler2D _MainTex1Specular;
			//sampler2D _MainTex2Specular;
			sampler2D _MainTex1Smooth;
			sampler2D _MainTex2Smooth;
			sampler2D _MainTex1AO;
			sampler2D _MainTex2AO;
			sampler2D _MainTex2LayerMask;
			sampler2D _MainTex2LayerTex; 
			sampler2D _MainTex2LayerNormal;
			sampler2D _MainTex2LayerSmooth;
			sampler2D _Mask;

			half _Glossiness;
			float3 _SpecularColor;

			struct Input {
				float2 uv_MainTex1;
				float2 uv_MainTex2LayerMask;
				float2 uv_MainTex2LayerTex;
				float2 uv_Mask;
			};

			float rand2(float2 coords) {
				return frac(sin(dot(coords, float2(12.9898, 78.233))) * 43758.5453);
			}

			void surf(Input i, inout SurfaceOutputStandardSpecular o)
			{

				// mix colors using mask
				fixed3 color1 = tex2D(_MainTex1, i.uv_MainTex1.xy).rgb;
				fixed3 color2 = tex2D(_MainTex2, i.uv_MainTex1.xy).rgb;
				fixed3 color2layer = tex2D(_MainTex2LayerTex, i.uv_MainTex2LayerTex.xy).rgb;

				fixed4 mask = tex2D(_Mask, i.uv_Mask.xy);
				fixed4 layermask = tex2D(_MainTex2LayerMask, i.uv_MainTex2LayerMask.xy);

				color2 = lerp(color2, color2layer, layermask.a);

				fixed3 c = color1 * mask.r + color2 * mask.g;

				// normals
				fixed3 normal1 = UnpackNormal(tex2D(_MainTex1Normal, i.uv_MainTex1.xy));
				fixed3 normal2 = UnpackNormal(tex2D(_MainTex2Normal, i.uv_MainTex1.xy));
				fixed3 normal2layer = tex2D(_MainTex2LayerNormal, i.uv_MainTex2LayerTex.xy).rgb;

				normal2 = lerp(normal2, normal2layer, layermask.a);

				fixed3 n = normal1 * mask.r + normal2 * mask.g;

				// speculars
				fixed3 spec1 = tex2D(_MainTex1Specular, i.uv_MainTex1.xy).rgb;
				//fixed3 spec2 = tex2D(_MainTex1Specular, i.uv_MainTex1.xy).rgb;

				fixed3 sp = spec1 * mask.r + spec1 * mask.g;

				//smooths

				fixed3 smooth1 = tex2D(_MainTex1Smooth, i.uv_MainTex1.xy).rgb;
				fixed3 smooth2 = tex2D(_MainTex2Smooth, i.uv_MainTex1.xy).rgb;
				fixed3 smooth2layer = tex2D(_MainTex2LayerSmooth, i.uv_MainTex2LayerTex.xy).rgb;

				smooth2 = lerp(smooth2, smooth2layer, layermask.a);

				half sm = smooth1 * mask.r + smooth2 * mask.g;


				//aos

				fixed3 ao1 = tex2D(_MainTex1AO, i.uv_MainTex1.xy).rgb;
				fixed3 ao2 = tex2D(_MainTex2AO, i.uv_MainTex1.xy).rgb;

				half ao = ao1 * mask.r + ao2 * mask.g;

				// output
				o.Albedo = c;
				o.Normal = n;
				o.Alpha = 1;
				o.Smoothness = sm;
				o.Specular = sp;
				o.Occlusion = ao;
			}
			ENDCG
	}
		FallBack "Diffuse"
}