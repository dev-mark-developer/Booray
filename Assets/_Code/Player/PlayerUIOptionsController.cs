using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using UnityEngine.U2D;

public class PlayerUIOptionsController : MonoBehaviour
{
    [SerializeField] private RectTransform pnlPlayerOptions;
    
    [SerializeField] private Button addFriendBtn;
  

    [SerializeField] private TextMeshProUGUI addFriendBtn_Txt;
    #region Profile 
    [SerializeField] private Button ViewProfileBtn;

    [SerializeField] private GameObject ProfilePanel;

    public TextMeshProUGUI CoinTxt;
    public TextMeshProUGUI PlayerNameTxt;
    public Image ProfileImage;
   
    public Image[] GPImagesPiechart;
    public float[] GPPievalues;

    public Image[] GWImagesPiechart;
    public float[] GWPievalues;
    #endregion





    public Action onAddfriendClicked_Event;
    public Action onAddfriendClicked_Events;

    public Action onOptionsClicked;

    public Action onInGameProfileViewClicked_Event;

   // public Action onInGameViewDeckSkinsClicked_Event;

    [SerializeField] private Button playerOptionsBtn;
    private bool isOptionsOpen;




    [Header("Tween Controls")]
    [SerializeField] private float tweenDuration_popOut;
    [SerializeField] private float tweenDuration_popIn;
    
    [SerializeField] private Ease easeType_popOut;
    [SerializeField] private Ease easeType_popIn;
    
    [SerializeField] private Vector3 targetScale;


    [SerializeField] private bool isLocal;


    bool alreadyCheckedFriend = false;

    private void Start()
    {
        ClosePlayerOptionsPanel();

        playerOptionsBtn.onClick.AddListener(SetOptionsPanelToggle);

        addFriendBtn.onClick.AddListener(delegate { onAddfriendClicked_Event?.Invoke(); });

        ViewProfileBtn.onClick.AddListener(delegate { onInGameProfileViewClicked_Event?.Invoke(); });
    }


    public void SetOptionsPanelToggle()
    {
        if(isLocal)
        {
            return;
        }

        if(isOptionsOpen)
        {
            ClosePlayerOptionsPanel();
        }
        else
        {
            OpenPlayerOptionsPanel();
        }
    }

    public void ResetAddFriendButton()
    {
        SetAddFriendButton(false);
        ClosePlayerOptionsPanel();
    }

    public void SetAddFriendButton(bool isInteractable,string Msg)
    {
        addFriendBtn.interactable = isInteractable;
        //addFriendBtn.image.color = Color.gray;

        addFriendBtn_Txt.text = Msg;
    }


    public enum OptionBtnState
    {
        addFriend,
        alreadySent,
        alreadyAdded,

    }

    public void SwitchOffPlayerProfile()
    {
        ViewProfileBtn.gameObject.SetActive(false);
    }

    public void SetAddFriendButton(bool isAlreadySent)
    {
        if(isAlreadySent)
        {
            addFriendBtn.interactable = false;
            addFriendBtn_Txt.text = "Request Sent";
          //  addFriendBtn.image.color = Color.gray;
        }
        else
        {
            addFriendBtn.interactable = true;
            addFriendBtn_Txt.text = "Add Friend";
           // addFriendBtn.image.color = Color.white;
        }
    }


    public void OpenPlayerOptionsPanel()
    {
        if(ReferencesHolder.isInSpectatorMode)
        {
            return;
        }

        if(!alreadyCheckedFriend)
        {

            Debug.Log(" OpenPlayerOptionsPanel() =>  => !alreadyCheckedFriend ");

            onOptionsClicked?.Invoke();
            alreadyCheckedFriend = true;
        }
        Debug.Log(" OpenPlayerOptionsPanel()   => ");

        pnlPlayerOptions.gameObject.SetActive(true);
        pnlPlayerOptions.DOScale(targetScale, tweenDuration_popOut).SetEase(easeType_popOut);

        isOptionsOpen = true;
    }

    public void ClosePlayerOptionsPanel()
    {
        pnlPlayerOptions.DOScale(Vector3.zero, tweenDuration_popIn).SetEase(easeType_popIn).OnComplete(() => pnlPlayerOptions.gameObject.SetActive(true));

        isOptionsOpen = false;
    }
    public void ShowProfilePanel()
    {
        ProfilePanel.SetActive(true);
     //   GameFireBaseInteractionManager.
    }



}
