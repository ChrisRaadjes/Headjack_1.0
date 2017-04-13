using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Headjack;
using TMPro;

public class VideoProjectButton : MonoBehaviour {

	public TextMeshProUGUI textTitle;
	public TextMeshProUGUI textDownloadButton;

	public RawImage videoThumbnail;

	public Button buttonPlay;
	public Button buttonDownload;

	// Project ID corresponding to this menuitem
	string videoProjectId;

	// Download State
	enum DownloadState
	{
		Available,
		Downloaded,
		Downloading,
	};

	public void SetProjectId(string _projectId)
	{
		videoProjectId = _projectId;
		textTitle.text = App.GetTitle (videoProjectId);

		// Could create a custom material for this that clips overlarge textures
		// rather than stretching them. For now only works nicely with 16:9.
		videoThumbnail.texture = App.GetImage(videoProjectId);

		// Update state
		UpdateDownloadState();
	}

	DownloadState UpdateDownloadState()
	{
		// Local files exist already, change download button to delete
		if (App.GotFiles (videoProjectId))
		{
			textDownloadButton.text = "Delete";
			return DownloadState.Downloaded;
		}

		// Project is currently download, change it to cancel
		if(App.ProjectIsDownloading(videoProjectId))
		{
			textDownloadButton.text = "Cancel";
			return DownloadState.Downloading;
		}

		// If above aren't true, project is ready to download
		textDownloadButton.text = "Download";
		return DownloadState.Available;
	
	}

	// Use this for initialization
	void Start ()
	{
		// Listeners
		buttonPlay.onClick.AddListener(ShowVideoDetails);
		buttonDownload.onClick.AddListener(DownloadVideoProject);
		buttonPlay.onClick.AddListener(ClearConfirmPressed);
	}

	public void ShowVideoDetails()
	{
		AppController.instance.EnterVideoDetailsState(videoProjectId);
	}

	public void PlayVideoProject() 
	{
		if (UpdateDownloadState() == DownloadState.Downloaded)
		{
			AppController.instance.Play (videoProjectId, false); // Play from downloaded content
		}
		else
		{
			AppController.instance.Play (videoProjectId, true); // Play using stream
		}
	}

	public void DownloadVideoProject() 
	{
		DownloadState downloadState = UpdateDownloadState ();
		switch (downloadState)
		{
		case DownloadState.Downloaded:

			// Delete downloaded project
			App.Delete(videoProjectId);
			break;

		case DownloadState.Downloading:

			// Cancel download of this project
			App.Cancel(videoProjectId);
			break;

		case DownloadState.Available:

		default:

			// Start the project download
			App.Download (videoProjectId, true, delegate(bool success, string error) {

				// Download is finished, update download button
				UpdateDownloadState ();

				// If download failed, show message to user (for 5 seconds) 
				if (!success)
				{
					App.ShowMessage ("Download Failed:" + error, 5f);
				}
			});
			break;
		}

		// Update download button text after the above action
		UpdateDownloadState ();
	}

	/// <summary>
	/// Immediatley clears confirm pressed so that the pause menu doesn't show when starting a video	/// </summary>
	public void ClearConfirmPressed()
	{
		VRUIInputModule.instance.confirmPressed = false;
	}
}
