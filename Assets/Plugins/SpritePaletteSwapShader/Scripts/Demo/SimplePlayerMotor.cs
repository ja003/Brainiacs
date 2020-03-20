using UnityEngine;

namespace ActionCode.Demo
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class SimplePlayerMotor : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private string _horizontalAxisInput = "Horizontal";
        [SerializeField] private string _jumpButtonInput = "Jump";

        [Header("Animator")]
        [SerializeField] private string _hSpeedParam = "hSpeed";
        [SerializeField] private string _groundedParam = "grounded";

        [Header("Physics")]
        [Range(0f, 20f)] public float speed = 10f;
        [Range(0f, 10f)] public float jumpForce = 5f;
        public LayerMask collisionMask = -1;

        [Header("Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private BoxCollider2D _collider;
        [SerializeField] private SpriteRenderer _renderer;

        public Vector2 Size { get; private set; }

        private float _horInput = 0f;
        private float _lastDirection = 1f;

        private int _hSpeedId;
        private int _groundedId;

        private bool _isGrounded = false;

        private readonly float SKIN = 0.04F;

        private void Reset()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _hSpeedId = Animator.StringToHash(_hSpeedParam);
            _groundedId = Animator.StringToHash(_groundedParam);
        }

        private void Update()
        {
            Size = _collider.bounds.size;
            _isGrounded = IsGroundCollision();

            if (Time.timeScale > 0.1f)
            {
                UpdateInput();
                UpdateAnimator();
            }
        }

        private void FixedUpdate()
        {
            UpdatePhysics();
        }

        private void UpdatePhysics()
        {
            bool pushingAgainstWall = _horInput * _lastDirection > 0 && IsForwardCollision();
            if (!pushingAgainstWall)
            {
                Vector3 horVelocity = Vector3.right * _horInput * speed * Time.deltaTime;
                transform.position += horVelocity;
            }
        }

        private void UpdateInput()
        {
            _horInput = Input.GetAxisRaw(_horizontalAxisInput);
            if (_horInput * _lastDirection < 0f) FlipHorizontally();

            if (_isGrounded && Input.GetButtonDown(_jumpButtonInput))
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            if (_horInput < 0f) _lastDirection = -1f;
            else if (_horInput > 0f) _lastDirection = 1f;
        }

        private void UpdateAnimator()
        {
            _animator.SetFloat(_hSpeedId, Mathf.Abs(_horInput * speed));
            _animator.SetBool(_groundedId, _isGrounded);
        }

        private void FlipHorizontally()
        {
            _renderer.flipX = !_renderer.flipX;
        }

        private bool IsForwardCollision()
        {
            Vector3 direction = Vector3.right * _lastDirection;
            Vector3 bottomPos = transform.position + direction * (Size.x / 2f);
            Vector3 topPos = bottomPos + Vector3.up * Size.y;

            return
                Physics2D.Raycast(topPos, direction, SKIN, collisionMask, 0f, 0f) ||
                Physics2D.Raycast(bottomPos, direction, SKIN, collisionMask, 0f, 0f);
        }

        private bool IsGroundCollision()
        {
            Vector3 leftPos = transform.position + Vector3.left * (Size.x / 2f);
            Vector3 rightPos = transform.position + Vector3.right * (Size.x / 2f);

            return
                Physics2D.Raycast(leftPos, Vector2.down, SKIN, collisionMask, 0f, 0f) ||
                Physics2D.Raycast(rightPos, Vector2.down, SKIN, collisionMask, 0f, 0f);
        }
    }
}