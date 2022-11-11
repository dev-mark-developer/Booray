using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Booray.Auth;
using Firebase.Messaging;
using System.Linq;
using TMPro;
using Photon.Pun;

public class NotificationUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Button ShowNotificationButton, BackButton;


    public GameObject NotificationPanel;
    public GameObject HomePanel;
    public GameObject NotificationItemPrefab;
   

    public GameObject empNotiItem;



    public Transform NotificationParent;

    [SerializeField] private TextMeshProUGUI msgCountTxt;
    [SerializeField] private Image msgCounterImg;


    private string RoomIdKey = "roomId";
    private string RoomTypeKey = "roomType";
    private string senderKey = "inviterName";

    private int msgCounter = 0;

    public  MainMenuPhotonManager MMP;

    [SerializeField] LobbyFirebaseManager lobbyFirebaseManager;

    void Start()
    {
        BackButton.onClick.AddListener(delegate { BackButtonMethod(); CheckIfAllAreRemoved(); SFXHandler.instance.PlayBtnClickSFX(); });
        //ReferencesHolder.NotificationItemList = new List<FirebaseMessage>();
        ShowNotificationButton.onClick.AddListener(delegate { ShowNotificationMethod(); SFXHandler.instance.PlayBtnClickSFX(); } );
        Debug.Log("----------->>>>>number of notifications in the tray: " + ReferencesHolder.NotificationItemList.Count);

        LoadAllFromNotifications();

        //NotificationHandler.instance.Notificationbutton = ShowNotificationButton.gameObject;
        //if (ReferencesHolder.NotificationItemList.Count > 0)
        //{
        //    NotificationHandler.instance.Notificationbutton.transform.GetChild(1).transform.gameObject.SetActive(true);
        //    NotificationHandler.instance.Notificationbutton.transform.GetChild(1).transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>().text = ReferencesHolder.NotificationItemList.Count.ToString();
        //}

        // LoadAllFromNotifications();
    }
    void ShowNotificationMethod()
    {
        Debug.Log("<<<<<<<<<<<<<<<<<<<<<<<<<<<Show notiiii>>>>>>>>>>>>>>>>>>>>>>");

        MainUIManager.Instance.HomeUI.DeactivePanelsMethod();

        NotificationPanel.SetActive(true);
        HomePanel.SetActive(false);

        MainUIManager.Instance.HomeUI.BackButton.gameObject.SetActive(true);
        if (NotificationParent.childCount > 1)
        {
            Debug.Log("yay ha child count:>>>>>>>>" + NotificationParent.childCount);
            empNotiItem.SetActive(false);

        }
        else
        {
            Debug.Log("yay ha child count:>>>>>>>>" + NotificationParent.childCount);
            empNotiItem.SetActive(true);
        }
        ResetMsgCounter();

        

        //MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
        //NotificationPanel.SetActive(true);
        //MainUIManager.Instance.HomeUI.BackButton.gameObject.SetActive(true);
        //if (NotificationHandler.instance.Notificationbutton != null)
        //{
        //    NotificationHandler.instance.Notificationbutton.transform.GetChild(1).transform.gameObject.SetActive(false);
        //    NotificationHandler.instance.Notificationbutton.transform.GetChild(1).transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>().text = "";
        //}

        ////ShowNotificationButton.transform.GetChild(1).transform.gameObject.SetActive(false);
        //Debug.Log("--------->before load button press and load");
        //LoadNotifications();
        //Debug.Log("--------->After load button press and load");
    }
    
    public void CheckIfAllAreRemoved()
    {
        if(NotificationParent.childCount<=1)
        {
            empNotiItem.SetActive(true);
        }
    }

    public void InstantiateSingleNotification(FirebaseMessage noti)
    {

      

        if (!NotificationPanel.activeSelf)
        {
            UpdateMsgCounter();
        }



        if (noti.Notification==null)
        {

            if(noti.Data.ContainsKey(RoomIdKey))
            {
                GameObject NotificationItemGameObject = Instantiate(NotificationItemPrefab, NotificationParent);

                NotificationItemGameObject.transform.SetSiblingIndex(0);

                NotificationItemUIManager NotificationCont = NotificationItemGameObject.GetComponent<NotificationItemUIManager>();
                empNotiItem.SetActive(false);
                var roomType = noti.Data[RoomTypeKey];

                Debug.Log($"roomType = {roomType}");

                var roomId = noti.Data[RoomIdKey];
                var sender = noti.Data[senderKey];



                NotificationCont.SetNotification(sender,roomId,roomType);
                NotificationCont.JoinInviteButton.gameObject.SetActive(true);
                NotificationCont.OnJoinInviteClickEvent = OnClick_JoinInvite;
                NotificationCont.OnRemoveCheckEvent = CheckIfAllAreRemoved;

            }
        }
        else
        {
            GameObject NotificationItemGameObject = Instantiate(NotificationItemPrefab, NotificationParent);
            empNotiItem.SetActive(false);

            NotificationItemGameObject.transform.SetSiblingIndex(0);

            NotificationItemUIManager NotificationCont = NotificationItemGameObject.GetComponent<NotificationItemUIManager>();
            NotificationCont.OnRemoveCheckEvent = CheckIfAllAreRemoved;
            if (noti.Data.ContainsKey(RoomIdKey))
            {
                NotificationCont.JoinInviteButton.gameObject.SetActive(true);
                NotificationCont.OnJoinInviteClickEvent = OnClick_JoinInvite;
                //NotificationCont.OnRemoveCheckEvent = CheckIfAllAreRemoved;
            }
            else
            {
                NotificationCont.JoinInviteButton.gameObject.SetActive(false);
            }



            NotificationCont.SetNotification(noti);
        }


        





      

        
        

        

        
    }

    //public void InstantiateEmpNotification()
    //{
    //    GameObject NotificationItemGameObject = Instantiate(empNotiItemPrefab, NotificationParent);

    //}

    void LoadAllFromNotifications()
    {
        Debug.Log("this is the count<<<<<<<<<"+ReferencesHolder.NotificationItemList.Count);
       
        //if(ReferencesHolder.NotificationItemList.Count<1)
        //{
        //    // InstantiateEmpNotification();
        //    Debug.Log("yay  noti ItemList Count ha>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>" +ReferencesHolder.NotificationItemList.Count);
        //    empNotiItem.SetActive(true);

        //    //return;
        //}
        //else
        //{
        //    empNotiItem.SetActive(false);

        //}


        foreach (FirebaseMessage noti in ReferencesHolder.NotificationItemList)
        {
            Debug.Log("this is the message:" + noti);
            InstantiateSingleNotification(noti);
        }
        ReferencesHolder.NotificationItemList.Clear();

        //MessageReceivedEventArgs obj = new MessageReceivedEventArgs(new FirebaseMessage());
     
        
    }

    //public void OnClick_JoinInvite(string roomId, string roomType)
    //{
    //    //Debug.Log("Join click for room id: " + ReferencesHolder.InvitedRoomId);
    //    //MMP.PhotonJoinRoom(ReferencesHolder.InvitedRoomId);
    //    //MainUIManager.Instance.Loader.SetActive(true);






    //    Debug.Log($"room tyPe = {roomType}");


    //    //var hash = PhotonNetwork.LocalPlayer.CustomProperties;

    //    ExitGames.Client.Photon.Hashtable newHash = new ExitGames.Client.Photon.Hashtable();
    //    newHash[ReferencesHolder.PPlayersKey_PlayerType] = true;

    //    //if (hash.ContainsKey(ReferencesHolder.PPlayersKey_PlayerType))
    //    //{
    //    //    hash[ReferencesHolder.PPlayersKey_PlayerType] = true;
    //    //}
    //    //else
    //    //{
    //    //    hash.Add(ReferencesHolder.PPlayersKey_PlayerType,true);
    //    //}

    //    PhotonNetwork.LocalPlayer.SetCustomProperties(newHash);


    //    if(!string.IsNullOrEmpty(roomType))
    //    {
    //        if (roomType.Equals(GameModeType.ClassicBooRay.ToString()))
    //        {
    //            ReferencesHolder.selectedLobby = GameModeType.ClassicBooRay;
    //        }
    //        else if (roomType.Equals(GameModeType.SpeedBet.ToString()))
    //        {
    //            ReferencesHolder.selectedLobby = GameModeType.SpeedBet;
    //        }
    //        else if (roomType.Equals(GameModeType.FullHouse.ToString()))
    //        {
    //            ReferencesHolder.selectedLobby = GameModeType.FullHouse;
    //        }
    //    }






    //    Debug.Log($"OnClick_JoinInvite - {roomId} ");



    //    MMP.PhotonJoinRoomThroughInvite(roomId);

    //    MainUIManager.Instance.Loader.SetActive(true);


    //}

    public void OnClick_JoinInvite(string roomId, string roomType, System.Action onFailCallback)
    {


        lobbyFirebaseManager.CheckIfTheresSlotinGameRoom(roomId,
            delegate { onFailCallback?.Invoke() ; LogErrorUIHandler.instance.OpenErrorPanel("Join Failed, Please try again."); },
            delegate(LobbyFirebaseManager.RoomFirebaseJoinStatus status) 
            {
                switch (status)
                {
                    case LobbyFirebaseManager.RoomFirebaseJoinStatus.slotFree:
                        {
                            Debug.Log($"room type = {roomType}");


                            ExitGames.Client.Photon.Hashtable newHash = new ExitGames.Client.Photon.Hashtable();
                            newHash[ReferencesHolder.PPlayersKey_PlayerType] = true;

                            PhotonNetwork.LocalPlayer.SetCustomProperties(newHash);


                            if (!string.IsNullOrEmpty(roomType))
                            {
                                if (roomType.Equals(GameModeType.ClassicBooRay.ToString()))
                                {
                                    ReferencesHolder.selectedLobby = GameModeType.ClassicBooRay;
                                }
                                else if (roomType.Equals(GameModeType.SpeedBet.ToString()))
                                {
                                    ReferencesHolder.selectedLobby = GameModeType.SpeedBet;
                                }
                                else if (roomType.Equals(GameModeType.FullHouse.ToString()))
                                {
                                    ReferencesHolder.selectedLobby = GameModeType.FullHouse;
                                }
                            }


                            Debug.Log($"OnClick_JoinInvite - {roomId} ");

                            MMP.PhotonJoinRoomThroughInvite(roomId);

                            MainUIManager.Instance.Loader.SetActive(true);
                            break;
                        }
                    case LobbyFirebaseManager.RoomFirebaseJoinStatus.roomFull:
                        {
                            LogErrorUIHandler.instance.OpenErrorPanel("Room is full, please try again.");
                            onFailCallback?.Invoke();
                            break;
                        }
                    case LobbyFirebaseManager.RoomFirebaseJoinStatus.GameIsActive:
                        {
                            LogErrorUIHandler.instance.OpenErrorPanel("Game is Currently playig. Please try again.");
                            onFailCallback?.Invoke();
                            break;
                        }
                }


                
            });








    }


    private void UpdateMsgCounter()
    {
        msgCounter += 1;

        if (msgCounterImg.gameObject.activeSelf == false)
        {
            msgCounterImg.gameObject.SetActive(true);

        }

        if (msgCounter > 100)
        {
            msgCountTxt.text = "100+";
        }
        else
        {
            msgCountTxt.text = msgCounter.ToString();

        }

    }

    private void ResetMsgCounter()
    {
        msgCounter = 0;

        msgCounterImg.gameObject.SetActive(false);
    }

    public void LoadNotifications()
    {
        //GameObject NotificationItemGameObject = Instantiate(NotificationItemPrefab, NotificationParent);
        //NotificationItemUIManager NotificationCont = NotificationItemGameObject.GetComponent<NotificationItemUIManager>();
        LoadAllFromNotifications();
        // NotificationCont.OnJoinInviteClickEvent = OnClick_JoinInvite;

    }
    void BackButtonMethod()
    {
        MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
        MainUIManager.Instance.HomeUI.HomePanel.SetActive(true);

    }
}
