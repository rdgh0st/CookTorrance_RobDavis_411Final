float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float3 CameraPosition;
float3 LightPosition;
float Shininess;
float Roughness; // clamp from 0-1
float4 AmbientColor;
float AmbientIntensity;
float4 SpecularColor;
float4 DiffuseColor;
float DiffuseIntensity;
texture decalMap;

sampler tsampler1 = sampler_state {
	texture = <decalMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

struct VertexShaderInput {
    float4 Position: POSITION0;
    float4 Normal: NORMAL0;
    float2 TexCoord: TEXCOORD0;
};

struct VertexShaderOutput {
    float2 TexCoord: TEXCOORD0;
    float4 Position: POSITION0;
	float4 Color: COLOR;
	float4 Normal: NORMAL0;
	float4 WorldPosition: POSITION1;
};

VertexShaderOutput CookTorranceVS(VertexShaderInput input) {
    VertexShaderOutput output;

	float4 worldPos = mul(input.Position, World);
	float4 viewPos = mul(worldPos, View);
	output.Position = mul(viewPos, Projection);
	output.WorldPosition = worldPos;
	output.Normal = mul(input.Normal, WorldInverseTranspose);
	output.Color = 0;
    output.TexCoord = input.TexCoord;

	return output;
}

float4 CookTorrancePS(VertexShaderOutput input) : COLOR {
    
    // potentially get bump normal and UV colors
    float3 N = normalize(input.Normal.xyz);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 L = normalize(LightPosition);
	float3 R = reflect(-L, N);
	float3 H = normalize(L + V);
    // float3 specular intensity is the cook torrance formula, multiply it with specular color
    float denom = 4 * dot(N, L) * dot(N, V);

    // GGXDistribution
    float alphaSquare = pow(input.Color.a, 2);
    float pi = 3.14159265358979323846;
    float denomGGX = pi * pow((pow(dot(N, H), 2) * (alphaSquare - 1)) + 1, 2);
    float GGXDistribution = alphaSquare / denomGGX;

    // FresnelSchlick
    float M = pow(min(0, max(1, 1 - dot(L, H))), 5);
    float3 FresnelSchlick = SpecularColor.rgb + (1 - SpecularColor.rgb) * M;

    // Geometry
    float K = pow((Roughness + 1), 2) / 8;
    float denomG1L = (dot(N, L) * (1 - K)) + K;
    float G1L = dot(N, L) / denomG1L;

    float denomG1V = (dot(N, V) * (1 - K)) + K;
    float G1V = dot(N, V) / denomG1V;

    float Geometry = G1L * G1V;

    float3 numer = GGXDistribution * FresnelSchlick * Geometry;
    float3 specular = numer / denom;
    specular = specular * SpecularColor;

    float4 texColor = tex2D(tsampler1, input.TexCoord);
	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));
    float4 color = float4(texColor * ambient + diffuse + specular, 1);
    return color; 
    /*
    float4 t = tex2D(tsampler1, input.TexCoord);
    float vh = dot(V, H);
    float nh = dot(N, H);
    float nl = dot(N, L);
    float nv = dot(N, V);
    float4 ambi = AmbientColor * AmbientIntensity;
    float4 diff = DiffuseColor * DiffuseIntensity * saturate(nl);
    if (nh <= 0 || nl <= 0)
        return t * ambi + diff;
    float F = _f0 + (1 - _f0) * (1 - pow(vh, 5));
    float D = exp((nh * nh - 1) / (_m * _m * nh * nh)) / (_m * _m * pow(nh, 4));
    float G = min(1, min(2 * nh * nl / vh, 2 * nh * nv / vh));
    float R = F * D * G / (nv * nl);
    float spec = _SpecColor * _Sk * R * nl;
    return t * ambi + diff + spec;
    */
}

technique CookTorrance {
	pass Pass1 {
		VertexShader = compile vs_4_0 CookTorranceVS();
		PixelShader = compile ps_4_0 CookTorrancePS();
	}
}