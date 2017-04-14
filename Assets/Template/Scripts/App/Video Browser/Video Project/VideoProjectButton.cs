using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Headjack;
using TMPro;

public class VideoProjectButton : MonoBehaviour {

	public TextMeshProUGUI textTitle;
	public RawImage videoThumbnail;
	public Button buttonOpenVideo;

	// Project ID corresponding to this menu item
	string videoProjectId;
		
	public void SetProjectId(string _projectId)
	{
		videoProjectId = _projectId;
		textTitle.text = App.GetTitle (videoProjectId);

		// Could create a custom material for this that clips overlarge textures
		// rather than stretching them. For now only works nicely with 16:9.
		videoThumbnail.texture = App.GetImage(videoProjectId);

		// Add listener to the button
		buttonOpenVideo.onClick.AddListener(ShowVideoDetails);
	}

	public void ShowVideoDetails()
	{
		AppController.instance.EnterVideoDetailsState(videoProjectId);
	}

	/// <summary>
	/// Immediatley clears confirm pressed so that the pause menu doesn't show when starting a video	/// </summary>
	public void ClearConfirmPressed()
	{
		VRUIInputModule.instance.confirmPressed = false;
	}
}
