using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Headjack;

public class VideoBrowser : MonoBehaviour {

	public static VideoBrowser instance;

	public RotationNode origin;

	public Animator animator;
	public CanvasGroup canvasGroup;

	[Header("Video Categories")]
	public GameObject categoryParent;
	public Transform categoryTemplateParent;
	public GameObject categoryTemplate;
	public Button buttonCategoryAll;
	public ExtendedInputHandler inputCategoryAll;
	public List<VideoCategoryToggle> categoriesList = new List<VideoCategoryToggle>();

	[Header("Video Projects")]
	public Transform videoProjectlist; 
	public GameObject videoProjectTemplate;

	[Header("Video Project Pooler")]

	public int initialPooledAmount = 20;
	public List<VideoProjectButton> videoProjectPool = new List<VideoProjectButton>();

	void Awake () 
	{
		instance = this;
	}

	void Start()
	{
		// Set listeners for toggles
		//toggleCategoryAll.onValueChanged.AddListener(refresh => RefreshCategoryAll(refresh));
	}

	public void Show(bool visibility = true)
	{
		//Null-check to prevent this from being called at the start
		if (origin != null)
		{
			origin.LookRotation = VRUIInputModule.instance.CameraRotationY;
		}

		canvasGroup.interactable = visibility;

		gameObject.SetActive(visibility);
		animator.SetBool("Show", visibility);

	}

	public void RefreshCategoryAll(bool refresh)
	{
		if (refresh)
		{
			RefreshVideoList();
		}
	}
		
	public void RefreshVideoList(string refreshCategoryID = null)
	{
		string[] videoProjects = null;

		if (refreshCategoryID == null || refreshCategoryID == "All")
		{
			videoProjects = App.GetProjects ();
		}
		else
		{
			// Link this to the pooler
			videoProjects = App.GetProjects(refreshCategoryID);
		}

		//Iterate over every category and set visuals NB: this is because toggles don't work for some reason in VRUIInputModule
		if (refreshCategoryID == "All")
		{
			foreach (VideoCategoryToggle category in categoriesList)
			{
				category.SetVisuals (refreshCategoryID);
			}
		}
		else
		{
			foreach (VideoCategoryToggle category in categoriesList)
			{
				category.SetVisuals (refreshCategoryID);
			}
		}
			
		// We know we need to hide all objects in the list 
		foreach (VideoProjectButton video in videoProjectPool)
		{
			video.gameObject.SetActive(false);
		}

		// Then use the pooled method to set new information for our category
		foreach (string videoProjectID in videoProjects)
		{
			AddVideoProject(videoProjectID);
		}
	}

	// Probably replace this with a pooler of some sort
	// Minor issue, doesn't seem to cause a performance problem? 
	// Reasses when adding categories.

	/*
	public void AddVideoProject(string newProjectID) 
	{
		GameObject currentVideoProjectTemplate; // Why did I declare this like this?

		currentVideoProjectTemplate = (GameObject)Instantiate (videoProjectTemplate, videoProjectlist, false);
		currentVideoProjectTemplate.gameObject.SetActive (true);
		currentVideoProjectTemplate.GetComponent<VideoProjectButton>().SetProjectId (newProjectID);
	}
	*/

	public void SetupObjectPool() 
	{
		string[] videos = App.GetProjects ();
		int allVideos = videos.Length;

		for (int i = 0; i < allVideos; i++)
		{
			var videoProject = Instantiate(videoProjectTemplate, videoProjectlist, false);
			videoProject.gameObject.SetActive (false);
			videoProjectPool.Add(videoProject.GetComponent<VideoProjectButton>());
		}
	}

	/// <summary>
	/// Find an inactive video project and set the information for it
	/// </summary>
	public void AddVideoProject(string newProjectID)
	{
		for (int i = 0; i < videoProjectPool.Count; i++)
		{
			if (!videoProjectPool [i].gameObject.activeInHierarchy)
			{
				videoProjectPool[i].gameObject.SetActive(true);
				videoProjectPool [i].SetProjectId(newProjectID);
			}
		}
	}

	#region categories
	public void GetProjectCategories()
	{
		string[] categories = null;
		categories = App.GetCategories();


		//Check whether we have any categories at all 
		if (categories.Length == 0)
		{
			categoryParent.SetActive (false);
		}
		else
		{
			categoryParent.SetActive (true);
		}

		// Add one category which will show all videos.
		AddCategory("All");

		foreach (string categoryID in categories)
		{
			AddCategory(categoryID);
		}
	}

	public void AddCategory(string newCategoryID)
	{
		GameObject currentCategoryTemplate = (GameObject)Instantiate (categoryTemplate, categoryTemplateParent, false);
		currentCategoryTemplate.gameObject.SetActive (true);

		var videoCategoryTemplate = currentCategoryTemplate.GetComponent<VideoCategoryToggle>();
		categoriesList.Add(videoCategoryTemplate);
		videoCategoryTemplate.Setup (newCategoryID);
	}




	#endregion
}
