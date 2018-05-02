  Shader "A"
  {
        Properties
        {
            _MainColor ("MainColor", Color) = (1,1,1,0)
            _MainTex ("MainTex", 2D) = "white" {}
            _EdgeTex ("EdgeTex", 2D) = "white" {}
            _DataMap ("DataMap", 2D) = "white" {}
            _NormalMap ("NormalMap", 2D) = "white" {}
            _TemperatureColorMap ("TemperatureColorMap", 2D) = "white" {}
            
            _Tessellate ("Tessellate", Float) = 16
            _CutOff("CutOff", Float) = 0.0001
            _EdgeOff("EdgeOff", Float) = 0.0006
			_EdgeFusio("EdgeFusio", Float) = 3
            _SpecularRange ("SpecularRange", Range(1,100)) = 60
            _Specular ("Specular", Range(0,1)) = 0.6
        }
        SubShader
        {
            Tags {"Queue" = "Transparent" "RenderType"="Transparent"}
            //LOD 300
        	//ZWrite On
        	//ZTest Less
        	Blend SrcAlpha OneMinusSrcAlpha
        	//Cull off
        	
            CGPROGRAM
            #pragma surface surf SimpleSpecular addshadow fullforwardshadows vertex:disp tessellate:tess nolightmap
            #pragma target 5.0

            struct appdata
            {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };
            struct Input
            {
                float2 uv_MainTex;
                float2 uv_EdgeTex;
                float2 uv_DataMap;
                float2 uv_NormalMap;
            };

            float _Tessellate;
            float _SpecularRange;
            float _Specular;
			float _CutOff;
			float _EdgeOff;
			float _EdgeFusio;

            sampler2D _MainTex;
            sampler2D _EdgeTex;
            sampler2D _DataMap;
            sampler2D _NormalMap;
            sampler2D _TemperatureColorMap;
			fixed4 _MainColor;

            float4 tess()
            {
                return _Tessellate;
            }
            
		    half4 LightingSimpleSpecular (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		    {
				half3 h = normalize (lightDir + viewDir);
				half diff = max (0, dot (s.Normal, lightDir));
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, _SpecularRange);

				half4 c;
				c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec * _Specular * s.Specular) * (atten * 2);
				c.a = s.Alpha;

				return c;
		    }

            void disp (inout appdata v)
            {
                float4 dm = tex2Dlod (_DataMap, float4(v.texcoord.xy,0,0));
                v.vertex.xyz += v.normal * dm.r;
            }

            void surf (Input IN, inout SurfaceOutput o)
            {
                float4 mt = tex2D (_MainTex, IN.uv_MainTex);
                float4 et = tex2D (_EdgeTex, IN.uv_EdgeTex);
                float4 dm = tex2D (_DataMap, IN.uv_DataMap);
                float4 nm = tex2D (_NormalMap, IN.uv_NormalMap);
                float4 tcm = tex2D (_TemperatureColorMap, float2(clamp(0.01, 0.99, dm.g), 0.5));
                
				if(dm.g > 0.1)
				{
					//反射
            		o.Albedo = (mt.rgb*(1-clamp(0, 1, dm.g)) + tcm.rgb*clamp(0, 1, dm.g)) * _MainColor.rgb;
					//透明
                	o.Alpha = 1 - clamp(0.7, 1, dm.g) + 0.7;
					//高光
					o.Specular = 1;
				}
                else if(dm.r<_CutOff)
				{
					discard;
				}
				else if(dm.r<_EdgeOff)
				{
					float vl = (_EdgeOff - _CutOff)/_EdgeFusio;
					float v = 0;
					// 向上融合
					if(_EdgeOff - dm.r < vl)
					{
						v = (_EdgeOff - dm.r) / vl;
            			//反射
            			o.Albedo = (et.rgb*v + mt.rgb*(1-v)) * _MainColor.rgb;
						//透明
                		o.Alpha = 1;
						//高光
						o.Specular = 1-v;
					}
					// 向下融合
					else if(dm.r - _CutOff < vl)
					{
						v = (dm.r - _CutOff) / vl;
            			//反射
            			o.Albedo = et.rgb * _MainColor.rgb;
						//透明
                		o.Alpha = v;
						//高光
						o.Specular = 0;
					}
					else
					{
            			//反射
						o.Albedo = et.rgb * _MainColor.rgb;
						//透明
                		o.Alpha = 1;
						//高光
						o.Specular = 0;
					}
				}
				else
				{
					//反射
            		o.Albedo = mt.rgb * _MainColor.rgb;
					//透明
                	o.Alpha = 1;
					//高光
					o.Specular = 1;
				}

          		//法线
                o.Normal = nm.rgb;
                
                //自发光
    			o.Emission = 0.1;
            }
            ENDCG
        }
        FallBack "Diffuse"
    }