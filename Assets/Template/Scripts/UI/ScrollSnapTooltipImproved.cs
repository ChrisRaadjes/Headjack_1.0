using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Headjack;
using TMPro;

public class ScrollSnapTooltipImproved : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
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

	[Header("Referenced Scrollsnap Variables")]
	public ScrollRect scrollRect;
	public ScrollSnapBase scrollSnap;

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
	public bool showSliderTooltip;

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
		/*
		if (showSliderTooltip)
			UpdateSliderTooltip (VRUIInputModule.instance.gazeControllerData.pointerEvent);
		*/

		PointerEventData eventData = EventSystem.current.gameObject.GetComponent<StandaloneInputModuleCustom> ().GetLastPointerEventDataPublic (-1);
		UpdateSliderTooltip (eventData);
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

	public void UpdateSliderTooltip(PointerEventData eventData) 
	{
		Vector2 localCursorPoint;

		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle (sliderRect, eventData.position, eventData.enterEventCamera, out localCursorPoint))
		{
			return;
		}

		// Adjust the tooltip location
		Vector2 _tmp = sliderTooltip.anchoredPosition;
		_tmp.x = localCursorPoint.x;
		sliderTooltip.anchoredPosition = _tmp;

		// Get the normalized position of where we are looking
		localCursorPoint -= sliderRect.rect.position;

		float val = Mathf.Clamp01(localCursorPoint[Axis] / sliderRect.rect.size[Axis]);
		float normalizedValue = (ReverseValue ? 1f - val : val);

		int predictedPage = GetPageForNormalizedValue(normalizedValue);

		Vector3 predictedPagePosition = Vector3.zero;
		scrollSnap.GetPositionforPage(predictedPage, ref predictedPagePosition);

		// Find values to normalize
		int childCount = scrollSnap._screensContainer.childCount;
		float maximumPosition = scrollSnap._screensContainer.GetChild (0).gameObject.transform.localPosition[Axis] - (scrollSnap._scroll_rect.viewport.rect.size[Axis] * 0.5f);
		Debug.Log ("Maximum position " + maximumPosition);
		float minimumPosition = scrollSnap._screensContainer.GetChild (childCount - 1).transform.localPosition[Axis] - (scrollSnap._scroll_rect.viewport.rect.size[Axis] * 0.5f);
		Debug.Log("Minimum position " + minimumPosition);

		// Normalize the value
		float pageNormalizedValue = (predictedPagePosition[Axis] - minimumPosition) / (maximumPosition - minimumPosition);
		Debug.Log ("Page: " + predictedPage + " Value: " + pageNormalizedValue + " Transform Position: " + predictedPagePosition);

		// Adjust the hit target preview based on the cursor point
		if (showPreview) 
		{
			tracker.Clear();

			// Adjust the position of the fill image using anchors
			if (sliderFillPreview != null) 
			{
				tracker.Add(slider, sliderFillPreview, DrivenTransformProperties.Anchors);

				Vector2 anchorMin = Vector2.zero;
				Vector2 anchorMax = Vector2.one;

				if(sliderFillImage != null && sliderFillImage.type == Image.Type.Filled) 
				{
					sliderFillImage.fillAmount = pageNormalizedValue;
				}
				else
				{
					if(ReverseValue)
						anchorMin[Axis] = 1 - pageNormalizedValue;
					else
						anchorMax[Axis] = pageNormalizedValue;
				}

				sliderFillPreview.anchorMin = anchorMin;
				sliderFillPreview.anchorMax = anchorMax;
			}

			// Adjust the position of the handle image using a moving anchored point
			if (sliderHandlePreview != null) 
			{
				tracker.Add (slider, sliderHandlePreview, DrivenTransformProperties.Anchors);

				Vector2 anchorMin = Vector2.zero;
				Vector2 anchorMax = Vector2.one;

				anchorMin [Axis] = anchorMax [Axis] = (ReverseValue ? (1 - pageNormalizedValue) : pageNormalizedValue);
				sliderHandlePreview.anchorMin = anchorMin;
				sliderHandlePreview.anchorMax = anchorMax;
			}
		}
	}

	public int GetPageForNormalizedValue (float normalizedValue)
	{
		//Find the bounds of the viewport and content
		Bounds viewBounds = new Bounds (scrollRect.viewport.rect.center, scrollRect.viewport.rect.size);
		Bounds contentBounds = new Bounds (scrollRect.content.rect.center, scrollRect.content.rect.size);
		RectTransform contentRect = scrollRect.content;

		// How much larger the content is then the scroll view.
		float hiddenLength = contentBounds.size[Axis] - viewBounds.size[Axis];

		// Where the position of the lower left corner of the contents bounds should be, in the space of the view
		float contentBoundsMinPosition = scrollSnap._scrollStartPosition + (normalizedValue * hiddenLength) * -1f;

		// The new content localPosition in the space of the view.
		float newLocalPosition = contentBoundsMinPosition;

		Vector3 localPosition = Vector3.zero;
		localPosition [Axis] = scrollSnap._scrollStartPosition;

		localPosition[Axis] = newLocalPosition;

		return scrollSnap.GetPageforPosition(localPosition);
	}
}

