using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DarkSouls.Combat;
using UnityEditor;
using UnityEngine;

namespace DarkSouls.Characters
{
    /// <summary>
    /// Class that looks for <see cref="Target"/> objects.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class TargetingEyes : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets a list of <see cref="Target"/> items that are in range.
        /// </summary>
        [field: SerializeField] public List<Target> Targets { get; set; } = new();

        /// <summary>
        /// Gets the target that the character is currently selected as their active target.
        /// </summary>
        public Target CurrentTarget { get; private set; }

        private const float DEFAULT_TARGETING_CAMERA_WEIGHT = 1f;
        private const float DEFAULT_TARGETING_CAMERA_RADIUS = 2f;
        private const int CONTROLLER_LAYER = 10;
        private const int PLAYER_EYES_LAYER = 9;
        private const int ENVIRONMENT_LAYER = 8;

        private Camera _mainCamera;
        private SphereCollider _sphereCollider;

        [field: SerializeField] private CinemachineTargetGroup targetingGroup;

        private void Awake()
        {
            _sphereCollider = GetComponent<SphereCollider>();
            _mainCamera = Camera.main;

            Physics.IgnoreLayerCollision(PLAYER_EYES_LAYER, CONTROLLER_LAYER);

            if (targetingGroup == null)
            {
                Debug.LogWarning("Targeting Eyes component does not have its TargetingGroup component set on the player character.");
            }
        }

        private void Update()
        {
            if (CurrentTarget == null) return;
            if (!CanEyesSeeCurrentTarget())
            {
                Debug.Log("Can no longer see my target so clearing...");
                ClearTarget();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Target target))
            {
                target.OnDestroyed += RemoveTarget;
                Targets.Add(target);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Target target))
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
        public void RemoveTarget(Target target)
        {
            if (target == null) return;
            if (CurrentTarget == target) ClearTarget();
            target.OnDestroyed -= RemoveTarget; 
            Targets.Remove(target);
        }

        /// <summary>
        /// Clears the <see cref="CurrentTarget"/>.
        /// </summary>
        public void ClearTarget()
        {
            if (CurrentTarget == null) return;
            targetingGroup.RemoveMember(CurrentTarget.transform);
            CurrentTarget = null;
        }

        /// <summary>
        /// Attempts to select the closest viable target.
        /// </summary>
        /// <returns>TRUE if able, FALSE otherwise.</returns>
        public bool TrySelectClosestTarget()
        {
            //we want to select the target closest to what we're aiming at
            //currently is just grabbing the first one.
            if (!Targets.Any()) return false;

            //cycle through each target and pull the target that is closest to center of screen
            Target bestPotentialTarget = null;
            var bestTargetPosition = Mathf.Infinity; //set it to the largest possible distance it can be

            foreach (var target in Targets)
            {
                //make sure x and y are between 0 & 1.  z doesn't matter because the range sphere on the player controller will keep things in a sane range
                var targetViewPosition = _mainCamera.WorldToViewportPoint(target.transform.position);
                /* the old way - the problem with this is if the camera is facing away but the character facing the target, it will target
                 * we want it to not be able to target things that we the player cannot see
                if (targetViewPosition.x is < 0 or > 1) continue;
                if (targetViewPosition.y is < 0 or > 1) continue;
                */

                //the alternative is to get a renderer on the target and if any is visible, thats fine
                if (!target.GetComponentInChildren<Renderer>().isVisible) continue;

                var distanceFromCenter = GetDistanceFromCenterScreen(targetViewPosition);
                if (distanceFromCenter.sqrMagnitude < bestTargetPosition)
                {
                    bestPotentialTarget = target;
                    bestTargetPosition = distanceFromCenter.sqrMagnitude;
                }
            }

            ClearTarget();
            CurrentTarget = bestPotentialTarget;
            if (CurrentTarget is null) return false;

            targetingGroup.AddMember(CurrentTarget.transform, DEFAULT_TARGETING_CAMERA_WEIGHT, DEFAULT_TARGETING_CAMERA_RADIUS);
            return true;
        }

        private Vector2 GetDistanceFromCenterScreen(Vector3 targetViewPosition)
        {
            const float CENTER_SCREEN = 0.5f;
            var xDistanceFromCenter = Math.Abs(targetViewPosition.x - CENTER_SCREEN);
            var yDistanceFromCenter = Math.Abs(targetViewPosition.y - CENTER_SCREEN);
            return new Vector2(xDistanceFromCenter, yDistanceFromCenter);
        }

        private bool CanEyesSeeCurrentTarget()
        {
            // targeting mode should always be facing the target but environment can get in the way and block line of sight
            // this method will see if we can still draw a line to that target
            if (Physics.Linecast(transform.position, CurrentTarget.transform.position, out var hitInfo))
            {
                Debug.DrawLine(transform.position, CurrentTarget.transform.position, Color.red);
                if (hitInfo.transform.gameObject.layer == ENVIRONMENT_LAYER)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
