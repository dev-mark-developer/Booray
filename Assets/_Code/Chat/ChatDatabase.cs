using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using Photon.Pun;

public class ChatDatabase : MonoBehaviour
{
    // Start is called before the first frame update
    FirebaseFirestore db;
    public GameObject ChatHandlerObj;
    Message message = new Message();
    //public static ChatDatabase database;
    public List<string> MessageItemList;
    bool allow = true;

    public Action<Message> OnListen;

    public Action<AggregateException> OnListenFail;
    
    void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;

        Listen(OnListen, OnListenFail);
        
    }
    public void Postmessage(Message message,Action callback, Action<AggregateException> fallback)
    {
        //db.Collection("Chat").Document("Rooms").Collection(PhotonNetwork.CurrentRoom.Name).Document().SetAsync(message).ContinueWithOnMainThread(task =>
        //{
        //    if(task.IsCanceled|| task.IsFaulted)
        //    {
        //        Debug.Log("Post me fault");
        //        fallback(task.Exception);
        //    }
        //    else
        //    {
        //        callback();
        //        Debug.Log("Post me fault nahi");
        //    }
        //});

        db.Collection(ReferencesHolder.FS_GameRooms_Collec).Document(PhotonNetwork.CurrentRoom.Name)
            .Collection(ReferencesHolder.FS_GameChat_Collec).Document().SetAsync(message)
            .ContinueWithOnMainThread( task => 
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.Log("Chat Message Post => Fail");
                    fallback(task.Exception);
                }
                else
                {
                    Debug.Log("Chat Message Post => Success");
                    callback();
                }


            });

    }
    public void Listen(Action<Message> callback, Action<AggregateException> fallback)
    {

        Query query = db.Collection(ReferencesHolder.FS_GameRooms_Collec).Document(PhotonNetwork.CurrentRoom.Name).
            Collection(ReferencesHolder.FS_GameChat_Collec).OrderBy("Time");

        ListenerRegistration listener = query.Listen(snapshot => 
        {
            foreach (DocumentChange change in snapshot.GetChanges())
            {
                if (change.ChangeType == DocumentChange.Type.Added)
                {
                    Debug.Log(string.Format("new message:{0}", change.Document.Id));
                    message = change.Document.ConvertTo<Message>();

                    MessageItemList.Add(message.message);
                    callback?.Invoke(message);




                }
                if (change.ChangeType == DocumentChange.Type.Modified)
                {
                    Debug.Log(string.Format("modified message:{0}", change.Document.Id));
                }
                if (change.ChangeType == DocumentChange.Type.Removed)
                {
                    Debug.Log(string.Format("Remmoved message:{0}", change.Document.Id));
                }
            }


        });

        //Query query = db.Collection("Chat").Document("Rooms").Collection(PhotonNetwork.CurrentRoom.Name).OrderBy("Time");
        //ListenerRegistration listner = query.Listen(snapshot =>
        //{
        //    foreach (DocumentChange change in snapshot.GetChanges())
        //    {
        //        if (change.ChangeType == DocumentChange.Type.Added)
        //        {
        //            Debug.Log(string.Format("new message:{0}", change.Document.Id));
        //            message = change.Document.ConvertTo<Message>();

        //                MessageItemList.Add(message.message);
        //                callback?.Invoke(message);
                      
                    
                    

        //        }
        //        if (change.ChangeType == DocumentChange.Type.Modified)
        //        {
        //            Debug.Log(string.Format("modified message:{0}", change.Document.Id));
        //        }
        //        if (change.ChangeType == DocumentChange.Type.Removed)
        //        {
        //            Debug.Log(string.Format("Remmoved message:{0}", change.Document.Id));
        //        }
        //    }
        //});


    }
}
