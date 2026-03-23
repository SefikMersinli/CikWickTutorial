using UnityEngine; // System.Diagnostics silindi

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientationTransform;

    [Header("Movement Settings")]
    [SerializeField] private KeyCode _movementKey;
    [SerializeField] private float movementSpeed; // İsim tutarlı hale getirildi

    [Header("Jump Settings")]
    [SerializeField] private KeyCode _JumpKey;
    [SerializeField] private float _JumpForce;
    [SerializeField] private float _JumpCooldown;
    [SerializeField] private bool _canJump = true;

    [Header("Sliding Settings")]
    [SerializeField] private KeyCode _slideKey;
    [SerializeField] private float _slideMultiplier;
    [SerializeField] private float _slideDrag;

    [Header("Ground Check Settings")]
    [SerializeField] private float PlayerHeight;
    [SerializeField] private LayerMask _groundlayer;
    [SerializeField] private float _groundDrag;

    private Rigidbody _playerRigidbody;
    private float _horizontalInput, _verticalInput;
    private Vector3 _movementDirection;
    private bool _isSliding;

    private void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerRigidbody.freezeRotation = true;
    }

    private void Update() {
        SetInputs();
        SetPlayerDrag();
        LimitPlayerSpeed();
    }

    private void FixedUpdate() {
        SetPlayerMovement();
    }

    private void SetInputs()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        // Kayma Kontrolü
        if (Input.GetKeyDown(_slideKey))
        {
            _isSliding = true;
            Debug.Log("Player Sliding!"); // Bu satır videodaki yazıyı çıkarır
        }
        else if (Input.GetKeyUp(_slideKey)) 
        {
            _isSliding = false;
            // Buraya istersen "Player Moving!" yazdırabilirsin
            Debug.Log("Player Moving!"); 
        }

        // Zıplama Kontrolü
        if (Input.GetKeyDown(_JumpKey) && _canJump && IsGrounded())
        {
            _canJump = false;
            SetPlayerJumping();
            Debug.Log("Player Jumped!"); // Zıplayınca da yazı çıksın
            Invoke(nameof(ResetJumping), _JumpCooldown);
        }
    }

    private void SetPlayerMovement()
    {
        _movementDirection = orientationTransform.forward * _verticalInput + orientationTransform.right * _horizontalInput;

        float currentSpeed = _isSliding ? movementSpeed * _slideMultiplier : movementSpeed;
        _playerRigidbody.AddForce(_movementDirection.normalized * currentSpeed, ForceMode.Force);
    }

    private void SetPlayerDrag()
    {
        if (_isSliding)
        {
            _playerRigidbody.linearDamping = _slideDrag;
        }
        else
        {
            _playerRigidbody.linearDamping = _groundDrag;
        }
        
    }

    private void LimitPlayerSpeed()
    {
        Vector3 flatVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        if (flatVelocity.magnitude > movementSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * movementSpeed;
            _playerRigidbody.linearVelocity = new Vector3(limitedVelocity.x, _playerRigidbody.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void SetPlayerJumping()
    {
        // Y eksenindeki hızı sıfırlayıp öyle zıplatmak daha stabil sonuç verir
        _playerRigidbody.linearVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        _playerRigidbody.AddForce(transform.up * _JumpForce, ForceMode.Impulse);
    }

    private void ResetJumping()
    {
        _canJump = true;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, _groundlayer);
    }
}