using UnityEngine;
using System.Collections;

public class DestroyParticle : MonoBehaviour
{
    private ParticleSystem _system;

    private void Awake()
    {
        _system = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!_system.IsAlive(true))
        {
            Destroy(gameObject);
        }
    }
}
