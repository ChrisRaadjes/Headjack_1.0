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


	bool showSliderTooltip;

	private int axis;  // Travel direction of the referenced slider
	public int Axis
	{
		get {
			if (scrollRect.horizontal == true && scrollRect.vertical == false)
			{
				return axis = 1;
			}
			else
			if (scrollRect.horizontal == false && scrollRect.vertical == true)
			{
				return axis = 0;
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
		if (showSliderTooltip)
			SomeFunc(VRUIInputModule.instance.gazeControllerData.pointerEvent);
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
		float normalizedPosition = GetNormalizedPosition(eventData);
		Vector3 predictedWorldPosition = GetPredictedContentPosition(normalizedPosition);
		int predictedPage = scrollSnap.GetPageforPosition (predictedWorldPosition);
		Debug.Log ("Preview page is " + predictedPage);
	}

	/// <summary>s
	/// Gets the closest page to the point the user clicked inside the scrollbar.
	/// </summary>
	public Vector3 GetPredictedContentPosition(float normalizedScrollValue)
	{
		//Find the bounds of the viewport and content
		Bounds viewBounds = new Bounds (scrollRect.viewport.rect.center, scrollRect.viewport.rect.size);
		Bounds contentBounds = new Bounds (scrollRect.content.rect.center, scrollRect.content.rect.size);
		RectTransform contentRect = scrollRect.content;

		// How much larger the content is then the scroll view.
		float hiddenLength = contentBounds.size[Axis] - viewBounds.size[axis];
		// Where the position of the lower left corner of the contents bounds should be, in the space of the view
		float contentBoundsMinPosition = contentRect.localPosition[axis] - (normalizedScrollValue * hiddenLength);
		// The new content localPosition in the space of the view.
		float newLocalPosition = contentRect.localPosition[Axis] + contentBoundsMinPosition - contentBounds.min[axis];

		Vector3 localPosition = contentRect.localPosition;
		localPosition[0] = newLocalPosition;
		localPosition[1] = newLocalPosition;
		//Vector3 localPositionToWorld = scrollRect.viewport.transform.TransformPoint (localPosition);
		return localPosition;
	}


	/// <summary>
	/// Get the normalized position of where the mouse clicked.
	/// </summary>
	public float GetNormalizedPosition(PointerEventData eventData)
	{
		Vector2 localCursorPoint = Vector2.zero;
		Debug.Log ("EVENT DATA " + eventData.position + eventData.enterEventCamera + localCursorPoint);

		RectTransformUtility.ScreenPointToLocalPointInRectangle (scrollbarRect, eventData.position, eventData.enterEventCamera, out localCursorPoint);



		localCursorPoint -=scrollbarRect.rect.position;
		float gazeScrubValue = Mathf.Clamp01(localCursorPoint[Axis] / scrollbarRect.rect.size[Axis]);
		return gazeScrubValue;
	}


}

