using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Headjack;
using TMPro;

public class SliderSeekTooltip : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
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
	private float newSeekValue;
	private Vector2 offset;

	public int Axis
	{
		get { return (slider.direction == Slider.Direction.BottomToTop || slider.direction == Slider.Direction.TopToBottom) ? 1 : 0; }
	}

	public bool ReverseValue
	{
		get { return (slider.direction == Slider.Direction.RightToLeft || slider.direction == Slider.Direction.TopToBottom); }
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{

		//Fuck it, brute force this: 
		VideoControls.instance.SeekVideoTime(newSeekValue);
		Debug.Log ("INPUT HANDLER: Setting seek at " + newSeekValue);

		onClick.Invoke();
	}

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) 
	{
		if (gameObject == eventData.pointerEnter ||
			IsGameObjectMatchInChildren (transform, eventData.pointerEnter))
		{
			showSliderTooltip = true;
			ShowSliderTooltip (true);
		}
	}

	void IPointerExitHandler.OnPointerExit(PointerEventData eventData) 
	{
		if (gameObject == eventData.pointerEnter ||
			IsGameObjectMatchInChildren (transform, eventData.pointerEnter))
		{
			showSliderTooltip = false;
			ShowSliderTooltip(false);
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
		ShowSliderTooltip(false);
	}

	public void Update() 
	{
		if (showSliderTooltip)
			UpdateSliderTooltip (VRUIInputModule.instance.gazeControllerData.pointerEvent);

		/*
		if (showSliderTooltip) 
		{
			PointerEventData eventData = EventSystem.current.gameObject.GetComponent<StandaloneInputModuleCustom> ().GetLastPointerEventDataPublic (-1);
		}
		*/

	}

	public void ShowSliderTooltip(bool visibility) 
	{
		sliderTooltip.gameObject.SetActive(visibility);

		if (showPreview) 
		{
			sliderFillPreview.gameObject.SetActive (visibility);
			sliderHandlePreview.gameObject.SetActive (visibility);
		}
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
	{
		Debug.Log ("Pointer is down");
		offset = Vector2.zero;
	}


	public void UpdateSliderTooltip(PointerEventData eventData)
	{
		Vector2 localCursorPoint;

		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle (sliderRect, eventData.position, eventData.enterEventCamera, out localCursorPoint)) {
			return;
		}

		// Adjust the tooltip location
		Vector2 _tmp = sliderTooltip.anchoredPosition;
		_tmp.x = localCursorPoint.x;
		sliderTooltip.anchoredPosition = _tmp;

		// Get the normalized position of where we are looking
		localCursorPoint -= sliderRect.rect.position;

		float val = Mathf.Clamp01 (localCursorPoint [Axis] / sliderRect.rect.size [Axis]);
		float gazeNormalizedValue = (ReverseValue ? 1f - val : val);

		// Adjust the hit target preview based on the cursor point
		if (showPreview) 
		{
			DrivePreviewImages(gazeNormalizedValue);
		}

		newSeekValue = gazeNormalizedValue;

		// Show the timespan of the video on preview.
		TimeSpan videoTimespan = TimeSpan.FromMilliseconds (ConvertDurationToMS (App.Player.Duration, gazeNormalizedValue));
		WriteVideoTime(videoTimespan);
	}
		
	/// <summary>
	/// Takes a double and converts it to a rounded milisecond time to use as a video timespan. 
	/// This function is called once per play of a video.
	/// </summary>
	public long ConvertDurationToMS(long duration, float percentage)
	{
		double durationDouble = (double)duration;
		double gazePointTime = durationDouble * ((double)percentage);
		return (long)gazePointTime;
	}

	/// <summary>
	/// Writes out the video time string;
	/// Should only be called once per video play.
	/// </summary>
	public void WriteVideoTime(TimeSpan videoTimespan)
	{
		string format = null;

		if (videoTimespan.Hours > 0) 
		{
			sliderTooltipText.text = string.Format ("{0:00}:{1:00}:{2:00}", videoTimespan.Hours, videoTimespan.Minutes, videoTimespan.Seconds);
		} 
		else 
		{
			if (videoTimespan.Minutes < 10) 
			{
				sliderTooltipText.text = string.Format("{0}:{1:00}", videoTimespan.Minutes, videoTimespan.Seconds);
			} 
			else 
			{
				sliderTooltipText.text = string.Format ("{0:00}:{1:00}", videoTimespan.Minutes, videoTimespan.Seconds);
			}
		}
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

