using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;
using UnityEngine.UI;
public class FriendRequestManager : MonoBehaviour
{
    // Start is called before the first frame update
    FirebaseAuth auth;
    FirebaseFirestore db;
    [SerializeField]
    Button RequestButton;
    void Start()
    {
        RequestButton.name = "3it6QOlc37bl8yGVMyTyg87g2KY2";
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
        RequestButton.onClick.AddListener(RequestMethod);
       
    }

    private void RequestMethod()
    {
        DocumentReference docRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(auth.CurrentUser.UserId).Collection(ReferencesHolder.FS_FriendReqSent_Collec).Document(RequestButton.name);
        var FriendReqSentData = new FriendReqDataDB
        {
            To = RequestButton.name,
            From = auth.CurrentUser.UserId,
            RequestStatus = true
            
            
        };
        docRef.SetAsync(FriendReqSentData).ContinueWithOnMainThread(task =>
        {

            Debug.Log("sender ka request send populate howa");

        });

        DocumentReference docRef2 = db.Collection(ReferencesHolder.FS_users_Collec).Document(RequestButton.name).Collection(ReferencesHolder.FS_FriendReqRecieve_Collec).Document(auth.CurrentUser.UserId);
        var FriendReqRecieveData = new FriendReqDataDB
        {
            To = RequestButton.name,
            From = auth.CurrentUser.UserId,
            RequestStatus = true


        };
        docRef2.SetAsync(FriendReqRecieveData).ContinueWithOnMainThread(task =>
        {

            Debug.Log("reciever ka request recieve populate howa");

        });
    }
}
