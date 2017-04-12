﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ExtendedInputHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler  
{

	// This class allows us to extend functionality of selectables
	// without completely overriding existing functionality. 
	// It also gives more callbacks than just OnValueChanged. 

	[System.Serializable]
	public class OnHover: UnityEvent{ };

	[System.Serializable]
	public class OnUnHover : UnityEvent{ };

	[System.Serializable]
	public class OnClick : UnityEvent{ };

	[System.Serializable] 
	public class OnDouble : UnityEvent { };

	[System.Serializable] 
	public class OnHoverEventData : UnityEvent { };

	public OnHover onHover;
	public OnUnHover onUnhover;
	public OnClick onClick;

	// Double click is not supported in VR
	// public OnDouble onDouble;

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		onClick.Invoke();
	}

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) 
	{
		if (onHover == null)
			return;
		if (gameObject == eventData.pointerEnter ||
		   IsGameObjectMatchInChildren (transform, eventData.pointerEnter))
		{
			onHover.Invoke();
		}
	}

	void IPointerExitHandler.OnPointerExit(PointerEventData eventData) 
	{
		if (onHover == null)
			return;
		if (gameObject == eventData.pointerEnter ||
			IsGameObjectMatchInChildren (transform, eventData.pointerEnter))
		{
			onHover.Invoke();
		}
	}

	private bool IsGameObjectMatchInChildren(Transform parent, GameObject gameObject)
	{	
		foreach (Transform child in parent)
		{
			if (!child.gameObject.activeSelf)
				continue;

			if(child.gameObject == gameObject 
				|| IsGameObjectMatchInChildren(child, gameObject)) // Search all children of object
			{
				return true;
			}
		}

		return false;
	}
}