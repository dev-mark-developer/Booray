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
public class InviteItemUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI FriendNameTxt;
    [SerializeField] private Image FriendImg;

    [SerializeField] private Button ViewProfileButton;
    public Button NextButton;
    [SerializeField] private Button PreviousButton;

    public Action<string> onInviteClickEvent;


    public Action onNextButtonClickEvent;
    public Action onPreviousButtonClickEvent;

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

    public void SetFriendsItem(PublicInfoDB friendsInfo)
    {
        friendsPublicInfo = friendsInfo;
        if (friendsInfo.AvatarUsed == true)
        {
            var sp = MainUIManager.Instance.avatarAtlus.GetSprite(friendsInfo.AvatarID);

            SetFriendImage(sp);
        }

        SetFriendName(friendsPublicInfo.UserName);
        ViewProfileButton.onClick.AddListener(delegate { onInviteClickEvent?.Invoke(friendsInfo.UserId); SFXHandler.instance.PlayBtnClickSFX(); });

        // NextButton.onClick.AddListener(delegate { OnNextclick.Invoke(); });
        // NextButton.onClick.AddListener(delegate { OnNextclick?.Invoke(); });


    }

    public void GetFriendsInfo(string Id, ref FirebaseFirestore dbRef)
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




                //if (Info.AvatarUsed == false)
                //{
                //    StartCoroutine(LoadFriendImage(Info.PictureURL, Info));
                //}
                //else
                //{
                //    Load_Friend(Info);
                //}

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
        Debug.Log("Set friend name chla (INVITE)");
    }

    public void SetFriendImage(Sprite sp)
    {
        FriendImg.sprite = sp;
        Debug.Log("Set friend image chla (INVITE)");
     

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

            // GameObject friendItemGameObject = Instantiate(FriendsItemPrefab, FriendsContentParent);
            // FriendsItemUIController friendCont = friendItemGameObject.GetComponent<FriendsItemUIController>();

            //  Debug.Log("avatar use nae kia ha");
            SetFriendImage(spriteImage);

            SetFriendsItem(publicinfo);
            //friendCont.onViewProfileClickEvent = OnClick_ViewFriend;
            // FriendItemList.Add(friendItemGameObject);






        }
    }
    public void NextButtonMethod()
    {
        //Debug.Log("ccccccccccccccccccccccccccccccchlaaaaaaaaaaaaaaa");
        //Debug.Log(ReferencesHolder.FriendLoadLimit);
        ////   ReferencesHolder.FriendLoadLimit = ReferencesHolder.FriendLoadLimit + 1;
        //ReferencesHolder.allowReloadFriends = true;
        //foreach (var friends in MainUIManager.Instance.FriendsUI.FriendItemList)
        //{
        //    Destroy(friends.gameObject);

        //}
        //MainUIManager.Instance.FriendsUI.LoadFriendList();

        Debug.Log("You");

        onNextButtonClickEvent?.Invoke();


    }
    public void PreviousButtonMethod()
    {
        onPreviousButtonClickEvent?.Invoke();
    }


}

