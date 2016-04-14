using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MinimapArmi : MonoBehaviour {

	public Vector3 NW;
	public Vector3 NE;
	public Vector3 SW;
	public Vector3 SE;

	public GameObject cameraObj;
	public Camera minimapCamera;
	public GameObject Minimap;
	public float MapWidth;
	public float MapHeight;

	public Canvas canvas;

	public Vector3 cameraRotation;

	public LineRenderer lineRenderer;

	public GameObject NWcornerDot;
	public GameObject NEcornerDot;
	public GameObject SWcornerDot;
	public GameObject SEcornerDot;

	void Start (){

	}

	void Update() {
		NE = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, cameraObj.transform.position.y));
		NEcornerDot.transform.position = new Vector3 (NE.x, 20, NE.z);
		NW = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, cameraObj.transform.position.y));
		NWcornerDot.transform.position = new Vector3 (NW.x, 20, NW.z);
		SW = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, cameraObj.transform.position.y));
		SWcornerDot.transform.position = new Vector3 (SW.x, 20, SW.z);
		SE = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, cameraObj.transform.position.y));
		SEcornerDot.transform.position = new Vector3 (SE.x, 20, SE.z);

		cameraRotation = cameraObj.transform.rotation.eulerAngles;
		minimapCamera.transform.eulerAngles = new Vector3 (90, cameraRotation.y, 0);

//		lineRenderer.SetPosition (0, new Vector3 (NE.x, 20, NE.z));
//		lineRenderer.SetPosition (1, new Vector3 (NW.x, 20, NW.z));
//		lineRenderer.SetPosition (2, new Vector3 (SW.x, 20, SW.z));
//		lineRenderer.SetPosition (3, new Vector3 (SE.x, 20, SE.z));
	}

	public void MinimapClick()
	{
//		var miniMapRect = Minimap.GetComponent<RectTransform>().rect;
//		var screenRect = new Rect(
//			Minimap.transform.position.x, 
//			Minimap.transform.position.y, 
//			miniMapRect.width, miniMapRect.height);
//
//		var mousePos = Input.mousePosition;
//		Debug.Log ("Hiiri: " + mousePos);
//		mousePos.y -= screenRect.y;
//		mousePos.x -= screenRect.x;
//
//		var camPos = new Vector3(
//			mousePos.x *  (MapWidth / screenRect.width),
//			mousePos.y *  (MapHeight / screenRect.height),
//			cameraObj.transform.position.z);
//		cameraObj.transform.position = camPos;
		Vector3 mousePos = (Input.mousePosition);
		Debug.Log ("Hiiri: " + mousePos);
		Vector2 minimapPos = Minimap.transform.position;
		Debug.Log ("Minimap: " + minimapPos);

//		//Get the 4 corners (in world space) of the raw image gameobject's rect transform on the GUI
//		Vector3[] corners = new Vector3[4];
//		Minimap.GetComponent<RectTransform>().GetWorldCorners(corners);
//		Rect newRect = new Rect(corners[0], corners[2] - corners[0]);
//
//		//Get the pixel offset amount from the current mouse position to the left edge of the minimap
//		//rect transform.  And likewise for the y offset position.
//		float xPositionDeltaPoint = Input.mousePosition.x - newRect.x;
//		float yPositionDeltaPoint = Input.mousePosition.y - newRect.y;
//
//		//Debug.Log("The x position delta is: " + xPositionDeltaPoint);
//		//Debug.Log("The y position delta is: " + yPositionDeltaPoint);
//
//
//		//The value "170" is the raw image size.
//		float compensateForScalingX = 250 * canvas.scaleFactor;
//		//"600" is the current reference resolution height on the Canvas Scaler script.
//		float compensateForScalingY = 250 * canvas.scaleFactor;
//
//		//If the game screen height resolution and the canvas scaler script's "y reference resolution" are
//		//exactly the same, then the division value will be zero.  Since you can't divide by zero, I need
//		//to check for this here.
//		if (compensateForScalingY == 0)
//		{
//			compensateForScalingY = 250f;
//		}
//
//		//The value "170" is the raw image size currently
//		float xPositionCameraCoordinates = (xPositionDeltaPoint / compensateForScalingX);
//		float yPositionCameraCoordinates = (yPositionDeltaPoint / compensateForScalingY);
//
//		Debug.Log (Camera.main.ViewportToWorldPoint(new Vector3(xPositionCameraCoordinates, yPositionCameraCoordinates,40)));
	}


}

