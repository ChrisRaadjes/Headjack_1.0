using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Headjack;
using TMPro;

public class SliderTooltip : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler  
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

	[Header("Referenced Slider Variables")]
	public Slider slider;
	public RectTransform sliderRect;

	[Header("Tooltip Variables")]
	public RectTransform sliderTooltip;
	public TextMeshProUGUI sliderTooltipText;

	[Header("Show Tooltip On Hover")]
	public bool showSliderTooltip;

	private int axis;  // Travel direction of the referenced slider
	public int Axis
	{
		get {
			if (slider.direction == Slider.Direction.BottomToTop || slider.direction == Slider.Direction.TopToBottom)
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
			ShowSliderTooltip ();

			showSliderTooltip = true;
			ShowSliderTooltip ();
		}
	}

	void IPointerExitHandler.OnPointerExit(PointerEventData eventData) 
	{
		if (gameObject == eventData.pointerEnter ||
			IsGameObjectMatchInChildren (transform, eventData.pointerEnter))
		{
			showSliderTooltip = false;
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
		Vector2 newAnchorMin = sliderTooltip.anchorMin;
		Vector2 newAnchorMax = sliderTooltip.anchorMax;

		newAnchorMin[Axis] = sliderRect.pivot[Axis];
		newAnchorMax[Axis] = sliderRect.pivot[Axis];

		sliderTooltip.anchorMin = newAnchorMin;
		sliderTooltip.anchorMax = newAnchorMax;

		// Zero out the position on the correct axis of the slider tooltip;
		Vector2 _tmp = sliderTooltip.anchoredPosition;
		_tmp[Axis] = 0f; 
		sliderTooltip.anchoredPosition = _tmp;

		//Hide it 
		HideSliderTooltip();
	}

	public void Update() 
	{
		if (showSliderTooltip)
			UpdateSliderTooltip (VRUIInputModule.instance.gazeControllerData.pointerEvent);
	}

	public void ShowSliderTooltip() 
	{
		//Debug.Log ("Show slider...");
		sliderTooltip.gameObject.SetActive(true);
	}

	public void HideSliderTooltip()
	{
		//Debug.Log ("Stop updating slider position");
		sliderTooltip.gameObject.SetActive(false);
	}

	public void UpdateSliderTooltip(PointerEventData eventData) 
	{
		//Debug.Log ("Updating slider tooltip");
		Vector2 localCursorPoint = Vector2.zero;

		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle (sliderRect, eventData.position, eventData.enterEventCamera, out localCursorPoint))
		{
			return;
		}

		//Debug.Log ("BEFORE: Slider Tooltip position is " + sliderTooltip.anchoredPosition);

		Vector2 _tmp = sliderTooltip.anchoredPosition;
		_tmp.x = localCursorPoint.x;
		sliderTooltip.anchoredPosition = _tmp;


		//Debug.Log ("Local Cursor Point X is " + localCursorPoint);
		//Debug.Log ("AFTER: Slider Tooltip position is " + sliderTooltip.anchoredPosition);

		if (HeadjackStartup.instance.playingProject)
		{
			localCursorPoint -= sliderRect.rect.position;

			float gazeScrubValue = Mathf.Clamp01(localCursorPoint[Axis] / sliderRect.rect.size[Axis]);

			TimeSpan gazeVideoTime = TimeSpan.FromMilliseconds(ConvertDuration(App.Player.Duration, gazeScrubValue));
			sliderTooltipText.text = gazeVideoTime.ToString();
		}
	}

	// Takes a double converts it to a rounded milisecond time for the timespan preview
	public long ConvertDuration(long duration, float percentage) 
	{
		double durationDouble = (double)duration;
		double gazePointTime = durationDouble * ((double)percentage);
		return (long)gazePointTime;
	}
}

