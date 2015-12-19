using UnityEngine;

public struct AttackData
{
    public Vector3 MousePosition;
}

public enum HitType
{
    WaterBallon,
    Rake
}

public abstract class Weapon : MonoBehaviour
{
    public GameObject Owner;

    [SerializeField]
    protected GameObject _attackParticle;
    
    [SerializeField]
    protected AudioClip _attackClip;

    [SerializeField]
    private string _animationStateName;

    [SerializeField]
    protected AttackShakeSettings _shakeSettings;

    private int _attackLayerID;
    private float _attackTime;
    protected Rigidbody2D _rb;
    protected Movement _movement;
    protected Animator _animator;
    protected FollowCamera _camera;

    protected virtual void Awake()
    {
        _movement = GetOwnerComponent<Movement>();
        _animator = GetOwnerComponent<Animator>();
        _rb = GetOwnerComponent<Rigidbody2D>();
        _attackLayerID = Animator.StringToHash("Attack");
        _camera = Camera.main.GetComponent<FollowCamera>();
    }

    public void Attack()
    {
        if (!CanAttack()) { return; }
        Sound.Instance.PlayClipAtPoint(_attackClip, transform.position);
        if (_attackParticle) { Instantiate(_attackParticle, transform.position, Quaternion.identity);  }
        _animator.Play(_animationStateName, 0);
        InnerAttack();
    }

    public virtual bool CanAttack() { return true; }
    protected abstract void InnerAttack();

    protected T GetOwnerComponent<T>()
	{
        return Owner.GetComponent<T>();
    }
}