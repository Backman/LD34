using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	private static readonly bool _deleteExisitingInstances = true;

	public static T Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = FindObjectOfType<T>();

				if (!_instance)
				{
					_instance = new GameObject(string.Format("Singleton<{0}>", typeof(T).Name)).AddComponent<T>();
				}
				else
				{
					var instances = new List<T>(FindObjectsOfType<T>());
					if (_deleteExisitingInstances && instances.Count > 1)
					{
						DeleteExistingInstances(_instance, instances);
					}
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		if (ShouldPersist())
		{
			Init();
		}
	}

	private bool ShouldPersist()
	{
		if (_instance == null)
		{
			_instance = this as T;
		}
		else if (_instance != this)
		{
			Destroy(gameObject);
			return false;
		}

		DontDestroyOnLoad(gameObject);
		return true;
	}

	protected virtual void Init() { }

	private static void DeleteExistingInstances(T instanceToSave, List<T> otherInstances)
	{
		for (int i = otherInstances.Count - 1; i >= 0; i--)
		{
			if (instanceToSave != otherInstances[i])
			{
				otherInstances.RemoveAt(i);
			}
		}
	}
}