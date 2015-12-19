using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class BotSpawning
{
    public static BotSpawning Instance { get; private set; }

    private BotSpawnSettings _settings;

    List<BotSpawnPoint> _SpawnPoints = new List<BotSpawnPoint>();
    float _SpawnAccumulator;
    float _spawnStart;

    public bool Spawning { get { return _settings != null && _spawnStart > Time.time; } }

    public void AddSpawnPoint(BotSpawnPoint point)
    {
        _SpawnPoints.Add(point);
    }

    public void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        if(!Spawning) return;
        
        _SpawnAccumulator += _settings.SpawnSpeed * Time.deltaTime;
        if (_SpawnAccumulator > 0)
        {
            float totalChance = 0f;
            for (int i = 0; i < _settings.BotSpawns.Length; i++)
            {
                var spawn = _settings.BotSpawns[i];
                totalChance += spawn.SpawnChance;
            }
            float chance = UnityEngine.Random.Range(0f, 1f)  * totalChance;
            float chanceAccumulator = 0f;
            for (int i = 0; i < _settings.BotSpawns.Length; i++)
            {
                var spawn = _settings.BotSpawns[i];
                if (chance < spawn.SpawnChance + chanceAccumulator)
                {
                    SpawnBot(spawn);
                    break;
                }
                chanceAccumulator += spawn.SpawnChance;
            }
        }
    }

    public void StartSpawn(BotSpawnSettings settings)
    {
        _settings = settings;
        _spawnStart = Time.time + settings.SpawnTime;
    }

    public void StopSpawn()
    {
        _settings = null;
    }

    private void SpawnBot(BotSpawnSettings.BotSpawnData spawn)
    {
        List<BotSpawnPoint> points = new List<BotSpawnPoint>();
        for (int i = 0; i < _SpawnPoints.Count; i++)
        {
            var sp = _SpawnPoints[i];
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(sp.transform.position);
            if (Mathf.Abs(viewportPoint.x) < 1f && Mathf.Abs(viewportPoint.y) < 1f)
                continue;
            points.Add(_SpawnPoints[i]);
        }
        if (points.Count == 0)
            return;
        var point = points[UnityEngine.Random.Range(0, points.Count)];
        _SpawnAccumulator -= spawn.SpawnCost;
        GameObject.Instantiate(spawn.Prefab, point.transform.position, Quaternion.identity);
    }
}
