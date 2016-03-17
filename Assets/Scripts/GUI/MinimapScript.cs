using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MinimapScript : MonoBehaviour {

    public Camera minimapCamera;
    public GameObject mainCamera;
    public RawImage minimapIMG;

    public float MapWidth;
    public float MapHeight;

    public MyCameraStatusEnum status;

    public static MinimapScript minimap;

    void Start () {

        MapWidth = 550f;
        MapHeight = 550f;
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
        //This part determines the rect size of minimap
        var minimapRect = minimapIMG.GetComponent<RectTransform>().rect;
        var screenRect = new Rect(
            minimapIMG.transform.position.x,
            minimapIMG.transform.position.y,
            minimapRect.width, minimapRect.height);

        //This part determines what the mouse position is on map, and excludes the map coordinates from it.
        // TODO: the problem might be at this part
        var mousePos = Input.mousePosition;
        mousePos.y -= screenRect.y;
        mousePos.x -= screenRect.x;

        //This is what translates to main camera position from mouse position on minimap.
        var camPos = new Vector3(
            mousePos.x * (MapWidth / screenRect.width),
            mainCamera.transform.position.y,
            mousePos.y * (MapHeight / screenRect.height));

        mainCamera.transform.position = camPos;

        //And debugs to see what the hell is going on
        Debug.Log(screenRect);
        Debug.Log("Map Width: " + MapWidth + " Map Height: " + MapHeight);
        Debug.Log("Mouse Pos." + mousePos);
        Debug.Log("Camera Pos." + camPos);

        //I tried to see if I can use this code to go MANUAL mode here. Nope. No can do. GG.

        status = MyCameraStatusEnum.MANUAL;
    }
}
