fixed Round4Digits(fixed v)
{
	return trunc(v * 100.0) / 100.0;
}

fixed4 SwapPixel(fixed4 color, sampler2D paletteTex, sampler2D swapTex, fixed swapTexelWidth)
{	
	fixed pixelWidth = Round4Digits(swapTexelWidth);
	for (fixed x = 0.0; x < 1.0; x += pixelWidth)
	{
		float2 pixelPosition = float2(x, 0);
		half4 palettePixel = tex2D(paletteTex, pixelPosition);
		int colorInterpolation = clamp(distance(color, palettePixel) * 100, 0, 1);
		color = lerp(tex2D(swapTex, pixelPosition), color, colorInterpolation);
	}
	return color;
}

fixed4 SwapPixelTable(fixed4 color, sampler2D paletteTableTex, half4 paletteTexel, int row)
{
	// paletteTexel.x =  1.0/width of texture paletteTableTex
	// paletteTexel.y =  1.0/height of texture paletteTableTex
	fixed pixelWidth = Round4Digits(paletteTexel.x);
	fixed pixelHeight = Round4Digits(paletteTexel.y);
	fixed y = (row + 1) * pixelHeight;
	for (fixed x = 0.0; x < 1.0; x += pixelWidth)
	{
		// original palette is always at table row 0
		half4 palettePixel = tex2D(paletteTableTex, float2(x, 0));
		int colorInterpolation = clamp(distance(color, palettePixel) * 100, 0, 1);
		color = lerp(tex2D(paletteTableTex, float2(x, y)), color, colorInterpolation);
	}
	return color;
}

