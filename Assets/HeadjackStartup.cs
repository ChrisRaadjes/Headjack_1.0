using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Headjack;

public class HeadjackStartup : MonoBehaviour {

	public static HeadjackStartup instance;
	public GameObject projectMenu;

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
		VideoBrowser.instance.ShowBrowser (true);

		/*
		Vector3 currentMenuItemPosition = projectMenu.transform.position; 
		GameObject currentMenuItem;
		foreach (string currentProjectID in App.GetProjects())
		{
			currentMenuItem = (GameObject)Instantiate (projectItemPrefab, projectMenu.transform, false);
			currentMenuItem.transform.position = currentMenuItemPosition;
			currentMenuItemPosition.x += 0.65f; //move the current menu position so next menu item is placed correctly

			//Set the project id of the newly created menu item (also sets project title and thumbnail)
			currentMenuItem.GetComponent<ProjectMenu>().SetProjectId(currentProjectID);
		}
		*/

		// Set quality settings
		SetQualitySettings();
	}

	/*
	public void StartVideoPlayback(string videoP	rojectId, bool stream) 
	{
		// To enable this user experience we must do several things:
		// 1. Check if the user is connected to wifi
		// 2. Open the video and play the first frame no matter whether it's dl or not
		// 3. Show the video UI; 

		App.Play (videoProjectId, true, true);
		App.

	}
	*/

	public void SetQualitySettings() 
	{
		QualitySettings.antiAliasing = 2;
	}


	// A static method to play a project used called by a menu item.
	// This static method disabled the menu before playing the project.
	public void Play(string projectID, bool stream) 
	{
		// Disable the menu before starting video
		//App.ShowCrosshair = false;
		VideoBrowser.instance.ShowBrowser(false);
		//projectMenu.SetActive (false);
		playingProject = true;

		App.Play(projectID, stream, true, delegate(bool succes, string error) {

			// when video is finished playing show menu
			App.ShowCrosshair = true;
			playingProject = false;
			App.DestroyVideoPlayer ();
			VideoBrowser.instance.ShowBrowser(true);
			//projectMenu.SetActive (true);
		});
			
		App.Player.Pause();
	}

	public void PlayAdvanced(string projectID, bool stream) 
	{
	}

	void Update () 
	{
		// When playing a video, pressing the universal back button returns the user to the menu
		// TODO: Replace this with more advanced controls. 
		if (playingProject && VRInput.Back.Pressed)
		{
			App.ShowCrosshair = true;
			playingProject = false;
			App.DestroyVideoPlayer ();
			projectMenu.SetActive (true);
		}
	}

	public void SomeFunc() 
	{
	}
}
