using UnityEngine;

namespace ActionCode.SpriteEffects
{
    /// <summary>
    /// Component used to controll the properties of Palette Table Swap shader.
    /// </summary>
    public class PaletteTableSwapController : AbstractShaderSwapController
    {
        public Texture2D paletteTable;

        private readonly int PALETTE_TABLE_TEX_ID = Shader.PropertyToID("_PaletteTableTex");
        private readonly int PALETTE_SWAP_INDEX_ID = Shader.PropertyToID("_PaletteSwapIndex");

        protected override void Start()
        {
            base.Start();
            UpdatePaletteTable();
        }

        /// <summary>
        /// Updates the palette table texture on the material.
        /// </summary>
        public void UpdatePaletteTable()
        {
            if (paletteTable)
            {
                material.SetTexture(PALETTE_TABLE_TEX_ID, paletteTable);
            }
        }

        /// <summary>
        /// Sets and updates the palette table texture on the material.
        /// </summary>
        /// <param name="paletteTable">The new palette table texture. The first row from this texture should be the original palette.</param>
        public void UpdateOriginalPalette(Texture2D paletteTable)
        {
            this.paletteTable = paletteTable;
            UpdatePaletteTable();
        }

        /// <summary>
        /// Updates the swap palette on the material based on the given index.
        /// </summary>
        /// <param name="index">The new swap palette index.</param>
        public void UpdateSwapPalette(int index)
        {
            material.SetInt(PALETTE_SWAP_INDEX_ID, index);
        }

        /// <summary>
        /// Sets the palette index using the given index.
        /// </summary>
        /// <param name="index">The index to swap.</param>
        public override void SetPalette(int index)
        {
            UpdateSwapPalette(index);
        }

        /// <summary>
        /// Swaps to the next palette.
        /// </summary>
        public override void NextPalette()
        {
            int index = material.GetInt(PALETTE_SWAP_INDEX_ID) + 1;
            SetPalette(index);
        }

        /// <summary>
        /// Swaps to the previous palette.
        /// </summary>
        public override void PreviousPalette()
        {
            int index = Mathf.Abs(material.GetInt(PALETTE_SWAP_INDEX_ID) - 1);
            SetPalette(index);
        }

        /// <summary>
        /// Swaps to the original palette.
        /// </summary>
        public override void OriginalPalette()
        {
            SetPalette(0);
        }

        protected override string[] GetShaderNames()
        {
            return new string[1] { "Sprites/Palette Table Swap" };
        }
    }
}