using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ScreenEdgeScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    
    public static ScreenEdgeScript ScreenEdge;

    public float screen_x;
    public float screen_y;
    public GameObject MainCamera;

    public float panSpeed;

    public bool isEdge;

    void Start()
    {
        ScreenEdge = this;
        isEdge = false;
    }

    void Update()
    {

    }

    //When mouse cursor enters the area, move the camera according to screen space
    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector3 movement = Vector3.zero;
        movement.x += screen_x;
        movement.z += screen_y;

        MainCamera.transform.Translate(movement * Time.deltaTime * panSpeed, Space.World); // move based on local space.

        isEdge = true;

        Debug.Log("Screen Edge movement");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isEdge = false;
    }
}
