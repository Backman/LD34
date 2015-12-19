using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class WaterSpawing
{
    public static WaterSpawing Instance { get; private set; }
    
    List<WaterSpawnPoint> _spawnPoints = new List<WaterSpawnPoint>();

    public GameObject WaterPickupPrefab;

    public void Awake()
    {
        Instance = this;
    }

    public void AddWaterSpawnPoint(WaterSpawnPoint spawnPoint)
    {
        _spawnPoints.Add(spawnPoint);
    }

    public void SpawnWater(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnWater();
        }
    }

    public void TrySpawnWater(int amount)
    {
        var enabledWater = 0;
        foreach (var spawnPoint in _spawnPoints)
        {
            if (spawnPoint.Water)
            {
                enabledWater++;
            }
        }
        SpawnWater(amount - enabledWater);
    }

    public void SpawnWater()
    {
        var spawnPoints = new List<WaterSpawnPoint>();
        foreach (var spawnPoint in _spawnPoints)
        {
            if (!spawnPoint.Water)
            {
                spawnPoints.Add(spawnPoint);
            }
        }

        if (spawnPoints.Count > 0)
        {
            var idx = Random.Range(0, spawnPoints.Count);
            var pickup = (GameObject)Object.Instantiate(WaterPickupPrefab, spawnPoints[idx].transform.position, Quaternion.identity);
            spawnPoints[idx].Water = pickup.GetComponent<WaterBallonPickup>();
        }
    }
}