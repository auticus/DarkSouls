using System.Collections.Generic;
using DarkSouls.Inventory;
using UnityEngine;

namespace DarkSouls.Characters
{
    /// <summary>
    /// Class that looks for <see cref="Interactable"/> objects.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class InteractableEyes : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets a list of <see cref="Interactable"/> items that are in range.
        /// </summary>
        [field: SerializeField] public List<Interactable> Interactables { get; set; } = new();

        private const int PLAYER_EYES_LAYER = 9;
        private SphereCollider _sphereCollider;
        
        private void Awake()
        {
            _sphereCollider = GetComponent<SphereCollider>();
            gameObject.layer = PLAYER_EYES_LAYER;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Interactable target))
            {
                Interactables.Add(target);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Interactable target))
            {
                RemoveTarget(target);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (transform == null || _sphereCollider == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _sphereCollider.radius);
        }

        /// <summary>
        /// Removes a given target.
        /// </summary>
        /// <param name="target">The target to remove.</param>
        public void RemoveTarget(Interactable target)
        {
            if (target == null) return;
            Interactables.Remove(target);
        }
    }
}
