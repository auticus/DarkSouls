using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkSouls.UI
{
    /// <summary>
    /// Class that sits on the UI Canvas Popup Alert parent game object and manages pop up messages to the user.
    /// </summary>
    public class PopUpUI : MonoBehaviour
    {
        /// <summary>
        /// Gets the TextBar game Object.
        /// </summary>
        [Header("Owning Game Objects")] [field: SerializeField]
        private GameObject TextBar;

        /// <summary>
        /// Gets the game object that houses the image.
        /// </summary>
        [field: SerializeField] 
        private GameObject ImageObject;
        
        /// <summary>
        /// Gets or sets the PopupText component that will display text to the user.
        /// </summary>
        [field: SerializeField]
        [Tooltip("The Popup that will display to the user.")]
        private TMP_Text PopupText { get; set; }

        /// <summary>
        /// Gets or sets the image that will be displayed on image popup.
        /// </summary>
        [field: SerializeField]
        private Image PopupImage { get; set; }

        /// <summary>
        /// Gets or sets the text that will accompany an image popup.
        /// </summary>
        [field: SerializeField]
        private TMP_Text PopupImageText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the image pop up component is currently displayed.
        /// </summary>
        [field: SerializeField]
        public bool IsImagePopUpDisplayed { get; set; }

        /// <summary>
        /// Displays the pop-up text component.
        /// </summary>
        /// <param name="text">The text to display.</param>
        public void ShowPopUpText(string text)
        {
            PopupText.text = text;
            TextBar.SetActive(true);
        }

        /// <summary>
        /// Hides the pop-up text component.
        /// </summary>
        public void HidePopUpText()
        {
            TextBar.SetActive(false);
        }

        /// <summary>
        /// Displays the image pop-up component.
        /// </summary>
        /// <param name="image">The image to display.</param>
        /// <param name="text">The text to display.</param>
        public void ShowImagePopUp(Sprite image, string text)
        {
            ImageObject.SetActive(true);
            IsImagePopUpDisplayed = true;

            PopupImageText.text = text;
            PopupImage.sprite = image;
        }

        /// <summary>
        /// Hides the image pop-up component.
        /// </summary>
        public void HideImagePopUp()
        {
            ImageObject.SetActive(false);
            IsImagePopUpDisplayed = false;
        }
    }
}
