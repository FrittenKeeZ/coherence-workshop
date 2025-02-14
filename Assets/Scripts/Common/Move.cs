using UnityEngine;
using Coherence.Toolkit;

public class Move : MonoBehaviour
{
    public Animator animator;
    public CoherenceSync sync;

    [field: Header("Inputs")]
    public Vector3 MoveInput { get; set; }
    public bool IsSprinting { get; set; }

    [Header("Horizontal movement")]
    public float movementSpeed = 7f;
    public float runMultiplier = 1.5f;
    public float rotationSpeed = 20f;
    public float acceleration = 10f;
    public float airAcceleration = 5f;
    public float deceleration = 15f;
    public float airDeceleration = 1;

    [Header("Jump")]
    public float weight = 45f;
    public float jumpLength = .5f; // time for a "full jump", releasing button within this time limits jump height
    public float jumpForce = 20;
    public float jumpCutOff = 0.5f; // when releasing button mid-jump, up-vel is multiplied by this value
    public float coyoteTime = .125f;
    public float maxFallSpeed = 20;

    [Header("Spring")]
    public float cruiseHeight = 1f;
    public float raycastLength = 1.5f;
    public float springStrength = 40f;
    public float dampenFactor = 0.05f;
    public LayerMask walkableLayers;

    private bool _isGrounded;
    private float _currentSpeed;
    private Rigidbody _rigidbody;
    private Vector3 _horizontalVelocity;
    private Vector3 _pushbackVelocity;
    private Vector3 _verticalVelocity;
    private Vector3 _springVector;
    private Vector3 _previousInputVector;
    private float _previousInputMagnitude;
    private float _timeSinceGrounded = 0;
    private float _jumpTimer = 0;
    private bool _isFalling = true;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isFrozen) return; 
        
        UpdateJumping();
    }

    private void FixedUpdate()
    {
        if (isFrozen)
        {
            UpdateAnimatorParameters();
            return; 
        }

        Vector3 velocity = _rigidbody.velocity;

        VerticalMovement(velocity);

        // Horizontal movement
        float inputMagnitude = MoveInput.magnitude;

        float inputAcceleration = inputMagnitude > _previousInputMagnitude
            ?
            _isGrounded ? acceleration : airAcceleration
            :
            _isGrounded
                ? deceleration
                : airDeceleration;

        Vector3 lerpedInputVector = Vector3.Lerp(_previousInputVector, MoveInput, Time.deltaTime * inputAcceleration);
        float lerpedMagnitude = lerpedInputVector.magnitude;
        
        // Reduce pushback (coming from hits or from crate throws)
        _pushbackVelocity *= .8f;

        _currentSpeed = IsSprinting ? movementSpeed * runMultiplier : movementSpeed;
        _horizontalVelocity = (lerpedInputVector + _pushbackVelocity) * _currentSpeed;

        // Caching for next frame
        _previousInputMagnitude = lerpedMagnitude;
        _previousInputVector = lerpedInputVector;

        // Cap vertical velocity when falling 
        if (_isFalling && _verticalVelocity.magnitude > maxFallSpeed)
            _verticalVelocity = _verticalVelocity.normalized * maxFallSpeed;

        // Apply both components to find final frame velocity
        _rigidbody.velocity = _horizontalVelocity + _verticalVelocity;
        if (!_isGrounded) _verticalVelocity = Vector3.Project(_rigidbody.velocity, Vector3.up);

        if (inputMagnitude > 0f)
        {
            Quaternion newRotation = Quaternion.LookRotation(MoveInput);
            transform.rotation = Quaternion.Lerp(_rigidbody.rotation, newRotation, Time.deltaTime * rotationSpeed);
        }

        UpdateAnimatorParameters();
    }

    public bool isFrozen => _rigidbody.isKinematic; 
    public void SetFrozen(bool value)
    {
        _rigidbody.isKinematic = value;
        if (value)
        {
            InterruptJump();
            _previousInputVector = Vector3.zero;
            _horizontalVelocity = Vector3.zero; 
            _verticalVelocity = Vector3.zero; 
            _isGrounded = true; 
            _pushbackVelocity = Vector3.zero; 
        }
    }

    private void VerticalMovement(Vector3 velocity)
    {
        // Vertical movement
        float rayLength = _isGrounded ? raycastLength : cruiseHeight;
        if (_jumpTimer > 0)
        {
            // Is jumping up
            ApplyGravity();
        }
        else
        {
            // Check for ground
            bool wasGrounded = _isGrounded;
            Ray ray = new(transform.position, Vector3.down);
            _isGrounded = Physics.Raycast(ray, out RaycastHit raycastHit, rayLength, walkableLayers,
                QueryTriggerInteraction.Ignore);
            if (_jumpTimer > 0)
                _isGrounded = false;

            if (_isGrounded)
            {
                // Ground spring
                float delta = raycastHit.distance - cruiseHeight;
                float spring = delta * springStrength - -velocity.y * dampenFactor;

                _springVector = spring * ray.direction;

                _verticalVelocity = Vector3.Lerp(_verticalVelocity, _springVector, Time.deltaTime * 20f);
                _timeSinceGrounded = 0;

                if (raycastHit.transform.CompareTag("Bouncy"))
                {
                    Jump(); 
                }
            }
            else
            {
                // Free falling
                if(wasGrounded) LeaveGround();
                ApplyGravity();
            }
        }
    }

    private void UpdateAnimatorParameters()
    {
        // This is used to blend between the Walk and Run animation clips in the Animator
        float animationSpeed = _horizontalVelocity.magnitude / _currentSpeed;
        if (IsSprinting) animationSpeed *= runMultiplier;

        animator.SetFloat("MoveSpeed", animationSpeed);
        animator.SetBool("Grounded", _isGrounded);
    }

    private void ApplyGravity()
    {
        _verticalVelocity += Vector3.up * (-weight * Time.deltaTime);
        _timeSinceGrounded += Time.deltaTime;
    }

    public void TryJump()
    {
        bool canJump = _isGrounded || _timeSinceGrounded < coyoteTime;
        if (canJump) Jump();
    }

    public void InterruptJump()
    {
        _jumpTimer = -1;
        _verticalVelocity *= jumpCutOff;
    }

    private void UpdateJumping()
    {
        if (_jumpTimer > 0)
        {
            _jumpTimer -= Time.deltaTime;
            if (_jumpTimer < 0f) InterruptJump();
        }

        _isFalling = !_isGrounded && Vector3.Dot(_rigidbody.velocity, Vector3.up) < 0;
    }

    /// <summary>
    /// Invoked when a jump happens. This method is not invoked when dropping off high ground without the jump input.
    /// </summary>
    public void Jump()
    {
        if (_isGrounded)
        {
            animator.SetTrigger("Jump");
            sync.SendCommand<Animator>(nameof(Animator.SetTrigger), Coherence.MessageTarget.Other, "Jump");
        }
        _jumpTimer = jumpLength;
        
        LeaveGround();

        _verticalVelocity = Vector3.up * jumpForce;
        Vector3 d = _rigidbody.velocity;
        d -= Vector3.Project(d, Vector3.up);
        d += _verticalVelocity;
        _rigidbody.velocity = d;
    }

    /// <summary>
    /// Invoked when jumping, but also when dropping off a high ground into the air.
    /// </summary>
    private void LeaveGround()
    {
        _isGrounded = false;
    }

    /// <summary>
    /// Applies a pushback force to the player, coming from a throw or from a hit.
    /// The pushback is added on top of player input,
    /// and will fade in time over the course of a few frames.
    /// </summary>
    public void ApplyThrowPushback(float throwSpeed)
    {
        _pushbackVelocity += -transform.forward * throwSpeed * .7f;
    }
}