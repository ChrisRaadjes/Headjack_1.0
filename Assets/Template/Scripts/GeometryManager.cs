using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryManager : MonoBehaviour {

	public static GeometryManager instance;

	public void Awake()
	{
		instance = this;
		Show (false);
	}

	public void Show(bool visibility = true) 
	{
		gameObject.SetActive(visibility);
	}
}
