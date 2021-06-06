sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float4 Main(float4 position : SV_POSITION, float2 coords : TEXCOORD0) : COLOR0
{
    float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 centreCoords = (coords - targetCoords) * (uScreenResolution / uScreenResolution.y);
	float length = sqrt(dot(centreCoords, centreCoords));
    float2 sampleCoords = coords;
	
    if (length < uIntensity && length != 0) {
        sampleCoords = sampleCoords + (((uIntensity / (length * 10)) * uOpacity / uScreenResolution) * centreCoords);
    }

    return tex2D(uImage0, sampleCoords);
}

technique Technique1
{
    pass BlackHoleDistort
    {
        PixelShader = compile ps_2_0 Main();
    }

}
