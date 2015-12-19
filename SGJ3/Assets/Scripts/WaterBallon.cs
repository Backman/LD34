using UnityEngine;

public class WaterBallon : MonoBehaviour
{
    [SerializeField]
    private GameObject ExplodeParticle;
    [SerializeField]
    private float _explosionRadius;

    [SerializeField]
    private AudioClip _hitClip;
    [SerializeField]
    private AudioClip _missClip;

    [HideInInspector]
    public float StunTime;
    [HideInInspector]
    public float Knockback;

    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rb.rotation = Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("TreeMouth") && !other.CompareTag("Tree"))
        {
            var layer = (1 << LayerMask.NameToLayer("Enemy"));
            var colliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius, layer);
            var hit = false;
            
            for (int i = 0; i < colliders.Length; i++)
			{
                var enemyCollider = colliders[i];
                var closestPoint = enemyCollider.bounds.ClosestPoint(transform.position);
                enemyCollider.GetComponent<BotControl>().Hit(closestPoint, Knockback, StunTime, HitType.WaterBallon);
                hit = true;
            }
            if (ExplodeParticle)
            {
                Instantiate(ExplodeParticle, transform.position, ExplodeParticle.transform.rotation);
            }
            
            Sound.Instance.PlayClipAtPoint(hit ? _hitClip : _missClip, transform.position);
            Destroy(gameObject);
        }
        else if (other.CompareTag("TreeMouth"))
        {
            if (GameManager.Instance.OnWaterTree != null)
            {
                if (ExplodeParticle)
                {
                    Instantiate(ExplodeParticle, transform.position, ExplodeParticle.transform.rotation);
                }
                
                Sound.Instance.PlayClipAtPoint(_hitClip, transform.position);
                GameManager.Instance.OnWaterTree();
                Destroy(gameObject);
            }
        }
    }
}