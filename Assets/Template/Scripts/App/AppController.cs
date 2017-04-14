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
		VideoDetails,
		SelectedVideo,
		PlayingVideo,
		PausedVideo,
	}

	public UIViewState viewState;

	public bool playingProject;
	public string lastProjectID;
	public bool lastProjectStreamed;



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

		// Hide the app cursor - we're using our own
		App.ShowCrosshair = false;

		// Assign the UGUI Camera 
		VRUIInputModule.instance.SetupUICamera();


		// Enter the UI view. Also retrieve the project category data.
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
		// Save the projectID string and stream state so we can replay them
		lastProjectID = projectID;
		lastProjectStreamed = stream;

		// Enter the playing video state to process input
		EnterPlayingVideoState();

		// Start playing
		App.Play(projectID, stream, true, delegate(bool succes, string error) {

			// When video is finished playing show video controls
			// where the pause/resume button is replaced with replay
			VideoControls.instance.ShowReplayButton (true);
			VideoControls.instance.Show(true);
			EnterPausedVideoState ();
		});

		/*
		// Old call 
		App.Play(projectID, stream, true, delegate(bool succes, string error) {

			// When video is finished playing show menu
			App.ShowCrosshair = true;
			playingProject = false;
			App.DestroyVideoPlayer();
			EnterBrowseVideoState();
		});
		*/

	}

	#region input code 

	void Update () 
	{
		if (viewState == UIViewState.BrowsingVideos)
		{
			UpdateInputBrowseVideo ();
		}
		else
		if (viewState == UIViewState.SelectedVideo)
		{
			Invoke ("UpdateInputSelectedVideo", 2f);
		}
		else
		if (viewState == UIViewState.VideoDetails)
		{
			UpdateInputSelectedVideo ();
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
		// Add universal input code for browsing video

	}

	public void UpdateVideoDetails()
	{
		// Add universal input code for viewing project details
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

		// Set a delay to prevent our input from immediately affecting our video player
		delayInput = delayInput - 1;

		//The delay is to make sure the pause screen doesn't trigger on entry.
		if(VRInput.Confirm.Pressed && delayInput < 1)
		{
			EnterPausedVideoState(false);
		}

		if (VRInput.Back.Pressed && delayInput < 1 || Input.GetKeyDown (KeyCode.Mouse1))
		{
			EnterPausedVideoState (false);
		}
	}

	public void UpdateInputPauseVideo() 
	{
		//Pressing back again should bring us to BrowseVideoPLayer. 

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
		VideoDetails.instance.Show (false);
		VideoControls.instance.Show (false);
	}

	public void EnterBrowseVideoState() 
	{
		VRInput.Confirm.Pressed = false;
		viewState = UIViewState.BrowsingVideos;

		VideoBrowser.instance.Show(true);
		VideoDetails.instance.Show (false);
		VideoControls.instance.Show(false);

		// Selection is required
		CustomCursor.instance.Show(true);

		//We re-entered the video browser so we need to refresh the list (maybe not always?) 
		VideoBrowser.instance.RefreshVideoList();
		VideoBrowser.instance.GetProjectCategories();
	}

	public void EnterVideoDetailsState(string videoProjectID)
	{
		viewState = UIViewState.VideoDetails;

		VideoBrowser.instance.Show(false);
		VideoDetails.instance.Show (true, videoProjectID);
		VideoControls.instance.Show (false);

		// Selection is required
		CustomCursor.instance.Show(true);
	}

	/// <summary>
	/// What's this for again? Go and find out
	/// </summary>
	public void EnterSelectedVideoState() 
	{
		viewState = UIViewState.SelectedVideo;

		VideoBrowser.instance.Show(false);
		VideoDetails.instance.Show (false);
		VideoControls.instance.Show (false);

		// Selection is required
		CustomCursor.instance.Show(true);
	}

	public void EnterPlayingVideoState() 
	{
		// Wait 25 frames before accepting any input from this state.
		delayInput = 25;

		viewState = UIViewState.PlayingVideo;
	
		/*
		if (App.Player)
		{
			App.Player.Resume ();
		}
		*/

		VideoBrowser.instance.Show(false);
		VideoDetails.instance.Show (false);
		VideoControls.instance.Show(false);

		//Initially no selection required
		App.ShowCrosshair = false;
		CustomCursor.instance.Show (false);

	}

	public void EnterPausedVideoState(bool pause = false) 
	{
		//Pause the video if it is playing
		if (pause)
		{
			App.Player.Pause ();
		}

		VideoControls.instance.SetPauseResumeIcon(App.Player.IsPlaying);

		viewState = UIViewState.PausedVideo;

		VideoBrowser.instance.Show (false);
		VideoDetails.instance.Show (false);
		VideoControls.instance.Show (true);

		// Selection required
		CustomCursor.instance.Show (true);
	}

	#endregion
}
