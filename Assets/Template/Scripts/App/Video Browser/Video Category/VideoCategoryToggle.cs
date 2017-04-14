using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Headjack;
using TMPro;

public class VideoCategoryToggle : MonoBehaviour {

	public TextMeshProUGUI textCategoryName;
	public Toggle toggleCategory;
	public string categoryId;

	void Start() 
	{
		// On click listener?
		toggleCategory.onValueChanged.AddListener(refresh => SetCategory(refresh));
	}

	public void Setup(string _categoryId) 
	{
		//Retrieve and set the category metadata
		categoryId = _categoryId;
		App.CategoryMetadata categoryData;
		categoryData = App.GetCategoryMetadata(categoryId);

		name = "Category_Template_" + categoryData.Name;
		textCategoryName.text = categoryData.Name;
	}

	public void SetCategory(bool refresh) 
	{
		if (refresh && categoryId != null)
		{
			VideoBrowser.instance.RefreshVideoList (categoryId);
		}
	}
}
