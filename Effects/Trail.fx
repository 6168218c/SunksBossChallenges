sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
float4x4 uTransform;
float uTime;

struct VSInput
{
	float2 Pos : POSITION0;
	float3 TexcoordsAndAlpha : TEXCOORD0;
};

struct PSInput
{
	float4 Pos : SV_POSITION;
	float3 TexcoordsAndAlpha : TEXCOORD0;
};

PSInput VSMain(VSInput input)
{
	PSInput output;
	output.TexcoordsAndAlpha = input.TexcoordsAndAlpha;
	output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
	return output;
}
//originally from @yiyang233
float4 PSMain(PSInput input) : COLOR0
{
	float3 coord = input.TexcoordsAndAlpha;
	float4 c = tex2D(uImage0, float2(coord.x + uTime , coord.y));
	c += tex2D(uImage2, float2(1 - coord.x * 5, coord.y));
	c *= coord.z;
	c *= tex2D(uImage1, float2(c.r,0.5));//��ɫͼ
	return c * 1.2;
}

technique Technique1 {
	pass Trail {
		VertexShader = compile vs_2_0 VSMain();
		PixelShader = compile ps_2_0 PSMain();
	}
}