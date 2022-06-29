﻿using System;
using DarkSouls.Animation;
using DarkSouls.Inventory;
using DarkSouls.Combat;
using UnityEngine;

namespace DarkSouls.Characters
{
    /// <summary>
    /// Centralized class that is responsible for bridging the gap between game components and the non player entities.
    /// </summary>
    public class NonPlayerCharacterController : MonoBehaviour, ICharacterController
    {
        private AnimationHandler _animationHandler;
        private Animator _animator;
        private CharacterAttributes _characterAttributes;
        private CharacterInventory _characterInventory;
        private WeaponSocketController _weaponSocketController;

        private readonly int isInteractingHash = Animator.StringToHash("isInteracting");
        private bool _isInteracting;

        /// <inheritdoc/>
        public bool IsInteracting
        {
            get => _isInteracting;
            set
            {
                _animator.SetBool(isInteractingHash, value);
                _isInteracting = value;
            }
        }

        /// <inheritdoc/>
        public float AerialTimer { get; set; }

        /// <summary>
        /// Gets or set a value indicating that the player is rolling.
        /// </summary>
        public bool IsRolling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the player is back stepping.
        /// </summary>
        public bool IsBackStepping { get; set; }

        /// <inheritdoc/>
        public bool IsAerial { get; set; } //todo: why not just make this IsGrounded = false?

        /// <summary>
        /// Gets or sets a value indicating that the player is performing an attack with their right hand.
        /// </summary>
        public bool IsAttacking { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the player is performing a heavy attack with their right hand.
        /// </summary>
        public bool IsHeavyAttacking { get; set; }

        /// <inheritdoc/>
        public bool IsGrounded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the player has been hit by something and is reacting to it via animation.
        /// </summary>
        public bool IsImpacted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the player is sprinting.
        /// </summary>
        public bool IsSprinting { get; set; }

        /// <summary>
        /// Gets a value indicating if the player in his current animation state may rotate.
        /// </summary>
        /// <returns></returns>
        public bool CanRotate { get; set; } = true;

        /// <inheritdoc/>
        public Action OnInteractingAnimationCompleteDoThis { get; set; }

        // Start is called before the first frame update WHEN A SCRIPT IS ENABLED
        void Start()
        {
            _animationHandler = GetComponent<AnimationHandler>();
            _animator = GetComponentInChildren<Animator>();
            _characterAttributes = GetComponent<CharacterAttributes>();
            _characterInventory = GetComponent<CharacterInventory>();
            _weaponSocketController = GetComponent<WeaponSocketController>();

            InitializePlayer();
        }

        // Update is called once per frame
        void Update()
        {
            _isInteracting = _animator.GetBool(isInteractingHash);
        }

        void LateUpdate()
        {
            /*
             * Note in tutorial he sets all of the input handler flags explicitly to false but we let the input handler handle that and fire events accordingly
             */

            if (IsAerial)
            {
                //deviation: he has in air timer on the PlayerLocomotion component but I moved it here because locomotion doesn't care how long its in the air
                AerialTimer += Time.deltaTime;
            }
        }

        public void FinishInteractiveAnimation()
        {
            //initial game state will fire this off since it happens on Enter of Empty
            //some animations like falling just enter the enter state and do nothing with this action so it will also be null on that case
            if (OnInteractingAnimationCompleteDoThis == null) return;

            OnInteractingAnimationCompleteDoThis.Invoke();
            OnInteractingAnimationCompleteDoThis = null;
        }


        /// <inheritdoc/>
        public void DamageCharacter(int damage)
        {
            Debug.Log($"NPC has been damaged for {damage}");
            _characterAttributes.DamageCharacter(damage);
            if (_characterAttributes.CurrentHealth > 0)
            {
                RespondToImpactHit();
            }
            else
            {
                KillCharacter();
            }
        }

        public void EnableAttackingWeaponCollider()
        {
            _weaponSocketController.SetColliderEnabledForWeapon(GetActiveAttackingHand(), enabled: true);
        }

        public void DisableAttackingWeaponCollider()
        {
            _weaponSocketController.SetColliderEnabledForWeapon(GetActiveAttackingHand(), enabled: false);
        }

        private void InitializePlayer()
        {
            _weaponSocketController.LoadRightHandSocketWithWeapon(_characterInventory.RightHand);
            _weaponSocketController.LoadLeftHandSocketWithWeapon(_characterInventory.LeftHand);

            _weaponSocketController.OnRightHandHit += RightHandHit;
            _weaponSocketController.OnLeftHandHit += LeftHandHit;
        }

        private void RespondToImpactHit()
        {
            //todo this will always impact from the front, will need to figure out what weapons used, and direction of attack
            _animationHandler.PlayTargetAnimation(AnimationHandler.ONE_HANDED_IMPACT_FRONT_STEPBACK_01, isInteractingAnimation: true);
            IsImpacted = true;
            OnInteractingAnimationCompleteDoThis = () =>
            {
                IsImpacted = false;
                _animationHandler.FinishInteractionAnimation();
            };
        }

        private void KillCharacter()
        {
            //todo this always will run death01 with one hand.  Will need to figure out weapons and direction of attack
            //and even figure out if you can mix ragdoll with the animation
            _animationHandler.PlayTargetAnimation(AnimationHandler.ONE_HANDED_DEATH_01, isInteractingAnimation: true);
        }

        private void RightHandHit(Target target)
        {
            target.HitTarget(_characterInventory.RightHand.BaseDamage);
        }

        private void LeftHandHit(Target target)
        {
            target.HitTarget(_characterInventory.LeftHand.BaseDamage);
        }

        private Hand GetActiveAttackingHand()
        {
            //currently we only have right handed weapon, shield attacks etc have not yet been implemented
            if (IsAttacking || IsHeavyAttacking) return Hand.Right;
            return Hand.None;
        }
    }
}
