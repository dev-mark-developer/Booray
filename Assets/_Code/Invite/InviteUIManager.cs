using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Firebase.Firestore;
using Firebase.Storage;
using Firebase.Extensions;
using UnityEngine.Networking;
using Firebase.Auth;
using PullToRefresh;
using System.Linq;
using DG.Tweening;
public class InviteUIManager : MonoBehaviour
{

 //   // [SerializeField] private TextMeshProUGUI friendListTxt;


 //   //[SerializeField] private GameObject friendsMainGO;


 //   [SerializeField] private GameObject currentFriendListPanel;








 //   [SerializeField] private Button friendListBtn;
 //  // [SerializeField] private Button friendRequestBtn;

 //   string UserId;




 //   //[SerializeField] Sprite btnSelectedSprite;
 //   //[SerializeField] Sprite BtnDefaultSprite;

 //   public Action onBackBtnClicked;

 //   public Action onCurrentFriendListSelected;
 //   public Action onFriendRequestListSelected;

 //   FirebaseAuth auth;
 //   FirebaseFirestore db;
 //   FirebaseStorage storage;
 //   StorageReference storageReference;

 //   public Transform FriendsContentParent;
 //   public GameObject FriendsItemPrefab;
 //   public GameObject EmptyFriendListPrefab;
 //   public GameObject FriendListNextPrefab;
 //   public GameObject FriendListPreviousPrefab;



 //   int FriendListcounter;

 //   //public Transform FriendReqContentParent;
 //   //public GameObject FriendReqItemPrefab;
 //   //public GameObject EmptyFriendReqListPrefab;
 //   //public GameObject RequestNextPrefab;
 //   //public GameObject RequestPreviousPrefab;


 //   public List<GameObject> FriendItemList;
 // //  public List<GameObject> FriendReqItemList;
 //   int FriendReqListcounter;
 //   [SerializeField]
 //   UIRefreshControl UIRefreshControl;
 //   [SerializeField]
 //   GameObject FriendListScroller;
 

 //   bool allowFriendLoad = true;
 //  // bool allowFriendReqLoad = true;

 //   bool FriendAvailable = false;
 // //  bool RequestAvailable = false;


 //   [Header("Tween Controls")]

 //   //[SerializeField] private RectTransform onscreenPoint;
 //   //[SerializeField] private RectTransform leftPoint;
 //   //[SerializeField] private RectTransform rightPoint;

 //   //[SerializeField] private float tweenDuration;
 //   //[SerializeField] private Ease easeTypeIn;
 //   //[SerializeField] private Ease easeTypeOut;





 //   private Tween friendPanelTween;
 ////   private Tween friendReqPanelTween;


 //   private RectTransform friendListRectTransform;
 //   private RectTransform friendReqRectTransform;

 //   [SerializeField] private int DataLoadLimit;
 //   [SerializeField] private GameObject NoMoreFriendObj;
    
 //   private void Start()
 //   {
      
 //    //   FriendReqItemList = new List<GameObject>();
 //       ReferencesHolder.lastLoadedFriendDocList = new List<DocumentSnapshot>();
 //       ReferencesHolder.lastLoadedRequestDocList = new List<DocumentSnapshot>();
 //       auth = FirebaseAuth.DefaultInstance;
 //       db = FirebaseFirestore.DefaultInstance;
 //       storage = FirebaseStorage.DefaultInstance;
 //       storageReference = storage.GetReferenceFromUrl("gs://accountmanger-ff8e7.appspot.com");
 //       MethodSubscription();
 //       LoadFriendList();
    


 //   //    friendReqRectTransform = friendRequestPanel.GetComponent<RectTransform>();
 //       friendListRectTransform = currentFriendListPanel.GetComponent<RectTransform>();




 //   }

 //   public void ShowRequestPanel()
 //   {
 //       //MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
 //       //MainUIManager.Instance.HomeUI.FriendPanelCanvas.sortingOrder = 2;
 //       //MainUIManager.Instance.HomeUI.BackButton.gameObject.SetActive(true);



 //   }
 //   private void MethodSubscription()
 //   {


 //       friendListBtn.onClick.AddListener(delegate { onCurrentFriendListSelected?.Invoke(); OpenCurrentFriendsList(); });

 //   }

 //   //public void SetCurrentFriendListPanelActive(bool state)
 //   //{
 //   //    currentFriendListPanel.SetActive(state);
 //   //}

 //   //public void SetFriendRequestPanelActive(bool state)
 //   //{
 //   //    friendRequestPanel.SetActive(state);
 //   //}

 //   //public void SetFriendMainPanelActive(bool state)
 //   //{


 //   //    friendsMainGO.SetActive(state);
 //   //}


 //   public void OpenCurrentFriendsListAnim()
 //   {
 //       //if (friendReqPanelTween != null)
 //       //    friendReqPanelTween.Kill();

 //       if (friendPanelTween != null)
 //           friendPanelTween.Kill();



 //       //friendPanelTween = friendListRectTransform.DOAnchorPosX(onscreenPoint.anchoredPosition.x, tweenDuration).SetEase(easeTypeIn);
 //       //friendReqPanelTween = friendReqRectTransform.DOAnchorPosX(rightPoint.anchoredPosition.x, tweenDuration).SetEase(easeTypeOut);



 //   }

 






 //   public void OpenCurrentFriendsList()
 //   {

 //       //OpenCurrentFriendsListAnim();

 //       //friendListBtn.gameObject.GetComponent<Image>().sprite = btnSelectedSprite;
 //       //friendRequestBtn.gameObject.GetComponent<Image>().sprite = BtnDefaultSprite;
 //       Debug.Log("yaaaaaaaay ha value...:" + FriendListcounter);
 //       Debug.Log("yaaay actual .... :" + FriendItemList.Count);
 //       if (FriendListcounter > FriendItemList.Count && FriendListcounter != 0)
 //       {
 //           foreach (var friends in FriendItemList)
 //           {
 //               Destroy(friends.gameObject);

 //           }
 //           LoadFriendList();
 //       }

 //   }
 //   public void RefereshFriendList()
 //   {
 //       Debug.Log("RRRRrRrrrrrRRRRefresHHhHhHhH!");
 //       foreach (var friends in FriendItemList)
 //       {

 //           Destroy(friends.gameObject);



 //       }
 //       FriendItemList.Clear();

 //       LoadFriendList();



 //   }







 //   public void LoadFriendList()
 //   {
 //       NoMoreFriendObj.SetActive(false);
 //       Debug.Log("LAst VaLuE " + ReferencesHolder.lastLoadedFriendDoc);
 //       if (!allowFriendLoad)
 //       {
 //           return;
 //       }

 //       allowFriendLoad = false;




 //       switch (ReferencesHolder.Provider)
 //       {
 //           case "Email":
 //               UserId = PlayerPrefs.GetString(ReferencesHolder.EmailUserId);

 //               Debug.Log("Providerwaa " + ReferencesHolder.Provider);
 //               Debug.Log("Providerwaa ka side effect " + UserId);
 //               break;
 //           case "Guest":
 //               UserId = PlayerPrefs.GetString(ReferencesHolder.GuestUserId);
 //               break;
 //           case "Facebook":
 //               UserId = PlayerPrefs.GetString(ReferencesHolder.FBUserId);
 //               break;
 //           case "Google":
 //               //UserId = PlayerPrefs.GetString(ReferencesHolder.GoogleUserId);
 //               UserId = ReferencesHolder.newUserId;
 //               break;
 //       }

 //       if (ReferencesHolder.lastLoadedFriendDocList.Count == 0)
 //       {

 //           Query collRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(UserId).Collection(ReferencesHolder.FS_FriendsData_Collec).Limit(DataLoadLimit);
 //           collRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
 //           {

 //               QuerySnapshot snapshot = task.Result;

 //               foreach (DocumentSnapshot document in snapshot.Documents)
 //               {
 //                   if (document.Exists)
 //                   {
 //                       if (FriendAvailable == false)
 //                       {
 //                           FriendAvailable = true;
 //                       }
 //                   }
 //                   Debug.Log(document.Id);


 //                   ReferencesHolder.lastLoadedFriendDoc = document;
 //                   GameObject friendItemGameObject = Instantiate(FriendsItemPrefab, FriendsContentParent);
 //                   InviteItemUIController friendCont = friendItemGameObject.GetComponent<InviteItemUIController>();

 //                   friendCont.GetFriendsInfo(document.Id, ref db);
 //                   friendCont.onInviteClickEvent = OnClick_InviteFriend;
 //                   FriendItemList.Add(friendItemGameObject);







 //               }

 //               Debug.Log("Read all data from the users collection.");
 //               if (FriendAvailable == false)
 //               {
 //                   Debug.Log("Friend list itemsxx " + FriendItemList.Count);
 //                   GameObject friendEmptyItemGameObject = Instantiate(EmptyFriendListPrefab, FriendsContentParent);
 //                   FriendItemList.Add(friendEmptyItemGameObject);

 //               }
 //               else
 //               {

 //                   GameObject friendItemItemGameObject = Instantiate(FriendListNextPrefab, FriendsContentParent);

 //                   var cont2 = friendItemItemGameObject.GetComponent<FriendsItemUIController>();

 //                   cont2.onNextButtonClickEvent = OnLoadNextFriends;

 //                   FriendItemList.Add(friendItemItemGameObject);




 //               }

 //               allowFriendLoad = true;
 //               FriendListScroller.GetComponent<UIRefreshControl>().EndRefreshing();
 //           });








 //       }
 //       else
 //       {

 //           Debug.Log(ReferencesHolder.lastLoadedFriendDocList.Count);
 //           if (true)
 //           {

 //               Query collRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(UserId).Collection(ReferencesHolder.FS_FriendsData_Collec).StartAfter(ReferencesHolder.lastLoadedFriendDocList.Last()).Limit(DataLoadLimit);
 //               collRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
 //               {

 //                   QuerySnapshot snapshot = task.Result;

 //                   GameObject friendPrevItemGameObject = Instantiate(FriendListPreviousPrefab, FriendsContentParent);

 //                   var cont = friendPrevItemGameObject.GetComponent<FriendsItemUIController>();

 //                   cont.onPreviousButtonClickEvent = OnLoadPrevFriends;

 //                   FriendItemList.Add(friendPrevItemGameObject);

 //                   foreach (DocumentSnapshot document in snapshot.Documents)
 //                   {
 //                       if (document.Exists)
 //                       {
 //                           if (FriendAvailable == false)
 //                           {
 //                               FriendAvailable = true;
 //                           }
 //                       }

 //                       Debug.Log(document.Id);

 //                       ReferencesHolder.lastLoadedFriendDoc = document;
 //                       GameObject friendItemGameObject = Instantiate(FriendsItemPrefab, FriendsContentParent);
 //                       InviteItemUIController friendCont = friendItemGameObject.GetComponent<InviteItemUIController>();

 //                       friendCont.GetFriendsInfo(document.Id, ref db);
 //                       friendCont.onInviteClickEvent = OnClick_InviteFriend;
 //                       FriendItemList.Add(friendItemGameObject);







 //                   }

 //                   Debug.Log("Read all data from the users collection.");
 //                   if (FriendAvailable == false)
 //                   {
 //                       Debug.Log("Friend list itemsxx " + FriendItemList.Count);
 //                       GameObject friendEmptyItemGameObject = Instantiate(EmptyFriendListPrefab, FriendsContentParent);
 //                       FriendItemList.Add(friendEmptyItemGameObject);

 //                   }
 //                   else
 //                   {
 //                       GameObject friendItemItemGameObject = Instantiate(FriendListNextPrefab, FriendsContentParent);

 //                       var cont2 = friendItemItemGameObject.GetComponent<FriendsItemUIController>();

 //                       cont2.onNextButtonClickEvent = OnLoadNextFriends;


 //                       FriendItemList.Add(friendItemItemGameObject);




 //                   }
 //                   Debug.Log("this isssssss  " + FriendItemList.Count);
 //                   if (FriendItemList.Count == 2)
 //                   {
 //                       Destroy(FriendItemList.Last());
 //                       NoMoreFriendObj.SetActive(true);
 //                   }
 //                   allowFriendLoad = true;
 //                   FriendListScroller.GetComponent<UIRefreshControl>().EndRefreshing();
 //               });
 //           }


  



 //       }
 //   }





 
   







  
 //   public void OnClick_InviteFriend(string friendId)
 //   {
 //       Debug.Log("thats the friend's id: " + friendId+"to invite");

 //       //db.Collection(ReferencesHolder.FS_users_Collec).Document(friendId)
 //       //    .Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc)
 //       //    .GetSnapshotAsync().ContinueWithOnMainThread(task =>
 //       //    {
 //       //        if (task.IsCanceled || task.IsFaulted)
 //       //        {
 //       //            Debug.Log("could not view friends");
 //       //        }
 //       //        else
 //       //        {

 //       //            PublicInfoDB Info = task.Result.ConvertTo<PublicInfoDB>();
 //       //            ReferencesHolder.FriendStatsName = Info.UserName;
 //       //            ReferencesHolder.FriendSpriteUrl = Info.PictureURL;
 //       //            ReferencesHolder.FriendId = Info.UserId;
 //       //            Debug.Log("yay value set hoe friend name ki " + ReferencesHolder.FriendStatsName);

 //       //            db.Collection(ReferencesHolder.FS_users_Collec).Document(friendId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_Stats_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
 //       //            {
 //       //                if (task.IsCanceled || task.IsFaulted)
 //       //                {

 //       //                }
 //       //                else
 //       //                {

 //       //                    StatsDB stats = task.Result.ConvertTo<StatsDB>();
 //       //                    ReferencesHolder.playerStats = stats;
 //       //                    Debug.Log("yay value set hoe friend name ki " + ReferencesHolder.FriendStatsName);
 //       //                }
 //       //            });

 //       //        }
 //       //    });
 //   }

   
    

 //   public IEnumerator LoadFriendImage(string MediaUrl, PublicInfoDB publicinfo)
 //   {
 //       Debug.Log("Load img me gya..");

 //       UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Create a request
 //       yield return request.SendWebRequest(); //Wait for the request to complete
 //       if (request.result == UnityWebRequest.Result.ConnectionError)
 //       {
 //           Debug.Log(request.error);

 //       }
 //       else
 //       {
 //           Debug.Log("texture mil gya.......!!!!");
 //           if (publicinfo.AvatarUsed == false)
 //           {

 //           }
 //           var tex = ((DownloadHandlerTexture)request.downloadHandler).texture;


 //           var spriteImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

 //           GameObject friendItemGameObject = Instantiate(FriendsItemPrefab, FriendsContentParent);
 //           InviteItemUIController friendCont = friendItemGameObject.GetComponent<InviteItemUIController>();

 //           Debug.Log("avatar use nae kia ha");
 //           friendCont.SetFriendImage(spriteImage);

 //           friendCont.SetFriendsItem(publicinfo);
 //           friendCont.onInviteClickEvent = OnClick_InviteFriend;
 //           FriendItemList.Add(friendItemGameObject);






 //       }
 //   }
 


 //   public void OnLoadNextFriends()
 //   {
 //       // update last friuend doucm




 //       ReferencesHolder.lastLoadedFriendDocList.Add(ReferencesHolder.lastLoadedFriendDoc);


 //       foreach (var friends in FriendItemList)
 //       {
 //           //if (FriendReqItemList.Count > 1)
 //           //  {
 //           Destroy(friends.gameObject);
 //           //  }



 //       }

 //       FriendItemList.Clear();



 //       LoadFriendList();

 //       // Load friends
 //   }

 //   public void OnLoadPrevFriends()
 //   {
 //       ReferencesHolder.lastLoadedFriendDocList.RemoveAt(ReferencesHolder.lastLoadedFriendDocList.Count - 1);

 //       Debug.Log(ReferencesHolder.lastLoadedFriendDocList + "<========");
 //       foreach (var friends in FriendItemList)
 //       {

 //           Destroy(friends.gameObject);




 //       }

 //       FriendItemList.Clear();


 //       LoadFriendList();
 //   }


   

 //   public void Load_Friend(PublicInfoDB publicinfo)
 //   {



 //       GameObject friendItemGameObject = Instantiate(FriendsItemPrefab, FriendsContentParent);
 //       InviteItemUIController friendCont = friendItemGameObject.GetComponent<InviteItemUIController>();
 //       friendCont.SetFriendsItem(publicinfo);
 //       friendCont.onInviteClickEvent = OnClick_InviteFriend;
 //       FriendItemList.Add(friendItemGameObject);




 //   }

}
