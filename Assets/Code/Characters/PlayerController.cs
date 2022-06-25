using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator _animator;
    private readonly int _isInteractingHash = Animator.StringToHash("isInteracting");
    private bool _isInteracting;
    private bool _canRotate = true;
    
    /// <summary>
    /// Gets a value indicating if the animator is in an animation state that cannot be changed until completion.
    /// </summary>
    public bool IsInteracting
    {
        get => _isInteracting;
        set
        {
            _animator.SetBool(_isInteractingHash, value);
            _isInteracting = value;
        } 
    }

    /// <summary>
    /// Gets or sets a value indicating how long the player has been in the air.
    /// </summary>
    public float AerialTimer { get; set; }

    /// <summary>
    /// Gets or set a value indicating that the player is rolling.
    /// </summary>
    public bool IsRolling { get; set; }

    /// <summary>
    /// Gets or sets a value indicating that the player is back stepping.
    /// </summary>
    public bool IsBackStepping { get; set; }

    /// <summary>
    /// Gets or sets a value indicating that the player is in the air.
    /// </summary>
    public bool IsAerial { get; set; } //todo: why not just make this IsGrounded = false?

    /// <summary>
    /// Gets or sets a value indicating that the player is performing an attack with their right hand.
    /// </summary>
    public bool IsAttacking { get; set; }

    /// <summary>
    /// Gets or sets a value indicating that the player is performing a heavy attack with their right hand.
    /// </summary>
    public bool IsHeavyAttacking { get; set; }

    /// <summary>
    /// Gets or sets a value indicating that the player is on the ground.
    /// </summary>
    public bool IsGrounded { get; set; }

    /// <summary>
    /// Gets or sets a value indicating that the player is sprinting.
    /// </summary>
    public bool IsSprinting { get; set; }

    /// <summary>
    /// Gets or sets a value indicating that the roll button was invoked.
    /// </summary>
    public bool RollButtonInvoked { get; set; }

    /// <summary>
    /// Gets a value indicating if the player in his current animation state may rotate.
    /// </summary>
    /// <returns></returns>
    public bool CanRotate
    {
        get => _canRotate;
        set => _canRotate = value;
    }

    /// <summary>
    /// Gets or sets an action that will execute when an interacting animation completes.
    /// </summary>
    public Action OnInteractingAnimationCompleteDoThis;

    // Start is called before the first frame update WHEN A SCRIPT IS ENABLED
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _isInteracting = _animator.GetBool(_isInteractingHash);
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
        if (OnInteractingAnimationCompleteDoThis == null)
        {
            Debug.Log("PlayerController :: FinishInteractiveAnimation action not set.  This is fine to ignore on the very start of the game as Enter State fires this off.  Otherwise if you see this any time after, this needs checked.");
            return;
        }

        OnInteractingAnimationCompleteDoThis.Invoke();
        OnInteractingAnimationCompleteDoThis = null;
    }
}
