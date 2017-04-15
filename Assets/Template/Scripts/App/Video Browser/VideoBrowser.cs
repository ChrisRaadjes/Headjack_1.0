using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
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

	[Header("Page Pooler")]
	public List<GameObject> videoPagePool = new List<GameObject>();

	[Header("Video Project Pooler")]
	public int initialPooledAmount = 20;
	public GameObject videoPageTemplate;
	public int videosPerPage;
	public HorizontalScrollSnap scrollSnap;
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
		SetCategoryVisuals(refreshCategoryID);
			
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

	public void SetCategoryVisuals(string refreshCategoryID) 
	{
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
	}

	public void SetupObjectPool() 
	{
		string[] videos = App.GetProjects ();
		int allVideos = videos.Length;
		int totalVideos = videos.Length + 1;

		int requiredPages = Mathf.CeilToInt((float)totalVideos / (float)videosPerPage);
		Debug.Log ("Required Pages is " + requiredPages);

		// Let's first setup the required pages
		for (int p = 0; p < requiredPages; p++)
		{
			var page = Instantiate(videoPageTemplate, transform, false);
			scrollSnap.AddChild (page, false);
			page.SetActive (true);
			videoPagePool.Add (page);
		}

	 	// Now it's time to instantiate our videos
		int videosHandled = 0;
		int currentPage = 0;

		for (int i = 0; i < allVideos; i++)
		{
			// Everytime we enter, check to see if our last page is full.
			if (videosHandled == videosPerPage)
			{
				videosHandled = 0;
				currentPage++;
			}

			//Instantiate the current video under a given page starting at 0;
			var videoProject = Instantiate(videoProjectTemplate, videoPagePool[currentPage].transform, false);
			videoProject.gameObject.SetActive (false);
			videoProjectPool.Add(videoProject.GetComponent<VideoProjectButton>());

			// Increment the amount of videos we've put in the current page
			videosHandled++;
		}
	}

	/// <summary>
	/// Find an inactive page & video project and set the information for it
	/// </summary>
	public void AddVideoProject(string newProjectID)
	{
		// Set the video projects inside that page
		for(int i = 0; i < videoProjectPool.Count; i++)
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

		SetCategoryVisuals("All");
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
