using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Messaging;
using UnityEngine.Networking;

public class InviteToRoomHandler : MonoBehaviour
{

    private string url = "https://fcm.googleapis.com/fcm/send";
    private string serverKey = "key=AAAA3P8Cq6A:APA91bH8lcxz05AFeLrE33NTpcupobLNgA9A6sAQncaK2hq0QLzjC5FDg6rI_71MEtNKXgFqt2heoB-9Dz-htnI7-RX6jE2-6sfAgLwQZsJa4gMCgHf3SrpMAVd7w0mU--sfeT58entB";


    FirebaseFirestore db;
    private void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;
    }


    public void SendNotificationToAll(string roomid)
    {
        foreach (string friendid in ReferencesHolder.AllInvitesList)
        {
            Debug.Log(friendid);
            Debug.Log($"{ReferencesHolder.FS_users_Collec}  - - {ReferencesHolder.FS_userData_Collec} - {ReferencesHolder.FS_publicInfo_Doc} ");

            db.Collection(ReferencesHolder.FS_users_Collec).Document(friendid).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if(task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("SendNotificationToAll -> a task failed to send Noti ");
                }

                if(task.Result.Exists)
                {
                    PublicInfoDB Info = task.Result.ConvertTo<PublicInfoDB>();
                    string RoomType = ReferencesHolder.selectedLobby.ToString();
                    SendRoomInviteNotification(ReferencesHolder.playerPublicInfo.UserName, Info.DeviceToken, roomid, RoomType);
                }

                
            });



        }
        ReferencesHolder.AllInvitesList.Clear();

    }
    public void SendRoomInviteNotification(string sendersUserName, string deviceToken, string RoomId, string RoomType)
    {

        Debug.Log("Creating FCM Notification ... ");

        var roomData = new RoomData
        {
            inviterName = sendersUserName,
            roomId = RoomId,
            roomType = RoomType
        };

        var notification = new Notification
        {
            body = $"{sendersUserName} has invited you to a room",
            title = "YOU GOT A ROOM INVITE!",
            sound = "default"
        };

        var message = new FCMessage
        {
            to = deviceToken,
            notification = notification,
            data = roomData
        };

        string jsonBody = JsonUtility.ToJson(message);

        Debug.Log($"json Body -> {jsonBody} ");

        StartCoroutine(PostNotification(jsonBody));
        //   notification.body = "sdasd";


    }


    private IEnumerator PostNotification(string rawJson)
    {
        Debug.Log(">>>>>>>>>>>In post method<<<<<<<<<<<<<<<<");
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(rawJson);

        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);

        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        uwr.SetRequestHeader("Content-Type", "application/json");
        uwr.SetRequestHeader("Authorization", serverKey);

        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Sending Invite Failed");
        }
        else
        {
            Debug.Log("Invite Sent " + uwr.downloadHandler.text);
        }

    }


    [ContextMenu("Test Send AGain Invite")]
    public void Test_SendInvite()
    {
        SendNotificationToAll(Photon.Pun.PhotonNetwork.CurrentRoom.Name);
    }


}
[System.Serializable]
public class RoomData
{
    public string inviterName;
    public string roomId;
    public string roomType;
}
[System.Serializable]
public class Notification
{
    public string body;
    public string title;
    public string sound;
}
[System.Serializable]
public class FCMessage
{
    public string to;
    public Notification notification;
    public RoomData data;
}
