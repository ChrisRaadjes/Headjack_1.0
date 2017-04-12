using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoPlayerButtonPauseResume : MonoBehaviour {

	public Image buttonIcon;
	public Sprite buttonIconPause;
	public Sprite buttonIconPlay;

	public void SetPauseResumeIcon(bool isPlaying = true)
	{
		if (isPlaying)
			buttonIcon.sprite = buttonIconPause;
		else
			buttonIcon.sprite = buttonIconPlay;
	}
}
