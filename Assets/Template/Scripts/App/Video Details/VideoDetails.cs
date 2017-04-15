using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Headjack;
using TMPro;

public class VideoDetails : MonoBehaviour {

	public static VideoDetails instance;

	public Animator animator;
	public CanvasGroup canvasGroup;

	[Header("Video Project ID")]
	[HideInInspector] public string currentVideoProjectId;

	[Header("Controls")]
	public Button buttonPlay;
	public Button buttonBack;

	[Header("Download Buttons")]
	public Button buttonDownload;
	public Button buttonCancelDownload;
	public Button buttonDelete;

	[Header("Download Progress")]
	public TextMeshProUGUI textDownloadProgress;
	public GameObject downloadProgressObject;
	public Image downloadProgress;

	[Header("Video Information")]
	public RawImage videoThumbnail;
	public TextMeshProUGUI textTitle;
	public TextMeshProUGUI textDescription;

	// Download state of the current project
	enum DownloadState
	{
		Available,
		Downloaded, 
		Downloading,
	};

	private DownloadState downloadState;

	void Awake()
	{
		instance = this;
	}

	void Start() 
	{
		buttonPlay.onClick.AddListener (PlayVideoProject);
		buttonBack.onClick.AddListener (ReturnToBrowser);

		// Button listeners
		buttonDownload.onClick.AddListener(DownloadVideoProject);
		buttonDelete.onClick.AddListener (DeleteProject);
		buttonCancelDownload.onClick.AddListener(CancelDownload);
	}

	void Update() 
	{
		// Update the visuals of the download progress if we're downloading.
		// The event of the Download Video Project automatically turns this off
		if (downloadState == DownloadState.Downloading)
		{
			float videoProgress = App.GetProjectProgress (currentVideoProjectId);
			float videoProgressPercentage = Mathf.Floor (videoProgress);
			textDownloadProgress.text = videoProgressPercentage.ToString() + "%";
			downloadProgress.fillAmount = (videoProgress / 100f);
		}
	}

	public void Show(bool visibility = true, string videoProjectId = null) 
	{
		if (visibility == true && videoProjectId != null)
		{
			currentVideoProjectId = videoProjectId;
			SetProjectDetails(currentVideoProjectId);
			UpdateDownloadState();
		}

		canvasGroup.interactable = visibility;

		gameObject.SetActive(visibility);
		animator.SetBool("Show", visibility);


	}

	public void SetProjectDetails(string videoProjectId)
	{
		// Set visuals for the project thumbnail
		// Does this set the main texture for this material? 
		Debug.Log("Setting details for project " + currentVideoProjectId);
		videoThumbnail.texture = App.GetImage(videoProjectId);

		// Set the text for this project
		textTitle.text = App.GetTitle(videoProjectId);
		textDescription.text = App.GetDescription (videoProjectId);
	}

	public void DownloadVideoProject() 
	{
		// I really fucking hate switch statements
		// This needs to be seperated to different buttons maybe? 
		App.Download (currentVideoProjectId, true, delegate(bool success, string error) 
		{
			// Download is finished, update download button
			UpdateDownloadState ();
			Debug.Log("Project is finished downloading!");
			
			// If download failed, show message to user (for 5 seconds) 
			if (!success)
			{
				App.ShowMessage ("Download Failed:" + error, 5f);
			}
		});

		UpdateDownloadState();
	}

	void UpdateDownloadState()
	{
		// Local files exist already, so we don't need to download the project.
		if (App.GotFiles (currentVideoProjectId))
		{
			downloadState = DownloadState.Downloaded;
			Debug.Log ("Download state: downloaded");
		}
		// Project is currently downloading.
		else
		if (App.ProjectIsDownloading (currentVideoProjectId))
		{
			downloadState = DownloadState.Downloading;
			Debug.Log ("Download state: downloading");
		}
		else
		{
			downloadState = DownloadState.Available;
			Debug.Log ("Download State: available");
		}

		// Set the download controls
		ShowDownloadControls();
	}

	/// <summary>
	/// Shows download controls depending on download state
	/// </summary>
	void ShowDownloadControls()
	{
		if (downloadState == DownloadState.Downloaded)
		{
			buttonDownload.gameObject.SetActive (false);
			downloadProgressObject.SetActive (false);
			buttonCancelDownload.gameObject.SetActive (false);

			buttonDelete.gameObject.SetActive (true);
		}
		else
		if (downloadState == DownloadState.Downloading)
		{
			buttonDownload.gameObject.SetActive (false);

			downloadProgressObject.SetActive(true);
			buttonCancelDownload.gameObject.SetActive(true);

			buttonDelete.gameObject.SetActive (false);
		}
		else
		if (downloadState == DownloadState.Available)
		{
			buttonDownload.gameObject.SetActive (true);

			downloadProgressObject.SetActive (false);
			buttonCancelDownload.gameObject.SetActive (false);
			buttonDelete.gameObject.SetActive (false);
		}
		else
		{
			return;
		}
	}
		
	void CancelDownload()
	{
		if(App.ProjectIsDownloading(currentVideoProjectId))
		{
			App.Cancel (currentVideoProjectId);
		}
	}
		
	void DeleteProject() 
	{
		if(App.GotFiles(currentVideoProjectId))
		{
			App.Delete(currentVideoProjectId);
			downloadProgress.fillAmount = 0f;
			UpdateDownloadState ();
		}
	}

	void PlayVideoProject()
	{
		if (downloadState == DownloadState.Downloaded)
		{
			AppController.instance.Play (currentVideoProjectId, false); // Play from downloaded content
		}
		else
		{
			AppController.instance.Play (currentVideoProjectId, true); // Play using stream
		}

		// Give the details of the currently playing video to the video controls.
		VideoControls.instance.currentVideoProjectId = currentVideoProjectId;
	}

	void ReturnToBrowser()
	{
		AppController.instance.EnterBrowseVideoState();
	}
}
