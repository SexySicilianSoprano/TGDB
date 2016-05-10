using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ScreenEdgeScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
  
    public float screen_x;
    public float screen_y;
    public GameObject mainCamera { get { return GameObject.Find("MyCamera"); } }

    public float panSpeed;

    public bool isEdge;

    public MyCamera MyCamera { get { return GameObject.Find("MyCamera").GetComponent<MyCamera>(); } }

    void Start()
    {
        isEdge = false;
    }

    void Update()
    {
        if (isEdge == true)
        {
            Vector3 movement = Vector3.zero;
            movement.x += screen_x;
            movement.z += screen_y;

            mainCamera.transform.Translate(movement * Time.deltaTime * panSpeed, Space.World); // move based on local space.
        }
    }

    //When mouse cursor enters the area, move the camera according to screen space
    public void OnPointerEnter(PointerEventData eventData)
    {
        MyCamera.isEdge = true;
        isEdge = true;


        Debug.Log("Screen Edge movement");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       MyCamera.isEdge = false;

        isEdge = false;
    }
}
