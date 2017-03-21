using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Headjack;

public class VideoBrowser : MonoBehaviour {

	public static VideoBrowser instance;

	public Animator animator;

	[Header("Video Projects")]
	public Transform videoProjectlist; 
	public GameObject videoProjectTemplate;

	void Awake () 
	{
		instance = this;
	}

	public void Show(bool visibility = true)
	{
		gameObject.SetActive(visibility);
		animator.SetBool("Show", visibility);
	}
		
	public void RefreshVideoList(string refreshCategoryID = null)
	{
		string[] videoProjects = null;

		if (refreshCategoryID == null)
		{
			videoProjects = App.GetProjects ();
		}
		else
		{
			videoProjects = App.GetProjects (refreshCategoryID = refreshCategoryID);
		}

		foreach (string videoProjectID in videoProjects)
		{
			AddVideoProject(videoProjectID);
		}
	}

	// Probably replace this with a pooler of some sort
	public void AddVideoProject(string newProjectID) 
	{
		GameObject currentVideoProjectTemplate;

		currentVideoProjectTemplate = (GameObject)Instantiate (videoProjectTemplate, videoProjectlist, false);
		currentVideoProjectTemplate.gameObject.SetActive (true);
		currentVideoProjectTemplate.GetComponent<VideoProjectButton>().SetProjectId (newProjectID);
	}
}
