/**
 * Unity palette swap sprite shader.
 * Swaps a texture containing a palette of colors (_PaletteTex property) for another palette (_SwapTex property).
 * The core CGPROGRAM was based on the 2019.1.0f2 Sprites/Diffuse shader.
*/
Shader "Sprites/Palette Swap Diffuse"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		[NoScaleOffset] _PaletteTex("Palette Texture", 2D) = "white" {}
		[NoScaleOffset] _SwapTex("Swap Texture", 2D) = "white" {}
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

		CGPROGRAM
		#pragma surface PaletteSurfFrag Lambert vertex:vert nofog nolightmap nodynlightmap keepalpha noinstancing
		#pragma multi_compile_local _ PIXELSNAP_ON
		#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
		#include "UnitySprites.cginc"
		#include "UnityPaletteSwap.cginc"


		sampler2D _PaletteTex;
		sampler2D _SwapTex;
		half4 _SwapTex_TexelSize;

		struct Input
		{
			float2 uv_MainTex;
			fixed4 color;
		};

		void vert(inout appdata_full v, out Input o)
		{
			v.vertex = UnityFlipSprite(v.vertex, _Flip);

			#if defined(PIXELSNAP_ON)
			v.vertex = UnityPixelSnap(v.vertex);
			#endif

			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _Color * _RendererColor;
		}

		void PaletteSurfFrag(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = SampleSpriteTexture(IN.uv_MainTex);
			c.rgb *= c.a;
			// _SwapTex_TexelSize.x =  1.0/width of texture swapTex 
			c = SwapPixel(c, _PaletteTex, _SwapTex, _SwapTex_TexelSize.x);
			c.rgb *= IN.color;
			o.Albedo = c.rgb * c.a;
			o.Alpha = c.a;
		}
		ENDCG
	}

	Fallback "Transparent/VertexLit"
}