using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to control the orientation of a given piece of UI in relation to the viewer
/// </summary>
public class RotationNode : MonoBehaviour 
{
	public float LookRotation
	{
		get
		{
			return transform.localRotation.eulerAngles.y;
		}
		set 
		{
			transform.localRotation = Quaternion.Euler (0f, value, 0f);
		}
	}
}
