using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Booray.Game;


public class ExchangeUI_DroppableAreaController : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Transform exchangeDroppableParent;
    public Image image;




    public Action onPointerEnterDroppableZone;
    public Action onPointerExitDroppableZone;

    [SerializeField] private Color onPointerEnterColorChange;
    [SerializeField] private Color onPointerExitColorChange;

   public int exchangeLimit = 5;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("On Drop");




        if (eventData.pointerDrag == null || !eventData.pointerDrag.tag.Equals("CardObject"))
        {
            return;
        }

        if(exchangeDroppableParent.childCount<exchangeLimit)
        {
            CardUIController cardController = eventData.pointerDrag.GetComponent<CardUIController>();

            cardController.cardUIDragDropHandler.isDroppedOnSensor = true;

            cardController.transform.SetParent(exchangeDroppableParent);
        }


        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnterDroppableZone?.Invoke();

        image.color = onPointerEnterColorChange;



        if (eventData.pointerDrag != null && eventData.pointerDrag.tag.Equals("CardObject"))
        {
            Debug.Log($" Entered Into {gameObject.name} ");

            if(exchangeDroppableParent.childCount < exchangeLimit)
                eventData.pointerDrag.GetComponent<CardUIController>().cardUIDragDropHandler.isAboveSensor = true;

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExitDroppableZone?.Invoke();

        image.color = onPointerExitColorChange;



        if (eventData.pointerDrag != null && eventData.pointerDrag.tag.Equals("CardObject"))
        {
            Debug.Log($" Entered Into {gameObject.name} ");



            eventData.pointerDrag.GetComponent<CardUIController>().cardUIDragDropHandler.isAboveSensor = false;

        }
    }

    
}
