using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaHandler : MonoBehaviour
{
    public bool forheader = true;

    
    void Start()
    {
        if (forheader)
        {
            Header();
        }
        else
        {

            WhiteBackground();
        }
    }

    public void Header()
    {
        var rectTransform = GetComponent<RectTransform>();

        float diff = Screen.height - Screen.safeArea.height;

     

        float sizeDiff = diff;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.rect.height + sizeDiff);

    }

    public void WhiteBackground()
    {
        var rectTransform = GetComponent<RectTransform>();

        float diff = Screen.height - Screen.safeArea.height;


        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, rectTransform.offsetMax.y - diff);




    }
}