using System;
using System.Linq;
using DarkSouls.Animation;
using DarkSouls.Characters;
using DarkSouls.Combat;
using DarkSouls.Input;
using DarkSouls.Inventory;
using DarkSouls.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Centralized class that is responsible for bridging the gap between game components and the character.
/// </summary>
public class PlayerController : MonoBehaviour
{
    private Animator _animator;
    private AnimationHandler _animationHandler;
    private CharacterAttributes _characterAttributes;
    private CharacterInventory _characterInventory;
    private InputHandler _inputHandler; //optional - only players will have this
    private PopUpUI _popupUI; //optional - only players should have this so do not let NPC controllers snag a reference.
    private WeaponSocketController _weaponSocketController;

    // Senses
    private InteractableEyes _interactableEyes;
    private TargetingEyes _targetingEyes;

    /// <summary>
    /// Gets a value indicating that the controller belongs to a player.
    /// </summary>
    public bool IsPlayerCharacter { get; private set; }
    
    /// <summary>
    /// The character state data.
    /// </summary>
    [field: SerializeField]
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
        
        //child objects
        _interactableEyes = GetComponentInChildren<InteractableEyes>();
        _targetingEyes = GetComponentInChildren<TargetingEyes>();
        
        IsPlayerCharacter = gameObject.CompareTag("Player");
    }

    // Start is called before the first frame update WHEN A SCRIPT IS ENABLED
    void Start()
    {
        InitializeCharacter();
        if (IsPlayerCharacter) InitializePlayerOnlyComponents();
    }

    // Update is called once per frame
    void Update()
    {
        State.IsInteracting = _animator.GetBool(State.IsInteractingHash);

        if (IsPlayerCharacter) PlayerOnlyUpdate();
    }

    void LateUpdate()
    {
        if (State.IsAerial)
        {
            State.AerialTimer += Time.deltaTime;
        }
    }

    private void InitializeCharacter()
    {
        _weaponSocketController.LoadRightHandSocketWithWeapon(_characterInventory.RightHand);
        _weaponSocketController.LoadLeftHandSocketWithWeapon(_characterInventory.LeftHand);

        _weaponSocketController.OnRightHandHit += RightHandHit;
        _weaponSocketController.OnLeftHandHit += LeftHandHit;
    }

    private void InitializePlayerOnlyComponents()
    {
        if (!TryGetComponent<InputHandler>(out _inputHandler))
        {
            Debug.LogError("Playable character does not have an input controller available");
        }
        else
        {
            _inputHandler.OnInputInteraction += InputHandler_InteractionButtonInvoked;
            _inputHandler.OnInputTargeting += InputHandler_TargetingButtonInvoked;
        }
        
        _popupUI = FindObjectOfType<PopUpUI>(); //only should be one of these in the scene and only a player should care about it
    }

    private void PlayerOnlyUpdate()
    {
        if (_interactableEyes.Interactables.Any())
        {
            var interactableText = _interactableEyes.Interactables.First().InteractionText;
            // todo: Implement issue #11 - multiple interactables
            _popupUI.ShowPopUpText(string.IsNullOrEmpty(interactableText)
                ? "## UNDEFINED ##"
                : interactableText);
        }
        else //make sure popup is not active
        {
            _popupUI.HidePopUpText();
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

    /// <summary>
    /// Pops up an image to the player with a text caption.
    /// </summary>
    /// <param name="image">The image to display.</param>
    /// <param name="text">The text caption to display with the image.</param>
    public void PopupImageToPlayer(Sprite image, string text)
    {
        _popupUI.ShowImagePopUp(image, text);
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
        // todo: currently listed as unneccessary assignment - when heavy attacks are used here make sure this warning goes away
        return GetActiveAttackingHand(out var isHeavyAttack);
    }

    private Hand GetActiveAttackingHand(out bool isHeavyAttack)
    {
        isHeavyAttack = State.IsHeavyAttacking;

        //currently we only have right handed weapon, shield attacks etc have not yet been implemented
        if (State.IsAttacking || State.IsHeavyAttacking) return Hand.Right;
        return Hand.None;
    }

    private void InputHandler_InteractionButtonInvoked()
    {
        // clear icon pop up if its displayed
        if (_popupUI.IsImagePopUpDisplayed)
        {
            _popupUI.HideImagePopUp();
        }

        // if an interactable is in range, interact with it
        // todo: implement issue #11 - multiple interactables
        if (!_interactableEyes.Interactables.Any()) return;
        var interactable = _interactableEyes.Interactables.First();
        interactable.Interact(this);
        _interactableEyes.RemoveTarget(interactable);
    }

    private void InputHandler_TargetingButtonInvoked()
    {
        if (_targetingEyes.CurrentTarget != null)
        {
            Debug.Log("Clearing Target");
            _targetingEyes.ClearTarget();
            return;
        }

        if (_targetingEyes.TrySelectClosestTarget())
        {
            Debug.Log($"Targeting {_targetingEyes.CurrentTarget.name}");
        }
    }
}