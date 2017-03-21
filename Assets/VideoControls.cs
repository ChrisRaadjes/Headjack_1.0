﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using Headjack;

public class VideoControls : MonoBehaviour {

	public static VideoControls instance;

	public RotationNode origin;

	public Animator animator;

	[Header("Video Control Interactables")]

	[Header("Pause Resume Button")]
	public Button buttonPauseResume;
	public Image buttonPauseResumeIcon;
	[Space]
	public Sprite buttonIconPause;
	public Sprite buttonIconPlay;

	[Header("Progress Bar")]
	public Slider videoProgressBar;
	public ExtendedInputHandler videoProgressBarInput;
	private bool setSeek = true;

	[Header("Other Controls")]
	public Button buttonBackToBrowse;

	[Space]
	public TextMeshProUGUI textCurrentTime;
	public TextMeshProUGUI textRemainingTime;
	[Header("Other Controls")]
	public Button buttonRewind;
	public Button buttonForward;
	public Button buttonBack;


	void Start () 
	{
		instance = this;

		//Set listeners
		buttonPauseResume.onClick.AddListener(PauseResumeVideo);
		buttonBack.onClick.AddListener(BackToVideoBrowser);

		// We use our custom input handler for the slider
		// because we don't want to be calling SeekMS every frame.
		videoProgressBarInput.onClick.AddListener(SeekVideoTime);

		//These are tests to be activated in the 2.0 template
		/*
		buttonForward.onClick.AddListener (ForwardVideo);
		buttonRewind.onClick.AddListener (RewindVideo);
		*/

	}

	void Update () 
	{
		if (setSeek)  // Set seek is set by SliderInputHanlder events, and immediatley consumed. 
		{
			videoProgressBar.value = App.Player.Seek;
		}
	}

	public void Show(bool visibility = true) 
	{
		//This won't work for when you add controllers.
		//origin.LookRotation = VRUIInputModule.instance.CameraRotation;
		
		gameObject.SetActive (visibility);
		animator.SetBool ("Show", visibility);	
	}

	/// 
	///  Button functions
	///

	public void PauseResumeVideo() 
	{
		Debug.Log ("App player object is " + App.Player.transform.name);
		App.Player.PauseResume();
		AppController.instance.EnterPlayingVideoState();
		SetPauseResumeIcon (App.Player.IsPlaying);
	}

	public void SetPauseResumeIcon(bool isPlaying = true)
	{
		if (isPlaying)
			buttonPauseResumeIcon.sprite = buttonIconPause;
		else
			buttonPauseResumeIcon.sprite = buttonIconPlay;
	}

	// Set seek doesn't work in this template
	public void SeekVideoTime()
	{
		//Setting seek, so flip bool to make sure Update() doesn't change slider value
		setSeek = false;

		Debug.Log("Slider progress is " + videoProgressBar.value);
		Debug.Log ("Attempting to seek");

		App.Player.Seek = videoProgressBar.value;

		// Consume the set seek
		setSeek = true; 
	}

	/// <summary>
	/// Test function only.
	/// </summary>
	public void ForwardVideo()
	{
		long duration = App.Player.Duration;
		long seekms = App.Player.SeekMs;
		App.Player.SeekMs = System.Math.Min (seekms + 15000, duration <= 0 ? (seekms + 15000) : duration);
	}

	/// <summary>
	/// Test function only.
	/// </summary>
	public void RewindVideo()
	{
		App.Player.SeekMs = System.Math.Max (App.Player.SeekMs - 15000, 0);
	}

	public void BackToVideoBrowser() 
	{
		AppController.instance.EnterBrowseVideoState();
	}
}
