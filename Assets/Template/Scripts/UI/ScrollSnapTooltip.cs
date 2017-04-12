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

public class ScrollSnapTooltip : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
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

	/*
	[Header("Referenced Slider Variables")]
	public Slider slider;
	public RectTransform sliderRect;

	[Header("Tooltip Variables")]
	public RectTransform sliderTooltip;
	public TextMeshProUGUI sliderTooltipText;

	[Header("Show Tooltip On Hover")]
	public bool showSliderTooltip;
	*/
	public ScrollRect scrollRect;
	public RectTransform scrollbarRect;
	public ScrollSnapBase scrollSnap;

	//Debug

	public TextMeshProUGUI debugTextContainerSize;
	public TextMeshProUGUI debugTextEventDataRaw;
	public TextMeshProUGUI debugTextEventDataProcessed;
	public TextMeshProUGUI debugTextStartingContainerPosition;
	public TextMeshProUGUI debugTextContainerPosition;
	public TextMeshProUGUI debugTextPredictedContainerPosition;
	public TextMeshProUGUI debugTextScrubValue;
	public TextMeshProUGUI debugTextPredictedPage;
	public TextMeshProUGUI debugTextContainerMinX;
	public TextMeshProUGUI debugTextContainerMaxX;
	public TextMeshProUGUI debugTextContainerMinY;
	public TextMeshProUGUI debugTextContainerMaxY;
	public TextMeshProUGUI debugTextContainerCenterPos;



	bool showSliderTooltip;

	private int axis;  // Travel direction of the referenced slider
	public int Axis
	{
		get {
			if (scrollRect.horizontal == true && scrollRect.vertical == false)
			{
				return axis = 0;
			}
			else
			if (scrollRect.horizontal == false && scrollRect.vertical == true)
			{
				return axis = 1;
			}
			else
			{
				return axis = 0;
				Debug.Log ("WARNING: Scrollrect setup to use both horizontal and vertical! Defaulting to 0");
			}
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
			showSliderTooltip = true;
		}
	}

	void IPointerExitHandler.OnPointerExit(PointerEventData eventData) 
	{
		if (gameObject == eventData.pointerEnter ||
			IsGameObjectMatchInChildren (transform, eventData.pointerEnter))
		{
			showSliderTooltip = false;
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
	}

	public void Update() 
	{
		PointerEventData eventData = EventSystem.current.gameObject.GetComponent<StandaloneInputModuleCustom>().GetLastPointerEventDataPublic(-1);

		GetNormalizedPosition(eventData);
	}

	public void ShowSliderTooltip() 
	{
	}

	public void HideSliderTooltip()
	{
	}

	public void UpdateSliderTooltip(PointerEventData eventData) 
	{
	}

	public void Somefunction() 
	{
		// We need to calculate which vector3 the normalized bounds position will have; and then pass that to the base.
	}

	public void SomeFunc(PointerEventData eventData) 
	{
		GetNormalizedPosition(eventData);
		//Vector3 predictedWorldPosition = GetPredictedContentPosition(normalizedPosition);
		//int predictedPage = scrollSnap.GetPageforPosition (predictedWorldPosition);
		//Debug.Log ("Preview page is " + predictedPage);
	}

	/// <summary>s
	/// Gets the closest page to the point the user clicked inside the scrollbar.
	/// </summary>
	public void GetPredictedContentPosition(float normalizedScrollValue)
	{
		//Find the bounds of the viewport and content
		Bounds viewBounds = new Bounds (scrollRect.viewport.rect.center, scrollRect.viewport.rect.size);
		Bounds contentBounds = new Bounds (scrollRect.content.rect.center, scrollRect.content.rect.size);
		RectTransform contentRect = scrollRect.content;

		debugTextContainerMinX.text = "Min X: " + scrollRect.content.rect.min.x;
		debugTextContainerMaxX.text = "Max X: " + scrollRect.content.rect.max.x;
		debugTextContainerMinY.text = "Min Y: " + scrollRect.content.rect.min.y;
		debugTextContainerMaxY.text = "Max Y: " + scrollRect.content.rect.max.y;
		debugTextContainerCenterPos.text = "Center: " + scrollRect.content.rect.center;


		// How much larger the content is then the scroll view.
		float hiddenLength = contentBounds.size[Axis] - viewBounds.size[Axis];
		// Where the position of the lower left corner of the contents bounds should be, in the space of the view
		//float contentBoundsMinPosition = contentRect.localPosition[Axis] + (normalizedScrollValue * hiddenLength);
		float contentBoundsMinPosition = scrollSnap._scrollStartPosition + (normalizedScrollValue * hiddenLength) * -1f;
		Debug.Log ("Content Bounds MIN POS: " + contentBoundsMinPosition);
		// The new content localPosition in the space of the view.
		float newLocalPosition = contentBoundsMinPosition;

		Vector3 localPosition = Vector3.zero;
		localPosition [Axis] = scrollSnap._scrollStartPosition;

		//Debug.Log ("Content rect CURRENT position is " + localPosition);
		debugTextContainerPosition.text = "Current Container Position: " + scrollSnap._screensContainer.localPosition;
		localPosition[Axis] = newLocalPosition;
		debugTextPredictedContainerPosition.text = "Predicted Container Position " + localPosition;
		//Debug.Log ("Content rect UPDATED position is " + localPosition);

		int predictedPagePosition = scrollSnap.GetPageforPosition(localPosition);
		//Debug.Log ("Content rect page is " + predictedPagePosition);
		debugTextPredictedPage.text = "Predicted Page: " + predictedPagePosition;
	}


	/// <summary>
	/// Get the normalized position of where the mouse clicked.
	/// </summary>
	public void GetNormalizedPosition(PointerEventData eventData)
	{
		debugTextContainerSize.text = "Container Size: " + scrollSnap._screensContainer.rect.size;

		debugTextStartingContainerPosition.text = "Starting Scroll Position " + scrollSnap._scrollStartPosition;

		Vector2 localCursorPoint = Vector2.zero;

		RectTransformUtility.ScreenPointToLocalPointInRectangle (scrollbarRect, eventData.position, eventData.enterEventCamera, out localCursorPoint);

		debugTextEventDataRaw.text = "Local Cursor Point: " + localCursorPoint;

		localCursorPoint -=scrollbarRect.rect.position;
		debugTextEventDataProcessed.text = "Processed Cursor Point: " + localCursorPoint;

		float gazeScrubValue = Mathf.Clamp01(localCursorPoint[Axis] / scrollbarRect.rect.size[Axis]);
		debugTextScrubValue.text = "Scrub Value: " + gazeScrubValue;
		//Debug.Log ("Normalized position " + gazeScrubValue);

		// Try and get a page prediction for this scrub value
		GetPredictedContentPosition(gazeScrubValue);
	}


}

