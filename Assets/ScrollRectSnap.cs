using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectSnap : MonoBehaviour {

	public RectTransform panel;      // Holds the scroll panel
	public Button[] buttons;		     // All objects
	public RectTransform center;     // Center measurement point

	private float[] distance;		 //All buttons distances from the center
	private bool dragging = false;   // Will be true while we drag the panel
	private int buttonDistance; 	 // Will hold the distance between the buttons
	private int minButtonNumber;    // To hold the number of the object with smallest distance to the center. 

	void Start()
	{
		int buttonLength = buttons.Length;
		distance = new float[buttons.Length];

		// Get distance between buttons
		buttonDistance = (int)Mathf.Abs(buttons[1].GetComponent<RectTransform>().anchoredPosition.x - 
										buttons[0].GetComponent<RectTransform>().anchoredPosition.x);
	}

}
