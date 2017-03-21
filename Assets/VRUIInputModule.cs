using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Headjack;

public class VRUIInputModule : BaseInputModule {

	public static VRUIInputModule instance { get { return _instance; } }
	private static VRUIInputModule _instance = null;

	// Storage class for controller specific data

	public class ControllerData
	{
		public PointerEventData pointerEvent;
		public GameObject currentPointObject;
		public GameObject currentPressedObject;
		public GameObject currentDraggingObject;
	};

	public ControllerData gazeControllerData = new ControllerData();
	private Transform uiRaycastSource;


	private Camera UICamera;
	public float CameraRotation
	{
		get
		{
			if (UICamera != null) 
			{
				return UICamera.transform.eulerAngles.x;
			}
			else 
			{
				Debug.LogAssertion ("No UI Camera found! Returning rotation 0"); 
				return 0f;
			}
		}
	}
	// Laser pointers

	protected override void Awake()
	{
		base.Awake ();

		if (_instance != null)
		{
			Debug.LogWarning ("Trying to instantiate multiple VR UI Input Modules.");
			DestroyImmediate (this.gameObject);
		}

		_instance = this;
	}

	protected override void Start() 
	{
		base.Start ();

		// Create a new camera that will be used for raycasts
		UICamera = new GameObject("VR UI Camera").AddComponent<Camera>();
		UICamera.clearFlags = CameraClearFlags.Nothing;
		UICamera.cullingMask = 0;
		UICamera.fieldOfView = 5;
		UICamera.nearClipPlane = 0.01f;

		// Find canvases in the scene and assign our custom UI Camera to them
		Canvas[] canvases = Resources.FindObjectsOfTypeAll<Canvas>();
		foreach (Canvas canvas in canvases)
		{
			canvas.worldCamera = UICamera;
		}
	}

	public void AddController() 
	{
		// Add controller here
	}

	public void RemoveController() 
	{
		// Remove controller here
	}

	//Assigns the raycast source cam;
	public void SetupUICamera()
	{
		CenterEye centerEye = (CenterEye)FindObjectOfType(typeof(CenterEye));
		uiRaycastSource = centerEye.transform;

		if (uiRaycastSource != null)
		{
			Debug.Log ("Center eye is transform of object " + uiRaycastSource.name);
		}
		else
		{
			Debug.Log ("Center eye transform not found");
		}
	}

	protected void UpdateCameraPosition()
	{
		// The UI Camera is aligned to the center eye camera of Headjack if we're using gaze
		// Null-check this to get rid of unneccesary errors.

		if (uiRaycastSource != null)
		{
			UICamera.transform.position = uiRaycastSource.position;
			UICamera.transform.rotation = uiRaycastSource.rotation;
		}
	}

	// Clear the current selection
	public void ClearSelection()
	{
		if (base.eventSystem.currentSelectedGameObject)
		{
			//Debug.Log ("Clearing currently selected object...");
			var go = base.eventSystem.currentSelectedGameObject;
			base.eventSystem.SetSelectedGameObject (null);
			//Debug.Log ("Cleared selected object: " + go.name);
		}
	}
		
	// Select a GameObject
	private void Select(GameObject go)
	{
		ClearSelection();

		if (ExecuteEvents.GetEventHandler<ISelectHandler>(go))
		{
			base.eventSystem.SetSelectedGameObject(go);
		}
	}

	float distance;
	float _distanceLimit;


	public override void Process() 
	{
		// Test if UICamera is looking at a GUI element
		UpdateCameraPosition ();

		ControllerData controllerData = gazeControllerData;

		if (controllerData.pointerEvent == null)
		{
			controllerData.pointerEvent = new PointerEventData (eventSystem);
		}
		else
		{
			controllerData.pointerEvent.Reset();
		}

		controllerData.pointerEvent.delta = Vector2.zero;
		controllerData.pointerEvent.position = new Vector2 (UICamera.pixelWidth * 0.5f, UICamera.pixelHeight * 0.5f);

		// Trigger a raycast
		eventSystem.RaycastAll(controllerData.pointerEvent, m_RaycastResultCache);
		controllerData.pointerEvent.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
		m_RaycastResultCache.Clear ();

		// Make sure our controllers knows about the raycasul result
		// We add 0.01 because that is the near plane distance of our camera and we want the correct distance
		if(controllerData.pointerEvent.pointerCurrentRaycast.distance > 0.0f) 
		{
			// controller.LimitLaserDistance(data.pointEvent.pointerCurrentRaycast.distance + 0.01f);
		}

		// Stop if no UI element was hit
		/*
		if(data.pointerEvent.pointerCurrentRaycast.gameObject == null)
		{
			return;
		}
		*/

		var hitControl = controllerData.pointerEvent.pointerCurrentRaycast.gameObject;
		/*
		// THIS ENTIRE SECTION IS NOT NEEDED
		// Send control enter and exit events to our controller
		if (data.currentPointObject != hitControl)
		{
			if (data.currentPointObject != null)
			{
				// controller onExitControl
			}

			if (hitControl != null)
			{
				// controller on enter control
			}
		}
		*/

		controllerData.currentPointObject = hitControl;

		// Handle enter and exit events on the GUI controls that are hit
		base.HandlePointerExitAndEnter(controllerData.pointerEvent, controllerData.currentPointObject);

		if (Input.GetKeyDown(KeyCode.Mouse0))
		{

			/// Start of button down. Need to handle this with the generic confirm button instead.
			ClearSelection ();

			controllerData.pointerEvent.pressPosition = controllerData.pointerEvent.position;
			controllerData.pointerEvent.pointerPressRaycast = controllerData.pointerEvent.pointerCurrentRaycast;
			controllerData.pointerEvent.pointerPress = null;

			// Update currently pressed object if the cursor is over an element
			if (controllerData.currentPointObject != null)
			{
				controllerData.currentPressedObject = controllerData.currentPointObject;
			}

			GameObject newPressed = ExecuteEvents.ExecuteHierarchy (controllerData.currentPressedObject, 
									   controllerData.pointerEvent,
				                       ExecuteEvents.pointerDownHandler);
			if (newPressed == null)
			{
				// Some UI elements might only have a click handler and not pointer down handler
				newPressed = ExecuteEvents.ExecuteHierarchy (controllerData.currentPressedObject, 
					controllerData.pointerEvent, 
					ExecuteEvents.pointerClickHandler);
				if (newPressed != null)
				{
					controllerData.currentPressedObject = newPressed;
				}
			}
			else
			{
				controllerData.currentPressedObject = newPressed;

				// We want to do click on button down at the same time, unlike regular mouse processing
				// Wich does click when mouse goes up over same object it went down on
				// Reason to do this is head tracking might be jittery and this makes it easier to click buttons.

				ExecuteEvents.Execute (newPressed, controllerData.pointerEvent, ExecuteEvents.pointerClickHandler);
			}

			if (newPressed != null)
			{
				controllerData.pointerEvent.pointerPress = newPressed;
				controllerData.currentPressedObject = newPressed;
				Select(controllerData.currentPressedObject);
			}
		}
		/// End of Button down

		/// Start of button up
		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			if(controllerData.currentDraggingObject != null)
			{
				ExecuteEvents.Execute (controllerData.currentDraggingObject, controllerData.pointerEvent, ExecuteEvents.endDragHandler);

					if (controllerData.currentPointObject != null)
				{
						ExecuteEvents.ExecuteHierarchy (controllerData.currentPointObject, controllerData.pointerEvent, ExecuteEvents.dropHandler);
				}

				controllerData.pointerEvent.pointerDrag = null;
				controllerData.currentDraggingObject = null;
			}

				if(controllerData.currentPressedObject)
			{
					ExecuteEvents.Execute (controllerData.currentPressedObject, controllerData.pointerEvent, ExecuteEvents.pointerUpHandler);
					controllerData.pointerEvent.rawPointerPress = null;
					controllerData.pointerEvent.pointerPress = null;
					controllerData.currentPressedObject = null;
			}
		}

		///
		/// Drag Handling
		///

		if (controllerData.currentDraggingObject != null)
		{
			ExecuteEvents.Execute (controllerData.currentDraggingObject, controllerData.pointerEvent, ExecuteEvents.dragHandler);
		}

		// Update selected element for keyboard focus
		if (base.eventSystem.currentSelectedGameObject != null)
		{
			ExecuteEvents.Execute (eventSystem.currentSelectedGameObject, GetBaseEventData (), ExecuteEvents.updateSelectedHandler);
		}
	}

	#region input save
	[HideInInspector] public bool confirmPressed;

	public void Update()
	{
		// Helper to immediatley clear pressed when transitioning between browse and play video
		if (VRInput.Confirm.Pressed) 
		{
			Debug.Log ("Confirm was pressed");
			confirmPressed = true;
		}

		if (VRInput.Confirm.Released) 
		{
			Debug.Log("Confirm was released");
			confirmPressed = false;
		}
	}
	#endregion
}
