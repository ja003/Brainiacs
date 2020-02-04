using UnityEngine;

namespace ActionCode.SpriteEffects
{
    /// <summary>
    /// Abstract component used to controll the swap shaders.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Renderer))]
    public abstract class AbstractShaderSwapController : MonoBehaviour
    {
        protected Material material;

        protected virtual void Reset()
        {
            SetupMaterial();
        }

        protected virtual void Start()
        {
            material = GetComponent<Renderer>().material;
        }

        /// <summary>
        /// Sets the palette using the given index.
        /// </summary>
        /// <param name="index">The index to swaps the palette.</param>
        public abstract void SetPalette(int index);

        /// <summary>
        /// Swaps to the next palette.
        /// </summary>
        public abstract void NextPalette();

        /// <summary>
        /// Swaps to the previous palette.
        /// </summary>
        public abstract void PreviousPalette();

        /// <summary>
        /// Swaps to the original palette.
        /// </summary>
        public abstract void OriginalPalette();

        protected abstract string[] GetShaderNames();

        private void SetupMaterial()
        {
            string[] shaderNames = GetShaderNames();
            Renderer renderer = GetComponent<Renderer>();
            Material material = renderer ? renderer.sharedMaterial : null;
            if (material)
            {
                foreach (string shaderName in shaderNames)
                {
                    if (material.shader.name.Equals(shaderName)) return;
                }

                if (shaderNames.Length > 0) material.shader = Shader.Find(shaderNames[0]);
            }
        }
    }
}