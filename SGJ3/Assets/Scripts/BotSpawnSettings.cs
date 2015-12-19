using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu]
public class BotSpawnSettings : ScriptableObject
{
    [Serializable]
    public struct BotSpawnData
    {
        public string Name;
        public GameObject Prefab;
        public float SpawnCost;
        public float SpawnChance;
    }
    public float SpawnSpeed;
    public float SpawnTime;
    public BotSpawnData[] BotSpawns;

}
