using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Headjack;

public class AppController : MonoBehaviour {

	public static AppController instance;
	public GameObject projectMenu;

	/// <summary>
	/// User interface view state which determines which controls perform which action.
	/// </summary>
	public enum UIViewState
	{
		LoadingApp,
		BrowsingVideos,
		SelectedVideo,
		PlayingVideo,
		PausedVideo,
	}

	public UIViewState viewState;

	public bool playingProject;



	// Use this for initialization
	void Start () 
	{
		instance = this;
		EnterLoadingAppState(); // Immediately enter the loading app state
		SetQualitySettings(); // Get rid of the APIs horrible quality settings

		// Download data from the server such as video and thumbnail information for this app
		// also creates the camera (autoCreateCamera = true) and a cardboadrd app starts in
		// VR Mode (cardboardStereoMode = true) 

		App.Initialize (delegate(bool succes, string error) {
			if (succes)
			{
				// After downloadin the app info, download and load all project thumbnails to show in menu
				App.DownloadAllTextures(Initialized);
			}
			else
			{
				App.ShowMessage("Please make sure your device is connected to the internet to launch this app.");
			}
		}, true, true);
	}

	void Initialized(bool success, string error)
	{
		// If download all project thumbnails failed, show a message to the user
		if (!success)
		{
			App.ShowMessage ("Could not download video thumbnails. Error:  " + error, 5f);
			return;
		}

		// Assign the UGUI Camera 
		VRUIInputModule.instance.SetupUICamera ();

		// Show the UI;
		EnterBrowseVideoState(); 
	}

	public void SetQualitySettings() 
	{
		QualitySettings.antiAliasing = 2;
	}

	// A static method to play a project used called by a menu item.
	// This static method disabled the menu before playing the project.
	public void Play(string projectID, bool stream) 
	{

		EnterPlayingVideoState();
		App.Play(projectID, stream, true, delegate(bool succes, string error) {

			// when video is finished playing show menu
			App.ShowCrosshair = true;
			playingProject = false;
			App.DestroyVideoPlayer ();
			EnterBrowseVideoState();
			//projectMenu.SetActive (true);
		});

	}

	public void PlayAdvanced(string projectID, bool stream) 
	{
	}

	#region input code 

	void Update () 
	{
		if (viewState == UIViewState.BrowsingVideos) 
		{
			UpdateInputBrowseVideo();
		} 
		else if (viewState == UIViewState.SelectedVideo) 
		{
			Invoke ("UpdateInputSelectedVideo", 2f);
		} 
		else if (viewState == UIViewState.PlayingVideo) 
		{
			Debug.Log ("Playing video state");
			UpdateInputPlayingVideo();
		} 
		else if (viewState == UIViewState.PausedVideo) 
		{
			UpdateInputPauseVideo();
		}
	}

	public void UpdateInputBrowseVideo() 
	{
		// Add universal input code for browsing video

	}

	public void UpdateInputSelectedVideo()
	{
		// Add universal input code for selecting a video
	}

	int delayInput;
	public void UpdateInputPlayingVideo()
	{
		// When playing a video, pressing the universal back button returns the user to the menu
		// TODO: Replace this with more advanced controls. 

		delayInput = delayInput - 1;

		if (delayInput == 0)
			Debug.Log ("DELAY INPUT IS: " + delayInput);

		//The delay is to make sure the pause screen doesn't trigger on entry.
		if(VRInput.Confirm.Pressed && delayInput < 1)
		{
			Debug.Log ("Showing pause UI");
			EnterPausedVideoState();
		}
	}

	public void UpdateInputPauseVideo() 
	{
		//Pressing confirm opens the video controls without pausing it.
		//Pressing back opens the video controls with it paused. 
		//Pressing back again removes the current view. 
		//All this code should be moved into the UpdateInputPlayingVideo state.

		//if (VRInput.Confirm.Pressed || Input.GetMouseButtonDown(2))
		if(Input.GetKey(KeyCode.C))
		{
			EnterPlayingVideoState();
		}

		if (VRInput.Back.Pressed) 
		{
			App.DestroyVideoPlayer();
			EnterBrowseVideoState();
		}
	}

	#endregion

	#region view state enters and exits

	public void EnterLoadingAppState()
	{
		viewState = UIViewState.LoadingApp;

		VideoBrowser.instance.Show (false);
		VideoControls.instance.Show (false);
	}

	public void EnterBrowseVideoState() 
	{
		VRInput.Confirm.Pressed = false;
		viewState = UIViewState.BrowsingVideos;

		VideoBrowser.instance.Show(true);
		VideoControls.instance.Show(false);

		// Selection is required
		App.ShowCrosshair = true;

		//We re-entered the video browser so we need to refresh the list (maybe not always?) 
		VideoBrowser.instance.RefreshVideoList();
	}

	public void EnterSelectedVideoState() 
	{
		viewState = UIViewState.SelectedVideo;

		VideoBrowser.instance.Show(false);
		VideoControls.instance.Show (false);

		// Selection is required
		App.ShowCrosshair = true;
	}

	public void EnterPlayingVideoState() 
	{
		// Wait 25 frames before accepting any input from this state.
		delayInput = 25;

		viewState = UIViewState.PlayingVideo;
	
		if (App.Player)
		{
			App.Player.Resume ();
		}

		VideoBrowser.instance.Show(false);
		VideoControls.instance.Show(false);

		//Initially no selection required
		App.ShowCrosshair = false;

	}

	public void EnterPausedVideoState() 
	{
		//Pause the video if it is playing
		App.Player.Pause();

		viewState = UIViewState.PausedVideo;

		VideoBrowser.instance.Show (false);
		VideoControls.instance.Show (true);

		// Selection required
		App.ShowCrosshair = true;
	}

	#endregion
}
