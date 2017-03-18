using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Headjack;

public class PlayVideoScript : MonoBehaviour {

	// Use this for initialization
	void Start () {

		App.Initialize (Initialized, true, true);
	}

	void Initialized(bool success, string error)
	{
		if (success)
		{
			if (App.GetProjects ().Length > 0)
			{
				// ID of the first project
				string firstProjectID = App.GetProjects () [0];
				// Stream the first project (over wifi only) 
				App.Play (firstProjectID, true, true, delegate(bool videoSuccess, string videoError) {
					App.ShowMessage ("You just watched the first project", 5f); 
				});
			}
			else
			{
				// Tell the user that this app contains no projects
				App.ShowMessage ("This app contains no videos to show", 5f);
			}
		}
		else
		{
			// Show the error message, because App.Initialize failed
			App.ShowMessage(error, 5f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}