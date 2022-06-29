using System;
using DarkSouls.Animation;
using DarkSouls.Characters;
using DarkSouls.Combat;
using DarkSouls.Inventory;
using DarkSouls.UI;
using UnityEngine;

/// <summary>
/// Centralized class that is responsible for bridging the gap between game components and the player.
/// </summary>
public class PlayerController : MonoBehaviour, ICharacterController
{
    private Animator _animator;
    private AnimationHandler _animationHandler;
    private CharacterAttributes _characterAttributes;
    private CharacterInventory _characterInventory;
    private WeaponSocketController _weaponSocketController;
    private UIController _ui;

    /// <inheritdoc/>
    public CharacterState State { get; private set; }

    /// <inheritdoc/>
    public Action OnInteractingAnimationCompleteDoThis { get; set; }

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        State = new CharacterState(_animator);

        _animationHandler = GetComponent<AnimationHandler>();
        _characterAttributes = GetComponent<CharacterAttributes>();
        _ui = GetComponent<UIController>();
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
        Debug.Log($"I've been damaged for {damage}!");
        _characterAttributes.DamageCharacter(damage);
        _ui.SetHealthBarValue(_characterAttributes.CurrentHealth);

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
        _ui.SetHealthBarMaximum(_characterAttributes.MaximumHealth);
        _ui.SetHealthBarValue(_characterAttributes.CurrentHealth);

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
        //currently we only have right handed weapon, shield attacks etc have not yet been implemented
        if (State.IsAttacking || State.IsHeavyAttacking) return Hand.Right;
        return Hand.None;
    }
}
