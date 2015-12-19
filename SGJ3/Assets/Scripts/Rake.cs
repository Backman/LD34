using System.Collections.Generic;
using UnityEngine;

public class Rake : Weapon
{
    [SerializeField]
    private float _dashForce;
    [SerializeField]
    private float _knockbackForce = 8f;
    [SerializeField]
    private float _stunTime = 0.5f;
    [SerializeField]
    private AudioClip _hitClip;
    [SerializeField]
    private float _hitSoundCooldown = 0.8f;

    private float _hitTime;

    private List<BotControl> _collided = new List<BotControl>();

    private void Start()
    {
    }

    private void Update()
    {
        for (int i = _collided.Count - 1; i >= 0; i--)
        {
            if (_collided[i] == null)
            {
                _collided.RemoveAt(i);
            }
        }
    }

    public void ClearCollided()
    {
        _collided.Clear();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var botControl = other.GetComponent<BotControl>();
        if (botControl && !_collided.Contains(botControl))
        {
            if (_hitTime < Time.time)
            {
                Sound.Instance.PlayClipAtPoint(_hitClip, transform.position);
                _hitTime = Time.time + _hitSoundCooldown;
            }
            var closestPoint = other.bounds.ClosestPoint(transform.position);
            botControl.Hit(closestPoint, _knockbackForce, _stunTime, HitType.Rake);
            _collided.Add(botControl);
        }
    }

    protected override void InnerAttack()
    {
        _camera.Shake.Shake(_shakeSettings.Settings, _shakeSettings.Force);
        var dir = Vector2.zero;
        dir.x = _movement.MoveDir == Movement.Direction.Left ? -1 : 1;
        var scale = transform.localScale;
        scale.x = dir.x;
        transform.localScale = scale;
        _rb.AddForce(dir * _dashForce, ForceMode2D.Impulse);
    }
}