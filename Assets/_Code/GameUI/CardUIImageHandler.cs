using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Booray.Game
{


public class CardUIImageHandler : MonoBehaviour
{
    

    [SerializeField] private Image cardImage;

    [SerializeField] private Color onHoverColor;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color cardDisabledColor;

    public void SetImage(Sprite sprite)
    {
        cardImage.sprite = sprite;

    }

    public void SetCardDisableColor()
    {
        cardImage.color = cardDisabledColor;
    }

    public void SetCardSelectedColor()
    {
        cardImage.color = selectedColor;
    }

    public void SetCardDefualtColor()
    {
        cardImage.color = defaultColor;
    }

    public void SetOnHoverColor()
    {
        cardImage.color = onHoverColor;
    }

    public void SetRaycastTarget(bool isRaycastTarget)
    {
        cardImage.raycastTarget = isRaycastTarget;
    }


    
}
}
