using UnityEngine;
using System.Collections;

public class GraphicsOptions : MonoBehaviour
{

    public void ChangeAntiAliasing(int value)
    {
        switch (value)
        {
            case 0:
                QualitySettings.antiAliasing = 0;
                break;

            case 1:
                QualitySettings.antiAliasing = 2;
                break;

            case 2:
                QualitySettings.antiAliasing = 4;
                break;

            case 3:
                QualitySettings.antiAliasing = 8;
                break;

        }



    }

    public void ChangeResolution(int value)
    {
        switch (value)
        {
            case 0:
                Screen.SetResolution(1280, 720, true, 0);
                break;

            case 1:
                Screen.SetResolution(1366, 768, true, 0);
                break;

            case 2:
                Screen.SetResolution(1600, 900, true, 0);
                break;

            case 3:
                Screen.SetResolution(1980, 1080, true, 0);
                break;

        }
    }

    public void SetTextureQuality(int value)
    {
        switch (value)
        {
            case 0:
                QualitySettings.masterTextureLimit = 3;
                break;

            case 1:
                QualitySettings.masterTextureLimit = 2;
                break;

            case 2:
                QualitySettings.masterTextureLimit = 1;
                break;

            case 3:
                QualitySettings.masterTextureLimit = 0;
                break;

        }
    }

    public void SetFullScreen(bool value)
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, value, 0);
    }

    public void SetVSync(bool value)
    {
        if (value) QualitySettings.vSyncCount = 1;
        else QualitySettings.vSyncCount = 0;
    }

    public void UseAnisotropicFiltering(bool value)
    {
        if (value) QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
        else QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
    }
}
