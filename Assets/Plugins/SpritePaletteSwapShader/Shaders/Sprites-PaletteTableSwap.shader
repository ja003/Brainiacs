/**
 * Unity palette swap sprite shader.
 * Swaps colors from a texture containing a palette table of colors (_PaletteTableTex property) for a palette row (_PaletteSwapIndex property).
 * The table first row (from bottom to top) is the target palette.
 * The core CGPROGRAM was based on the 2019.1.0f2 Sprites/Default shader.
*/
Shader "Sprites/Palette Table Swap"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		[NoScaleOffset] _PaletteTableTex("Palette Table Texture", 2D) = "white" {}
		_PaletteSwapIndex("Palette Swap Index", Int) = 1
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
				#pragma fragment PaletteTableSwapFrag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_local _ PIXELSNAP_ON
				#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
				#include "UnitySprites.cginc"
				#include "UnityPaletteSwap.cginc"

				sampler2D	_PaletteTableTex;
				half4		_PaletteTableTex_TexelSize;
				int			_PaletteSwapIndex;

				half4 PaletteTableSwapFrag(v2f IN) : SV_Target
				{
					half4 c = SampleSpriteTexture(IN.texcoord);
					c.rgb *= c.a;
					c = SwapPixelTable(c, _PaletteTableTex, _PaletteTableTex_TexelSize, _PaletteSwapIndex);
					c.rgb *= IN.color;
					return c;
				}
			ENDCG
		}
		}
}