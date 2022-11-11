using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Firestore;
using System;
using Firebase.Auth;
using Firebase.Extensions;

public class AddFriendUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI emailWarningTxt;
    [SerializeField] TMP_InputField emailInputField;

    [SerializeField] Button searchBtn;

    FirebaseFirestore db;

    [SerializeField] GameObject friendPrefab;
    [SerializeField] GameObject noFriendFoundMsg;

    [SerializeField] Transform friendsParent;

    //List<friend>

    private void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;
    }


    public void FindUserViaEmail(string emailId, Action onFailCallback, Action<List<string>,bool> onSuccessCallback)
    {
        Query collecRef = db.Collection(ReferencesHolder.FS_users_Collec).WhereEqualTo("Email", emailId);

        collecRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsCanceled || task.IsFaulted)
            {
                Debug.Log("FindUserViaEmail_Fail");
                onFailCallback?.Invoke();
                return;
            }

            QuerySnapshot snapshot = task.Result;

            List<string> userIDList = new List<string>();

            bool ifAny = false;

            foreach(DocumentSnapshot document in snapshot.Documents)
            {
                Debug.Log($"Document {document.Id}");

                userIDList.Add(document.Id);

                ifAny = true;


            }

            if(ifAny)
            {
                // in a way add friends
                onSuccessCallback?.Invoke(userIDList, true);
            }
            else
            {
                // add a no friend found
                onSuccessCallback?.Invoke(userIDList, false);
            }



        });


        

    }

    public void SpawnFriendsList(List<string> usersList)
    {
        foreach(var id in usersList)
        {
            GameObject friendItemGameObject = Instantiate(friendPrefab, friendsParent);
            FriendsItemUIController friendCont = friendItemGameObject.GetComponent<FriendsItemUIController>();

            friendCont.GetFriendsInfo(id, ref db);
           // friendCont.onViewProfileClickEvent = OnClick_ViewFriend;
           // FriendItemList.Add(friendItemGameObject);
        }

        
    }




}
