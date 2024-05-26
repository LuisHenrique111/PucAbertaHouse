Shader "Example/URPUnlitShaderTessallated"
{
	Properties
	{
		_Tess("Tessellation", Range(1, 80)) = 20
		_MaxTessDistance("Max Tess Distance", Range(1, 300)) = 20
		_Noise("Noise", 2D) = "black" {}

		_Weight("Displacement Amount", Range(0, 1)) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

		Pass
		{
			Tags{ "LightMode" = "UniversalForward" }


			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"    
			#include "CustomTessellation.hlsl"

			#pragma hull hull
			#pragma domain domain
			#pragma vertex TessellationVertexProgram
			#pragma fragment frag

			sampler2D _Noise;
			float _Weight;

			ControlPoint TessellationVertexProgram(Attributes v)
			{
				ControlPoint p;

				p.vertex = v.vertex;
				p.uv = v.uv;
				p.normal = v.normal;
				p.color = v.color;

				return p;
			}

			Varyings vert(Attributes input)
			{
				Varyings output;
				
				float4 Noise = tex2Dlod(_Noise, float4(input.uv + (_Time.x * 0.1), 0, 0));

				input.vertex.xyz += normalize(input.normal) *  Noise.r * _Weight;
				output.vertex = TransformObjectToHClip(input.vertex.xyz);
				output.color = input.color;
				output.normal = input.normal;
				output.uv = input.uv;
				
				return output;
			}

			[UNITY_domain("tri")]
			Varyings domain(TessellationFactors factors, OutputPatch<ControlPoint, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
			{
				Attributes v;
				Interpolate(vertex)
				Interpolate(uv)
				Interpolate(color)
				Interpolate(normal)

				return vert(v);
			}

			half4 frag(Varyings IN) : SV_Target
			{
				float4 Noise = tex2D(_Noise, IN.uv + (_Time.x * 0.1));
				return Noise;
			}
			ENDHLSL
		}
	}
}