using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Headjack;

public class ProjectMenu : MonoBehaviour {

	public TextMesh textTitle;
	public TextMesh textButtonDowload;
	public MeshRenderer rendererThumbnail;
	public MeshCollider colliderButtonDownload;
	public MeshCollider colliderButtonPlay;

	// Project ID corresponding to this menuitem
	string projectId;

	//Download State
	enum DownloadState
	{
		Available,
		Downloaded,
		Downloading,
	};

	public void SetProjectId(string _projectId)
	{
		projectId = _projectId;
		textTitle.text = App.GetTitle (projectId);
		rendererThumbnail.material.mainTexture = App.GetImage(projectId);

		// Update state
		UpdateDownloadState();
	}

	DownloadState UpdateDownloadState()
	{
		// Local files exist already, change download button to delete
		if (App.GotFiles (projectId))
		{
			textButtonDowload.text = "Delete";
			return DownloadState.Downloaded;
		}

		// Project is currently download, change it to cancel
		if(App.ProjectIsDownloading(projectId))
		{
			textButtonDowload.text = "Cancel";
			return DownloadState.Downloading;
		}

		// If above aren't true, project is ready to download
		textButtonDowload.text = "Download";
		return DownloadState.Available;
	
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Every frame, check whether a confirm input has been registered
		if (VRInput.Confirm.Pressed && projectId != null)
		{
			// Confirm has been pressed on any of the VR input devices
			// check if one of the buttons was selected 

			if (App.IsCrosshairHit (colliderButtonDownload))
			{
				DownloadState downloadState = UpdateDownloadState ();
				switch (downloadState)
				{
				case DownloadState.Downloaded:

						// Delete downloaded project
					App.Delete (projectId);
					break;

				case DownloadState.Downloading:

						// Cancel download of this project
					App.Cancel (projectId);
					break;

				case DownloadState.Available:
					
				default:
						
						// Start the project download
					App.Download (projectId, true, delegate(bool success, string error) {

						// Download is finished, update download button
						UpdateDownloadState ();

						//If download failed, show messae to user (for 5 seconds) 
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
			else if(App.IsCrosshairHit (colliderButtonPlay))
			{
				// Play button was pressed, play the downloaded video if the project is downloaded
				// otherwise stream the project
				if (UpdateDownloadState () == DownloadState.Downloaded)
				{
					AppController.instance.Play (projectId, false);
				}
				else
				{
					AppController.instance.Play (projectId, true);
				}
			}
		}
	}
}
