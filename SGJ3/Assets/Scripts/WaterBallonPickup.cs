using UnityEngine;
using System.Collections;

public class WaterBallonPickup : Pickup 
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Apply(new PickupData() { Pickuper = other.gameObject });
        }
    }

    protected override void InnerApply(PickupData data)
    {
        data.Pickuper.GetComponent<GardenerController>().Canon.AddAmmo();
        if (_pickupParticle)
        {
            Instantiate(_pickupParticle, transform.position, Quaternion.identity);
        }
        // Play pickup sound
        Destroy(gameObject);
    }
}
