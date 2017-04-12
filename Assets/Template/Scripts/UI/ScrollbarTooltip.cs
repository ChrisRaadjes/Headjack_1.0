using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Headjack;
using TMPro;

public class ScrollbarTooltip : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler  
{
	// This class allows us to extend functionality of selectables
	// without completely overriding existing functionality. 
	// It also gives more callbacks than just OnValueChanged. 

	[System.Serializable]
	[HideInInspector] public class OnHover: UnityEvent{ };

	[System.Serializable]
	[HideInInspector] public class OnUnHover : UnityEvent{ };

	[System.Serializable]
	[HideInInspector]public class OnClick : UnityEvent{ };

	[System.Serializable] 
	[HideInInspector] public class OnDouble : UnityEvent { };

	[System.Serializable] 
	[HideInInspector] public class OnHoverEventData : UnityEvent { };

	[Header("Interactable Interfaces")]
	public OnHover onHover;
	public OnUnHover onUnhover;
	public OnClick onClick;

	[Header("Scrollbar")]
	public Scrollbar scrollbar;
	public RectTransform scrollbarRect;

	[Header("Tooltip")]
	public RectTransform scrollbarTooltip;
	public TextMeshProUGUI scrollbarTooltipText;

	[Header("Show Tooltip")]
	public bool showTooltip;

	private int axis;  // Travel direction of the referenced scrollbar
	public int Axis
	{
		get {
			if (scrollbar.direction == Scrollbar.Direction.BottomToTop || scrollbar.direction == Scrollbar.Direction.TopToBottom)
				return axis = 1;
			else // If the direction is left to right or right to left 
				return axis = 0;
		}
		set {
			axis = value;
		}
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		onClick.Invoke();
	}

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) 
	{
		if (gameObject == eventData.pointerEnter ||
		   IsGameObjectMatchInChildren (transform, eventData.pointerEnter))
		{
			ShowScrollbarTooltip ();

			showTooltip = true;
			ShowScrollbarTooltip ();
		}
	}

	void IPointerExitHandler.OnPointerExit(PointerEventData eventData) 
	{
		if (gameObject == eventData.pointerEnter ||
			IsGameObjectMatchInChildren (transform, eventData.pointerEnter))
		{
			showTooltip = false;
			HideSliderTooltip ();
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

	public void Start()
	{
		//Debug.Log ("AXIS VALUE IS " + Axis);

		// Set the anchored position of the slider tooltip to match the slider pivot itself
		Vector2 newAnchorMin = scrollbarTooltip.anchorMin;
		Vector2 newAnchorMax = scrollbarTooltip.anchorMax;

		newAnchorMin[Axis] = scrollbarRect.pivot[Axis];
		newAnchorMax[Axis] = scrollbarRect.pivot[Axis];

		scrollbarTooltip.anchorMin = newAnchorMin;
		scrollbarTooltip.anchorMax = newAnchorMax;

		// Zero out the position on the correct axis of the slider tooltip;
		Vector2 _tmp = scrollbarTooltip.anchoredPosition;
		_tmp[Axis] = 0f; 
		scrollbarTooltip.anchoredPosition = _tmp;

		//Hide it 
		HideSliderTooltip();
	}

	public void Update() 
	{
		if (showTooltip)
			UpdateSliderTooltip (VRUIInputModule.instance.gazeControllerData.pointerEvent);
	}

	public void ShowScrollbarTooltip() 
	{
		//Debug.Log ("Show slider...");
		scrollbarTooltip.gameObject.SetActive(true);
	}

	public void HideSliderTooltip()
	{
		scrollbarTooltip.gameObject.SetActive(false);
	}

	public void UpdateSliderTooltip(PointerEventData eventData) 
	{
		// TODO: Calculate the items we're going to be reviewing here.
	}
}

