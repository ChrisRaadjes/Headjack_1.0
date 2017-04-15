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
	public ToggleGroup categoriesToggleGroup;
	public int displayedCategories;
	public Transform categoryList;
	public GameObject categoryTemplate;
	public Toggle toggleCategoryAll;
	public ExtendedInputHandler inputCategoryAll;
	public Button buttonOpenCategoryList;

	[Header("Video Projects")]
	public Transform videoProjectlist; 
	public GameObject videoProjectTemplate;

	void Awake () 
	{
		instance = this;
	}

	void Start()
	{
		// Set listeners for toggles
		toggleCategoryAll.onValueChanged.AddListener(refresh => RefreshCategoryAll(refresh));
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

		if (refreshCategoryID == null)
		{
			videoProjects = App.GetProjects ();
		}
		else
		{
			videoProjects = App.GetProjects(refreshCategoryID = refreshCategoryID);
		}

		foreach (string videoProjectID in videoProjects)
		{
			AddVideoProject(videoProjectID);
		}
	}

	// Probably replace this with a pooler of some sort
	// Minor issue, doesn't seem to cause a performance problem? 
	// Reasses when adding categories.

	public void AddVideoProject(string newProjectID) 
	{
		GameObject currentVideoProjectTemplate; // Why did I declare this like this?

		currentVideoProjectTemplate = (GameObject)Instantiate (videoProjectTemplate, videoProjectlist, false);
		currentVideoProjectTemplate.gameObject.SetActive (true);
		currentVideoProjectTemplate.GetComponent<VideoProjectButton>().SetProjectId (newProjectID);
	}

	#region categories
	public void GetProjectCategories()
	{
		string[] categories = null;
		categories = App.GetCategories();

		/*
		//Check whether we have any categories at all 
		if (categories.Length == 0)
		{
			// Hide the category bar
		}
		else
		if (categories.Length > 1 && categories.Length <= 5)
		{
			//Show the category bar, but hide the more categories button
		}
		else
		{
			// Show the category bar & the more categories buttons
		}
		*/
		
	
		// We limit the amount of categories we display in the mainbar to 5
		foreach (string categoryID in categories)
		{
			AddCategory(categoryID);
		}
	}

	public void AddCategory(string newCategoryID)
	{
		GameObject currentCategoryTemplate = (GameObject)Instantiate (categoryTemplate, categoryList, false);
		currentCategoryTemplate.gameObject.SetActive (true);
		currentCategoryTemplate.GetComponent<VideoCategoryToggle> ().Setup(newCategoryID);
	}
	#endregion
}
