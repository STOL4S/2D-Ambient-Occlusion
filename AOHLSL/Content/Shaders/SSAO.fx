#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

int DISPLAY_MODE = 0;

float R_OFFSET = 0;

float SSAO_RADIUS = 0.003f;
float SSAO_STRENGTH = 1.5f;

Texture2D InputTexture : register(s0);
sampler2D InputSampler = sampler_state
{
    Texture = <InputTexture>;
};

Texture2D SSAO_Texture;
SamplerState SSAO_Sampler
{
    Texture = <SSAO_Sampler>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
    float2 TexCoords : TEXCOORD0;
};

float4 DepthMapPixelShader(VertexShaderOutput input) : COLOR
{
    float4 InputColor = tex2D(InputSampler, input.TexCoords);

    if (InputColor.a == 0)
    {
        return float4(0, 0, 0, 0);
    }
	else
    {
        return float4(R_OFFSET, input.TexCoords.y, 0, InputColor.a);
    }
}

technique DepthTechnique
{
	pass P0
	{
        PixelShader = compile PS_SHADERMODEL DepthMapPixelShader();
    }
};

float4 SSAOPixelShader(VertexShaderOutput input) : COLOR
{
    float Occlusion = 1.0f;
    
    //WORLD SPACE POSITION
    float4 D_Buffer = tex2D(InputSampler, input.TexCoords);

    //IGNORE BACKGROUND PIXEL
    if (D_Buffer.r == 0)
    {
        //IF YOU ARE AN ALPHA PIXEL (BACKGROUND)
        //CHECK SURROUNDING PIXELS IN THE FOLLOWING PATTERN:
        //*** SCAN THIS ROW
        //*&* SCAN THE LEFT AND RIGHT PIXEL HERE
        //XXX DO NOT SCAN THIS ROW!
        //ONLY CHECK NEXT TO THE PIXEL AND ABOVE IT TO PREVENT
        //AMBIENT OCCLUSION FROM BEING GENERATED ON TOP OF SURFACES
        
        for (int j = 0; j <= 2; j++)
        {
            float4 DD_Buffer = tex2D(InputSampler, float2(input.TexCoords.x, input.TexCoords.y - (j * SSAO_RADIUS)));
            
            float YDist = abs(D_Buffer.b - DD_Buffer.b) - 0.5;
            
            if (DD_Buffer.r > 0)
            {
                Occlusion -= (SSAO_STRENGTH / (4.0f * (j + 1))) * (1 - DD_Buffer.b);
            }
        }
    }
    else
    {
        //ASSUME YOU ARE A DARKER PIXEL ON POSITION BUFFER
        //YOU ARE LOOKING FOR LIGHTER PIXELS TO OCCLUDE YOU
        for (int j = -2; j <= 2; j++)
        {
            for (int k = -2; k <= 2; k++)
            {
                //IF PIXEL IS BEHIND NEIGHBORING PIXEL
                //AND NOT A BACKGROUND PIXEL
                float4 DD_Buffer = tex2D(InputSampler, float2(input.TexCoords.x + (j * SSAO_RADIUS), input.TexCoords.y + (k * SSAO_RADIUS)));

                //GET DISTANCE
                float Dist = abs(j + k);
                if (DD_Buffer.r != 0)
                {
                    if (D_Buffer.r < DD_Buffer.r)
                    {
                        Occlusion -= ((SSAO_STRENGTH) / 32.0f * (Dist / 1.5f)) * (0.5 - DD_Buffer.b);
                    }

                }
            }

        }

        //SCAN UNDER YOURSELF
        for (int j = 1; j <= 4; j++)
        {
            float4 DD_Buffer = tex2D(InputSampler, float2(input.TexCoords.x, input.TexCoords.y + (j * SSAO_RADIUS)));
            
            float YDist = abs(D_Buffer.b - DD_Buffer.b) - 0.5;
            
            if (DD_Buffer.r == 0)
            {
                Occlusion -= (SSAO_STRENGTH / (4.0f * (j + 1))) * (1 - D_Buffer.b);
            }
        }
    }
    
    if (Occlusion != 1.0f)
    {
        return float4(Occlusion, Occlusion, Occlusion, 1);
    }
    else
    {
        return float4(255, 255, 255, 0);
    }
}

technique SSAOTechnique
{
    pass P0
    {
        PixelShader = compile ps_4_0 SSAOPixelShader();
    }
};

float4 CompositePixelShader(VertexShaderOutput input) : COLOR
{
    float4 SSAO_Color = SSAO_Texture.Sample(SSAO_Sampler, input.TexCoords);
    float4 Scene_Color = tex2D(InputSampler, input.TexCoords);
    
    if (DISPLAY_MODE == 0)
        return (SSAO_Color) + (Scene_Color * 0.0000000001);
    else if (DISPLAY_MODE == 1)
        return (SSAO_Color) * (Scene_Color);
    else
        return 0;

}

technique CompositeTechnique
{
    pass P0
    {
        PixelShader = compile ps_4_0 CompositePixelShader();
    }
};