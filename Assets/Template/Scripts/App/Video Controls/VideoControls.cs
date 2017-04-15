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
	public CanvasGroup canvasGroup;

	public RotationNode origin;

	public Animator animator;

	[Header("Video Control Interactables")]

	[Header("Video Info")]
	[HideInInspector] public string currentVideoProjectId;
	public TextMeshProUGUI textVideoTitle;

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

	[Header("Volume Button")]
	public Slider volumeSlider;
	public ExtendedInputHandler volumeSliderInput;
	public Button buttonMuteVolume;
	public Image iconMuteVolume;
	public Sprite iconVolumeUnmuted;
	public Sprite iconVolumeMuted;

	[Header("Debug Controls")]
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

		buttonMuteVolume.onClick.AddListener(MuteVolume);
		//toggleMuteVolume.onValueChanged.AddListener(mute => MuteVolume(mute));

		//These are tests to be activated in the 2.0 template
		/*
		buttonForward.onClick.AddListener (ForwardVideo);
		buttonRewind.onClick.AddListener (RewindVideo);
		*/

	}
		
	void Update () 
	{
		if (setSeek)  // Set seek is set by SliderInputHandler events, and immediatley consumed. So we only update if it's off.
		{
			videoProgressBar.value = App.Player.Seek;
		}

		volumeSlider.value = App.Player.Volume;
		Debug.Log ("Volume of player is at " + App.Player.Volume);

		// Write the timespan values
		TimeSpan currentTime = TimeSpan.FromMilliseconds(VideoTimeUtility.ConvertTimeToMSFromPercentage(App.Player.Duration, App.Player.Seek));
		TimeSpan remainingTime = TimeSpan.FromMilliseconds(VideoTimeUtility.ConvertTimeToMs((App.Player.Duration - App.Player.SeekMs)));
		
		VideoTimeUtility.WriteVideoTime(textCurrentTime, currentTime);
		VideoTimeUtility.WriteVideoTime(textRemainingTime, remainingTime);
	}

	public void Show(bool visibility, string videoProjectId = null)
	{
		//Null-check to prevent this from being called at the start
		if (origin != null)
		{
			origin.LookRotation = VRUIInputModule.instance.CameraRotationY;
		}

		if (videoProjectId != null)
		{
			textVideoTitle.text = App.GetProjectMetadata(videoProjectId).Title;
		}

		canvasGroup.interactable = visibility;
		
		gameObject.SetActive (visibility);
		animator.SetBool ("Show", visibility);	
	}

	/// 
	///  Button functions
	///

	public void PauseResumeVideo() 
	{
		App.Player.PauseResume();
		//AppController.instance.EnterPlayingVideoState()

		SetPauseResumeIcon(App.Player.IsPlaying);
	}

	public void SetPauseResumeIcon(bool isPlaying = true)
	{
		if (isPlaying)
			buttonPauseResumeIcon.sprite = buttonIconPlay;
		else
			buttonPauseResumeIcon.sprite = buttonIconPause;
	}

	// Set seek doesn't work in this template
	public void SeekVideoTime(float seekTime)
	{
		//Setting seek, so flip bool to make sure Update() doesn't change slider value
		setSeek = false;

		App.Player.Pause();
		App.Player.Seek = seekTime;
		App.Player.Resume();

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
		
	#region volume controls
	float previousVolume;
	public void MuteVolume()
	{
		previousVolume = App.Player.Volume;

		App.Player.Mute = !App.Player.Mute;

		if(App.Player.Mute)
		{
			App.Player.Volume = 0f;
		}
		else
		{
			App.Player.Volume = previousVolume;
		}

		SetVolumeIcon();
	}

	public void SetVolume(float volume)
	{
		if (volume == 0f)
		{
			MuteVolume ();
		}
		else
		{
			App.Player.Volume = volume;
			SetVolumeIcon ();
		}
	}

	public void SetVolumeIcon() 
	{
		if (App.Player.Volume == 0f || App.Player.Mute)
		{
			iconMuteVolume.sprite = iconVolumeMuted;
		}
		else
		{
			iconMuteVolume.sprite = iconVolumeUnmuted;
		}
	}
	#endregion

	#region replay functionality
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
	#endregion
}
