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
    /// Gets a value indicating if the player in his current animation state may rotate.
    /// </summary>
    /// <returns></returns>
    public bool CanRotate
    {
        get => _canRotate;
        set => _canRotate = value;
    }

    // Start is called before the first frame update WHEN A SCRIPT IS ENABLED
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _isInteracting = _animator.GetBool(_isInteractingHash);
    }
}
