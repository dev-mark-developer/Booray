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
using UCharts;
using DG.Tweening;




namespace Booray.Auth
{
    public class FriendsUIManager : MonoBehaviour
    {

        // [SerializeField] private TextMeshProUGUI friendListTxt;


        [SerializeField] private GameObject friendsMainGO;


        [SerializeField] private GameObject currentFriendListPanel;
        [SerializeField] private GameObject friendRequestPanel;
        [SerializeField] private GameObject friendStatsPanel;


        [SerializeField] FriendStatsUIManager friendsStatUImanagerInstance;




        [SerializeField] private Button friendListBtn;
        [SerializeField] private Button friendRequestBtn;
        [SerializeField] private Button BackButton;
        [SerializeField] private Button SearchFriendButton;
        [SerializeField] private Button AddfriendOpenButton;
        [SerializeField] private Button AddFriendBackButton;


        string UserId;




        [SerializeField] Sprite btnSelectedSprite;
        [SerializeField] Sprite BtnDefaultSprite;

        public Action onBackBtnClicked;

        public Action onCurrentFriendListSelected;
        public Action onFriendRequestListSelected;

        FirebaseAuth auth;
        FirebaseFirestore db;
        FirebaseStorage storage;
        StorageReference storageReference;

        public Transform FriendsContentParent;
        public GameObject FriendsItemPrefab;
        public GameObject FindFriendsObj;
        public GameObject EmptyFriendListPrefab;
        public GameObject FriendListNextPrefab;
        public GameObject FriendListPreviousPrefab;

        #region Find Friend Prefabs
        public Transform FindFriendsContentParent;
        public GameObject FindFriendsItemPrefab;
        public GameObject FindEmptyFriendListPrefab;
        public GameObject FindFriendListNextPrefab;
        public GameObject FindFriendListPreviousPrefab;
        public GameObject FindFriendPanel;

        public TextMeshProUGUI findFriendWarningTxt;


        #endregion

        public Transform InviteContentParent;
        public GameObject InviteItemPrefab;
        public GameObject InviteEmptyPrefab;
        public GameObject InviteNextPrefab;
        public GameObject InvitePreviousPrefab;



        int FriendListcounter;

        public Transform FriendReqContentParent;
        public GameObject FriendReqItemPrefab;
        public GameObject EmptyFriendReqListPrefab;
        public GameObject RequestNextPrefab;
        public GameObject RequestPreviousPrefab;


        public List<GameObject> FriendItemList;
        public List<GameObject> FriendReqItemList;
        public List<GameObject> InviteItemList;
        int FriendReqListcounter;
        [SerializeField]
        UIRefreshControl UIRefreshControl;
        [SerializeField]
        GameObject FriendListScroller;
        [SerializeField]
        GameObject FriendReqScroller;
        [SerializeField]
        GameObject FriendInviteScroller;

        bool allowFriendLoad = true;
        bool allowFriendReqLoad = true;
        bool allowFriendInviteLoad = true;


        bool FriendAvailable = false;
        bool FriendInviteAvailable = false;
        bool RequestAvailable = false;


        [Header("Tween Controls")]

        [SerializeField] private RectTransform onscreenPoint;
        [SerializeField] private RectTransform leftPoint;
        [SerializeField] private RectTransform rightPoint;

        [SerializeField] private float tweenDuration;
        [SerializeField] private Ease easeTypeIn;
        [SerializeField] private Ease easeTypeOut;





        private Tween friendPanelTween;
        private Tween friendReqPanelTween;


        private RectTransform friendListRectTransform;
        private RectTransform friendReqRectTransform;

        [SerializeField] private int DataLoadLimit;
        [SerializeField] private GameObject NoMoreFriendObj;
        [SerializeField] private GameObject NoMoreRequestObj;
        [SerializeField] private GameObject NoMoreFriendInviteObj;
        public List<GameObject> FindFriendItemList;
        public TMP_InputField FindFriendField;

        private void Start()
        {
            //   ReferencesHolder.FriendLoadLimit = 1;
            FriendItemList = new List<GameObject>();
            FindFriendItemList = new List<GameObject>();
            FriendReqItemList = new List<GameObject>();
            ReferencesHolder.lastLoadedFriendDocList = new List<DocumentSnapshot>();

            ReferencesHolder.lastLoadedFriendInviteDocList = new List<DocumentSnapshot>();
            ReferencesHolder.lastLoadedRequestDocList = new List<DocumentSnapshot>();
            ReferencesHolder.AllInvitesList = new List<string>();

            //   ReferencesHolder.lastLoadedFriendInviteDoc=new List<DocumentSnapshot>()
            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseFirestore.DefaultInstance;
            storage = FirebaseStorage.DefaultInstance;
            storageReference = storage.GetReferenceFromUrl("gs://ultimate-booray.appspot.com");
            MethodSubscription();
            // LoadFriendList(FriendsItemPrefab, FriendListNextPrefab, FriendListPreviousPrefab, EmptyFriendListPrefab, FriendsContentParent);
            //  LoadFriendInviteList(InviteItemPrefab, InviteNextPrefab, InvitePreviousPrefab, InviteEmptyPrefab, InviteContentParent);
            //LoadFriendRequests();
            SearchFriendButton.onClick.AddListener(delegate { OpenCurrentFindFriendsList(); SFXHandler.instance.PlayBtnClickSFX(); });
            BackButton.onClick.AddListener(delegate { BackButtonMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
            friendReqRectTransform = friendRequestPanel.GetComponent<RectTransform>();
            friendListRectTransform = currentFriendListPanel.GetComponent<RectTransform>();




        }

        public void ShowFriendsPanel()
        {
            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                //LoadFriendList(FriendsItemPrefab, FriendListNextPrefab, FriendListPreviousPrefab, EmptyFriendListPrefab, FriendsContentParent);
                //LoadFriendRequests();
                RefreshFriendRequest();
                RefereshFriendList();
                Debug.Log("------>>>>>>>>>>>>>>>show friends panel chal gya");
                MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
                MainUIManager.Instance.HomeUI.FriendsPanel.SetActive(true);
                //MainUIManager.Instance.HomeUI.FriendPanelCanvas.sortingOrder = 4;
                MainUIManager.Instance.HomeUI.BackButton.gameObject.SetActive(true);
            }
            else
            {
                if (MainUIManager.Instance.HomeUI.StatsPanel.activeInHierarchy != true)
                {
                    MainUIManager.Instance.HomeUI.HomePanel.SetActive(true);
                }

                //LogErrorUIHandler.instance.OpenErrorPanel("Internet not reachable or connected");
            }




        }
        private void MethodSubscription()
        {

            AddfriendOpenButton.onClick.AddListener(delegate { OpenFindFriendPanel(); SFXHandler.instance.PlayBtnClickSFX(); });
            AddFriendBackButton.onClick.AddListener(delegate { AddFriendBackMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
            friendListBtn.onClick.AddListener(delegate { onCurrentFriendListSelected?.Invoke(); OpenCurrentFriendsList(); SFXHandler.instance.PlayBtnClickSFX(); });
            friendRequestBtn.onClick.AddListener(delegate { onFriendRequestListSelected?.Invoke(); OpenFriendRequestPanel(); SFXHandler.instance.PlayBtnClickSFX(); });
        }

        public void SetCurrentFriendListPanelActive(bool state)
        {
            currentFriendListPanel.SetActive(state);
        }

        public void SetFriendRequestPanelActive(bool state)
        {
            friendRequestPanel.SetActive(state);
        }

        public void SetFriendMainPanelActive(bool state)
        {


            friendsMainGO.SetActive(state);
        }
        public void OpenFindFriendPanel()
        {
            MainUIManager.Instance.HomeUI.DeactiveHomePanel();
            MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
            FindFriendPanel.SetActive(true);

        }

        public void OpenCurrentFriendsListAnim()
        {
            if (friendReqPanelTween != null)
                friendReqPanelTween.Kill();

            if (friendPanelTween != null)
                friendPanelTween.Kill();



            friendPanelTween = friendListRectTransform.DOAnchorPosX(onscreenPoint.anchoredPosition.x, tweenDuration).SetEase(easeTypeIn);
            friendReqPanelTween = friendReqRectTransform.DOAnchorPosX(rightPoint.anchoredPosition.x, tweenDuration).SetEase(easeTypeOut);



        }

        public void OpenRequestFriendsListAnim()
        {
            if (friendReqPanelTween != null)
                friendReqPanelTween.Kill();

            if (friendPanelTween != null)
                friendPanelTween.Kill();



            friendPanelTween = friendListRectTransform.DOAnchorPosX(leftPoint.anchoredPosition.x, tweenDuration).SetEase(easeTypeIn);
            friendReqPanelTween = friendReqRectTransform.DOAnchorPosX(onscreenPoint.anchoredPosition.x, tweenDuration).SetEase(easeTypeOut);

        }







        public void OpenCurrentFriendsList()
        {

            OpenCurrentFriendsListAnim();

            //friendListBtn.gameObject.GetComponent<Image>().sprite = btnSelectedSprite;
            //friendRequestBtn.gameObject.GetComponent<Image>().sprite = BtnDefaultSprite;
            friendListBtn.transform.GetChild(1).GetComponent<Image>().gameObject.SetActive(true);
            friendRequestBtn.transform.GetChild(1).GetComponent<Image>().gameObject.SetActive(false);
            Debug.Log("yaaaaaaaay ha value...:" + FriendListcounter);
            Debug.Log("yaaay actual .... :" + FriendItemList.Count);
            if (FriendListcounter > FriendItemList.Count && FriendListcounter != 0)
            {
                foreach (var friends in FriendItemList)
                {
                    Destroy(friends.gameObject);

                }
                LoadFriendList(FriendsItemPrefab, FriendListNextPrefab, FriendListPreviousPrefab, EmptyFriendListPrefab, FriendsContentParent);
            }

        }
        public void RefereshFriendList()
        {
            Debug.Log("RRRRrRrrrrrRRRRefresHHhHhHhH!");
            foreach (var friends in FriendItemList)
            {

                Destroy(friends.gameObject);



            }
            FriendItemList.Clear();

            LoadFriendList(FriendsItemPrefab, FriendListNextPrefab, FriendListPreviousPrefab, EmptyFriendListPrefab, FriendsContentParent);



        }

        public void RefereshFriendInviteList()
        {
            Debug.Log("RRRRrRrrrrrRRRRefresHHhHhHhH!");
            foreach (var friends in InviteItemList)
            {

                Destroy(friends.gameObject);



            }
            InviteItemList.Clear();

            LoadFriendInviteList(InviteItemPrefab, InviteNextPrefab, InvitePreviousPrefab, InviteEmptyPrefab, InviteContentParent);



        }





        public void LoadFriendList(GameObject ItemPrefab, GameObject NextPrefab, GameObject PreviousPrefab, GameObject EmptyPrefab, Transform ContentParent)
        {
            NoMoreFriendObj.SetActive(false);

            Debug.Log("LAst VaLuE " + ReferencesHolder.lastLoadedFriendDoc);
            if (!allowFriendLoad)
            {
                return;
            }

            allowFriendLoad = false;




            //switch (ReferencesHolder.Provider)
            //{
            //    case "Email":
            //        UserId = PlayerPrefs.GetString(ReferencesHolder.EmailUserId);

            //        Debug.Log("Providerwaa " + ReferencesHolder.Provider);
            //        Debug.Log("Providerwaa ka side effect " + UserId);
            //        break;
            //    case "Guest":
            //        UserId = PlayerPrefs.GetString(ReferencesHolder.GuestUserId);
            //        break;
            //    case "Facebook":
            //        UserId = PlayerPrefs.GetString(ReferencesHolder.FBUserId);
            //        break;
            //    case "Google":
            //        //UserId = PlayerPrefs.GetString(ReferencesHolder.GoogleUserId);
            //        UserId = ReferencesHolder.newUserId;
            //        break;
            //}

            if (ReferencesHolder.lastLoadedFriendDocList.Count == 0)
            {

                Debug.Log($"user Coollec {ReferencesHolder.FS_users_Collec} | {ReferencesHolder.playerPublicInfo.UserId} | {ReferencesHolder.FS_FriendsData_Collec} | {DataLoadLimit}");

                Query collRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_FriendsData_Collec).Limit(DataLoadLimit);
                collRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {

                    QuerySnapshot snapshot = task.Result;

                    //GameObject friendPrevItemGameObject = Instantiate(FriendListPreviousPrefab, FriendsContentParent);

                    //var cont = friendPrevItemGameObject.GetComponent<FriendsItemUIController>();

                    //cont.onPreviousButtonClickEvent = OnLoadPrevFriends;

                    //FriendItemList.Add(friendPrevItemGameObject);

                    foreach (DocumentSnapshot document in snapshot.Documents)
                    {
                        if (document.Exists)
                        {
                            if (FriendAvailable == false)
                            {
                                FriendAvailable = true;
                            }
                        }
                        Debug.Log(document.Id);


                        ReferencesHolder.lastLoadedFriendDoc = document;
                        GameObject friendItemGameObject = Instantiate(ItemPrefab, ContentParent);
                        FriendsItemUIController friendCont = friendItemGameObject.GetComponent<FriendsItemUIController>();

                        friendCont.GetFriendsInfo(document.Id, ref db);
                        friendCont.onViewProfileClickEvent = OnClick_ViewFriend;
                        FriendItemList.Add(friendItemGameObject);

                        //    GameObject InviteItemGameObject = Instantiate(InviteItemPrefab, InviteContentParent);







                    }

                    Debug.Log("Read all data from the users collection.");
                    if (FriendAvailable == false)
                    {
                        Debug.Log("Friend list itemsxx " + FriendItemList.Count);
                        GameObject friendEmptyItemGameObject = Instantiate(EmptyPrefab, ContentParent);
                        FriendItemList.Add(friendEmptyItemGameObject);
                        FindFriendsObj.SetActive(false);

                    }
                    else
                    {
                        FindFriendsObj.SetActive(true);
                        if (FriendItemList.Count < DataLoadLimit)
                        {
                        }
                        else
                        {
                            GameObject friendItemItemGameObject = Instantiate(NextPrefab, ContentParent);

                            var cont2 = friendItemItemGameObject.GetComponent<FriendsItemUIController>();

                            cont2.onNextButtonClickEvent = OnLoadNextFriends;


                            FriendItemList.Add(friendItemItemGameObject);
                        }





                    }

                    allowFriendLoad = true;
                    FriendListScroller.GetComponent<UIRefreshControl>().EndRefreshing();
                });








            }
            else
            {

                Debug.Log(ReferencesHolder.lastLoadedFriendDocList.Count);
                if (true)
                {

                    Query collRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_FriendsData_Collec).StartAfter(ReferencesHolder.lastLoadedFriendDocList.Last()).Limit(DataLoadLimit);
                    collRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                    {

                        QuerySnapshot snapshot = task.Result;

                        GameObject friendPrevItemGameObject = Instantiate(PreviousPrefab, ContentParent);

                        var cont = friendPrevItemGameObject.GetComponent<FriendsItemUIController>();

                        cont.onPreviousButtonClickEvent = OnLoadPrevFriends;

                        FriendItemList.Add(friendPrevItemGameObject);

                        foreach (DocumentSnapshot document in snapshot.Documents)
                        {
                            if (document.Exists)
                            {
                                if (FriendAvailable == false)
                                {
                                    FriendAvailable = true;
                                }
                            }

                            Debug.Log(document.Id);

                            ReferencesHolder.lastLoadedFriendDoc = document;
                            GameObject friendItemGameObject = Instantiate(ItemPrefab, ContentParent);
                            FriendsItemUIController friendCont = friendItemGameObject.GetComponent<FriendsItemUIController>();

                            friendCont.GetFriendsInfo(document.Id, ref db);
                            friendCont.onViewProfileClickEvent = OnClick_ViewFriend;
                            FriendItemList.Add(friendItemGameObject);







                        }

                        Debug.Log("Read all data from the users collection.");
                        if (FriendAvailable == false)
                        {
                            Debug.Log("Friend list itemsxx " + FriendItemList.Count);
                            GameObject friendEmptyItemGameObject = Instantiate(EmptyPrefab, ContentParent);
                            FriendItemList.Add(friendEmptyItemGameObject);
                            FindFriendsObj.SetActive(false);

                        }
                        else
                        {
                            FindFriendsObj.SetActive(true);
                            if (FriendItemList.Count < DataLoadLimit)
                            {
                            }
                            else
                            {
                                GameObject friendItemItemGameObject = Instantiate(NextPrefab, ContentParent);

                                var cont2 = friendItemItemGameObject.GetComponent<FriendsItemUIController>();

                                cont2.onNextButtonClickEvent = OnLoadNextFriends;


                                FriendItemList.Add(friendItemItemGameObject);
                            }








                        }
                        Debug.Log("this isssssss  " + FriendItemList.Count);
                        if (FriendItemList.Count == 2)
                        {
                            Destroy(FriendItemList.Last());
                            NoMoreFriendObj.SetActive(true);
                        }
                        allowFriendLoad = true;
                        FriendListScroller.GetComponent<UIRefreshControl>().EndRefreshing();
                    });
                }


                //allowFriendLoad = true;
                //FriendListScroller.GetComponent<UIRefreshControl>().EndRefreshing();



            }
        }



        public void LoadFriendInviteList(GameObject ItemPrefab, GameObject NextPrefab, GameObject PreviousPrefab, GameObject EmptyPrefab, Transform ContentParent)
        {
            EmptyPrefab.SetActive(false);
            Debug.Log("LAst VaLuE " + ReferencesHolder.lastLoadedFriendDoc);
            if (!allowFriendInviteLoad)
            {
                return;
            }

            allowFriendInviteLoad = false;




            //switch (ReferencesHolder.Provider)
            //{
            //    case "Email":
            //        UserId = PlayerPrefs.GetString(ReferencesHolder.EmailUserId);

            //        Debug.Log("Providerwaa " + ReferencesHolder.Provider);
            //        Debug.Log("Providerwaa ka side effect " + UserId);
            //        break;
            //    case "Guest":
            //        UserId = PlayerPrefs.GetString(ReferencesHolder.GuestUserId);
            //        break;
            //    case "Facebook":
            //        UserId = PlayerPrefs.GetString(ReferencesHolder.FBUserId);
            //        break;
            //    case "Google":
            //        //UserId = PlayerPrefs.GetString(ReferencesHolder.GoogleUserId);
            //        UserId = ReferencesHolder.newUserId;
            //        break;
            //}

            if (ReferencesHolder.lastLoadedFriendInviteDocList.Count == 0)
            {

                Query collRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_FriendsData_Collec).Limit(DataLoadLimit);
                collRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {

                    QuerySnapshot snapshot = task.Result;

                    //GameObject friendPrevItemGameObject = Instantiate(FriendListPreviousPrefab, FriendsContentParent);

                    //var cont = friendPrevItemGameObject.GetComponent<FriendsItemUIController>();

                    //cont.onPreviousButtonClickEvent = OnLoadPrevFriends;

                    //FriendItemList.Add(friendPrevItemGameObject);

                    foreach (DocumentSnapshot document in snapshot.Documents)
                    {

                        if (document.Exists)
                        {
                            if (FriendInviteAvailable == false)
                            {
                                FriendInviteAvailable = true;
                                InviteEmptyPrefab.SetActive(false);
                            }
                        }
                        Debug.Log(document.Id);


                        ReferencesHolder.lastLoadedFriendInviteDoc = document;
                        GameObject friendItemGameObject = Instantiate(ItemPrefab, ContentParent);
                        FriendsItemUIController friendCont = friendItemGameObject.GetComponent<FriendsItemUIController>();

                        friendCont.GetFriendsInfo(document.Id, ref db);
                        friendCont.onInviteFriendClickEvent = OnClick_InviteFriend;
                        InviteItemList.Add(friendItemGameObject);

                        //    GameObject InviteItemGameObject = Instantiate(InviteItemPrefab, InviteContentParent);







                    }

                    Debug.Log("Read all data from the users collection.");
                    if (FriendInviteAvailable == false)
                    {
                        InviteEmptyPrefab.SetActive(true);
                        Debug.Log("Friend list itemsxx " + InviteItemList.Count);
                        // GameObject friendEmptyItemGameObject = Instantiate(EmptyPrefab, ContentParent);
                        //InviteItemList.Add(friendEmptyItemGameObject);
                        // EmptyPrefab.SetActive(true);
                    }
                    else
                    {

                        if (InviteItemList.Count < DataLoadLimit)
                        {
                        }
                        else
                        {
                            GameObject friendItemItemGameObject = Instantiate(NextPrefab, ContentParent);

                            var cont2 = friendItemItemGameObject.GetComponent<FriendsItemUIController>();

                            cont2.onNextButtonClickEvent = OnLoadNextFriendsInvite;

                            InviteItemList.Add(friendItemItemGameObject);

                            Debug.Log("next of invite instantiate........1");
                        }





                    }

                    allowFriendInviteLoad = true;
                    FriendInviteScroller.GetComponent<UIRefreshControl>().EndRefreshing();
                });








            }
            else
            {

                Debug.Log(ReferencesHolder.lastLoadedFriendInviteDocList.Count);
                if (true)
                {

                    Query collRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_FriendsData_Collec).StartAfter(ReferencesHolder.lastLoadedFriendInviteDocList.Last()).Limit(DataLoadLimit);
                    collRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                    {

                        QuerySnapshot snapshot = task.Result;

                        GameObject friendPrevItemGameObject = Instantiate(PreviousPrefab, ContentParent);

                        var cont = friendPrevItemGameObject.GetComponent<FriendsItemUIController>();

                        cont.onPreviousButtonClickEvent = OnLoadPrevFriendsInvite;

                        InviteItemList.Add(friendPrevItemGameObject);

                        foreach (DocumentSnapshot document in snapshot.Documents)
                        {
                            if (document.Exists)
                            {
                                if (FriendInviteAvailable == false)
                                {
                                    FriendInviteAvailable = true;
                                    InviteEmptyPrefab.SetActive(false);
                                }
                            }

                            Debug.Log(document.Id);

                            ReferencesHolder.lastLoadedFriendInviteDoc = document;
                            GameObject friendItemGameObject = Instantiate(ItemPrefab, ContentParent);
                            FriendsItemUIController friendCont = friendItemGameObject.GetComponent<FriendsItemUIController>();

                            friendCont.GetFriendsInfo(document.Id, ref db);
                            friendCont.onInviteFriendClickEvent = OnClick_InviteFriend;
                            InviteItemList.Add(friendItemGameObject);







                        }

                        Debug.Log("Read all data from the users collection.");
                        if (FriendInviteAvailable == false)
                        {
                            InviteEmptyPrefab.SetActive(true);
                            Debug.Log("Friend list itemsxx " + InviteItemList.Count);
                            // GameObject friendEmptyItemGameObject = Instantiate(EmptyPrefab, ContentParent);
                            //InviteItemList.Add(friendEmptyItemGameObject);
                            // NoMoreFriendInviteObj.SetActive(true);

                            //  EmptyPrefab.SetActive(true);

                        }
                        else
                        {
                            if (InviteItemList.Count < DataLoadLimit)
                            {

                            }
                            else
                            {
                                GameObject friendItemItemGameObject = Instantiate(NextPrefab, ContentParent);

                                var cont2 = friendItemItemGameObject.GetComponent<FriendsItemUIController>();

                                cont2.onNextButtonClickEvent = OnLoadNextFriendsInvite;


                                InviteItemList.Add(friendItemItemGameObject);
                                Debug.Log("next of invite instantiate........2");
                            }





                        }
                        Debug.Log("this isssssss  " + InviteItemList.Count);
                        if (InviteItemList.Count == 2)
                        {
                            Destroy(InviteItemList.Last());
                            NoMoreFriendInviteObj.SetActive(true);
                        }
                        allowFriendInviteLoad = true;
                        FriendInviteScroller.GetComponent<UIRefreshControl>().EndRefreshing();

                    });
                }


                //allowFriendLoad = true;
                //FriendListScroller.GetComponent<UIRefreshControl>().EndRefreshing();



            }
        }

        public void OpenFriendRequestPanel()
        {
            OpenRequestFriendsListAnim();

            // friendListBtn.gameObject.GetComponent<Image>().sprite = BtnDefaultSprite;
            // friendRequestBtn.gameObject.GetComponent<Image>().sprite = btnSelectedSprite;

            friendListBtn.transform.GetChild(1).GetComponent<Image>().gameObject.SetActive(false);
            friendRequestBtn.transform.GetChild(1).GetComponent<Image>().gameObject.SetActive(true);

            Debug.Log("yay hein requestsssssCOUNTER " + FriendReqListcounter);
            Debug.Log("yay hein actual reqs  " + FriendReqItemList.Count);
            if (FriendReqListcounter > FriendReqItemList.Count && FriendReqListcounter != 0)
            {
                foreach (var friends in FriendReqItemList)
                {
                    Destroy(friends.gameObject);

                }
                LoadFriendRequests();
            }
            //if (FriendReqItemList.Count == 0)
            //{
            //    GameObject friendReqEmptyItemGameObject = Instantiate(EmptyFriendReqListPrefab, FriendReqContentParent);
            //    FriendReqItemList.Add(friendReqEmptyItemGameObject);
            //}





        }
        public void RefreshFriendRequest()
        {

            Debug.Log("request count1: " + FriendReqItemList.Count);


            foreach (var friends in FriendReqItemList)
            {

                Destroy(friends.gameObject);

            }

            FriendReqItemList.Clear();

            LoadFriendRequests();
            //if (FriendReqItemList.Count == 0)
            //{
            //    GameObject friendReqEmptyItemGameObject = Instantiate(EmptyFriendReqListPrefab, FriendReqContentParent);
            //    FriendReqItemList.Add(friendReqEmptyItemGameObject);
            //}

        }
        public void LoadFriendRequests()
        {

            Debug.Log(" Here_");

            NoMoreRequestObj.SetActive(false);
            if (!allowFriendReqLoad)
            {
                Debug.Log(" Here_");
                return;
            }

            allowFriendReqLoad = false;
            Debug.Log(" Here_");
            #region Commented Out
            //switch (ReferencesHolder.Provider)
            //{
            //    case "Email":
            //        UserId = PlayerPrefs.GetString(ReferencesHolder.EmailUserId);

            //        Debug.Log("Providerwaa " + ReferencesHolder.Provider);
            //        Debug.Log("Providerwaa ka side effect " + UserId);
            //        break;
            //    case "Guest":
            //        UserId = PlayerPrefs.GetString(ReferencesHolder.GuestUserId);
            //        break;
            //    case "Facebook":
            //        UserId = PlayerPrefs.GetString(ReferencesHolder.FBUserId);
            //        break;
            //    case "Google":
            //        UserId = ReferencesHolder.newUserId;
            //        break;
            //}
            #endregion


            if (ReferencesHolder.lastLoadedRequestDocList.Count == 0)
            {
                Debug.Log(" Here_");
                Query collRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_FriendReqRecieve_Collec).Limit(DataLoadLimit);
                collRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    Debug.Log(" Here_");
                    QuerySnapshot snapshot = task.Result;

                    //GameObject friendPrevItemGameObject = Instantiate(FriendListPreviousPrefab, FriendsContentParent);

                    //var cont = friendPrevItemGameObject.GetComponent<FriendsItemUIController>();

                    //cont.onPreviousButtonClickEvent = OnLoadPrevFriends;

                    //FriendItemList.Add(friendPrevItemGameObject);

                    foreach (DocumentSnapshot document in snapshot.Documents)
                    {
                        Debug.Log(" Here_");
                        if (document.Exists)
                        {
                            if (RequestAvailable == false)
                            {
                                RequestAvailable = true;
                            }
                        }
                        Debug.Log(document.Id);
                        Debug.Log(" Here_");

                        ReferencesHolder.lastLoadedRequestDoc = document;
                        GameObject friendReqItemGameObject = Instantiate(FriendReqItemPrefab, FriendReqContentParent);
                        FriendReqItemUIController friendReqCont = friendReqItemGameObject.GetComponent<FriendReqItemUIController>();
                        friendReqCont.GetFriendsReqInfo(document.Id, ref db);
                        friendReqCont.onAcceptRequestClicked = OnClick_AcceptRequest;
                        friendReqCont.onCancelRequestClicked = OnClick_CancelRequest;
                        FriendReqItemList.Add(friendReqItemGameObject);







                    }

                    Debug.Log("Read all data from the users collection.");
                    if (RequestAvailable == false || FriendReqItemList.Count == 0)
                    {
                        Debug.Log("Friend list itemsxx " + FriendReqItemList.Count);
                        GameObject friendReqEmptyItemGameObject = Instantiate(EmptyFriendReqListPrefab, FriendReqContentParent);
                        FriendReqItemList.Add(friendReqEmptyItemGameObject);

                    }
                    else
                    {
                        if (FriendReqItemList.Count < DataLoadLimit)
                        {
                        }
                        else
                        {
                            GameObject friendReqItemItemGameObject = Instantiate(RequestNextPrefab, FriendReqContentParent);

                            var cont2 = friendReqItemItemGameObject.GetComponent<FriendReqItemUIController>();

                            cont2.ReqonNextButtonClickEvent = OnLoadNextRequests;

                            FriendReqItemList.Add(friendReqItemItemGameObject);
                        }



                    }

                    allowFriendReqLoad = true;
                    FriendReqScroller.GetComponent<UIRefreshControl>().EndRefreshing();
                });








            }
            else
            {

                Debug.Log(ReferencesHolder.lastLoadedRequestDocList.Count);
                if (true)
                {

                    Query collRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_FriendReqRecieve_Collec).StartAfter(ReferencesHolder.lastLoadedRequestDocList.Last()).Limit(DataLoadLimit);
                    collRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                    {

                        QuerySnapshot snapshot = task.Result;

                        GameObject friendReqPrevItemGameObject = Instantiate(RequestPreviousPrefab, FriendReqContentParent);

                        var cont = friendReqPrevItemGameObject.GetComponent<FriendReqItemUIController>();

                        cont.ReqonPreviousButtonClickEvent = OnLoadPrevRequest;

                        FriendReqItemList.Add(friendReqPrevItemGameObject);

                        foreach (DocumentSnapshot document in snapshot.Documents)
                        {
                            if (document.Exists)
                            {
                                if (RequestAvailable == false)
                                {
                                    RequestAvailable = true;
                                }
                            }

                            Debug.Log(document.Id);

                            ReferencesHolder.lastLoadedRequestDoc = document;
                            GameObject friendReqItemGameObject = Instantiate(FriendReqItemPrefab, FriendReqContentParent);
                            FriendReqItemUIController friendReqCont = friendReqItemGameObject.GetComponent<FriendReqItemUIController>();

                            friendReqCont.GetFriendsReqInfo(document.Id, ref db);
                            friendReqCont.onAcceptRequestClicked = OnClick_AcceptRequest;
                            friendReqCont.onCancelRequestClicked = OnClick_CancelRequest;
                            FriendReqItemList.Add(friendReqItemGameObject);







                        }

                        Debug.Log("Read all data from the users collection.");
                        if (RequestAvailable == false || FriendReqItemList.Count == 0)
                        {
                            Debug.Log("Friend list itemsxx " + FriendReqItemList.Count);
                            GameObject friendReqEmptyItemGameObject = Instantiate(EmptyFriendReqListPrefab, FriendReqContentParent);
                            FriendReqItemList.Add(friendReqEmptyItemGameObject);

                        }
                        else
                        {
                            if (FriendReqItemList.Count < DataLoadLimit)
                            {
                            }
                            else
                            {
                                GameObject friendReqItemItemGameObject = Instantiate(RequestNextPrefab, FriendReqContentParent);

                                var cont2 = friendReqItemItemGameObject.GetComponent<FriendReqItemUIController>();

                                cont2.ReqonNextButtonClickEvent = OnLoadNextFriends;


                                FriendReqItemList.Add(friendReqItemItemGameObject);
                            }





                        }
                        Debug.Log("this isssssss req " + FriendReqItemList.Count);
                        if (FriendReqItemList.Count == 2)
                        {
                            Destroy(FriendReqItemList.Last());
                            NoMoreRequestObj.SetActive(true);
                        }
                        allowFriendReqLoad = true;
                        FriendReqScroller.GetComponent<UIRefreshControl>().EndRefreshing();

                    });
                }


                //allowFriendLoad = true;
                //FriendListScroller.GetComponent<UIRefreshControl>().EndRefreshing();



            }


        }
        public void OnClick_InviteFriend(string friendId)
        {
            bool allow = true;

            foreach (string id in ReferencesHolder.AllInvitesList)
            {
                if (id == friendId)
                {
                    allow = false;
                }
            }
            if (allow == true)
            {
                Debug.Log("invitation added in List : " + friendId);
                ReferencesHolder.AllInvitesList.Add(friendId);
            }

        }
        public void OnClick_ViewFriend(string friendId)
        {
            MainUIManager.Instance.SetLoaderState(true);

            Debug.Log("thats the friend's id: " + friendId);

            db.Collection(ReferencesHolder.FS_users_Collec).Document(friendId)
                .Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc)
                .GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        Debug.Log("could not view friends");
                    }
                    else
                    {

                        PublicInfoDB Info = task.Result.ConvertTo<PublicInfoDB>();
                        ReferencesHolder.FriendStatsName = Info.UserName;
                        ReferencesHolder.FriendSpriteUrl = Info.PictureURL;
                        ReferencesHolder.FriendId = Info.UserId;
                        Debug.Log("yay value set hoe friend name ki " + ReferencesHolder.FriendStatsName);

                        db.Collection(ReferencesHolder.FS_users_Collec).Document(friendId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_Stats_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
                        {
                            if (task.IsCanceled || task.IsFaulted)
                            {

                            }
                            else
                            {

                                StatsDB stats = task.Result.ConvertTo<StatsDB>();
                                ReferencesHolder.playerStats = stats;
                                Debug.Log("yay value set hoe friend name ki " + ReferencesHolder.FriendStatsName);
                            }
                        }).ContinueWithOnMainThread(tast =>
                        {

                            MainUIManager.Instance.SetLoaderState(false);
                            MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
                            friendStatsPanel.SetActive(true);
                            friendsStatUImanagerInstance.GetBarGraphStats();
                            friendsStatUImanagerInstance.FriendUserNameTxt.text = Info.UserName;
                            friendsStatUImanagerInstance.FriendCoinTxt.text = Info.Coins.ToString();
                            //  StartCoroutine(friendsStatUImanagerInstance.FriendsPlayedGraph.GetComponent<PieChart>().DrawPieCharts());
                            if (Info.AvatarUsed == true)
                            {
                                Sprite sp = MainUIManager.Instance.avatarAtlus.GetSprite(Info.AvatarID);
                                friendsStatUImanagerInstance.FriendImg.sprite = sp;

                            }
                            else if (Info.AvatarUsed == false)
                            {
                                StartCoroutine(friendsStatUImanagerInstance.LoadImage(Info.PictureURL));
                            }

                            //friendStatsPanel.GetComponent<FriendStatsUIManager>().enabled = true;
                            //friendStatsPanel.GetComponent<FriendStatsUIManager>().GetBarGraphStats();
                            //myTimer obj = Camera.main.GetComponent<myTimer>();
                            //obj.StartCoroutine(obj.Counter());
                            //  StartCoroutine(friendStatsPanel.GetComponent<FriendStatsUIManager>().FriendsPlayedGraph.GetComponent<PieChart>().DrawPieCharts());
                            //   StartCoroutine(friendStatsPanel.GetComponent<FriendStatsUIManager>().FriendsWinGraph.GetComponent<PieChart>().DrawPieCharts());

                        });

                    }
                });//.ContinueWithOnMainThread(task =>
                   //{

            //    friendStatsPanel.SetActive(true);
            //    friendStatsPanel.GetComponent<FriendStatsUIManager>().enabled = true;
            //    friendStatsPanel.GetComponent<FriendStatsUIManager>().GetBarGraphStats();
            //    //myTimer obj = Camera.main.GetComponent<myTimer>();
            //    //obj.StartCoroutine(obj.Counter());
            //    StartCoroutine( friendStatsPanel.GetComponent<FriendStatsUIManager>().FriendsPlayedGraph.GetComponent<PieChart>().DrawPieCharts());
            //     StartCoroutine(friendStatsPanel.GetComponent<FriendStatsUIManager>().FriendsWinGraph.GetComponent<PieChart>().DrawPieCharts());
            //    //db.Collection(ReferencesHolder.FS_users_Collec).Document(UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_Stats_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            //    //{
            //    //    if (task.IsFaulted || task.IsCanceled)
            //    //    {
            //    //        /*Loader.SetActive(false)*/

            //    //    }
            //    //    if (task.IsCompleted)
            //    //    {

            //    //        StatsDB stats = task.Result.ConvertTo<StatsDB>();
            //    //        ReferencesHolder.playerStats = stats;


            //    //        if (gameObject.tag == "FriendsGameWonChart")
            //    //        {
            //    //            friendStatsPanel.GetComponent<FriendStatsUIManager>().FriendsPlayedGraph.GetComponent<PieChart>().m_Data[0].Value = stats.ClassicBorrayWin;
            //    //            friendStatsPanel.GetComponent<FriendStatsUIManager>().FriendsPlayedGraph.GetComponent<PieChart>().m_Data[1].Value = stats.SpeedBetWin;
            //    //            friendStatsPanel.GetComponent<FriendStatsUIManager>().FriendsPlayedGraph.GetComponent<PieChart>().m_Data[2].Value = stats.FullHouseWin;
            //    //            friendStatsPanel.GetComponent<FriendStatsUIManager>().FriendsPlayedGraph.GetComponent<PieChart>().m_Data[3].Value = stats.TournamentWin;

            //    //        }
            //    //        if (gameObject.tag == "FriendsGamePlayedChart")
            //    //        {
            //    //            friendStatsPanel.GetComponent<FriendStatsUIManager>().FriendsPlayedGraph.GetComponent<PieChart>().m_Data[0].Value = stats.ClassicBorrayWin + stats.ClassicBorrayLoss;
            //    //            friendStatsPanel.GetComponent<FriendStatsUIManager>().FriendsPlayedGraph.GetComponent<PieChart>().m_Data[1].Value = stats.SpeedBetWin + stats.SpeedBetLoss;
            //    //            friendStatsPanel.GetComponent<FriendStatsUIManager>().FriendsPlayedGraph.GetComponent<PieChart>().m_Data[2].Value = stats.FullHouseWin + stats.FullHouseLoss;
            //    //            friendStatsPanel.GetComponent<FriendStatsUIManager>().FriendsPlayedGraph.GetComponent<PieChart>().m_Data[3].Value = stats.TournamentWin + stats.TournamentLoss;



            //    //        }

            //    //    }






            //    //});
            //});
        }
        public void LoadFriendPie(string friendId)
        {

        }
        public void OnClick_AcceptRequest(string friendId)
        {

            db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_FriendReqRecieve_Collec).Document(friendId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                //Debug.Log("agya skin1");
                if (task.IsCanceled || task.IsFaulted)
                {
                    // AuthUI.Loader.SetActive(false);

                    LogErrorUIHandler.instance.OpenErrorPanel("Some thing went wrong check your internet connection");

                }
                if (task.IsCompleted)
                {
                    if (task.Result.Exists)
                    {
                        Debug.Log("accepting request: " + friendId);
                        DocumentReference DocRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(auth.CurrentUser.UserId).Collection(ReferencesHolder.FS_FriendReqRecieve_Collec).Document(friendId);
                        DocRef.DeleteAsync().ContinueWithOnMainThread(task =>
                        {
                            if (task.IsCanceled || task.IsFaulted)
                            {
                                Debug.Log("could not accept friend request");
                                RefreshFriendRequest();
                            }
                            else
                            {

                                DocumentReference docRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(auth.CurrentUser.UserId).Collection(ReferencesHolder.FS_FriendsData_Collec).Document(friendId);
                                var FriendData = new FriendsDataDB
                                {
                                    FriendId = friendId


                                };
                                docRef.SetAsync(FriendData).ContinueWithOnMainThread(task =>
                                {

                                    Debug.Log("friend added to friend list of friend request reciever's side");




                                });








                            }



                        }).ContinueWithOnMainThread(task =>
                        {
                            DocumentReference docRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(friendId).Collection(ReferencesHolder.FS_FriendsData_Collec).Document(auth.CurrentUser.UserId);
                            var FriendData = new FriendsDataDB
                            {
                                FriendId = friendId


                            };
                            docRef.SetAsync(FriendData).ContinueWithOnMainThread(task =>
                            {

                                Debug.Log("friend added to friend list of friend request friend's side");

                            });

                        });

                    }
                    else
                    {
                        LogErrorUIHandler.instance.OpenErrorPanel("Sender Rejects the request");
                        RefreshFriendRequest();
                    }





                }
            });
}

        public void OnClick_CancelRequest(string friendId)
        {
            bool exist = false;
            Query collecRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(friendId).Collection(ReferencesHolder.FS_FriendReqSent_Collec).WhereEqualTo("RequestStatus", true);

            collecRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {

                QuerySnapshot snapshot = task.Result;
               


                foreach (DocumentSnapshot document in snapshot.Documents)
                {

                    Debug.Log(document.Id);


                    exist = true;
                    //GameObject FindfriendItemGameObject = Instantiate(ItemPrefab, ContentParent);
                    //FriendsItemUIController friendCont = FindfriendItemGameObject.GetComponent<FriendsItemUIController>();



                    //friendCont.GetFriendsInfo(document.Id, ref db);
                    ////  friendCont.GetFriendsFriendData(document.Id, ReferencesHolder.playerPublicInfo.UserId);
                    //friendCont.onViewProfileClickEvent = OnClick_ViewFriend;
                    //friendCont.onAddFriendClickEvent = SendReqToFriend
                    ///* SendFriendRequestToPlayer(playerInfo.UserId, () => { playerOptionsUIController.SetAddFriendButton(false, "Already Sent"); }, () => { playerOptionsUIController.SetAddFriendButton(true); })*/
                    //;
                    //FindFriendItemList.Add(FindfriendItemGameObject);
                    //  ReferencesHolder.lastLoadedFindFriendDoc = document;










                }
                if (exist == false)
                {
                    LogErrorUIHandler.instance.OpenErrorPanel("Sender canceled the request");
                    //Debug.Log("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<in>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                    //GameObject FindfriendEmptyItemGameObject = Instantiate(EmptyPrefab, ContentParent);
                    //FindFriendItemList.Add(FindfriendEmptyItemGameObject);



                }
                else
                {
                    Debug.Log("canceling request: " + friendId);
                    db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_FriendReqRecieve_Collec).Document(friendId).DeleteAsync().ContinueWithOnMainThread(task =>
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
                    db.Collection(ReferencesHolder.FS_users_Collec).Document(friendId).Collection(ReferencesHolder.FS_FriendReqSent_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).DeleteAsync().ContinueWithOnMainThread(task =>
                    {

                        if (task.IsCanceled || task.IsFaulted)
                        {
                            Debug.Log("could not delete request sent from senders document");
                        }
                        else
                        {


                            Debug.Log(" delete request sent from senders document");

                        }



                    });
                }


            });


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

                GameObject friendItemGameObject = Instantiate(FriendsItemPrefab, FriendsContentParent);
                FriendsItemUIController friendCont = friendItemGameObject.GetComponent<FriendsItemUIController>();

                Debug.Log("avatar use nae kia ha");
                friendCont.SetFriendImage(spriteImage);

                friendCont.SetFriendsItem(publicinfo);
                friendCont.onViewProfileClickEvent = OnClick_ViewFriend;
                FriendItemList.Add(friendItemGameObject);






            }
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


                GameObject friendReqItemGameObject = Instantiate(FriendReqItemPrefab, FriendReqContentParent);
                FriendReqItemUIController friendReqCont = friendReqItemGameObject.GetComponent<FriendReqItemUIController>();

                Debug.Log("avatar use nae kia ha");
                friendReqCont.SetFriendReqImage(spriteImage);
                friendReqCont.SetFriendReqItem(publicinfo);
                friendReqCont.onAcceptRequestClicked = OnClick_AcceptRequest;
                friendReqCont.onCancelRequestClicked = OnClick_CancelRequest;
                FriendReqItemList.Add(friendReqItemGameObject);





            }

        }


        public void OnLoadNextFriends()
        {
            // update last friuend doucm




            ReferencesHolder.lastLoadedFriendDocList.Add(ReferencesHolder.lastLoadedFriendDoc);


            foreach (var friends in FriendItemList)
            {
                //if (FriendReqItemList.Count > 1)
                //  {
                Destroy(friends.gameObject);
                //  }



            }

            FriendItemList.Clear();



            LoadFriendList(FriendsItemPrefab, FriendListNextPrefab, FriendListPreviousPrefab, EmptyFriendListPrefab, FriendsContentParent);
            if (FriendsContentParent.transform.childCount > 2)
            {

            }
            // Load friends
        }

        public void OnLoadPrevFriends()
        {
            ReferencesHolder.lastLoadedFriendDocList.RemoveAt(ReferencesHolder.lastLoadedFriendDocList.Count - 1);

            Debug.Log(ReferencesHolder.lastLoadedFriendDocList + "<========");
            foreach (var friends in FriendItemList)
            {

                Destroy(friends.gameObject);




            }

            FriendItemList.Clear();


            LoadFriendList(FriendsItemPrefab, FriendListNextPrefab, FriendListPreviousPrefab, EmptyFriendListPrefab, FriendsContentParent);
        }

        public void OnLoadNextFriendsInvite()
        {
            // update last friuend doucm




            ReferencesHolder.lastLoadedFriendInviteDocList.Add(ReferencesHolder.lastLoadedFriendInviteDoc);


            foreach (var friends in InviteItemList)
            {
                //if (FriendReqItemList.Count > 1)
                //  {
                Destroy(friends.gameObject);
                //  }



            }

            InviteItemList.Clear();



            LoadFriendInviteList(InviteItemPrefab, InviteNextPrefab, InvitePreviousPrefab, InviteEmptyPrefab, InviteContentParent);

            // Load friends
        }

        public void OnLoadPrevFriendsInvite()
        {
            ReferencesHolder.lastLoadedFriendInviteDocList.RemoveAt(ReferencesHolder.lastLoadedFriendInviteDocList.Count - 1);

            Debug.Log(ReferencesHolder.lastLoadedFriendInviteDocList + "<========");
            foreach (var friends in InviteItemList)
            {

                Destroy(friends.gameObject);




            }

            InviteItemList.Clear();


            LoadFriendInviteList(InviteItemPrefab, InviteNextPrefab, InvitePreviousPrefab, InviteEmptyPrefab, InviteContentParent);
        }

        public void OnLoadNextRequests()
        {
            ReferencesHolder.lastLoadedRequestDocList.Add(ReferencesHolder.lastLoadedRequestDoc);


            foreach (var friends in FriendReqItemList)
            {
                //if (FriendReqItemList.Count > 1)
                //  {
                Destroy(friends.gameObject);
                //  }



            }

            FriendReqItemList.Clear();



            LoadFriendRequests();
        }
        public void OnLoadPrevRequest()
        {
            ReferencesHolder.lastLoadedRequestDocList.RemoveAt(ReferencesHolder.lastLoadedRequestDocList.Count - 1);

            //  Debug.Log(ReferencesHolder.lastLoadedFriendDocList + "<========");
            foreach (var friends in FriendReqItemList)
            {

                Destroy(friends.gameObject);




            }

            FriendReqItemList.Clear();


            LoadFriendRequests();
        }

        public void Load_Friend(PublicInfoDB publicinfo)
        {



            GameObject friendItemGameObject = Instantiate(FriendsItemPrefab, FriendsContentParent);
            FriendsItemUIController friendCont = friendItemGameObject.GetComponent<FriendsItemUIController>();
            friendCont.SetFriendsItem(publicinfo);
            friendCont.onViewProfileClickEvent = OnClick_ViewFriend;
            FriendItemList.Add(friendItemGameObject);




        }
        public void Load_FriendRequest(PublicInfoDB publicinfo)
        {
            GameObject friendReqItemGameObject = Instantiate(FriendReqItemPrefab, FriendReqContentParent);
            FriendReqItemUIController friendReqCont = friendReqItemGameObject.GetComponent<FriendReqItemUIController>();
            friendReqCont.SetFriendReqItem(publicinfo);
            friendReqCont.onAcceptRequestClicked = OnClick_AcceptRequest;
            friendReqCont.onCancelRequestClicked = OnClick_CancelRequest;
            FriendReqItemList.Add(friendReqItemGameObject);

        }
        #region Find Friend Work
        public void OpenCurrentFindFriendsList()
        {
            SearchFriendButton.interactable = false;
            if (String.IsNullOrEmpty(FindFriendField.text))
            {
                findFriendWarningTxt.text = "Please Write Email of the user to search!";
                findFriendWarningTxt.gameObject.SetActive(true);
                SearchFriendButton.interactable = true;
                return;
            }
            if (FindFriendField.text == ReferencesHolder.playerPublicInfo.Email)
            {

                findFriendWarningTxt.text = "You can not search your own email!";
                findFriendWarningTxt.gameObject.SetActive(true);
                SearchFriendButton.interactable = true;
                return;
            }


            Debug.Log("Open find friend list chla>>>>>>>>");
            OpenCurrentFriendsListAnim();

            friendListBtn.gameObject.GetComponent<Image>().sprite = btnSelectedSprite;
            friendRequestBtn.gameObject.GetComponent<Image>().sprite = BtnDefaultSprite;
            Debug.Log("yaaaaaaaay ha value...:" + FriendListcounter);
            Debug.Log("yaaay actual .... :" + FriendItemList.Count);

            foreach (var friends in FindFriendItemList)
            {
                Destroy(friends.gameObject);

            }

            FindFriendItemList.Clear();
            LoadFindFriendList(FindFriendsItemPrefab, FindFriendListNextPrefab, FindFriendListPreviousPrefab, FindEmptyFriendListPrefab, FindFriendsContentParent);


        }

        public void LoadFindFriendList(GameObject ItemPrefab, GameObject NextPrefab, GameObject PreviousPrefab, GameObject EmptyPrefab, Transform ContentParent)
        {

            EmptyPrefab.gameObject.SetActive(false);
            Query collecRef = db.Collection(ReferencesHolder.FS_users_Collec).WhereEqualTo("Email", FindFriendField.text);

            collecRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {

                QuerySnapshot snapshot = task.Result;



                foreach (DocumentSnapshot document in snapshot.Documents)
                {

                    Debug.Log(document.Id);



                    GameObject FindfriendItemGameObject = Instantiate(ItemPrefab, ContentParent);
                    FriendsItemUIController friendCont = FindfriendItemGameObject.GetComponent<FriendsItemUIController>();



                    friendCont.GetFriendsInfo(document.Id, ref db);
                    //  friendCont.GetFriendsFriendData(document.Id, ReferencesHolder.playerPublicInfo.UserId);
                    friendCont.onViewProfileClickEvent = OnClick_ViewFriend;
                    friendCont.onAddFriendClickEvent = SendReqToFriend
                    /* SendFriendRequestToPlayer(playerInfo.UserId, () => { playerOptionsUIController.SetAddFriendButton(false, "Already Sent"); }, () => { playerOptionsUIController.SetAddFriendButton(true); })*/
                    ;
                    FindFriendItemList.Add(FindfriendItemGameObject);
                    //  ReferencesHolder.lastLoadedFindFriendDoc = document;










                }
                if (FindFriendItemList.Count <= 0)
                {
                    //Debug.Log("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<in>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                    //GameObject FindfriendEmptyItemGameObject = Instantiate(EmptyPrefab, ContentParent);
                    //FindFriendItemList.Add(FindfriendEmptyItemGameObject);

                    EmptyPrefab.gameObject.SetActive(true);

                }


            }).ContinueWithOnMainThread(task =>
            {
                SearchFriendButton.interactable = true;
            });











        }


        public void SendReqToFriend(PublicInfoDB friendId)
        {

            SendFriendReq_FB(friendId.UserId, delegate { Debug.Log("Already exist"); },
            delegate { Debug.Log("Send"); },
            delegate { Debug.Log("Error"); });
        }

        public void SendFriendReq_FB(string toUserID, System.Action CallBackSent, System.Action CallBackRecieve, Action Fallback)
        {
            Debug.Log("Sending Req");


            db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId)
            .Collection(ReferencesHolder.FS_FriendReqSent_Collec).Document(toUserID).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result.Exists)
                {
                    CallBackSent?.Invoke();
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



                    db.Collection(ReferencesHolder.FS_users_Collec).Document(fromUserID)
                        .Collection(ReferencesHolder.FS_FriendReqSent_Collec).Document(toUserID)
                        .SetAsync(FriendReqSentData).ContinueWithOnMainThread(tast =>
                        {
                            Debug.Log("RequestSent");

                            CallBackSent?.Invoke();

                        });

                    db.Collection(ReferencesHolder.FS_users_Collec).Document(toUserID)
                        .Collection(ReferencesHolder.FS_FriendReqRecieve_Collec).Document(fromUserID)
                        .SetAsync(FriendReqSentData).ContinueWithOnMainThread(tast =>
                        {
                            Debug.Log("RequestReceived");

                            CallBackRecieve?.Invoke();

                        });


                }
            });
        }
        public void AddFriendBackMethod()
        {
            MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
            MainUIManager.Instance.HomeUI.HomePanel.SetActive(true);
            FindFriendField.text = "";
            ClearFindFriendsWarning();

            FindEmptyFriendListPrefab.gameObject.SetActive(false);

            foreach (var friends in FindFriendItemList)
            {
                Destroy(friends.gameObject);

            }

        }
        public void ClearFindFriendsWarning()
        {
            findFriendWarningTxt.text = "";
            findFriendWarningTxt.gameObject.SetActive(false);
        }

        #endregion
        void BackButtonMethod()
        {
            MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
            MainUIManager.Instance.HomeUI.HomePanel.SetActive(true);

        }
    }

}



