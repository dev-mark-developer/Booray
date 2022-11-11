using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Booray.Game;
public class ExchangeUIController : MonoBehaviour
{
    [SerializeField] private ExchangeUI_DroppableAreaController exchangeDroppableSensorInstance;

    [SerializeField] private Button exchangeBtn;
    [SerializeField] private GameObject exchangePanel;



    [SerializeField] private Image dropSensorImg;


   

    public event Action onExchangeClicked;

    private void Start()
    {
        exchangeBtn.onClick.AddListener(delegate { onExchangeClicked?.Invoke(); });
    }

    public void SetActiveExchangePanel(bool state)
    {
        exchangePanel.SetActive(state);
    }

    public void SetActiveExchangePanel(bool state,int exchangeLimit)
    {
        exchangePanel.SetActive(state);

        exchangeDroppableSensorInstance.exchangeLimit = exchangeLimit;
    }

    public List<CardUIController> GetCardObjectsInExchangePanel()
    {
        List<CardUIController> cardObjectList = new List<CardUIController>();

        for (int i = 0; i < exchangeDroppableSensorInstance.exchangeDroppableParent.childCount; i++)
        {
            Transform transformOfCardObject = exchangeDroppableSensorInstance.exchangeDroppableParent.GetChild(i);

            if (transformOfCardObject.tag.Equals("CardObject"))
            {
                cardObjectList.Add(transformOfCardObject.gameObject.GetComponent<CardUIController>());
            }

        }

        return cardObjectList;
    }

    //public int GetAmountOfCardsInExchangePanel()
    //{
    //    int amountOfCards = 0;


    //}


    public void SetImageRaycast(bool state)
    {
        dropSensorImg.raycastTarget = state;
    }

}
