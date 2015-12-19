using UnityEngine;
using System.Collections;

public class BotSpawnPoint : MonoBehaviour
{
    void Awake()
    {
        BotSpawning.Instance.AddSpawnPoint(this);
    }
}
