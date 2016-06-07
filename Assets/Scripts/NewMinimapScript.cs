using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This script is done by Armi, and it has all the main functions on the minimap. Needs more commenting, although it's not hard to figure out the meaning of this code.
/// </summary>

public class NewMinimapScript : MonoBehaviour
{

    private Vector3 NW;
    private Vector3 NE;
    private Vector3 SW;
    private Vector3 SE;

    public GameObject cameraObj;
    public Camera minimapCamera;
    public Camera minimapBoxCamera;
    public GameObject Minimap;
    public float MapWidth;
    public float MapHeight;
    public GameObject fowMinimap;

    public Canvas canvas;

    public Vector3 cameraRotation;
    public float cameraDistance;
    public Vector3 minmapcampos;

    public GameObject NWcornerDot;
    public GameObject NEcornerDot;
    public GameObject SWcornerDot;
    public GameObject SEcornerDot;

    public Vector3[] corners;

    public Texture2D minimapRevealTexture;

    public GameObject minimapMask;

    public GameObject arrow;

    
    FogOfWar _fog;
    Texture2D _texture;
    GUIStyle _panelStyle;
    

    Camera _camera;
    Transform _cameraTransform;


    void Start()
    { 
        _fog = GetComponent<FogOfWar>();
        _camera = GetComponent<Camera>();
        _cameraTransform = transform;

        if (_texture == null)
        {
            _texture = new Texture2D(_fog.texture.width, _fog.texture.height);
            _texture.wrapMode = TextureWrapMode.Clamp;
        }
    }

    void FixedUpdate()
    {
        cameraDistance = Vector3.Distance(cameraObj.transform.position, cameraObj.GetComponent<MyCamera>().focusPoint.transform.position);

        NE = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, cameraDistance));
        NEcornerDot.transform.position = new Vector3(NE.x, 20, NE.z);
        NW = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, cameraDistance));
        NWcornerDot.transform.position = new Vector3(NW.x, 20, NW.z);
        SW = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, cameraDistance));
        SWcornerDot.transform.position = new Vector3(SW.x, 20, SW.z);
        SE = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, cameraDistance));
        SEcornerDot.transform.position = new Vector3(SE.x, 20, SE.z);

        cameraRotation = cameraObj.transform.rotation.eulerAngles;
        minimapCamera.transform.eulerAngles = new Vector3(90, cameraRotation.y, 0);
        minimapBoxCamera.transform.eulerAngles = new Vector3(90, cameraRotation.y, 0);
        fowMinimap.transform.eulerAngles = new Vector3(0, 0, cameraRotation.y);
        arrow.transform.eulerAngles = new Vector3(0, 0, cameraRotation.y);
        GameObject.Find("MinimapFocusBox").transform.eulerAngles = cameraRotation;


    }

    public void MinimapClick()
    {

        Vector3 mousePos = (Input.mousePosition);
        Vector2 minimapPos = Minimap.transform.position;

        //Get the 4 corners (in world space) of the raw image gameobject's rect transform on the GUI
        corners = new Vector3[4];
        Minimap.GetComponent<RectTransform>().GetWorldCorners(corners);
        Rect newRect = new Rect(corners[0], corners[2] - corners[0]);


        //Get the pixel offset amount from the current mouse position to the left edge of the minimap
        //rect transform.  And likewise for the y offset position.
        float xPositionDeltaPoint = Input.mousePosition.x - newRect.x;
        float yPositionDeltaPoint = Input.mousePosition.y - newRect.y;

        minmapcampos = minimapCamera.ViewportToWorldPoint(new Vector3(xPositionDeltaPoint / newRect.width, yPositionDeltaPoint / newRect.height, 0f));
        cameraObj.GetComponent<MyCamera>().status = MyCameraStatusEnum.MANUAL;
        cameraObj.transform.position = new Vector3(minmapcampos.x + 20f, cameraObj.transform.position.y, minmapcampos.z - 40f);

    }

    
    void OnGUI()
    {

        byte[] original = _fog.texture.GetRawTextureData();
        Color32[] pixels = new Color32[original.Length];
        for (int i = 0; i < pixels.Length; ++i)
            pixels[i] = original[i] < 255 ? new Color32(20, 20, 20, 0) : new Color32(0, 0, 0, 255);
        minimapRevealTexture.SetPixels32(pixels);
        minimapRevealTexture.Apply();



    }
    

}

