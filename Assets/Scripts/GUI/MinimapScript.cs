using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MinimapScript : MonoBehaviour, /*IPointerEnterHandler, IPointerExitHandler, */IPointerUpHandler, IPointerDownHandler{

    public Camera minimapCamera;
    public GameObject mainCamera;
    public RawImage minimapIMG;

    public float MapWidth;
    public float MapHeight;

    public bool isClicked;

    public static MinimapScript minimap;
    public Rect screenRect; //this is the Rect that determines camera movement status

    void Awake()
    {
        minimap = this;
    }

    void Start () {

        //if the minimap is perfect with minimapsize being 200 and whole map being 750, then to make it working, we need to calculate the map size from minimap size.
        //ie. 750 / 200 = 3,75
        //So, if minimap is 300, we'd get the correct size for map by calculating
        //3,75 * 300

        //This part determines the rect size of minimap
        var minimapRect = minimapIMG.GetComponent<RectTransform>().rect;
        screenRect = new Rect(
            minimapIMG.transform.position.x,
            minimapIMG.transform.position.y,
            minimapRect.width, minimapRect.height);

        MapWidth = 3.75f * minimapRect.width;
        MapHeight = 3.75f * minimapRect.height;
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

        if (screenRect.Contains(Input.mousePosition))
        {
            Debug.Log("No toimiiko tää");
        }

    }

 

    public void MinimapClick()
    {

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

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isClicked = false;
        Debug.Log("False");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isClicked = true;
        Debug.Log("True");
    }
}
