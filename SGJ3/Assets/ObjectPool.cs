using UnityEngine;
using System.Collections.Generic;

public class ObjectPool
{
    public static Dictionary<GameObject, Stack<GameObject>> _PoolMap = new Dictionary<GameObject, Stack<GameObject>>();

    public static GameObject GetInstance(GameObject prefab)
    {
        return GetInstance(prefab, Vector3.zero, Quaternion.identity);
    }
    public static GameObject GetInstance(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        Stack<GameObject> pool;
        if (_PoolMap.TryGetValue(prefab, out pool) == false)
            pool = _PoolMap[prefab] = new Stack<GameObject>();

        GameObject obj;
        if (pool.Count == 0)
        {
            obj = GameObject.Instantiate(prefab);
        }
        else
        {
            obj = pool.Pop();
        }
        foreach (var item in obj.GetComponentsInChildren<Pooling>())
        {
            item.Prefab = prefab;
        }
        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        return obj;
    }

    public static void Clear()
    {
        _PoolMap = new Dictionary<GameObject, Stack<GameObject>>();
    }
    public static void Return(GameObject prefab, GameObject instance)
    {
        Stack<GameObject> pool;
        if (_PoolMap.TryGetValue(prefab, out pool) == false)
            pool = _PoolMap[prefab] = new Stack<GameObject>();
        instance.SetActive(false);
        pool.Push(instance);
    }
}
