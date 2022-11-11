using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaController : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    RectTransform panelSafeArea;

    Rect currentSafeArea = new Rect();


    private void Start()
    {
        currentSafeArea = Screen.safeArea;
        panelSafeArea = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        if (panelSafeArea == null)
            return;

        Rect safeArea = Screen.safeArea;
        Vector2 anchorMin = safeArea.position ;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= canvas.pixelRect.width;
        anchorMin.y /= canvas.pixelRect.height;

        anchorMax.x /= canvas.pixelRect.width;
        anchorMax.y /= canvas.pixelRect.height;

        ////panelSafeArea.anchorMin = anchorMin;
        ////panelSafeArea.anchorMax = anchorMax;

        panelSafeArea.anchorMin = anchorMin;
        panelSafeArea.anchorMax = anchorMax;

        currentSafeArea = Screen.safeArea;

    }

    private void Update()
    {
        if(currentSafeArea != Screen.safeArea)
        {

            Debug.Log("running");
            ApplySafeArea();
        }
    }

}
