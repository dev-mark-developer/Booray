using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TweeningTwoStateButton : MonoBehaviour,IPointerClickHandler
{

    [SerializeField] SliderButtonTween sliderBtnTween;

    

    public UnityEvent<bool> onSwitched_Event;

    private bool isSwitchedOn;

    public void OnPointerClick(PointerEventData eventData)
    {

        
        
        isSwitchedOn = !isSwitchedOn;
        
        onSwitched_Event?.Invoke(isSwitchedOn);

        sliderBtnTween.StartTween(isSwitchedOn);
            
    }



    public void ChangeImageState(bool state)
    {
       
    }

    public void SetState(bool state)
    {
       // Debug.Log($"SetState -> {state} ");

        isSwitchedOn = state;
        //ChangeImageState(state);
        sliderBtnTween.StartTween(isSwitchedOn);
    }


}
