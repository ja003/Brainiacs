/**
 * Unity palette swap sprite shader.
 * Swaps colors from a texture containing a palette table of colors (_PaletteTableTex property) for a palette row (_PaletteSwapIndex property).
 * The table first row (from bottom to top) is the target palette.
 * The core CGPROGRAM was based on the 2019.1.0f2 Sprites/Diffuse shader.
*/
Shader "Sprites/Palette Table Swap Diffuse"
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

		CGPROGRAM
		#pragma surface PaletteTableSurfFrag Lambert vertex:vert nofog nolightmap nodynlightmap keepalpha noinstancing
		#pragma multi_compile_local _ PIXELSNAP_ON
		#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
		#include "UnitySprites.cginc"

		sampler2D	_PaletteTableTex;
		half4		_PaletteTableTex_TexelSize;
		int			_PaletteSwapIndex;

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

		void PaletteTableSurfFrag(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = SampleSpriteTexture(IN.uv_MainTex);
			c.rgb *= c.a;
			fixed x = 0;
			fixed i = _PaletteSwapIndex * _PaletteTableTex_TexelSize.y;

			// finds the swap color corresponding to the original palette texture (row 0)
			// _PaletteTableTex_TexelSize.z = width of texture _PaletteTableTex 
			// _PaletteTableTex_TexelSize.x =  1.0/width of texture _PaletteTableTex 
			// _PaletteTableTex_TexelSize.y =  1.0/height of texture _PaletteTableTex 
			while (x < _PaletteTableTex_TexelSize.z) {
				half4 palettePixel = tex2D(_PaletteTableTex, half2(x, 0));
				half3 delta = abs(c.rgb - palettePixel.rgb);
				c = (delta.r + delta.g + delta.b) < 0.001 ? tex2D(_PaletteTableTex, float2(x, i)) : c;
				x += _PaletteTableTex_TexelSize.x;
			}

			c.rgb *= IN.color;
			o.Albedo = c.rgb * c.a;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Transparent/VertexLit"
}