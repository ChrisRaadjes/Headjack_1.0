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

	[Space]
	public TextMeshProUGUI textCurrentTime;
	public TextMeshProUGUI textRemainingTime;
	[Header("Other Controls")]
	public Button buttonRewind;
	public Button buttonForward;
	public Button buttonBack;


	void Start () 
	{
		//Set listeners
		buttonPauseResume.onClick.AddListener(PauseResumeVideo);
		buttonBack.onClick.AddListener(BackToVideoBrowser);

		// We use our custom input handler for the slider
		// because we don't want to be calling SeekMS every frame.
		videoProgressBarInput.onClick.AddListener(SeekVideoTime);
		videoProgressBarInput.onClick.AddListener(ClickTest);
		videoProgressBarInput.onHover.AddListener(HoverTest);

		buttonForward.onClick.AddListener (ForwardVideo);
		buttonRewind.onClick.AddListener (RewindVideo);
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
		animator.SetBool ("Visible", visibility);	
	}

	/// 
	///  Button functions
	///

	public void PauseResumeVideo() 
	{
		Debug.Log ("App player object is " + App.Player.transform.name);
		App.Player.PauseResume();
		SetPauseResumeIcon (App.Player.IsPlaying);
	}

	public void SetPauseResumeIcon(bool isPlaying = true)
	{
		if (isPlaying)
			buttonPauseResumeIcon.sprite = buttonIconPause;
		else
			buttonPauseResumeIcon.sprite = buttonIconPlay;
	}

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

	public void ForwardVideo()
	{
		long duration = App.Player.Duration;
		long seekms = App.Player.SeekMs;
		App.Player.SeekMs = System.Math.Min (seekms + 15000, duration <= 0 ? (seekms + 15000) : duration);
	}

	public void RewindVideo()
	{
		App.Player.SeekMs = System.Math.Max (App.Player.SeekMs - 15000, 0);
	}

	public void ClickTest()
	{
		// Test to see if SliderInputHandler is working
		Debug.Log("You have clicked me");
	}

	public void HoverTest()
	{
		// Test to see if SliderInputHandler is working
		Debug.Log("Hovering over a slider");
	}

	public void BackToVideoBrowser() 
	{
		// It would be easier to port over a state controller maybe rather than keep track of what UI
		// needs to be shown where and where. 
	}
}
