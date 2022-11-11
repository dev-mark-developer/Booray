using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Booray.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.Networking;
using Firebase.Messaging;

public class NotificationItemUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NotificationTxt;
    [SerializeField] private TextMeshProUGUI NotificationTitleTxt;
    public Button JoinInviteButton;
    [SerializeField] private Button RemoveNotificationButton;
    public Action<string,string,Action> OnJoinInviteClickEvent;

    private FirebaseMessage msgHolder;


    public Action OnRemoveCheckEvent;

    private string roomName;
    private string roomType;
    private void Start()
    {
        RemoveNotificationButton.onClick.AddListener(delegate { RemoveNotificationButtonMethod(); OnRemoveCheckEvent?.Invoke();  });
    }
    public void SetNotification(FirebaseMessage NotificationData)
    {
        if(NotificationData ==null)
        {
            Debug.Log($" Notification data is null");
            return;
        }

        msgHolder = NotificationData;

        if(msgHolder == null)
        {
            Debug.Log($" Notification data is null");
            return;
        }

        if(NotificationData.Notification == null)
        {
            Debug.Log($" NotificationData.Notification data is null");
            return;
        }
        else
        {
            Debug.Log($" {NotificationData.Notification.Title}  | {NotificationData.Notification.Body}");
        }
        
        if(NotificationData.Data.ContainsKey("roomId"))
        {
            roomName = NotificationData.Data["roomId"];
            roomType = NotificationData.Data["roomType"];
        }

        NotificationTxt.text = NotificationData.Notification.Body;
        NotificationTitleTxt.text = NotificationData.Notification.Title;
        JoinInviteButton.onClick.AddListener(delegate { JoinInviteButton.interactable = false; OnJoinInviteClickEvent?.Invoke(roomName, roomType,delegate { if(JoinInviteButton.gameObject !=null)JoinInviteButton.interactable = true; }); SFXHandler.instance.PlayBtnClickSFX(); });
    }
    public void RemoveNotificationButtonMethod()
    {
        Destroy(this.gameObject);
    }
    public void SetNotification(string userName, string roomId,string roomtype)
    {





        roomName = roomId;
        roomType = roomtype;

        NotificationTxt.text = $"{userName} has invited you to a room";
        NotificationTitleTxt.text = "YOU GOT A ROOM INVITE!";
        JoinInviteButton.onClick.AddListener(
            delegate
            {
                JoinInviteButton.interactable = false;
                OnJoinInviteClickEvent?
                .Invoke(roomName, roomType, delegate { JoinInviteButton.interactable = true; });
                SFXHandler.instance.PlayBtnClickSFX(); 
            });
    }
  
}
