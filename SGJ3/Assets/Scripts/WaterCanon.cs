using UnityEngine;
using System.Collections;

public struct BallonData
{
    public Vector2 Direction;
    public bool IsValid;
    public bool FlipX;
}

public class WaterCanon : Weapon
{
    [SerializeField]
    private WaterCanonData _cannonData;

    private BallonData _ballon;

    private int _curAmmoCount;
    
    public int CurrentAmmo { get { return _curAmmoCount; } }
    public int MaxAmmo { get { return _cannonData.MaxAmmoCount; } }

    private void Start()
    {
        _curAmmoCount = _cannonData.StartAmmoCount;
        EnsureAmmoCount();
		FindObjectOfType<BallonCounter>().EnableAmmo(_curAmmoCount);
    }

    private void FixedUpdate()
    {
        if (_ballon.IsValid)
        {
            CreateBallon(transform.position, _ballon.Direction);
            _ballon.IsValid = false;
        }
    }

    public void AddAmmo(int amount = 1)
    {
        _curAmmoCount += amount;
        EnsureAmmoCount();
        if (GameManager.Instance.OnAddAmmo != null)
        {
            GameManager.Instance.OnAddAmmo(amount, _curAmmoCount);
        }
    }

    public void RemoveAmmo(int amount = 1)
    {
        _curAmmoCount--;
        EnsureAmmoCount();
        if (GameManager.Instance.OnRemoveAmmo != null)
        {
            GameManager.Instance.OnRemoveAmmo(amount, _curAmmoCount);
        }
    }

    public override bool CanAttack()
    {
        return _curAmmoCount > 0;
    }

    protected override void InnerAttack()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var aimDir = (mousePos - transform.position).normalized;
        var angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        var aimLeft = aimDir.x < 0f;
        var aimAngle = aimLeft ? _cannonData.AimAngle + 180f : _cannonData.AimAngle;
        Owner.GetComponent<SpriteRenderer>().flipX = aimLeft;
        angle = Mathf.Clamp(angle, -aimAngle, aimAngle);
        angle *= Mathf.Deg2Rad;
        _ballon.FlipX = !aimLeft;
        _ballon.Direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        _ballon.IsValid = true;
    }

    private void EnsureAmmoCount()
    {
        _curAmmoCount = Mathf.Clamp(_curAmmoCount, 0, _cannonData.MaxAmmoCount);
        if (_curAmmoCount == 0)
        {
            GameManager.Instance.TrySpawnWater();
        }
    }

    private void CreateBallon(Vector2 position, Vector2 direction)
    {
        var go = (GameObject)Instantiate(_cannonData.BallonPrefab.gameObject, position, Quaternion.identity);
        go.GetComponent<Rigidbody2D>().AddForce(direction * _cannonData.AttackForce, ForceMode2D.Impulse);
        var ballon = go.GetComponent<WaterBallon>();
        ballon.StunTime = _cannonData.StunTime;
        ballon.Knockback = _cannonData.HitKnockback;
        _camera.Shake.Shake(_shakeSettings.Settings, _shakeSettings.Force);
        var knockbackDir = direction.x < 0f ? Vector2.right : Vector2.left;
        _rb.AddForce(-direction * _cannonData.Knockback, ForceMode2D.Impulse);
        
        RemoveAmmo();
    }

    private void OnDrawGizmos()
    {
        if(!_cannonData) return;
        float aimAngle;
        if (_movement)
        {
            aimAngle = _movement.AimDir == Movement.Direction.Left ? _cannonData.AimAngle + 180f : _cannonData.AimAngle;
        }
        else
        {
            aimAngle = _cannonData.AimAngle;
        }
        var angle1 = -aimAngle * Mathf.Deg2Rad;
        var angle2 = aimAngle * Mathf.Deg2Rad;
        var dir1 = new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0f);
        var dir2 = new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0f);

        Gizmos.DrawLine(transform.position, transform.position + dir1);
        Gizmos.DrawLine(transform.position, transform.position + dir2);
    }
}
