using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Headjack;
using TMPro;

public class VideoCategoryToggle : MonoBehaviour {

	public Animator animator;
	public TextMeshProUGUI textCategoryName;
	public Button buttonCategory;
	//public Toggle toggleCategory;
	public string categoryId;

	void Start() 
	{
		// On click listener?
		buttonCategory.onClick.AddListener(SetCategory);
		//toggleCategory.onValueChanged.AddListener(refresh => SetCategory(refresh));
	}

	public void Setup(string _categoryId) 
	{
		//Retrieve and set the category metadata
		categoryId = _categoryId;

		if(categoryId == "All")
		{
			name = "Category_Template_" + "All";
			textCategoryName.text = "All";
		}
		else
		{
			App.CategoryMetadata categoryData;
			categoryData = App.GetCategoryMetadata(categoryId);

			name = "Category_Template_" + categoryData.Name;
			textCategoryName.text = categoryData.Name;
		}
	}

	public void SetCategory()
	{
		VideoBrowser.instance.RefreshVideoList(categoryId);
	}

	public void SetVisuals(string _categoryID)
	{
		bool categoryMatchesCurrent = (categoryId == _categoryID);
		animator.SetBool ("Toggle", categoryMatchesCurrent);
	}
}
