using UnityEngine;

namespace ActionCode.SpriteEffects
{
    /// <summary>
    /// Input component used to swap palettes from a <see cref="AbstractShaderSwapController"/>.
    /// </summary>
    public class PaletteInputSwapper : MonoBehaviour
    {
        public AbstractShaderSwapController shaderController;
        [Header("Input")]
        public string nextPaletteButtonInput = "NextPalette";
        public string previousPaletteButtonInput = "PreviousPalette";
        public string originalPaletteButtonInput = "OriginalPalette";

        private void Awake()
        {
            FindShaderController();
        }

        private void Update()
        {
            if (shaderController) UpdateInput();
        }

        protected virtual void FindShaderController()
        {
            foreach (AbstractShaderSwapController controller in
                GetComponentsInChildren<AbstractShaderSwapController>())
            {
                if (controller.enabled)
                {
                    shaderController = controller;
                    break;
                }
            }
        }

        protected virtual void UpdateInput()
        {
			//if (Input.GetButtonDown(nextPaletteButtonInput) || Input.GetKeyDown(KeyCode.X)) shaderController.NextPalette();
			//else if (Input.GetButtonDown(previousPaletteButtonInput)) shaderController.PreviousPalette();
			//else if (Input.GetButtonDown(originalPaletteButtonInput)) shaderController.OriginalPalette();

			if(Input.GetKeyDown(KeyCode.X)) shaderController.NextPalette();
		}
    }
}