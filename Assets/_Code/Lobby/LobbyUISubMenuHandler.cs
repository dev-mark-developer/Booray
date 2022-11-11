using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class LobbyUISubMenuHandler : MonoBehaviour
{

    [SerializeField] private GameObject subMenuPanel;

    [Header("Player Type Choice Sub Menu UI Ref")]

    [SerializeField] private GameObject playerTypeEnterPanel;
    [SerializeField] private Button joinAsSpectatorBtn;
    [SerializeField] private Button joinAsPlayerBtn;

    [SerializeField] private Button cancelPlayerTypeBtn;

    public Action onJoinRoomAsPlayerBtnClicked_Event;
    public Action onJoinRoomAsSpecBtnClicked_Event;


    [Header("Room Password Type Sub Menu UI Ref")]

    [SerializeField] private GameObject roomPasswordEnterPanel;
    [SerializeField] private Button validatePasswordBtn;
    [SerializeField] private TMP_InputField passwordIF;

    [SerializeField] private TMP_Text passwordIF_warningTxt;

    [SerializeField] private Button cancelRoomPasswordBtn;

    public Action onValidiatePasswordBtnClicked_Event;

    [Header(" Become VIP Sub Menu UI Ref ")]

    [SerializeField] GameObject becomeVIPPanel;
    [SerializeField] Button cancleVipPanelBtn;
    [SerializeField] Button becomeVIPBtn;


    public Action onBecomeVIPBtnClicked_Event;

    public Action onCancelSubMenu_Event;

    private void Start()
    {
        joinAsPlayerBtn.onClick.AddListener(delegate { onJoinRoomAsPlayerBtnClicked_Event?.Invoke(); });
        joinAsSpectatorBtn.onClick.AddListener(delegate { onJoinRoomAsSpecBtnClicked_Event?.Invoke(); });

        validatePasswordBtn.onClick.AddListener(delegate { onValidiatePasswordBtnClicked_Event?.Invoke(); });

        passwordIF.onSelect.AddListener(delegate { SetWarningTxt(false); });

        cancelPlayerTypeBtn.onClick.AddListener(delegate { SetActivePlayerTypeSubMenu(false); onCancelSubMenu_Event?.Invoke(); });
        cancelRoomPasswordBtn.onClick.AddListener(delegate
        {
            SetActiveRoomPasswordSubMenu(false);

            onCancelSubMenu_Event?.Invoke();
        });

        cancleVipPanelBtn.onClick.AddListener(delegate { SetActiveBecomeVIPSubMenu(false); onCancelSubMenu_Event?.Invoke(); });

        becomeVIPBtn.onClick.AddListener( delegate { onBecomeVIPBtnClicked_Event?.Invoke(); });

    }




    #region Player Type Menu

    public void SetActivePlayerTypeSubMenu(bool state)
    {
        subMenuPanel.SetActive(state);
        playerTypeEnterPanel.SetActive(state);
        


    }

    public void SetInteractibilityJoinAsPlayer(bool state)
    {
        joinAsPlayerBtn.interactable = state;
    }

    public void SetInteractibilityJoinAsSpectator(bool state)
    {
        joinAsSpectatorBtn.interactable = state;
    }


    #endregion


    #region Password Menu

    public void SetActiveRoomPasswordSubMenu(bool state)
    {
        subMenuPanel.SetActive(state);
        roomPasswordEnterPanel.SetActive(state);

        if (!state)
        {
            passwordIF.text = "";
            SetWarningTxt(false);
        }

    }

    public string GetPasswordIF()
    {
        return passwordIF.text;
    }

    public void SetWarningTxt(bool state)
    {
        if (state)
        {
            passwordIF_warningTxt.text = "Wrong Password... Please try Again";
        }
        else
        {
            passwordIF_warningTxt.text = "";
        }

    }



    #endregion



    #region BecomeVIP


    public void SetActiveBecomeVIPSubMenu(bool state)
    {


        subMenuPanel.SetActive(state);

        if (state)
            playerTypeEnterPanel.SetActive(false);

        becomeVIPPanel.SetActive(state);
    }

    



    #endregion




}
