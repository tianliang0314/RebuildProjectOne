// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "B"
{
    Properties
    {
        _Height ("Height", Float) = 1
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _DataMap ("DataMap", 2D) = "gray" {}
        _NormalMap ("NormalMap", 2D) = "bump" {}
        _TemperatureColorMap ("TemperatureColorMap", 2D) = "bump" {}
    }
    Category
    {
        SubShader
        {
            Tags {"Queue"="Transparent" "RenderType"="Transparent"}// "IgnoreProjector"="True" "ForceNoShadowCasting"="True"}
       		//LOD 300
            pass
            {
            		//Name "PassName"
            		Tags {"LightMode"="ForwardBase"}//"RequireOptions "=""}
            		
		        	ZWrite On//On | Off
		        	ZTest LEqual//(Less | Greater | LEqual | GEqual | Equal | NotEqual | Always)
		        	Blend SrcAlpha OneMinusSrcAlpha// SourceBlendMode DestBlendMode
		        	Cull Back//Back | Front | Off
		        	Lighting On//On | Off
//		        	AlphaTest (Less | Greater | LEqual | GEqual | Equal | NotEqual | Always) CutoffValue
//		        	Color Color-value
//		        	ColorMask RGB | A | 0 | any combination of R, G, B, A
//		        	Offset OffsetFactor, OffsetUnits
//		        	SeparateSpecular On | Off
//		        	ColorMaterial AmbientAndDiffuse | Emission
//		        	Fog
//		        	{
//		        		Mode Off | Global | Linear | Exp | Exp2
//		        		Color ColorValue
//		        		Density FloatValue
//		        		Range FloatValue, FloatValue
//		        	}
//					BindChannels
//					{
//						Bind "Vertex", vertex
//						Bind "texcoord", texcoord
//						Bind "Color", color
//					}
//					Stencil
//					{
//						Ref 2
//						Comp always
//						Pass replace
//						ZFail decrWrap
//					}

        	
	            CGPROGRAM
	            #pragma vertex vert
        		#pragma fragment frag
        		
        		#include "UnityCG.cginc"
        		#include "Lighting.cginc"
        		#include "AutoLight.cginc"

	            float _Height;
	            float4 _Color;

	            sampler2D _MainTex;
	            sampler2D _DataMap;
	            sampler2D _NormalMap;
	            sampler2D _TemperatureColorMap;

				struct vert_in
				{
				    float4 vertex : POSITION;
				    float4 tangent : TANGENT;
				    float3 normal : NORMAL;
				    float4 texcoord : TEXCOORD0;
				    float4 texcoord1 : TEXCOORD1;
				    float4 color : COLOR;
				};
				struct vert_frag
				{
					float4 position : SV_POSITION;
	                float4 texcoord : TEXCOORD0;
				    float4 posWorld : TEXCOORD1;  
				    float3 normalDir : TEXCOORD2;  
				    float3 lightDir : TEXCOORD3;  
				    float3 viewDir : TEXCOORD4;
				    float3 vertexLighting : TEXCOORD5;  
				    LIGHTING_COORDS(6, 7)

				};
				struct frag_out
				{
					float4 color : COLOR;
				};
				
				vert_frag vert(vert_in vi)
				{
					vert_frag vf;
					vf.position = UnityObjectToClipPos(vi.vertex);
					vf.posWorld = mul(unity_ObjectToWorld, vi.vertex);  
					vf.normalDir =  normalize(mul(float4(vi.normal, 0.0), unity_WorldToObject).xyz);  
					vf.lightDir = WorldSpaceLightDir(vi.vertex);  
					vf.viewDir = WorldSpaceViewDir(vi.vertex);  
					vf.vertexLighting = 0;
					vf.texcoord = vi.texcoord;
					
					// SH/ambient and vertex lights  
					#ifdef LIGHTMAP_OFF  
					float3 shLight = ShadeSH9 (float4(vi.normalDir, 1.0));  
					vf.vertexLighting = shLight;  
					#ifdef VERTEXLIGHT_ON
					float3 vertexLight = Shade4PointLights (  
					unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,  
					unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,  
					unity_4LightAtten0, vf.posWorld, vf.normalDir);  
					vf.vertexLighting += vertexLight;  
					#endif
					#endif
					
					// pass lighting information to pixel shader  
					TRANSFER_VERTEX_TO_FRAGMENT(vf);
					
					return vf;  


//					vert_frag vf;
//					
//					vf.position = mul(UNITY_MATRIX_MVP, vi.vertex);
//					
//	            	float d = tex2Dlod(_DataMap, float4(vi.texcoord.xy,0,0)).r * _Height;
//					vf.position.xyz += vi.normal * d;
//					
//					vf.texcoord = vi.texcoord;
//					
//					return vf;
				}
				frag_out frag(vert_frag vf)
				{
					frag_out fo;
					
					float3 normalDirection = normalize(vf.normalDir);   
					float3 viewDirection = normalize(_WorldSpaceCameraPos - vf.posWorld.xyz);  
					float3 lightDirection;  
					float attenuation;  
					
					if (0.0 == _WorldSpaceLightPos0.w) // directional light?  
					{  
						attenuation = 1.0; // no attenuation  
						lightDirection = normalize(_WorldSpaceLightPos0.xyz);  
					}   
					else // point or spot light  
					{  
						float3 vertexToLightSource =  _WorldSpaceLightPos0.xyz - vf.posWorld.xyz;
						float distance = length(vertexToLightSource);  
						attenuation = 1.0 / distance; // linear attenuation   
						lightDirection = normalize(vertexToLightSource);  
					}
					
					// LIGHT_ATTENUATION not only compute attenuation, but also shadow infos  
					// attenuation = LIGHT_ATTENUATION(input);  
					// Compare to directions computed from vertex  
					//              viewDirection = normalize(input.viewDir);  
					//              lightDirection = normalize(input.lightDir);  
					
					// Because SH lights contain ambient, we don't need to add it to the final result  
					float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.xyz;  
					
					float3 diffuseReflection = attenuation * _LightColor0.rgb * _Color.rgb * max(0.0, dot(normalDirection, lightDirection)) * 2;  
					
					float3 specularReflection;  
					if (dot(normalDirection, lightDirection) < 0.0)  // light source on the wrong side?  
					{  
						specularReflection = float3(0.0, 0.0, 0.0);  // no specular reflection  
					}  
					else // light source on the right side  
					{
						specularReflection = attenuation * _LightColor0.rgb * _Color.rgb * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), 255);  
					}

					fo.color =  float4(float3(vf.vertexLighting +  diffuseReflection + specularReflection + tex2D (_MainTex, vf.texcoord.xy)), 1.0);
					
					return fo;
				}
				
	            ENDCG
            }
            //Pass
            //{
			//}
        }
		//SubShader
		//{
		//}
    }
    FallBack "Diffuse"
}