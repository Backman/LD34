using UnityEngine;
using System.Collections;

public abstract class Pooling : MonoBehaviour
{
    public GameObject Prefab { get; set; }
}
public class ParticlePooling : Pooling
{
    ParticleSystem _Particle;
    void Awake()
    {
        _Particle = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (_Particle.IsAlive() == false && _Particle.particleCount == 0)
        {
            ObjectPool.Return(Prefab, this.gameObject);
        }
    }

}
