#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4 BorderColor;
float1 BorderWidth;

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 tcol = tex2D(SpriteTextureSampler, input.TextureCoordinates);
	
    if (tcol.a == 0)
    {
        if (tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0,              BorderWidth)).a   != 0 ||
            tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0,             -BorderWidth)).a   != 0 ||
            tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(BorderWidth,    0)).a             != 0 ||
            tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-BorderWidth,   0)).a             != 0 ||
            tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-BorderWidth,   BorderWidth)).a   != 0 ||
            tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-BorderWidth,  -BorderWidth)).a   != 0 ||
            tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(BorderWidth,    BorderWidth)).a   != 0 ||
            tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(BorderWidth,   -BorderWidth)).a   != 0) 
        {
            tcol = BorderColor;
        }
    }

	return tcol;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};