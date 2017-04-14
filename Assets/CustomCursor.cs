using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Headjack;

public class CustomCursor : MonoBehaviour {

	public static CustomCursor instance;

	public Renderer cursorSprite;
	public GameObject cursorGazeProgressParent;
	public Renderer cursorGazeProgressBar;
	public float cursorGazeProgress;
	public TrailRenderer cursorTrail;
	public float cursorTrailFadeTime = 0.25f;
	public float cursorShowForMagnitude = 1f;

	Vector3 lastCameraPosition;
	Vector3 cursorMovementVelocity;
	Quaternion lastCameraRotation;

	float smoothSpeed = 100f;

	void Awake()
	{
		instance = this;
		cursorGazeProgressBar.material.SetFloat ("Progress", 0f);
	}

	void Update () 
	{
		lastCameraPosition = App.camera.position + (App.camera.forward * 2f);
		lastCameraRotation = App.camera.rotation;

		if(VRUIInputModule.instance.gazeTimer < 0.01) 
		{
			cursorGazeProgressParent.SetActive(false);
		}
		else
		{
			cursorGazeProgressParent.SetActive(true);
		}

		cursorGazeProgressBar.material.SetFloat("_Progress", (VRUIInputModule.instance.gazeTimer / VRUIInputModule.instance.timerTime));
	}

	void LateUpdate() 
	{
		transform.position = Vector3.SmoothDamp(transform.position, lastCameraPosition, ref cursorMovementVelocity, 0.025f);
		transform.rotation = Quaternion.Slerp(transform.rotation, lastCameraRotation, 0.025f);
	

		if (cursorMovementVelocity.magnitude > cursorShowForMagnitude)
		{
			cursorTrail.gameObject.SetActive (true);
		}
		else
		{
			cursorTrail.gameObject.SetActive (false);
		}

		// Debug for cursor data
		/*
		Debug.Log ("Cursor Magnitude is " + cursorMovementVelocity.magnitude);
		Debug.Log ("Cursor Trail Alpha is " + cursorTrail.material.color.a);
		*/
	}

	// If we want to use this; need to write a vertex colour shader instead! 
	 
	/*
	IEnumerator CoLerpTrailColor(float startColor, float endColor) 
	{
		float lerpTime = 0f;
		while (lerpTime < cursorTrailFadeTime)
		{
			float percentage = (lerpTime / cursorTrailFadeTime);
			float alphaColor = Mathf.Lerp (0f, 1f, percentage);

			Vector4 newColor = cursorTrail.material.color;
			newColor.z = alphaColor;
			cursorTrail.material.color = newColor;

			lerpTime += Time.deltaTime;

			yield return null;
		}

		yield return null;
	}
	*/
}
