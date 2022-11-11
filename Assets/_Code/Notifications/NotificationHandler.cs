using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Messaging;
using Firebase.Firestore;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase;
using Booray.Auth;

public class NotificationHandler : MonoBehaviour
{
    FirebaseFirestore db;
    public static NotificationHandler instance;
  

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

      




    }


    public void Start()
    {
        Debug.Log("Noti handler start start");
        // ReferencesHolder.AllInvitesList = new List<string>();

        db = FirebaseFirestore.DefaultInstance;


        //  RefreshToken();

        FirebaseMessaging.TokenReceived += TokenReceived;
        FirebaseMessaging.MessageReceived += MessageReceived;
        //ReferencesHolder.NotificationItemList = new List<FirebaseMessage>();
        // Notificationbutton = GameObject.Find("NotificationButton");
        //if (PlayerPrefs.GetInt("FirstTime") == 1)
        //{
        //    //RefreshToken();
        //   // ReAssignTopic();
        //}
        //if (PlayerPrefs.GetInt("FirstTime") == 0)
        //{
        //    //RefreshToken();
        //    PlayerPrefs.SetInt("FirstTime", 1);
        //    PlayerPrefs.Save();
        //}

        Debug.Log("Noti handler start end");

    }
   

    public async void RefreshToken()
    {
        Debug.Log("Noti handler Refresh start ");
        await FirebaseMessaging.DeleteTokenAsync();
        string token = await  FirebaseMessaging.GetTokenAsync();

        Debug.Log(">>>>>>>>>>>>>Token Received" + token);

       
        PlayerPrefs.SetString("DeviceToken", token);
        PlayerPrefs.Save();


        await db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).UpdateAsync(new Dictionary<string, object> { { "token", token } });
        await db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).UpdateAsync("DeviceToken", token);
        Debug.Log("Noti handler Refresh end");





    }


    private void TokenReceived(object sender, TokenReceivedEventArgs e)
    {
        Debug.Log("Noti handler Token recieve Start");
        Debug.Log(">>>>>>>>>>>>>Token Received" + e.Token);
        //ReferencesHolder.RecievedDeviceToken = e.Token;
        PlayerPrefs.SetString("DeviceToken", e.Token);
        PlayerPrefs.Save();
        Debug.Log("Noti handler Token recieve end");
      //  RefreshToken();
        // FirebaseMessaging.SubscribeAsync("/topics/Tournament");

    }
    private void MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        // e invite | news
        // 
        Debug.Log("8*#$*#$%^*#$%*$W##^#$^*&#$^*#$^*$%*%$&*$%^&*$%^&*$%*$&$%&$%&*$&*$%&*%$&*%*$%");
        Debug.Log("Messgae ID - >" + e.Message.MessageId);
        Debug.Log("Messgae TYPE -> " + e.Message.MessageType);
        Debug.Log("To -> " + e.Message.To);
        Debug.Log("Noti Opened -> " + e.Message.NotificationOpened);
        Debug.Log("Error -> " + e.Message.Error);
        Debug.Log("Error Description -> " + e.Message.ErrorDescription);


        //Debug.Log($"! Got a Notification! -> {e.Message.Notification.Title} ");

        if(e.Message.Data!=null)
        {
            Debug.Log("Message data is not null ");

            if (e.Message.Data.ContainsKey("roomId"))
            {
                Debug.Log($" room id ={e.Message.Data["roomId"]}");
            }
        }



        if (SceneManager.GetActiveScene().buildIndex == ReferencesHolder.mainMenuSceneIndex)
        {
            if (MainUIManager.Instance != null)
            {

                MainUIManager.Instance.NotificationUI.InstantiateSingleNotification(e.Message);
                // Mainui manahger ko bolo notificatio n ui manager me ye cheeze add karde
            }
            else
            {
                // ap list me add kardo
                if (ReferencesHolder.NotificationItemList == null)
                    ReferencesHolder.NotificationItemList = new List<FirebaseMessage>();
                ReferencesHolder.NotificationItemList.Add(e.Message);
            }

        }
        else
        {
            // tu app list me add kardo
            if (ReferencesHolder.NotificationItemList == null)
                ReferencesHolder.NotificationItemList = new List<FirebaseMessage>();

            ReferencesHolder.NotificationItemList.Add(e.Message);
        }





        //if(isInvite)
        //{



        //    //var roomid = e.Message.Data["roomId"];
        //    //var roomType = e.Message.Data["roomType"];

        //    //Debug.Log(roomid);
        //    //Debug.Log(roomType);
        //    //ReferencesHolder.InvitedRoomId = roomid;
        //    //ReferencesHolder.InvitedRoomType = roomType;
        //}
        //else
        //{

        //}






        //  Scene currentScene = SceneManager.GetActiveScene();
        //  Debug.Log(">>>>>>Message recieved after notification<<<<<<<");
        //  //Debug.Log($"{e.Message.Link} - {e.Message.Notification.Body} -{e.Message.NotificationOpened} - {e.Message.Notification.Title}  ");
        ////  Debug.Log("notification received" + e.Message.MessageId + " " + e.Message.MessageType);

        //  Debug.Log(e.Message.Notification.Body);

        //  Debug.Log("this the message id: "+e.Message.MessageId);
        //  Debug.Log("thats the title of notification: "+e.Message.Notification.Title);

        //  //e.Message.Data.ContainsKey("roomId");
        //  if (e.Message.Data.ContainsKey("roomId"))
        //  {
        //      Debug.Log(e.Message.Data["roomId"]);
        //      Debug.Log(e.Message.Data["roomType"]);
        //      ReferencesHolder.InvitedRoomId = e.Message.Data["roomId"];
        //      ReferencesHolder.InvitedRoomType = e.Message.Data["roomType"];
        //  }



        //  if (currentScene.name == "GameScene")
        //  {
        //      ReferencesHolder.NotifiedUser = false;

        //  }
        //  else
        //  {
        //      ReferencesHolder.NotifiedUser = true;
        //  }

        //  ReferencesHolder.NotificationItemList.Add(e.Message);
        //  if (ReferencesHolder.NotificationItemList.Count > 0&& Notificationbutton!=null)
        //  {
        //      Notificationbutton.transform.GetChild(1).transform.gameObject.SetActive(true);
        //      Notificationbutton.transform.GetChild(1).transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>().text = ReferencesHolder.NotificationItemList.Count.ToString();
        //  }
        //  Debug.Log("----------->>>>>number of notifications in the tray: " + ReferencesHolder.NotificationItemList.Count);


        //  Debug.Log(">>>>>>Message recieved after notification End<<<<<<<");
    }

    public void ReAssignTopic()
    {
        Debug.Log("Re Assign topic shrowaat");

        Debug.Log("Reassign_UserCollection:"+ReferencesHolder.FS_users_Collec + "Reassign_UserId" + ReferencesHolder.playerPublicInfo.UserId + "Reassign_TournamenttopicCollection" + ReferencesHolder.FS_UserTournaments_Collec);

        Query collRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_UserTournaments_Collec);

        collRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {

            QuerySnapshot snapshot = task.Result;


            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Debug.Log("Yay topic ha beta>>>>>>>>" + document.Id);
                SubscribeToTopic(document.Id);
                //if (document.Exists)
                //{
                //    Debug.Log("Yay topic ha beta>>>>>>>>" + document.Id);
                //    SubscribeToTopic(document.Id);


                //}
            }
                
        });
        Debug.Log("Re Assign topic end");
    }
    public async void SubscribeToTopic(string topic)
    {
        Debug.Log("Topic subscribe k ander");
        await FirebaseMessaging.SubscribeAsync($"/topics/{topic}");

        Debug.Log("topic ReSuB....... . . ..");


    }


}











