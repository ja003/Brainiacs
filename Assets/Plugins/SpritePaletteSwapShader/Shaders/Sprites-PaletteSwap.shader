/**
 * Unity palette swap sprite shader.
 * Swaps a texture containing a palette of colors (_PaletteTex property) for another palette (_SwapTex property).
 * The core CGPROGRAM was based on the 2019.1.0f2 Sprites/Default shader.
*/
Shader "Sprites/Palette Swap"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		[NoScaleOffset] _PaletteTex("Palette Texture", 2D) = "black" {}
		[NoScaleOffset] _SwapTex("Swap Texture", 2D) = "black" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex SpriteVert
			#pragma fragment PaletteSwapFrag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile_local _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"
			#include "UnityPaletteSwap.cginc"

			sampler2D _SwapTex;
			sampler2D _PaletteTex;
			half4 _SwapTex_TexelSize;

			half4 PaletteSwapFrag(v2f IN) : SV_Target
			{
				half4 c = SampleSpriteTexture(IN.texcoord);
				c.rgb *= c.a;
				// _SwapTex_TexelSize.x =  1.0/width of texture swapTex 
				c = SwapPixel(c, _PaletteTex, _SwapTex, _SwapTex_TexelSize.x);
				c.rgb *= IN.color;
				return c;
			}
			ENDCG
		}
	}
}
