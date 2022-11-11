using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Booray.Game;
using UnityEngine.EventSystems;

public class DeckSkinBtnUIHandler : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI skinNameTxt;
    [SerializeField] private Image skinImg;

    [SerializeField] private Image btnImg;

    public Toggle skinToggle;

    [SerializeField] private CardSkinObject cardSkinObj;

    [SerializeField] private Sprite cardPreviewSprite;
    [SerializeField] private Sprite cardPreviewLockedSprite;

    [SerializeField] private Sprite btnSelectedSprite;
    [SerializeField] private Sprite btnNormalSprite;

    [SerializeField] private string skinName;

    public Action<CardSkinObject> onDeckSkinBtnClicked;


    private void Awake()
    {
        skinToggle.onValueChanged.AddListener(SetSelected);
        
    }

    

    private void Start()
    {
        skinNameTxt.text = cardSkinObj.skinName;
    
        
        //LockBtn();
       // gameObject.GetComponent<Toggle>().OnPointerClick.AddListener(delegate { SFXHandler.instance.PlayBtnClickSFX(); });
        //LockBtn();
        //skinBtn.onValueChanged.AddListener(delegate {   onDeckSkinBtnClicked?.Invoke(cardSkinObj); });
    }

    public void SetSelected(bool state)
    {
        

        Debug.Log($"Toggle is being selected to {state} ");
        if(state)
        {
            btnImg.sprite = btnSelectedSprite;
            onDeckSkinBtnClicked?.Invoke(cardSkinObj);
           
        }
        else
        {
            btnImg.sprite = btnNormalSprite;
        }
    }


    public void LockBtn()
    {
        skinToggle.interactable = false;
        skinToggle.isOn = false;
        skinImg.sprite = cardPreviewLockedSprite;
        
    }

    public void UnlockBtn()
    {
        skinToggle.interactable = true;
        
        skinImg.sprite = cardPreviewSprite;
    }

    public string GetSkinId()
    {
        return cardSkinObj.skinID;
    }

    public CardSkinObject GetSkinObject()
    {
        return cardSkinObj;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SFXHandler.instance.PlayBtnClickSFX();
    }
  
}
