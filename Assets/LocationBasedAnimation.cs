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

	void Start () 
	{
		Debug.Log ("MAX axis: " + referenceRect.rect.max [axis]);
		Debug.Log ("MIN region: " + referenceRect.rect.size[axis] * margin);
	}

	void Update ()
	{
		//SomeFunc ();
		SomeFunc6();
	}

	public void SomeFunc() 
	{
		// The percentage through or manual animation depends on the object position

		// We need to adjust the position var of the target object by the parent reference position
		Vector2 targetRectPosition = targetRect.anchoredPosition;
		Debug.Log ("POSITION IS " + targetRect.anchoredPosition);
		targetRectPosition -= referenceRect.rect.position;
		Debug.Log ("ADJUSTED POSITION IS " + targetRect.anchoredPosition);

		float percentage = Mathf.Clamp01 (targetRectPosition[axis] / referenceRect.rect.size [axis]);

		canvasGroup.alpha = percentage;
	}

	public void SomeFunc2()
	{
	}

	public void SomeFunc3()
	{
		Vector2 convertedRect = RectTransformExtension.switchToRectTransform (targetRect, referenceRect);
		Debug.Log ("CONVERTED POSITION IS " + convertedRect);
	}

	public void SomeFunc4()
	{
		Debug.Log ("MAX axis: " + referenceRect.rect.max [axis] + "MAX axis margin " + (referenceRect.rect.max [axis] * 0.2f));
		Debug.Log ("MIN axis: " + referenceRect.rect.min [axis] + "MIN axis margin " + (referenceRect.rect.min [axis] * 0.2f));

		Vector2 convertedRect = RectTransformExtension.switchToRectTransform (targetRect, referenceRect);
		float percentage = 0f;

		if (convertedRect [axis] > referenceRect.rect.max [axis] * 0.2f)
		{
			percentage = Mathf.Clamp01 ((convertedRect[axis]-(referenceRect.rect.max[axis]*0.2f))/ (referenceRect.rect.max[axis]-(referenceRect.rect.max[axis]*0.2f)));
			//canvasGroup.alpha = 1f-percentage;
			animator.SetFloat("Percentage", (1f-percentage));
			Debug.Log ("POSITION " + convertedRect[axis] + " PERCENTAGE: " + percentage + " REVERSED: " + (1f - percentage));
		}
		else if (convertedRect[axis] < (referenceRect.rect.min [axis] * 0.2f))
		{
			percentage = Mathf.Clamp01 (convertedRect[axis]+((referenceRect.rect.min[axis]*0.2f)*-1f)/ referenceRect.rect.min[axis]);
			Debug.Log ("POSITION " + convertedRect[axis] + " PERCENTAGE: " + percentage + " REVERSED: " + (1f - percentage));

			animator.SetFloat("Percentage", (1f-percentage));
		}
		else
		{
			animator.SetFloat("Percentage", 1f);
		}
	}

	public void SomeFunc5()
	{
		Vector2 convertedRect = RectTransformExtension.switchToRectTransform (targetRect, referenceRect);
		Debug.Log ("Position of object is " + convertedRect);
	}

	public void SomeFunc6()
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
