using System;
using DarkSouls.Animation;
using DarkSouls.Characters;
using DarkSouls.Combat;
using DarkSouls.Inventory;
using UnityEngine;

/// <summary>
/// Centralized class that is responsible for bridging the gap between game components and the character.
/// </summary>
public class PlayerController : MonoBehaviour
{
    private Animator _animator;
    private AnimationHandler _animationHandler;
    private CharacterAttributes _characterAttributes;
    private CharacterInventory _characterInventory;
    private WeaponSocketController _weaponSocketController;

    /// <summary>
    /// The character state data.
    /// </summary>
    public CharacterState State { get; private set; }

    public Action OnInteractingAnimationCompleteDoThis { get; set; }

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        State = new CharacterState(_animator);

        _animationHandler = GetComponent<AnimationHandler>();
        _characterAttributes = GetComponent<CharacterAttributes>();
        _characterInventory = GetComponent<CharacterInventory>();
        _weaponSocketController = GetComponent<WeaponSocketController>();
    }

    // Start is called before the first frame update WHEN A SCRIPT IS ENABLED
    void Start()
    {
        InitializePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        State.IsInteracting = _animator.GetBool(State.IsInteractingHash);
    }

    void LateUpdate()
    {
        /*
         * Note in tutorial he sets all of the input handler flags explicitly to false but we let the input handler handle that and fire events accordingly
         */

        if (State.IsAerial)
        {
            //deviation: he has in air timer on the PlayerLocomotion component but I moved it here because locomotion doesn't care how long its in the air
            State.AerialTimer += Time.deltaTime;
        }
    }

    /// <summary>
    /// Finalizer method for interactive animations.
    /// </summary>
    public void FinishInteractiveAnimation()
    {
        //initial game state will fire this off since it happens on Enter of Empty
        //some animations like falling just enter the enter state and do nothing with this action so it will also be null on that case
        if (OnInteractingAnimationCompleteDoThis == null) return;
        
        OnInteractingAnimationCompleteDoThis.Invoke();
        OnInteractingAnimationCompleteDoThis = null;
    }

    
    /// <summary>
    /// Will damage the character by the amount passed in.
    /// </summary>
    /// <param name="damage"></param>
    public void DamageCharacter(int damage)
    {
        Debug.Log($"I've been damaged for {damage}!");
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

    /// <summary>
    /// Called by the animator control to trigger a stamina drain.
    /// </summary>
    public void DrainCharacterStamina()
    {
        var attackingHand = GetActiveAttackingHand(out bool isHeavyAttack);
        var staminaDrain = attackingHand == Hand.Right 
                ? _characterInventory.GetRightHandStaminaDrain(isHeavyAttack)
                : _characterInventory.GetLeftHandStaminaDrain(isHeavyAttack);
        
        _characterAttributes.DrainStamina(staminaDrain);
    }

    /// <summary>
    /// Called by animator control to enable the weapon collider.
    /// </summary>
    public void EnableAttackingWeaponCollider()
    {
        _weaponSocketController.SetColliderEnabledForWeapon(GetActiveAttackingHand(), enabled: true);
    }

    /// <summary>
    /// Called by animator control to disable the weapon collider.
    /// </summary>
    public void DisableAttackingWeaponCollider()
    {
        _weaponSocketController.SetColliderEnabledForWeapon(GetActiveAttackingHand(), enabled: false);
    }

    /// <summary>
    /// Called by the animator control to enable combo availability.
    /// </summary>
    public void EnableComboAvailability()
    {
        State.IsAbleToCombo = true;
    }

    /// <summary>
    /// Called by the animator control to disable combo availability.
    /// </summary>
    public void DisableComboAvailability()
    {
        State.IsAbleToCombo = false;
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
        State.IsImpacted = true;
        OnInteractingAnimationCompleteDoThis = () =>
        {
            State.IsImpacted = false;
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
        return GetActiveAttackingHand(out var isHeavyAttack);
    }

    private Hand GetActiveAttackingHand(out bool isHeavyAttack)
    {
        isHeavyAttack = State.IsHeavyAttacking;

        //currently we only have right handed weapon, shield attacks etc have not yet been implemented
        if (State.IsAttacking || State.IsHeavyAttacking) return Hand.Right;
        return Hand.None;
    }
}
