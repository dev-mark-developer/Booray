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

public class FriendsItemUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI FriendNameTxt;
    [SerializeField] private Image FriendImg;

    [SerializeField] private Button ViewProfileButton;
    [SerializeField] private Button InviteFriendButton;
    public Button AddFriendButton;
    public Button NextButton;
    public Button FindFriendCancelRequestButton;
    [SerializeField] private Button PreviousButton;

    public Action<string> onViewProfileClickEvent;
    public Action<string> onInviteFriendClickEvent;
    public Action<PublicInfoDB> onAddFriendClickEvent;



    public Action onNextButtonClickEvent;
    public Action onPreviousButtonClickEvent;

    public PublicInfoDB friendsPublicInfo;

    FirebaseFirestore dbRef;

    private void Start()
    {
        dbRef = FirebaseFirestore.DefaultInstance;
        if (gameObject.tag == "Next")
        {
            NextButton.onClick.AddListener(delegate { NextButtonMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
        }
        if (gameObject.tag == "Previous")
        {
            PreviousButton.onClick.AddListener(delegate { PreviousButtonMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
        }


        // AddFriendButton.onClick.AddListener(delegate { onAddFriendClickEvent?.Invoke(friendsPublicInfo); });

    }

    public void SetFriendsItem(PublicInfoDB friendsInfo)
    {

        friendsPublicInfo = friendsInfo;
        dbRef.Collection(ReferencesHolder.FS_users_Collec).Document(friendsPublicInfo.UserId)
         .Collection(ReferencesHolder.FS_FriendsData_Collec).GetSnapshotAsync()
         .ContinueWithOnMainThread(querysnapshot =>
         {

             QuerySnapshot snapshot = querysnapshot.Result;

             foreach (var doc in snapshot.Documents)
             {
                 Debug.Log($"Docxx -> {doc.Id}");
                 Debug.Log(ReferencesHolder.playerPublicInfo.UserId);
                 if (doc.Id.Contains(ReferencesHolder.playerPublicInfo.UserId))
                 {
                     Debug.Log("Ander");
                     ViewProfileButton.gameObject.SetActive(true);
                     AddFriendButton.gameObject.SetActive(false);
                 }

             }
             if (ViewProfileButton.gameObject.activeInHierarchy == false)
             {
                 Debug.Log("Ander else k");
                 AddFriendButton.gameObject.SetActive(true);
                 ViewProfileButton.gameObject.SetActive(false);
             }


         });
        if (friendsInfo.AvatarUsed == true)
        {
            var sp = MainUIManager.Instance.avatarAtlus.GetSprite(friendsInfo.AvatarID);

            SetFriendImage(sp);
        }

        SetFriendName(friendsPublicInfo.UserName);

        //GetFriendsFriendData(friendsPublicInfo.UserName, ReferencesHolder.newUserId, ref FirebaseFirestore dbRef);
        if (gameObject.tag == "FriendItem")
        {
            ViewProfileButton.onClick.AddListener(delegate { onViewProfileClickEvent?.Invoke(friendsInfo.UserId); SFXHandler.instance.PlayBtnClickSFX(); });
        }
        if (gameObject.tag == "InviteItem")
        {
            foreach (string id in ReferencesHolder.AllInvitesList)
            {
                if (id == friendsInfo.UserId)
                {
                    InviteFriendButton.interactable = false;
                    InviteFriendButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Invited";
                }
            }
            InviteFriendButton.onClick.AddListener(delegate { onInviteFriendClickEvent?.Invoke(friendsInfo.UserId); SFXHandler.instance.PlayBtnClickSFX(); DisableAfterInvite(); });
        }

        if (AddFriendButton != null)
        {
            AddFriendButton.onClick.AddListener(delegate { SendReqToFriend(friendsPublicInfo); });
            FindFriendCancelRequestButton.onClick.AddListener(delegate { OnClick_CancelRequest(friendsPublicInfo.UserId); });

        }




    }
    public void FindFriendAlreadyExists()
    {

    }
    public void SendReqToFriend(PublicInfoDB friendId)
    {
        AddFriendButton.interactable = false;

        AddFriendButton.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
        // interaction add btn
        //GameObject AddButton;
        //AddButton = GameObject.Find("AddFriend");
        SendFriendReq_FB(friendId.UserId, delegate { Debug.Log("Already exist"); },
            delegate { Debug.Log("Send"); },
            delegate { Debug.Log("Error"); });
        Query collecRef = dbRef.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_FriendReqSent_Collec);

        collecRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {

            QuerySnapshot snapshot = task.Result;



            foreach (DocumentSnapshot document in snapshot.Documents)
            {

                Debug.Log(document.Id);
                //AddFriendButton.interactable = false;
                //AddFriendButton.GetComponentInChildren<TextMeshProUGUI>().text = " Already Sent";

            }





        }).ContinueWithOnMainThread(task =>
        {
            AddFriendButton.interactable = false;
            Invoke("DelayInAlreadySentText", 1.5f);

        });
    }
    public void DelayInAlreadySentText()
    {
        AddFriendButton.GetComponentInChildren<TextMeshProUGUI>().text = "Request Pending";
        FindFriendCancelRequestButton.gameObject.SetActive(true);
        FindFriendCancelRequestButton.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel Request";
    }
    public void SendFriendReq_FB(string toUserID, System.Action CallBackSent, System.Action CallBackRecieve, Action Fallback)
    {
        Debug.Log("Sending Req");


        dbRef.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId)
        .Collection(ReferencesHolder.FS_FriendReqSent_Collec).Document(toUserID).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                AddFriendButton.interactable = true;
                AddFriendButton.transform.GetChild(0).GetComponent<Image>().color = Color.white;
            }


            if (task.Result.Exists)
            {
                CallBackSent?.Invoke();
                AddFriendButton.interactable = false;

            }
            else
            {

                var fromUserID = ReferencesHolder.playerPublicInfo.UserId;

                var FriendReqSentData = new FriendReqDataDB
                {
                    To = toUserID,
                    From = ReferencesHolder.playerPublicInfo.UserId,
                    RequestStatus = true

                };



                dbRef.Collection(ReferencesHolder.FS_users_Collec).Document(fromUserID)
                    .Collection(ReferencesHolder.FS_FriendReqSent_Collec).Document(toUserID)
                    .SetAsync(FriendReqSentData).ContinueWithOnMainThread(tast =>
                    {
                        Debug.Log("RequestSent");

                        CallBackSent?.Invoke();
                        AddFriendButton.interactable = false;
                        AddFriendButton.GetComponentInChildren<TextMeshProUGUI>().text = "Sent";

                    });

                dbRef.Collection(ReferencesHolder.FS_users_Collec).Document(toUserID)
                    .Collection(ReferencesHolder.FS_FriendReqRecieve_Collec).Document(fromUserID)
                    .SetAsync(FriendReqSentData).ContinueWithOnMainThread(tast =>
                    {
                        Debug.Log("RequestReceived");

                        CallBackRecieve?.Invoke();


                    });


            }
        });
    }
    public void OnClick_CancelRequest(string friendId)
    {
        Debug.Log("canceling request: " + friendId);

        dbRef.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_FriendReqSent_Collec).Document(friendId).DeleteAsync().ContinueWithOnMainThread(task =>
        {

            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.Log("could not cancel friend request");
            }
            else
            {


                Debug.Log("friend request canceled by reciever");

            }
            


        });
        dbRef.Collection(ReferencesHolder.FS_users_Collec).Document(friendId).Collection(ReferencesHolder.FS_FriendReqRecieve_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).DeleteAsync().ContinueWithOnMainThread(task =>
        {

            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.Log("could not delete request sent from senders document");
            }
            else
            {


                Debug.Log(" delete request sent from senders document");
                FindFriendCancelRequestButton.GetComponentInChildren<TextMeshProUGUI>().text = "Canceled";

            }



        });


    }

    public void GetFriendsFriendData(string friendID, string myId, ref FirebaseFirestore dbRef)
    {
        dbRef.Collection(ReferencesHolder.FS_users_Collec).Document(friendID)
            .Collection(ReferencesHolder.FS_FriendsData_Collec).GetSnapshotAsync()
            .ContinueWithOnMainThread(querysnapshot =>
            {

                QuerySnapshot snapshot = querysnapshot.Result;

                foreach (var doc in snapshot.Documents)
                {
                    Debug.Log($"Docxxx -> {doc.Id}");

                }


            });



        //dbRef.Collection(ReferencesHolder.FS_users_Collec).Document(friendID)
        //    .Collection(ReferencesHolder.FS_FriendsData_Collec).Document(myId).GetSnapshotAsync()
        //    .ContinueWithOnMainThread(snapshot => 
        //    {
        //        if(snapshot.IsCanceled || snapshot.IsFaulted)
        //        {
        //            Debug.Log("Cant get friend data");
        //            return;
        //        }

        //        Debug.Log($"My ID -> {myId}");

        //        // snapshot.Result.Id

        //        if(snapshot.Result.Exists)
        //        {

        //            Debug.Log("Exists");
        //            var ff = snapshot.Result.ConvertTo<FriendsDataDB>();

        //            Debug.Log(ff.FriendId);

        //            // matlab id hai 
        //            // view profile btn kholdo
        //            ViewProfileButton.gameObject.SetActive(true);
        //            AddFriendButton.gameObject.SetActive(false);

        //        }
        //        else
        //        {
        //            Debug.Log("Not exists");
        //            var ff = snapshot.Result.ConvertTo<FriendsDataDB>();
        //            Debug.Log(ff.FriendId);

        //            // add profile ka btn kholdo
        //            AddFriendButton.gameObject.SetActive(true);
        //            ViewProfileButton.gameObject.SetActive(false);

        //        }

        //    });
    }


    public void GetFriendsInfo(string Id, ref FirebaseFirestore dbRef)
    {

        Debug.Log("Getting friends Info");
        dbRef.Collection(ReferencesHolder.FS_users_Collec).Document(Id).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {

            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.Log("Cant get friend data");
            }
            else
            {
                PublicInfoDB Info = task.Result.ConvertTo<PublicInfoDB>();


                friendsPublicInfo = Info;



                if (!Info.AvatarUsed)

                {

                    StartCoroutine(LoadFriendImage(Info.PictureURL, Info));
                }

                else
                {
                    SetFriendsItem(Info);
                }



            }



        });



    }




    public void SetFriendName(string friendname)
    {
        FriendNameTxt.text = friendname;
        Debug.Log("Set friend name chla");
    }

    public void SetFriendImage(Sprite sp)
    {
        FriendImg.sprite = sp;

    }

    public IEnumerator LoadFriendImage(string MediaUrl, PublicInfoDB publicinfo)
    {
        Debug.Log("Load img me gya..");

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Create a request
        yield return request.SendWebRequest(); //Wait for the request to complete
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);

        }
        else
        {
            Debug.Log("texture mil gya.......!!!!");
            if (publicinfo.AvatarUsed == false)
            {

            }
            var tex = ((DownloadHandlerTexture)request.downloadHandler).texture;


            var spriteImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));


            SetFriendImage(spriteImage);

            SetFriendsItem(publicinfo);






        }
    }
    public void NextButtonMethod()
    {


        Debug.Log("You");

        onNextButtonClickEvent?.Invoke();


    }
    public void PreviousButtonMethod()
    {
        onPreviousButtonClickEvent?.Invoke();
    }
    void DisableAfterInvite()
    {
        InviteFriendButton.interactable = false;
        InviteFriendButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Invited";
    }

}



