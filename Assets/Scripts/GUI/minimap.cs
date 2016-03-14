using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class minimap : MonoBehaviour {

    public Camera minimapCamera;
    public Camera mainCamera;
    public RawImage minimapIMG;

    public float MapWidth;
    public float MapHeight;

    void Start () {

        MapWidth = minimapCamera.pixelWidth;
        MapHeight = minimapCamera.pixelHeight;
    }

	// Update is called once per frame
	void Update () {

        /*//If left button is pressed and click is in the minimap Rect
	    if (Input.GetMouseButtonDown(0) && GetComponent<Camera>().pixelRect.Contains(Input.mousePosition))
        {
            RaycastHit hit;
            Debug.Log("Eka vaihe");

            Ray ray = minimapCamera.ScreenPointToRay(Input.mousePosition);
            Debug.Log("Toinen vaihe!");

            if (Physics.Raycast(ray, out hit))
            {
                mainCamera.transform.position = hit.point;
                Debug.Log("Kolmas Vaihe!!");
                //hit.point contains the point where the Ray hits the object named MinimapCollider
                Debug.Log(hit.point);
            }

        }*/

	}

    public void MinimapClick()
    {
        var minimapRect = minimapIMG.GetComponent<RectTransform>().rect;
        var screenRect = new Rect(
            minimapIMG.transform.position.x,
            minimapIMG.transform.position.y,
            minimapRect.width, minimapRect.height);

        var mousePos = Input.mousePosition;
        mousePos.z -= screenRect.y;
        mousePos.x -= screenRect.x;

        Debug.Log(mousePos + "Mouse Pos.");

        var camPos = new Vector3(
            mousePos.x * (MapWidth / screenRect.width),
            mousePos.z * (MapHeight / screenRect.height),
            Camera.main.transform.position.z);

        Camera.main.transform.position = camPos;

        Debug.Log(camPos + "Camera Pos.");
    }
}
