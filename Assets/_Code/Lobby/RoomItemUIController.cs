using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class RoomItemUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomNameTxt;
    [SerializeField] private TextMeshProUGUI spectatorsTxt;
    [SerializeField] private TextMeshProUGUI anteAmountTxt;
    [SerializeField] private TextMeshProUGUI playerAmountTxt;
    [SerializeField] private Button roomJoinBtn;
    [SerializeField] private GameObject ultimateBadgeGO;

    public RoomParametersLobby roomParams;

    public Action<RoomParametersLobby> onJoinClickedEvent;

    public string Password {  get; private set; }

    private void Start()
    {
        
    }

    public void SetRoomparameter(RoomParametersLobby paramsRoom)
    {
        roomParams = paramsRoom;

        SetRoomName($"{roomParams.RoomName}");
        SetSpectatorsInRoom(paramsRoom.NoOfSpectatorsInRoom);
        SetAnteValueInRoom(paramsRoom.AnteValueOfRoom);
        SetPlayerAmountTxt(paramsRoom.NoOfPlayersInRoom,paramsRoom.MaximumPlayers);
        SetJoinBtnInteractibility(true);
        Debug.Log($" is Ultimate = {paramsRoom.isUltimate} ");
        SetUltimateBadgeActive(paramsRoom.isUltimate);



        roomJoinBtn.onClick.AddListener(delegate { onJoinClickedEvent?.Invoke(roomParams); /*roomJoinBtn.interactable = false;*/ });
    }

    public void SetRoomName(string roomname)
    {
        roomNameTxt.text = roomname;
    }
    public void SetRoomName()
    {
        roomNameTxt.text = roomParams.RoomName;
    }

    public void SetSpectatorsInRoom(int amount)
    {
        spectatorsTxt.text = $"{amount}/<size=20>{10}";
    }
    

    public void SetAnteValueInRoom(int amount)
    {
        anteAmountTxt.text = $"{amount}";
    }
    public void SetAnteValueInRoom()
    {
        SetAnteValueInRoom(roomParams.AnteValueOfRoom);
    }

    public void SetPlayerAmountTxt(int amount,int maximumAmount)
    {
        playerAmountTxt.text = $"{amount}/{maximumAmount}";
    }

    public void SetPlayerAmountTxt()
    {
        SetPlayerAmountTxt(roomParams.NoOfPlayersInRoom, roomParams.MaximumPlayers);
    }

    public void SetJoinBtnInteractibility(bool state)
    {
        roomJoinBtn.interactable = state;
    }

    public void SetUltimateBadgeActive(bool state)
    {
        ultimateBadgeGO.SetActive(state);
    }




}
