using UnityEngine;
using System.Collections;

public struct MovementData
{
    public float XMove;
    public bool Jump;
}

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public enum Direction
    {
        Left,
        Right
    }

    public enum State
    {
        Idle,
        StartJumping,
        Jumping,
        Falling,
        FallRecovery,
        Running,
    }

    public MovementSettings Settings;

    public event System.Action<float> OnFallImpact;
    public event System.Action<float> OnJump;
    public event System.Action OnStartJump;

    bool _Grounded;
    bool _StartingJump;
    float _LastJumpTime;
    float _JumpHoldTime;
    float _LandingTime;
    float _FallImpact;
    Rigidbody2D _rb;
    MovementData _input;

    private SpriteRenderer _renderer;

    private Direction _moveDir;
    private Direction _aimDir = Direction.Right;
    [HideInInspector]
    public Direction FacingDir;

    public Direction AimDir { get { return _aimDir; } }
    public State GetState()
    {
        if (_StartingJump)
            return State.StartJumping;
        else if (_Grounded == false && _rb.velocity.y >= 0)
            return State.Jumping;
        else if (_Grounded == false && _rb.velocity.y < 0)
            return State.Falling;
        else if (InFallImpact())
            return State.FallRecovery;
        else if (_Grounded && Mathf.Abs(_rb.velocity.x) > 0 && _input.XMove != 0f)
            return State.Running;
        else
            return State.Idle;
    }

    public bool Jumping { get { return _StartingJump || _rb.velocity.y > 0; } }
    public bool Falling { get { return _Grounded == false && _rb.velocity.y < 0; } }
    public bool Grounded { get { return _Grounded; } }
    public bool Moving { get { return Mathf.Abs(_rb.velocity.x) > 0 && _input.XMove != 0f; } }
    public bool FallRecovery { get { return InFallImpact(); } }

    public Direction MoveDir
    {
        get
        {
            return _renderer.flipX ? Direction.Left : Direction.Right;
        }
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void SetInput(MovementData data)
    {
        _input = data;
        _input.XMove = Mathf.Clamp(_input.XMove, -1f, 1f);
    }

    bool InFallImpact()
    {
        float fallT = Mathf.Clamp01((Settings.MinFallImpact - _FallImpact) / (Settings.MaxFallImpact - Settings.MinFallImpact));
        float fallDelay = Mathf.Lerp(Settings.MinLandingDelay, Settings.MaxLandingDelay, fallT);
        if (_FallImpact > Settings.MinFallImpact && _LandingTime + fallDelay > Time.time)
        {
            _rb.AddForce(Vector2.down * Settings.FallImpactForce * fallT);
            return true;
        }
        return false;
    }


    bool RaycastCheck(Vector2 position, float distance, int layerMask)
    {
        var raycast = Physics2D.Raycast(position, Vector2.down, distance, layerMask);
        return raycast.collider != null && Vector2.Dot(Vector2.down, raycast.normal) < -0.3f;
    }

    private bool IsGrounded()
    {
        Bounds bounds = GetComponent<Collider2D>().bounds;
        Vector2 left = transform.position - new Vector3(bounds.extents.x, 0);
        Vector2 middle = transform.position;
        Vector2 right = transform.position + new Vector3(bounds.extents.x, 0);
        int layerMask = (1 << LayerMask.NameToLayer("Map"));
        float distance = 0.05f;
        return RaycastCheck(left, distance, layerMask) ||
                RaycastCheck(middle, distance, layerMask) ||
                RaycastCheck(right, distance, layerMask);
    }

    void FixedUpdate()
    {
        var prevGrounded = _Grounded;
        _Grounded = IsGrounded();
        bool landingDelay = InFallImpact();

        if (!prevGrounded && _Grounded)
        {
            if (OnFallImpact != null)
            {
                OnFallImpact(_rb.velocity.y);
            }
        }

        if (landingDelay == false)
        {
            if (_StartingJump == false && _Grounded && _LastJumpTime + Settings.JumpCooldown < Time.time && _input.Jump)
            {
                _StartingJump = true;
                _LastJumpTime = Time.time;
                _JumpHoldTime = 0f;
                if (OnStartJump != null)
                    OnStartJump();
            }
            if (_StartingJump && _input.Jump)
            {
                _JumpHoldTime += Time.deltaTime;
            }
            if (_StartingJump && (_LastJumpTime + Settings.JumpDelay < Time.time || _input.Jump == false))
            {
                float jumpStrength = Mathf.Clamp01(_JumpHoldTime / Settings.JumpDelay);
                _rb.AddForce(new Vector2(0, Mathf.Lerp(Settings.MinJumpForce, Settings.MaxJumpForce, jumpStrength)), ForceMode2D.Impulse);
                _StartingJump = false;
                if (OnJump != null)
                    OnJump(jumpStrength);
            }
        }

        Vector2 velocity = _rb.velocity;

        bool willBeGrounded = _rb.velocity.y < 0 && Physics2D.Raycast(transform.position, Vector2.down, Settings.GroundingCheckDistance, 1 << LayerMask.NameToLayer("Map")).collider != null;

        float deceleration = Mathf.Sign(_rb.velocity.x) * Mathf.Max(0, (Mathf.Abs(_rb.velocity.x) - Settings.DecelerationScale * Settings.Deceleration.Evaluate(Mathf.Abs(_rb.velocity.x) / Settings.DecelerationCurveMax) * Time.deltaTime));
        _rb.velocity = new Vector2(deceleration, _rb.velocity.y);
        if (_Grounded)
        {
            _rb.gravityScale = 1;
        }
        else if (velocity.y > 0)
        {
            _rb.gravityScale = Settings.UpGravity;
        }
        else if (velocity.y <= 0)
        {
            if (willBeGrounded)
                _rb.gravityScale = 1.5f;
            else
                _rb.gravityScale = Settings.DownGravity;
        }



        if (_StartingJump == false && landingDelay == false)
        {
            float xVelocity = Mathf.Max(0, velocity.x * _input.XMove);
            float acceleration = Settings.AccelerationScale * Settings.Acceleration.Evaluate(xVelocity / Settings.MaxSpeed) * _input.XMove;
            _rb.AddForce(new Vector2(acceleration, 0));
        }

        FlipSprite();
    }

    private void FlipSprite()
    {
        if (_input.XMove > 0f && _renderer.flipX)
        {
            FacingDir = Direction.Right;
            _renderer.flipX = false;
        }
        else if (_input.XMove < 0f && !_renderer.flipX)
        {
            FacingDir = Direction.Left;
            _renderer.flipX = true;
        }
    }
}