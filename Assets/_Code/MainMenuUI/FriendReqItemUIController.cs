using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Firebase.Firestore;
using Firebase.Extensions;
using Booray.Auth;
using UnityEngine.Networking;

public class FriendReqItemUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI FriendReqNameTxt;
    [SerializeField] private TextMeshProUGUI FriendReqStatusTxt;
    [SerializeField] private Image FriendReqImg;

    [SerializeField] private Button AcceptRequestButton;

    [SerializeField] private Button CancelRequestButton;
    public Button NextButton;
    [SerializeField] private Button PreviousButton;


    public Action<string> onAcceptRequestClicked;
    public Action<string> onCancelRequestClicked;

    public Action ReqonNextButtonClickEvent;
    public Action ReqonPreviousButtonClickEvent;

    public PublicInfoDB friendsPublicInfo;

    private void Start()
    {

        if (gameObject.tag == "Next")
        {
            NextButton.onClick.AddListener(delegate { NextButtonMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
        }
        if (gameObject.tag == "Previous")
        {
            PreviousButton.onClick.AddListener(delegate { PreviousButtonMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
        }
    }

    public void SetFriendReqItem(PublicInfoDB friendsInfo)
    {
        friendsPublicInfo = friendsInfo;
        if (friendsInfo.AvatarUsed == true)
        {
            var sp = MainUIManager.Instance.avatarAtlus.GetSprite(friendsInfo.AvatarID);

            SetFriendReqImage(sp);
        }
        SetFriendReqName(friendsPublicInfo.UserName);
        AcceptRequestButton.onClick.AddListener(delegate { onAcceptRequestClicked?.Invoke(friendsInfo.UserId); SetRequestStatusText("Request Accepted"); SFXHandler.instance.PlayBtnClickSFX(); });
        CancelRequestButton.onClick.AddListener(delegate { onCancelRequestClicked?.Invoke(friendsInfo.UserId); SetRequestStatusText("Request Canceled"); SFXHandler.instance.PlayBtnClickSFX(); });



    }

    public void GetFriendsReqInfo(string Id, ref FirebaseFirestore dbRef)
    {


        dbRef.Collection(ReferencesHolder.FS_users_Collec).Document(Id).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {

            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.Log("Cant get friend data");
            }
            else
            {
                PublicInfoDB Info = task.Result.ConvertTo<PublicInfoDB>();





                if (!Info.AvatarUsed)

                {

                    StartCoroutine(LoadFriendReqImage(Info.PictureURL, Info));
                }

                else
                {
                    SetFriendReqItem(Info);
                }



            }



        });



    }


    public void SetFriendReqName(string friendname)
    {
        FriendReqNameTxt.text = friendname;
        Debug.Log("Set friend name chla");
    }

    public void SetFriendReqImage(Sprite sp)
    {
        FriendReqImg.sprite = sp;
     
    }
    public void SetRequestStatusText(string status)
    {
        AcceptRequestButton.gameObject.SetActive(false);
        CancelRequestButton.gameObject.SetActive(false);
        FriendReqStatusTxt.text = status;
    }
    public IEnumerator LoadFriendReqImage(string MediaUrl, PublicInfoDB publicinfo)
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

            var tex = ((DownloadHandlerTexture)request.downloadHandler).texture;


            var spriteImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));


            //GameObject friendReqItemGameObject = Instantiate(FriendReqItemPrefab, FriendReqContentParent);
            //FriendReqItemUIController friendReqCont = friendReqItemGameObject.GetComponent<FriendReqItemUIController>();

            Debug.Log("avatar use nae kia ha");
            SetFriendReqImage(spriteImage);
            SetFriendReqItem(publicinfo);
            //friendReqCont.onAcceptRequestClicked = OnClick_AcceptRequest;
            //friendReqCont.onCancelRequestClicked = OnClick_CancelRequest;
            //FriendReqItemList.Add(friendReqItemGameObject);





        }

    }
    public void NextButtonMethod()
    {
      

        Debug.Log("You");

        ReqonNextButtonClickEvent?.Invoke();


    }
    public void PreviousButtonMethod()
    {
        ReqonPreviousButtonClickEvent?.Invoke();
    }
}
