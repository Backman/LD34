using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class BallonCounter : MonoBehaviour
{
	[SerializeField]
	private Image _ballonImage;
	[SerializeField]
	private Color _enabledColor;
	[SerializeField]
	private Color _disabledColor;
	
	[SerializeField]
	private float _padding = -20f;
	[SerializeField]
	private float _spacing = 30f;
	[SerializeField]
	private int _maxPerCol = 10;

	private WaterCanon _canon;

	private Image[] _images;

	private void Awake()
	{
		GameManager.Instance.OnAddAmmo += OnAddAmmo;
		GameManager.Instance.OnRemoveAmmo += OnRemoveAmmo;
	//}

	//private void Start()
	//{
		var canon = FindObjectOfType<WaterCanon>();
		var rect = GetComponent<RectTransform>();
		
		_images = new Image[canon.MaxAmmo];
		float x = 0f;
		for (int i = 0; i < canon.MaxAmmo; i += _maxPerCol)
		{
			float y = _padding;
			for (int j = 0; j < _maxPerCol; j++)
			{
				var image = (Image)Instantiate(_ballonImage, rect.position, Quaternion.identity);
				image.name = "Ammo count " + (j + i + 1);
				image.color = _disabledColor;
				image.gameObject.SetActive(true);
				image.transform.SetParent(transform, true);
				image.rectTransform.localScale = Vector3.one;
				var pos = image.rectTransform.anchoredPosition;

				pos.y = y;
				pos.x = x;
				image.rectTransform.anchoredPosition = pos;
				y -= _spacing;

				_images[j + i] = image;
			}
			x += _spacing;
		}
	}

	private void Update()
	{
		//float y = _padding;
		//for (int i = 0; i < _images.Length; i++)
		//{
		//	var imageRect = _images[i].GetComponent<RectTransform>();
		//	var pos = imageRect.anchoredPosition;
		//	pos.y = y;
		//	imageRect.anchoredPosition = pos;
		//	y -= _spacing;
		//}
	}

	public void EnableAmmo(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			_images[i].color = _enabledColor;
		}
	}
	
	private void DisableAmmo(int amount)
	{
		for (int i = _images.Length - 1; i >= amount; i--)
		{
			_images[i].color = _disabledColor;
		}
	}

	private void OnAddAmmo(int addedAmount, int currentAmount)
	{
		EnableAmmo(currentAmount);
	}

	private void OnRemoveAmmo(int addedAmount, int currentAmount)
	{
		DisableAmmo(currentAmount);
	}
}
