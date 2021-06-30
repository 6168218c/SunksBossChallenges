sampler uImage0 : register(s0);

float3 uColor;
float3 colorLerp(float3 c1, float3 c2, float f)
{
	return float3(lerp(c1.r, c2.r, f), lerp(c1.g, c2.g, f), lerp(c1.b, c2.b, f));
	//return lerp(c1.rgb,c2.rgb,f);
}
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0 
{
	float4 c = tex2D(uImage0,coords);
	float lumiosity = (c.r + c.g + c.b) / 3;
	float3 c1;
	if (lumiosity < 0.5)
	{
		c1 = colorLerp(float3(0, 0, 0), uColor, lumiosity * 2);
	}
	else
	{
		c1 = colorLerp(uColor, float3(1, 1, 1), 1 - lumiosity * 2);
	}
	return float4(c1.rgb,c.a);
}




technique Technique1 
{
	pass Colorlize 
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}