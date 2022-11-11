using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;


namespace Booray.Game
{



    public class CardUIDragDropHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler,IPointerUpHandler
    {
    
        public Transform cardPosition;
    
        [SerializeField] private Canvas canvasInstance;
    
    
        [SerializeField] private CardUIImageHandler cardUIImage;
    
        [SerializeField] private Transform cardHandParentTransform;
        [SerializeField] private Transform cardDraggingParentTransform;
    
         
    
        private RectTransform rectTransform;
    
        
        public bool isDraggable = true;
        private bool isDragging;
        public bool isDroppedOnSensor = false;
        public bool isAboveSensor = false;
    
        private Vector2 defaultSize;
    
        public Action<bool> onCardDragging;
    
    
        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            
            defaultSize = rectTransform.sizeDelta;
        }
    
    
    
    
    
        //------========----------=========-----------------------------
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(isDraggable)
            {
                cardUIImage.SetOnHoverColor();
    
            }
        }
    
        public void OnPointerExit(PointerEventData eventData)
        {
            
            if(isDraggable)
            {
                cardUIImage.SetCardDefualtColor();
    
            }
        }
        //_+_+_+_+_+_+_+__+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_
    
    
       
    
    
    
        //------========----------=========-----------------------------
        public void OnPointerDown(PointerEventData eventData)
        {
            if(isDraggable)
            {
                cardUIImage.SetCardSelectedColor();
    
                
            }
        }
    
    
    
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(isDraggable)
            {
                isDroppedOnSensor = false;
    
                rectTransform.sizeDelta = defaultSize;
    
                transform.SetParent(cardDraggingParentTransform);

                onCardDragging?.Invoke(true);
            }
        }
    
        public void OnDrag(PointerEventData eventData)
        {
            if(isDraggable)
            {
    
                rectTransform.anchoredPosition += eventData.delta / canvasInstance.scaleFactor;
    
            }
        }
    
        public void OnEndDrag(PointerEventData eventData)
        {

            

        }
    
    
        public void OnPointerUp(PointerEventData eventData)
        {
            onCardDragging?.Invoke(false);

            if (isDroppedOnSensor || isAboveSensor)
            {
                if(!isDroppedOnSensor && isAboveSensor)
                {
                    cardUIImage.SetCardDefualtColor();
                    RevertCardToHand();
                }


                Debug.Log(" <- On Card Dragging ->");
                return;
            }

            

            cardUIImage.SetCardDefualtColor();
    
            RevertCardToHand();
    
    
            //if (isDraggable && !isAboveSensor)
            //{
            //    cardUIImage.SetCardDefualtColor();
    
            //    if(!isDroppedOnSensor)
            //    {
            //        RevertCardToHand();
            //    }
            //}  
        }
    
    
        //+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+_+
        
        public void RevertCardToHand()
        {
            transform.SetParent(cardHandParentTransform);
        }
    
        public void SetUpReferencesForDrag(Canvas canvas, Transform cardHolderParent, Transform tempDrag)
        {
            canvasInstance = canvas;
            cardHandParentTransform = cardHolderParent;
            cardDraggingParentTransform = tempDrag;
        }
    
    }

}
