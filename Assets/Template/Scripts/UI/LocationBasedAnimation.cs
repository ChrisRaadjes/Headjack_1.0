using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class LocationBasedAnimation : MonoBehaviour {

	[Header("Animator")]
	public Animator animator;

	[Header("Reference RectTransform")]
	public RectTransform referenceRect;

	[Header("Object RectTransform")]
	public RectTransform targetRect;
	public RectTransform childRect;

	[Header("Animated Properties")]
	public CanvasGroup canvasGroup;

	[Header("Reference Scrollbar")]
	public Scrollbar scrollbar;

	[Header("Margin")]
	public float margin = 0.2f;

	public int axis = 1;

	void Start() 
	{
		
		Debug.Log ("MAX axis: " + referenceRect.rect.max [axis]);
		Debug.Log ("MIN region: " + referenceRect.rect.size[axis] * margin);
	}

	void Update()
	{
		SetAnimationFromLocalPosition();
	}

	public void SetAnimationFromLocalPosition()
	{
		Vector2 localPoint = Vector2.zero;
		Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint (null, targetRect.position);

		RectTransformUtility.ScreenPointToLocalPointInRectangle (referenceRect, screenPoint, null, out localPoint);

		localPoint -= referenceRect.rect.position;

		float percentage = 1f;
		float localPointOnAxis = localPoint[axis];
		float referenceAxisSize = referenceRect.rect.size [axis];
		float marginSize = referenceAxisSize * margin;
		//childRect.anchoredPosition = Vector2.zero;

		if (localPointOnAxis < referenceAxisSize * margin)
		{
			percentage = (localPointOnAxis / (referenceAxisSize * margin));
		}
		else
		if (localPointOnAxis > referenceAxisSize * (1f - margin))
		{
			float positionRelativeToMargin = (localPointOnAxis - (referenceAxisSize * (1f - margin)));
			percentage = 1f - (positionRelativeToMargin / marginSize);
			
			/*
			// Visually should make the child static.
			Vector2 offsetChild = childRect.anchoredPosition;
			offsetChild [axis] = -positionRelativeToMargin;
			childRect.anchoredPosition = offsetChild;
			*/
		}
			
		//Debug.Log ("Current point is" + localPoint + " and percentage is " + percentage);
		animator.SetFloat("Percentage", percentage);
	}
}
