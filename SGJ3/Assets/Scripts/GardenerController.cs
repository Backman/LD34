using UnityEngine;
using System.Collections;
using InControl;

public class GardenerController : MonoBehaviour
{

	private WaterCanon _canon;
	private Rake _rake;
	private Movement _movement;
	private Animator _animator;

	private SpriteRenderer _renderer;
	private Camera _camera;

	public WaterCanon Canon { get { return _canon; } }
	public Rake Rake { get { return _rake; } }

	[SerializeField]
	GameObject GroundShadow;
	[SerializeField]
	GameObject JumpParticle;
	[SerializeField]
	CameraShakeSettings JumpShake;
	[SerializeField]
	AnimationCurve JumpShakeForce;
	[SerializeField]
	GameObject FallImpactParticle;
	[SerializeField]
	AudioClip HeayvFallClip;
	[SerializeField]
	AudioClip JumpClip;
	[SerializeField]
	AudioClip StartJumpClip;
	[SerializeField]
	float JumpClipForceThreshold;

	private bool _isAttacking
	{
		get
		{
			var state = _animator.GetCurrentAnimatorStateInfo(0);
			return state.IsName("Rake Attack") || state.IsName("Canon Attack");
		}
	}

	private void Start()
	{
		_movement = GetComponent<Movement>();
		_canon = GetComponentInChildren<WaterCanon>();
		_rake = GetComponentInChildren<Rake>();
		_renderer = GetComponent<SpriteRenderer>();
		_animator = GetComponent<Animator>();
		_camera = Camera.main;
		_movement.OnFallImpact += _movement_OnFallImpact;
		_movement.OnJump += _movement_OnJump;
		_movement.OnStartJump += _movement_OnStartJump;
	}

	AudioSource _startJumpZOne;

	private void _movement_OnStartJump()
	{
		_startJumpZOne = Sound.Instance.PlayClipAtPoint(StartJumpClip, transform.position);
	}

	IEnumerator FadeOutSource(AudioSource source, float duration)
	{
		float startTime = Time.time;
		float startVolume = source.volume;
		while (startTime > Time.time)
		{
			float t = (Time.time - startTime) / duration;
			t = 1 - t;
			source.volume = startVolume * t;
			yield return null;
		}
		Destroy(source.gameObject);
	}

	private void _movement_OnJump(float obj)
	{
		if (JumpParticle != null)
			ObjectPool.GetInstance(JumpParticle, transform.position, Quaternion.identity);

		_camera.GetComponent<FollowCamera>().Shake.Shake(JumpShake, JumpShakeForce.Evaluate(obj), Vector2.down);
		if (obj > JumpClipForceThreshold)
		{
			if (_startJumpZOne)
			{
				StartCoroutine(FadeOutSource(_startJumpZOne, 0.02f));
			}
			Sound.Instance.PlayClipAtPoint(JumpClip, transform.position);
		}
	}

	private void _movement_OnFallImpact(float obj)
	{
		if (FallImpactParticle != null)
		{
			ObjectPool.GetInstance(FallImpactParticle, transform.position, Quaternion.identity);
		}
		
		if (Mathf.Abs(obj) > 10f)
		{
			Sound.Instance.PlayClipAtPoint(HeayvFallClip, transform.position);
		}
	}

	void OnDestroy()
	{
		ObjectPool.Clear();
	}

	private void Update()
	{
		var inputDevice = InputManager.ActiveDevice;
		var move = inputDevice.LeftStickX;
		var jump = inputDevice.Action1.IsPressed;

		var cannon = inputDevice.Action2.IsPressed;
		var rake = inputDevice.Action3.IsPressed;

		MovementData movementData = default(MovementData);
		if (!_isAttacking)
		{
			_rake.ClearCollided();
			movementData = new MovementData()
			{
				XMove = move,
				Jump = jump
			};
			if (_movement.FacingDir == Movement.Direction.Left && !_renderer.flipX)
			{
				_renderer.flipX = true;
			}
			else if (_movement.FacingDir == Movement.Direction.Right && _renderer.flipX)
			{
				_renderer.flipX = false;
			}
		}
		_movement.SetInput(movementData);

		UpdateAnimator();
		if (GroundShadow.activeSelf != _movement.Grounded)
			GroundShadow.SetActive(_movement.Grounded);

		if (cannon && !_isAttacking && _canon.CanAttack())
		{
			_animator.Play("Canon Attack", 0);
		}
		else if (rake && !_isAttacking && _rake.CanAttack())
		{
			_animator.Play("Rake Attack", 0);
		}


		MusicSystem.Instance.SetTreeDistance(Mathf.Abs(transform.position.x));
	}

	public void RakeAttack()
	{
		_rake.Attack();
	}

	public void WaterCanonAttack()
	{
		_canon.Attack();
	}

	private void PlayState(string state, int layer)
	{
		if (_animator.GetCurrentAnimatorStateInfo(layer).IsName(state) == false)
			_animator.Play(state, layer);
	}

	private void UpdateAnimator()
	{
		if (_isAttacking) return;

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
}
