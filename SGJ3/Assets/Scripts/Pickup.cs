using UnityEngine;

public struct PickupData
{
    public GameObject Pickuper;
}

public abstract class Pickup : MonoBehaviour
{
    [SerializeField]
    protected AudioClip _clip;
    
    [SerializeField]
    protected GameObject _pickupParticle;

    public virtual void Apply(PickupData data) 
    {
        Sound.Instance.PlayClipAtPoint(_clip, transform.position);
        InnerApply(data);
    }

    protected abstract void InnerApply(PickupData data);
}