using UnityEngine;
using System.Collections;

public class EnemyLaser : MonoBehaviour
{
    public float Speed = 1f;
    public AudioClip Clip;

    private Rigidbody2D _rb;
    private bool _hit;

    public Vector2 Direction { get; set; }
    public float Damage { get; set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
	
    private void FixedUpdate()
    {
        _rb.velocity = Direction * Speed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var tree = other.collider.GetComponentInParent<TreeOfLife>();
        if (!_hit && tree)
        {
            tree.Damage(Damage, other.contacts[0].point);
            _hit = true;
            Direction = Vector2.zero;
            Sound.Instance.PlayClipAtPoint(Clip, transform.position, 0.8f, Random.Range(0.9f, 1.1f));
            Destroy(gameObject, 0.5f);
        }
    }
}
