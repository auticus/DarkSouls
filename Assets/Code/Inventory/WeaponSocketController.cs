﻿using System;
using DarkSouls.Characters;
using DarkSouls.Combat;
using UnityEngine;

namespace DarkSouls.Inventory
{
    /// <summary>
    /// Controller class that will manage what is in the player's hands.
    /// </summary>
    public class WeaponSocketController : MonoBehaviour
    {
        private WeaponSocket _leftHand;
        private WeaponSocket _rightHand;
        private DamageCollider _rightHandCollider;
        private DamageCollider _leftHandCollider;

        public event Action<Target> OnRightHandHit;
        public event Action<Target> OnLeftHandHit;

        private void Awake()
        {
            var sockets = GetComponentsInChildren<WeaponSocket>();
            foreach (var socket in sockets)
            {
                if (socket.LeftHandSocket) _leftHand = socket;
                else if (socket.RightHandSocket) _rightHand = socket;
            }
        }

        public void LoadRightHandSocketWithWeapon(Weapon weapon)
        {
            _leftHand.LoadWeaponModel(weapon);
            RegisterWeaponSocketCollider(_leftHand, ref _leftHandCollider);
        }

        public void LoadLeftHandSocketWithWeapon(Weapon weapon)
        {
            _rightHand.LoadWeaponModel(weapon);
            RegisterWeaponSocketCollider(_rightHand, ref _rightHandCollider);
        }

        public void RegisterWeaponSocketCollider(WeaponSocket weapon, ref DamageCollider handCollider)
        {
            if (handCollider != null)
            {
                handCollider.OnTargetHit -= weapon.RightHandSocket ? OnRightHandHitTarget : OnLeftHandHitTarget;
            }

            var collider = weapon.WeaponModel.GetComponentInChildren<DamageCollider>();
            handCollider = collider;
            handCollider.OnTargetHit += weapon.RightHandSocket ? OnRightHandHitTarget : OnLeftHandHitTarget;
        }

        public void SetColliderEnabledForWeapon(Hand handSocket, bool enabled)
        {
            if (handSocket == Hand.Right)
            {
                if (enabled) _rightHandCollider.EnableDamageCollider();
                else _rightHandCollider.DisableDamageCollider();
            }
            else if (handSocket == Hand.Left)
            {
                if (enabled) _leftHandCollider.EnableDamageCollider();
                else _leftHandCollider.DisableDamageCollider();
            }
        }

        private void OnRightHandHitTarget(Target obj)
            => OnRightHandHit?.Invoke(obj);
        

        private void OnLeftHandHitTarget(Target obj)
            => OnLeftHandHit?.Invoke(obj);
        
    }
}
