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

        private PlayerController _controller;
        private Camera _mainCamera;
        private SphereCollider _sphereCollider;
        
        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _mainCamera = Camera.main;
            _sphereCollider = GetComponent<SphereCollider>();
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
