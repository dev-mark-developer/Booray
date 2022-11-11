using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TwoStateButton : MonoBehaviour,IPointerClickHandler
{
    [SerializeField]private Sprite offStateSprite;
    [SerializeField] private Sprite onStateSprite;


    [SerializeField] private Image img;

    public UnityEvent<bool> onSwitched_Event;

    private bool isSwitchedOn;

    public bool interactable;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(interactable)
        //Logging.Log($"Switching State to {!isSwitchedOn}");
        
        isSwitchedOn = !isSwitchedOn;
        
        onSwitched_Event?.Invoke(isSwitchedOn);

        ChangeImageState(isSwitchedOn);
            
    }



    public void ChangeImageState(bool state)
    {
        if(state)
        {
            img.sprite = onStateSprite;
        }
        else
        {
            img.sprite = offStateSprite;
        }
    }

    public void SetState(bool state)
    {
        isSwitchedOn = state;
        ChangeImageState(state);
    }


}
