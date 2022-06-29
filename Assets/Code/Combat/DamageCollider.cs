using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkSouls.Combat
{
    /// <summary>
    /// Class that sits on a weapon collider and controls when it hits something.
    /// </summary>
    public class DamageCollider : MonoBehaviour
    {
        private Collider _collider;
        private readonly List<Collider> _alreadyHitColliders = new List<Collider>();

        public event Action<Target> OnTargetHit;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.gameObject.SetActive(true);
            _collider.isTrigger = true;
            _collider.enabled = false;
        }

        public void EnableDamageCollider()
        {
            _alreadyHitColliders.Clear();
            _collider.enabled = true;
        }

        public void DisableDamageCollider()
        {
            _collider.enabled = false;
        }
        
        private void OnTriggerEnter(Collider collision)
        {
            if (_alreadyHitColliders.Contains(collision)) return;
            
            _alreadyHitColliders.Add(collision);
            var target = collision.gameObject.GetComponent<Target>();
            if (target != null)
            {
                OnTargetHit?.Invoke(target);
            }
        }
    }
}
