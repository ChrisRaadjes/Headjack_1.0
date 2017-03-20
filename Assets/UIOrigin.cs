using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to control the orientation of a given piece of UI in relation to the viewer
/// </summary>
public class UIOrigin : MonoBehaviour {

	private Transform lookRotation;
	public float LookRotation
	{
		set 
		{ 
			Vector3 _tmp = lookRotation.rotation.eulerAngles;
			_tmp.x = value;
			lookRotation.rotation.eulerAngles = _tmp;
		}
	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
