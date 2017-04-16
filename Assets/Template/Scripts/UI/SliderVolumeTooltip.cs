using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Headjack;
using TMPro;

public class SliderVolumeTooltip : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
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

	[Header("Optional Variables")]
	public bool showPreview;
	public RectTransform sliderFillPreview;
	public Image sliderFillImage;
	public RectTransform sliderHandlePreview;
	private DrivenRectTransformTracker tracker;

	[Header("Show Tooltip On Hover")]
	private bool showSliderTooltip;
	private Vector2 offset;

	private float newVolume;

	public int Axis
	{
		get { return (slider.direction == Slider.Direction.LeftToRight || slider.direction == Slider.Direction.TopToBottom) ? 1 : 0; }
	}

	public bool ReverseValue
	{
		get { return (slider.direction == Slider.Direction.RightToLeft || slider.direction == Slider.Direction.TopToBottom); }
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{

		VideoControls.instance.SetVolume (newVolume);
		//Debug.Log ("Setting Volume at " + newVolume);
		
		onClick.Invoke();
	}

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) 
	{
		if (gameObject == eventData.pointerEnter ||
			IsGameObjectMatchInChildren (transform, eventData.pointerEnter))
		{
			//showSliderTooltip = true;
			//ShowSliderTooltip (true);
		}
	}

	void IPointerExitHandler.OnPointerExit(PointerEventData eventData) 
	{
		if (gameObject == eventData.pointerEnter ||
			IsGameObjectMatchInChildren (transform, eventData.pointerEnter))
		{
			//showSliderTooltip = false;
			//ShowSliderTooltip(false);
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
		//Hide it 
		//ShowSliderTooltip(false);
	}

	public void Update() 
	{
		if (showSliderTooltip)
			UpdateSliderTooltip (VRUIInputModule.instance.gazeControllerData.pointerEvent);
	}

	public void ShowSliderTooltip(bool visibility) 
	{
		/*
		sliderTooltip.gameObject.SetActive(visibility);

		if (showPreview) 
		{
			sliderFillPreview.gameObject.SetActive (visibility);
			sliderHandlePreview.gameObject.SetActive (visibility);
		}
		*/
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
	{
		//Debug.Log ("Pointer is down");
		offset = Vector2.zero;
	}


	public void UpdateSliderTooltip(PointerEventData eventData)
	{
		Vector2 localCursorPoint;

		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle (sliderRect, eventData.position, eventData.enterEventCamera, out localCursorPoint)) {
			return;
		}
			
		// Get the normalized position of where we are looking
		localCursorPoint -= sliderRect.rect.position;

		float val = Mathf.Clamp01 (localCursorPoint [Axis] / sliderRect.rect.size [Axis]);
		float gazeNormalizedValue = (ReverseValue ? 1f - val : val);

		// Adjust the hit target preview based on the cursor point
		if (showPreview) 
		{
			DrivePreviewImages(gazeNormalizedValue);
		}

		newVolume = gazeNormalizedValue;
		sliderTooltipText.text = (Mathf.Floor(gazeNormalizedValue * 100f)).ToString();
	}

	public void DrivePreviewImages(float gazeNormalizedValue) 
	{
		tracker.Clear ();

		// Adjust the position of the fill image using anchors
		if (sliderFillPreview != null) {
			tracker.Add (slider, sliderFillPreview, DrivenTransformProperties.Anchors);

			Vector2 anchorMin = Vector2.zero;
			Vector2 anchorMax = Vector2.one;

			if (sliderFillImage != null && sliderFillImage.type == Image.Type.Filled) {
				sliderFillImage.fillAmount = gazeNormalizedValue;
			} else {
				if (ReverseValue)
					anchorMin [Axis] = 1 - gazeNormalizedValue;
				else
					anchorMax [Axis] = gazeNormalizedValue;
			}

			sliderFillPreview.anchorMin = anchorMin;
			sliderFillPreview.anchorMax = anchorMax;
		}

		// Adjust the position of the handle image using a moving anchored point.
		if (sliderHandlePreview != null) {
			tracker.Add (slider, sliderHandlePreview, DrivenTransformProperties.Anchors);

			Vector2 anchorMin = Vector2.zero;
			Vector2 anchorMax = Vector2.one;

			anchorMin [Axis] = anchorMax [Axis] = (ReverseValue ? (1 - gazeNormalizedValue) : gazeNormalizedValue);
			sliderHandlePreview.anchorMin = anchorMin;
			sliderHandlePreview.anchorMax = anchorMax;
		}
	}
}

