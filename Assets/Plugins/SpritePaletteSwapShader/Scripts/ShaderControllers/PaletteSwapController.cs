using UnityEngine;

namespace ActionCode.SpriteEffects
{
    /// <summary>
    /// Component used to controll the properties of Palette Swap shader.
    /// </summary>
    public class PaletteSwapController : AbstractShaderSwapController
    {
        public Texture2D originalPalette;
        public Texture2D[] swapPalettes;

        protected int currentSwapPaletteIndex = -1;

        private readonly int PALETTE_TEX_ID = Shader.PropertyToID("_PaletteTex");
        private readonly int SWAP_TEX_ID = Shader.PropertyToID("_SwapTex");

        protected override void Start()
        {
            base.Start();
            UpdateOriginalPalette();
        }

        /// <summary>
        /// Updates the original palette texture on the material.
        /// </summary>
        public void UpdateOriginalPalette()
        {
            if (originalPalette)
            {
                material.SetTexture(PALETTE_TEX_ID, originalPalette);
                OriginalPalette();
            }
        }

        /// <summary>
        /// Sets and updates the original palette texture on the material.
        /// </summary>
        /// <param name="originalPalette">The new original palette. This texture should have 1 pixel height.</param>
        public void UpdateOriginalPalette(Texture2D originalPalette)
        {
            this.originalPalette = originalPalette;
            UpdateOriginalPalette();
        }

        /// <summary>
        /// Updates the swap palette on the material.
        /// </summary>
        /// <param name="palette">The new swap palette. This texture should have 1 pixel height.</param>
        public void UpdateSwapPalette(Texture2D palette)
        {
            material.SetTexture(SWAP_TEX_ID, palette);
        }

        /// <summary>
        /// Sets the swap palette using <see cref="swapPalettes"/> given index.
        /// </summary>
        /// <param name="index">The index of <see cref="swapPalettes"/> to swap in.</param>
        public override void SetPalette(int index)
        {
            if (index < 0 || index >= swapPalettes.Length) return;

            currentSwapPaletteIndex = index;
            UpdateSwapPalette(swapPalettes[currentSwapPaletteIndex]);
        }

        /// <summary>
        /// Swaps to the next palette based on the <see cref="swapPalettes"/>
        /// </summary>
        public override void NextPalette()
        {
            int index = (currentSwapPaletteIndex + 1) % swapPalettes.Length;
            SetPalette(index);
        }

        /// <summary>
        /// Swaps to the previous palette based on the <see cref="swapPalettes"/>
        /// </summary>
        public override void PreviousPalette()
        {
            int index = Mathf.Abs(currentSwapPaletteIndex - 1) % swapPalettes.Length;
            SetPalette(index);
        }

        /// <summary>
        /// Swaps to the original palette.
        /// </summary>
        public override void OriginalPalette()
        {
            currentSwapPaletteIndex = -1;
            UpdateSwapPalette(originalPalette);
        }

        protected override string[] GetShaderNames()
        {
            return new string[1] { "Sprites/Palette Swap" };
        }
    }
}