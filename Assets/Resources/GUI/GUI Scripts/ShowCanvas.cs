using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Show canvas. Like the name says, show the specific canvas, and if it's open already, close it.
/// </summary>

public class ShowCanvas : MonoBehaviour {

    public Animator animator;
    public GameObject constPanel;
    public GameObject unitPanel;
    public Button constBtn;
    public Button unitBtn;

    //public bool animate = false;


    public void ToggleCanvasConst()
    {
        //This code of line sets the rule for canvas to be active if it isn't already, and vice versa
        constPanel.SetActive(!constPanel.activeSelf);

        if (constPanel.activeSelf == false && unitPanel.activeSelf == true)
        {
            animator.enabled = false;
            return;
        }

        else if (constPanel.activeSelf == true && unitPanel.activeSelf == false)
        {
            animator.SetTrigger("open");
            return;
            //animate = false;
        }

        else if (constPanel.activeSelf == true && unitPanel.activeSelf == true)
        {
            unitPanel.SetActive(!unitPanel.activeSelf);
        }

        else if (constPanel.activeSelf == false && unitPanel.activeSelf == false)
        {
            animator.enabled = true;
            animator.SetTrigger("close");
            //animate = true;
            return;
        }

        /* if (animate == true)
        {
            animator.SetTrigger("new state");
        }*/
    }


    public void ToggleCanvasUnits() {
        //This code of line sets the rule for canvas to be active if it isn't already, and vice versa
        unitPanel.SetActive(!unitPanel.activeSelf);

        if (constPanel.activeSelf == true && unitPanel.activeSelf == false)
        {
            animator.enabled = false;
            return;
        }

        else if (constPanel.activeSelf == false && unitPanel.activeSelf == true)
        {
            animator.SetTrigger("open");
            return;
        }

        else if (constPanel.activeSelf == true && unitPanel.activeSelf == true)
        {
            constPanel.SetActive(!constPanel.activeSelf);
        }

        else if (constPanel.activeSelf == false && unitPanel.activeSelf == false)
        {
            animator.enabled = true;
            animator.SetTrigger("close");
            return;
            //animate = true;
        }

        /* if (animate == true)
        {
            animator.SetTrigger("new state");
        } */
    }

}
