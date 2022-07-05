using UnityEngine;

namespace DarkSouls.Inventory
{
    /// <summary>
    /// Base class for all interactable items that the player can interact with.
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        public const string INTERACTIVE_TAG = "Interactable";

        public float DebugGizmoRadius = 0.5f;

        /// <summary>
        /// The text that should be displayed when interacting with the item.
        /// </summary>
        public string InteractionText;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, DebugGizmoRadius);
        }

        public virtual void Interact(PlayerController controller)
        {}
    }
}
