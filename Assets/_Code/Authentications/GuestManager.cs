
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Storage;
using Firebase.Extensions;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Firebase.Messaging;

namespace Booray.Auth
{
    public class GuestManager : MonoBehaviour
    {
        FirebaseFirestore db;
        FirebaseAuth auth;

        [SerializeField]
        List<string> defaultUnlockedSkinIDs;


        [SerializeField] GameObject Mainscreen;
        [SerializeField] Button GuestLoginButton;
        [SerializeField] GameObject Loader;
        [SerializeField] GameObject MainCanvas;
        private string defaultAvatarID = "panda";
        private bool GSecurity=false;
        public void Awake()
        {
            //auth = FirebaseAuth.DefaultInstance;
            //db = FirebaseFirestore.DefaultInstance;

        }
        public void Start()
        {
            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseFirestore.DefaultInstance;
        //    FirebaseMessaging.TokenReceived += TokenReceived;
            Firebase.Messaging.FirebaseMessaging.SubscribeAsync("/topics/Tournament");
            GuestLoginButton.onClick.AddListener(delegate { GuestLogin(); SFXHandler.instance.PlayBtnClickSFX(); });
            if (PlayerPrefs.GetInt(ReferencesHolder.GuestSignedUp) == 1&& PlayerPrefs.GetString(ReferencesHolder.GuestUserId) !="")
            {
                GSecurity = true;
                GetHomeData();

            }

        }
        //private void TokenReceived(object sender, TokenReceivedEventArgs e)
        //{
        //    Debug.Log(">>>>>>>>>>>>>Token Received" + e.Token);
        //    ReferencesHolder.RecievedDeviceToken = e.Token;
        //    PlayerPrefs.SetString("DeviceToken", e.Token);
        //    PlayerPrefs.Save();
        //  // FirebaseMessaging.SubscribeAsync("/topics/Tournament");
           
        //}
        public void GuestLogin()
        {
           

            Loader.SetActive(true);
            auth.SignInAnonymouslyAsync().ContinueWithOnMainThread((System.Action<System.Threading.Tasks.Task<FirebaseUser>>)(task => {
                if (task.IsCanceled)
                {
                    Debug.Log("SignInAnonymouslyAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.Log("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    return;
                }
                if (task.IsCompleted)
                {
                    ReferencesHolder.Provider = "Guest";

                    Firebase.Auth.FirebaseUser newUser = task.Result;
                    Debug.LogFormat("User signed in successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
                    ReferencesHolder.newUserId = newUser.UserId;
                    DocumentReference docRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(newUser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_Stats_Doc);
                    var stats = new StatsDB
                    {
                        ClassicBorrayWin = 0,
                        ClassicBorrayLoss = 0,
                        SpeedBetWin = 0,
                        SpeedBetLoss = 0,
                        FullHouseWin = 0,
                        FullHouseLoss = 0,
                        TournamentWin = 0,
                        TournamentLoss = 0

                    };
                    docRef.SetAsync(stats).ContinueWithOnMainThread(task => {
                        Debug.Log("Guest user info has been added");


                        PlayerPrefs.SetString(ReferencesHolder.GuestUserId, newUser.UserId);
                        PlayerPrefs.SetInt(ReferencesHolder.GuestSignedUp, 1);
                        PlayerPrefs.Save();
                    });
                    DocumentReference docRef2 = db.Collection(ReferencesHolder.FS_users_Collec).Document(newUser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);

                    db.Collection(ReferencesHolder.FS_users_Collec).Document(newUser.UserId).SetAsync(new Dictionary<string, object> { { "token", PlayerPrefs.GetString("DeviceToken") } });
                    /* Unmerged change from project 'Assembly-CSharp.Player'
                    Before:
                                        var publicinfo = new PublicInfo
                    After:
                                        var publicinfo = new global::PublicInfo
                    */
                    var publicinfo = new PublicInfoDB
                    {
                        UserId = newUser.UserId,
                        UserName = "Guest" + newUser.UserId.Substring(22),
                        Coins = 1000,
                        AvatarUsed = true,
                        AvatarID = defaultAvatarID,
                        DeviceToken = PlayerPrefs.GetString("DeviceToken"),
                        IsVipMember = false

                    };
                    docRef2.SetAsync((object)publicinfo).ContinueWithOnMainThread(task =>
                    {
                        DocumentReference docRef3 = db.Collection(ReferencesHolder.FS_users_Collec).Document(newUser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_DeckSkins_Doc);
                        DeckSkinDB deckSkinData = new DeckSkinDB()
                        {
                            Skins = defaultUnlockedSkinIDs.ToArray()
                        };

                        docRef3.SetAsync(deckSkinData);
                        Debug.Log("new user public Info added");
                        //PlayerPrefs.SetString("Email", EmailUI.SignUpUserNameField.text);
                        PlayerPrefs.Save();
                        GetHomeData();

                    });
                   

                }

            }));
           
        }

        public void GetHomeData()
        {
            ReferencesHolder.Provider = "Guest";
            Loader.SetActive(true);
            Debug.Log(PlayerPrefs.GetString(ReferencesHolder.GuestUserId));
            ReferencesHolder.newUserId = PlayerPrefs.GetString(ReferencesHolder.GuestUserId);
            Debug.Log("Guest home data chla ");


            ReferencesHolder.Provider = "Guest";

            db.Collection(ReferencesHolder.FS_users_Collec).Document(PlayerPrefs.GetString(ReferencesHolder.GuestUserId)).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_DeckSkins_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                Debug.Log("agya skin1");
                if (task.IsCanceled || task.IsFaulted)
                {
                    Loader.SetActive(false);
                    MainCanvas.SetActive(true);
                 

                }
                if (task.IsCompleted)
                {
                    if (task.Result.Exists)
                    {
                        Debug.Log("agya skin2");
                        DeckSkinDB Skins = task.Result.ConvertTo<DeckSkinDB>();
                        // Debug.Log(Skins.Skins);
                        Debug.Log("agya skin3");
                        ReferencesHolder.AvailableSkins = Skins;
                    }
                    else
                    {
                        Debug.Log("Wrong path for guest deck skin");
                        Loader.SetActive(false);
                        MainCanvas.SetActive(true);
                        PlayerPrefs.SetString(ReferencesHolder.GuestUserId, "");
                        PlayerPrefs.Save();

                    }


                }

            });
            db.Collection(ReferencesHolder.FS_users_Collec).Document(PlayerPrefs.GetString(ReferencesHolder.GuestUserId)).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Loader.SetActive(false);
                   
                }
                if (task.IsCompleted)
                {
                    if (task.Result.Exists)
                    {
                        PublicInfoDB info = task.Result.ConvertTo<PublicInfoDB>();
                        //PublicInfoDB Info = task.Result.ConvertTo<PublicInfoDB>();

                        ReferencesHolder.playerPublicInfo = info;
                        //Debug.Log("yay ha naam" + Info.UserName);
                        //ReferencesHolder.userName = Info.UserName;
                        //ReferencesHolder.Coins = Info.Coins.ToString();
                        //ReferencesHolder.AvatarUsed = true;
                        //ReferencesHolder.AvatarID = Info.AvatarID;
                        //ReferencesHolder.userID = Info.UserId;
                         
                        OpenHomeScreen();



                    }
                    else
                    {
                        Loader.SetActive(false);
                        MainCanvas.SetActive(true);
                        PlayerPrefs.SetString(ReferencesHolder.GuestUserId, "");
                        PlayerPrefs.Save();
                    }
                   
                }
                
               




            });



        }
        private void OpenHomeScreen()
        {
            Debug.Log("open home screen method");

         SceneManager.LoadScene(ReferencesHolder.mainMenuSceneIndex);
           // SceneManager.LoadScene("Example (Horizontal & Vertical)");

        }


        

    }
}

