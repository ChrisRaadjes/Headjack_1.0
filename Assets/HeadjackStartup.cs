using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Headjack;

public class HeadjackStartup : MonoBehaviour {

	public static HeadjackStartup instance;
	public GameObject projectMenu;

	/// <summary>
	/// User interface view state which determines which controls perform which action.
	/// </summary>
	public enum UIViewState
	{
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

		// Show crosshair in menu view so menu buttons can be selected by gaze
		App.ShowCrosshair = true;

		// Assign the UGUI Camera 
		VRUIInputModule.instance.SetupUICamera ();

		//Set the default browser view with no category (i.e "All");
		VideoBrowser.instance.RefreshVideoList ();
		VideoBrowser.instance.Show (true);
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
			
		App.Player.Pause();
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
			UpdateInputSelectedVideo();
		} 
		else if (viewState == UIViewState.PlayingVideo) 
		{
			UpdateInputPlayingVideo();
		} 
		else if (viewState == UIViewState.PausedVideo) 
		{
			UpdateInputPauseVideo();
		}
	}

	public void UpdateInputBrowseVideo() 
	{
		
	}

	public void UpdateInputSelectedVideo()
	{
	}

	public void UpdateInputPlayingVideo()
	{
		// When playing a video, pressing the universal back button returns the user to the menu
		// TODO: Replace this with more advanced controls. 
		if (VRInput.Back.Pressed || Input.GetMouseButtonDown(1))
		{
			App.DestroyVideoPlayer ();
		}
	}

	public void UpdateInputPauseVideo() 
	{
	}

	#endregion

	#region view state enters and exits

	//TODO: Do I need the exit states? Probably not

	public void EnterBrowseVideoState() 
	{
		viewState = UIViewState.BrowsingVideos;

		VideoBrowser.instance.Show(true);
		VideoControls.instance.Show(false);

		// Selection is required
		App.ShowCrosshair = true;

		//We re-entered the video browser so we need to refresh the list
		VideoBrowser.instance.RefreshVideoList ();

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
		viewState = UIViewState.PlayingVideo;

		VideoBrowser.instance.Show(false);
		VideoControls.instance.Show(false);

		//No selection required
		App.ShowCrosshair = false;
	}

	public void EnterPausedVideoState() 
	{
		viewState = UIViewState.PausedVideo;

		VideoBrowser.instance.Show(false);
		VideoControls.instance.Show(true);

		// Selection required
		App.ShowCrosshair = true;
	}

	#endregion
}
