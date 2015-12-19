using UnityEngine;

public class WaterSpawnPoint : MonoBehaviour
{
    public WaterBallonPickup Water { get; set; }

    private void Awake()
    {
        WaterSpawing.Instance.AddWaterSpawnPoint(this);
    }
}