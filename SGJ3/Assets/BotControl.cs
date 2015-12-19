using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Movement))]
public class BotControl : MonoBehaviour
{
    public float Direction;
    public float JumpDistance;

    [SerializeField]
    private Color _blinkColor = Color.red;
    [SerializeField]
    private float _fadeTime = 0.5f;
    [SerializeField]
    private MovementSettings _rustMovementSettings;

    [SerializeField]
    private bool _needsToBeRusted;

    [SerializeField]
    private float _attackDistance = 2.5f;
    [SerializeField]
    private float _damage = 5f;
    [SerializeField]
    private float _attackCooldown = 0.2f;
    [SerializeField]
    private GameObject _laser;

    private Transform _attackTransform;

    [SerializeField]
    private GameObject _deathEffect;
    [SerializeField]
    private AudioClip _explosionClip;

    private Movement _movement;
    private Animator _animator;
    private Rigidbody2D _rb;

    private TreeOfLife _tree;
    private float _attackTime;
    private Material _material;
    private float _stunTime;

    public bool Stunned { get { return _stunTime > 0f; } }
    public bool Attacking { get { return _animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"); } }
    public bool Dead { get { return _animator.GetCurrentAnimatorStateInfo(0).IsName("Death"); } }
    private bool _isRust;

    private bool _InJumpZone;

    public void InJumpZone()
    {
        _InJumpZone = true;
    }

    void Awake()
    {
        _movement = GetComponent<Movement>();
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _tree = FindObjectOfType<TreeOfLife>();
        _material = GetComponent<SpriteRenderer>().material;
        Direction = 0 - transform.position.x;

        _attackTransform = transform.FindChild("attack_start");
    }

    void Update()
    {
        Vector2 pos = transform.position;
        pos.y += 0.25f;
        int layer = (1 << LayerMask.NameToLayer("Map")) | (1 << LayerMask.NameToLayer("Enemy"));
        var dir = new Vector2(Direction, 0f).normalized;
        RaycastHit2D[] raycast = Physics2D.RaycastAll(pos, dir, JumpDistance, layer);
        bool jump = false;
        for (int i = 0; i < raycast.Length; ++i)
        {
            if (raycast[i].collider.gameObject != gameObject)
            {
                jump = true;
                break;
            }
        }

        var attack = false;
        RaycastHit2D attackHit = Physics2D.Raycast(_attackTransform.position, dir, _attackDistance, 1 << LayerMask.NameToLayer("Tree"));
        if (attackHit.collider)
        {
            attack = true;
        }

        MovementData movementData = default(MovementData);
        if (!attack && !Stunned)
        {
            movementData = new MovementData()
            {
                XMove = Direction,
                Jump = jump || _InJumpZone,
            };
        }
        _movement.SetInput(movementData);

        UpdateAnimator();

        if (attack && !Stunned && !Dead && _attackTime < Time.time /* && DistanceToTree() < _attackDistance */)
        {
            if (_laser)
            {
                var go = (GameObject)Instantiate(_laser.gameObject, _attackTransform.position, Quaternion.Euler(dir.x, dir.y, 0f));
                var laser = go.GetComponent<EnemyLaser>();
                laser.Direction = dir;
                laser.Damage = _damage;
            }
            else
            {
                _tree.Damage(_damage, attackHit.point);
            }
            _attackTime = Time.time + _attackCooldown;
            PlayState("Attack", 0);
        }
        _stunTime -= Time.deltaTime;
        _stunTime = Mathf.Max(0f, _stunTime);
        _InJumpZone = false;
    }

    public void Hit(Vector2 hitPoint, float knockback, float stunTime, HitType hitType)
    {
        Hit(hitPoint, knockback, stunTime);
        switch (hitType)
        {
            case HitType.Rake:
                if (!_needsToBeRusted)
                {
                    KillBot();
                }
                else if (_isRust)
                {
                    KillBot();
                }
                break;
            case HitType.WaterBallon:
                if (!_isRust)
                {
                    _isRust = true;
                    _material.SetFloat("_HueShift", 0.82f);
                    _movement.Settings = _rustMovementSettings;
                }
                break;
            default:
                break;
        }
    }

    private void Hit(Vector2 hitPoint, float knockbackForce, float stunTime)
    {
        StartCoroutine(Blink(hitPoint));
        Vector2 forceDir = Vector2.zero;
        forceDir.x = (transform.position - new Vector3(hitPoint.x, hitPoint.y, 0f)).x < 0f ? -1f : 1f;
        _rb.AddForce(forceDir * knockbackForce, ForceMode2D.Impulse);
        _rb.AddForce(Vector2.up * 1.5f, ForceMode2D.Impulse);
        if (_stunTime <= 0f)
        {
            _stunTime = stunTime;
        }
        WrekFaceHandler.Instance.OnRekFace();
    }

    public void KillBot(float delay = 0f)
    {
        PlayState("Death", 0);
        Sound.Instance.PlayClipAtPoint(_explosionClip, transform.position, 0.8f, Random.Range(0.9f, 1.1f));
        if (_deathEffect) { Instantiate(_deathEffect, transform.position, Quaternion.identity); }
        Destroy(gameObject, delay);
    }

    private IEnumerator Blink(Vector2 hitPoint)
    {
        _material.SetVector("_ImpactPoint", hitPoint);
        _material.SetColor("_BlinkColor", _blinkColor);
        float t = 0f;
        var fadeToColor = new Color(0f, 0f, 0f, 0f);
        while (t <= _fadeTime)
        {
            var color = Color.Lerp(_blinkColor, fadeToColor, t / _fadeTime);
            _material.SetColor("_BlinkColor", color);
            t += Time.deltaTime;
            yield return null;
        }
        _material.SetColor("_BlinkColor", fadeToColor);
    }
    
    private void PlayState(string state, int layer)
    {
		if (_animator.GetCurrentAnimatorStateInfo(layer).IsName(state) == false)
		{
			_animator.Play(state, layer);
		}
    }

    private void UpdateAnimator()
    {
        if(Attacking) return;
        
        switch (_movement.GetState())
        {
            case Movement.State.Idle:
                PlayState("Idle", 0);
                break;
            case Movement.State.StartJumping:
            case Movement.State.Jumping:
                PlayState("Jump", 0);
                break;
            case Movement.State.Falling:
                PlayState("Falling", 0);
                break;
            case Movement.State.FallRecovery:
                PlayState("FallRecovery", 0);
                break;
            case Movement.State.Running:
                PlayState("Run", 0);
                break;
            default:
                break;
        }
    }

    private float DistanceToTree()
    {
        return (!_tree) ? 10000f : Mathf.Abs(_tree.transform.position.x - transform.position.x);
    }
}
