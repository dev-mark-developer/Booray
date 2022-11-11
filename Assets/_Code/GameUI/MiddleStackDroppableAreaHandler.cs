using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Booray.Game;
public class MiddleStackDroppableAreaHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{

    public Transform middleStackDroppableParent;

    public event Action<Card> onDroppedAtSensorEvent;

    public Image sensorImage;

    [SerializeField] private Color onPointerEnterColorChange;
    [SerializeField] private Color onPointerExitColorChange;



    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("On Drop");

        if (eventData.pointerDrag == null || !eventData.pointerDrag.tag.Equals("CardObject"))
        {
            return;
        }


        CardUIController cardController = eventData.pointerDrag.GetComponent<CardUIController>();

        cardController.cardUIDragDropHandler.isDroppedOnSensor = true;

        cardController.transform.SetParent(middleStackDroppableParent);

        Card card = cardController.card;

        cardController.gameObject.SetActive(false);

        onDroppedAtSensorEvent?.Invoke(card);




    }

    public void resetSensorColor()
    {

        sensorImage.color = onPointerExitColorChange;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        sensorImage.color = onPointerEnterColorChange;
        
        if (eventData.pointerDrag != null && eventData.pointerDrag.tag.Equals("CardObject"))
        {
            Debug.Log($" Entered Into {gameObject.name} ");




            eventData.pointerDrag.GetComponent<CardUIController>().cardUIDragDropHandler.isAboveSensor = true;



        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        sensorImage.color = onPointerExitColorChange;
        
        if (eventData.pointerDrag != null && eventData.pointerDrag.tag.Equals("CardObject"))
        {
            Debug.Log($" Entered Into {gameObject.name} ");


            eventData.pointerDrag.GetComponent<CardUIController>().cardUIDragDropHandler.isAboveSensor = false;


        }
    }
}
