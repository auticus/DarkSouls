using System;
using DarkSouls.Inventory;
using UnityEngine;

namespace DarkSouls.Characters
{
    /// <summary>
    /// Class that will probe for what objects the character can see.
    /// </summary>
    [Serializable]
    public class Eyes
    {
        public const Interactable NO_INTERACTABLE_FOUND = null;

        /// <summary>
        /// Gets or sets the radius that the character can scan for interactables in front of them.
        /// </summary>
        [field: SerializeField]
        [Tooltip("The radius that the character can scan for interactables in front of them")]
        public float InteractableScanRadius { get; set; } = 0.3f;

        /// <summary>
        /// Gets or sets the distance that the character can scan for interactables in front of them.
        /// </summary>
        [field: SerializeField]
        [Tooltip("The distance that the character can scan for interactables in front of them")]
        public float InteractableScanDistance { get; set; } = 1.0f;

        private PlayerController _controller;

        public Eyes(PlayerController controller)
        {
            _controller = controller;
        }

        /// <summary>
        /// Scans the area around the character for any Interactable items and returns it back if it finds one.
        /// </summary>
        /// <returns></returns>
        public Interactable ScanAreaInFrontOfMeForInteractable()
        {
            // todo: you can add layer mask here (the gravity class is ignoring layers 8 and 11, but currently not sure why so not doing here)
            if (Physics.SphereCast(
                    _controller.transform.position, 
                    InteractableScanRadius,
                    _controller.transform.forward,
                    out var hit,
                    InteractableScanDistance))
            {
                if (hit.collider.CompareTag(Interactable.INTERACTIVE_TAG))
                {
                    var interactable = hit.collider.GetComponent<Interactable>();
                    return interactable;
                }
            }

            return NO_INTERACTABLE_FOUND;
        }
    }
}
