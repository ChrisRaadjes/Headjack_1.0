using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictWorldPosition : MonoBehaviour {

	public Transform parent;
	public Transform child;

	void Start () 
	{

		Vector3 newLocalPosition = child.localPosition;
		Debug.Log (newLocalPosition);
		newLocalPosition -= new Vector3 (20f, 0f, 0f);
		Debug.Log (newLocalPosition);
		Vector3 result = transform.TransformPoint(newLocalPosition);
		Debug.Log (child.position);
		Debug.Log (result);

	}

	void Update () {
		
	}
}
