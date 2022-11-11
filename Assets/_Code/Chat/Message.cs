using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
[FirestoreData]
public class Message
{
 
    [FirestoreProperty]
    public Timestamp Time { get; set; }


    [FirestoreProperty]
    public string User { get; set; }

    [FirestoreProperty]
    public string UserName { get; set; }

    [FirestoreProperty]
    public string message { get; set; }

    [FirestoreProperty]
    public string PictureURL { get; set; }

    [FirestoreProperty]
    public string PictureName { get; set; }


    [FirestoreProperty]
    public bool AvatarUsed { get; set; }
    [FirestoreProperty]
    public string AvatarID { get; set; }

    [FirestoreProperty]
    public string ChatItem { get; set; }

}
