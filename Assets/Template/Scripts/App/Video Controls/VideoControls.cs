using System;
using System.Collections;
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
	public Button buttonReplayVideo;
	public Button buttonBackToBrowse;
	[Space]
	public TextMeshProUGUI textCurrentTime;
	public TextMeshProUGUI textRemainingTime;

	[Header("Other Controls")]
	public Button buttonRewind;
	public Button buttonForward;


	void Start () 
	{
		instance = this;

		//Set listeners
		buttonPauseResume.onClick.AddListener(PauseResumeVideo);
		buttonReplayVideo.onClick.AddListener(ReplayVideo);
		buttonBackToBrowse.onClick.AddListener(BackToVideoBrowser);

		ShowReplayButton(false);

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
		if (setSeek)  // Set seek is set by SliderInputHandler events, and immediatley consumed. 
		{
			videoProgressBar.value = App.Player.Seek;
		}

		/*
		// Write the timespan values
		TimeSpan currentTime = TimeSpan.FromMilliseconds(VideoTimeUtility.ConvertTimeToMSFromPercentage(App.Player.Duration, App.Player.Seek));
		TimeSpan remainingTime = TimeSpan.FromMilliseconds(VideoTimeUtility.ConvertTimeToMs((App.Player.Duration - App.Player.SeekMs)));
		
		VideoTimeUtility.WriteVideoTime(textCurrentTime, currentTime);
		VideoTimeUtility.WriteVideoTime(textRemainingTime, remainingTime);
		*/
	}

	public void Show(bool visibility)
	{
		//Null-check to prevent this from being called at the start
		if (origin != null)
		{
			origin.LookRotation = VRUIInputModule.instance.CameraRotationY;
		}
		
		gameObject.SetActive (visibility);
		animator.SetBool ("Show", visibility);	
	}

	/// 
	///  Button functions
	///

	public void PauseResumeVideo() 
	{
		App.Player.PauseResume();
		AppController.instance.EnterPlayingVideoState();
		SetPauseResumeIcon(App.Player.IsPlaying);
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
		App.DestroyVideoPlayer ();

		// Check to see if we're playing and if so destroy it before continuing
		/*
		if (App.Player.IsPlaying)
		{
			App.DestroyVideoPlayer ();
		}
		*/

		AppController.instance.EnterBrowseVideoState();
	}

	private bool showReplayButton;
	public void ShowReplayButton(bool show) 
	{
		// Swap out the pause resume button for the replay button.
		// Gets called by UpdateInputPlayingVideo (true)
		// and UpdateInputPauseVideo(false)

		showReplayButton = show;
		buttonPauseResume.gameObject.SetActive(!show);
		buttonReplayVideo.gameObject.SetActive(show);
	}

	public void ReplayVideo()
	{
		// For debug purposes return the isPlaying state
		Debug.Log("VIDEO PLAYER IS PLAYING: " + App.Player.IsPlaying);

		// Restart the video from frame 0
		//App.Player.Seek = 0f;

		//This is a brute hack, but only way this works for now
		App.DestroyVideoPlayer();
		ShowReplayButton(false);

		// Resume playing the video
		AppController.instance.Play(AppController.instance.lastProjectID, AppController.instance.lastProjectStreamed);

		//Swap out the replay button for the pause resume button
	}
}
