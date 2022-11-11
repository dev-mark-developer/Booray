using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Google;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Storage;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Firebase.Messaging;
using TMPro;
using Firebase;

using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Photon.Pun;
using Photon.Realtime;

namespace Booray.Auth
{
    public class AuthenticationsManager : MonoBehaviour
    {
        #region variables declaration 
        [SerializeField] List<string> defaultUnlockedSkinIDs;

        FirebaseAuth auth;
        FirebaseFirestore db;
        FirebaseStorage storage;
        StorageReference storageReference;
        //public GameObject Loader;
        public GameObject MainCanvas;
        private string pathOfTextures;
        string DpURL;
        private bool Signedup = false;
        public string accesstoken;

        public TextMeshProUGUI infoText;
        public string webClientId = "949171170208-pevs6ts14fmled463qjha2g6hfobiang.apps.googleusercontent.com";



        string GoogleToken;
        private GoogleSignInConfiguration configuration;
        private string defaultAvatarID = "panda";
        private bool GSecurity = false;





        public const string MatchEmailPattern =
          @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
          + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
          + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
          + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        public int RandomCode;

        private bool AvatarUsedValue = true;
        private bool Security = false;
        bool EmailAlreadyExist = false;

        public string LoginUserId;



        [SerializeField] AuthUIManager AuthUI;
        #endregion

        private void Awake()
        {


            pathOfTextures = Application.persistentDataPath + "/WebTexturesFolder";

            #region Google Initialization
            configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
            #endregion

            #region Facebook Intialization 
            if (!FB.IsInitialized)
            {
                FB.Init(InitCallBack, OnHideUnity);
            }
            else
            {
                FB.ActivateApp();
            }

            //Debug.Log("yay FBauto value:" + PlayerPrefs.GetInt(ReferencesHolder.FBSignedUp));
            //Debug.Log("yay current FBuserid ha" + PlayerPrefs.GetString(ReferencesHolder.FBUserId));
            #endregion
        }

        private void Start()
        {
            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseFirestore.DefaultInstance;
            storage = FirebaseStorage.DefaultInstance;
            storageReference = storage.GetReferenceFromUrl("gs://ultimate-booray.appspot.com");
            FirebaseMessaging.SubscribeAsync("/topics/Tournament");

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            AuthUI.FacbookButton.onClick.AddListener(delegate { Facebook_Login(); SFXHandler.instance.PlayBtnClickSFX(); });
            AuthUI.GoogleButton.onClick.AddListener(delegate { SignInWithGoogle(); SFXHandler.instance.PlayBtnClickSFX(); });
            AuthUI.GuestLoginButton.onClick.AddListener(delegate { GuestLogin(); SFXHandler.instance.PlayBtnClickSFX(); });

            AuthUI.LoginButton.onClick.AddListener(delegate { LoginValidation(); SFXHandler.instance.PlayBtnClickSFX(); });
            AuthUI.RegisterButton.onClick.AddListener(delegate { SendVerifyEmail(); AuthUI.ResetVerifyPanel(); SFXHandler.instance.PlayBtnClickSFX(); });
            AuthUI.MainLoginButton.onClick.AddListener(delegate { MainLoginButtonMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
            AuthUI.VerifyEmailButton.onClick.AddListener(delegate { SignUp(); SFXHandler.instance.PlayBtnClickSFX(); });
            AuthUI.ForgetPasswordChangeButton.onClick.AddListener(delegate { ForgetPasswordValidation(); SFXHandler.instance.PlayBtnClickSFX(); });
            AuthUI.ResendPinButton.onClick.AddListener(delegate { ReVerifyEmail(); SFXHandler.instance.PlayBtnClickSFX(); });
            AuthUI.OpenGalleryButton.onClick.AddListener(delegate { OpenGalleryButtonMethod(); SFXHandler.instance.PlayBtnClickSFX(); });

            var autoLogin = PlayerPrefs.GetInt(ReferencesHolder.EmailSignedUp);

            Debug.Log($" Auto Login ->  {autoLogin} -> {PlayerPrefs.GetString(ReferencesHolder.EmailUserId, "-")} ");
            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                if (PlayerPrefs.GetInt(ReferencesHolder.FBSignedUp) == 1)
                {
                    ReferencesHolder.Provider = "Facebook";
                    Facebook_Login();
                }
                if (PlayerPrefs.GetInt(ReferencesHolder.GoogleSignedUp) == 1)
                {
                    //Debug.Log("google ka auto chla");

                    ReferencesHolder.Provider = "Google";
                    SignInWithGoogle();



                }

                if (PlayerPrefs.GetInt(ReferencesHolder.GuestSignedUp) == 1 && PlayerPrefs.GetString(ReferencesHolder.GuestUserId) != "")
                {
                    GSecurity = true;
                    ReferencesHolder.Provider = "Guest";
                    GetHomeData(ReferencesHolder.GuestUserId);

                }
                if (autoLogin == 1)
                {
                    Security = true;
                    ReferencesHolder.Provider = "Email";
                    //ReferencesHolder.Provider = "Guest";
                    //  GetHomeData(ReferencesHolder.EmailUserId);
                    GetHomeData(ReferencesHolder.EmailUserId);

                }
            }
            else
            {
                AuthUI.HideLoader();
            }
           
        }

       
        #region Facebook Default Function
        private void InitCallBack()
        {
            if (!FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                Debug.Log("Failed to initialize");
            }
        }
        private void OnHideUnity(bool isgameshown)
        {
            if (!isgameshown)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        #endregion

        #region Google Default Function
        public void SignInWithGoogle()
        {
           LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                OnSignIn();
            }
            else
            {
                AuthUI.HideLoader();
            }
             }
        public void SignOutFromGoogle() { OnSignOut(); }
        public void AddToInformation(string str) { infoText.text += "\n" + str; }
        private void OnSignIn()
        {


            if (ReferencesHolder.googleAlreadyConfigured == false)
            {
                GoogleSignIn.Configuration = configuration;

                GoogleSignIn.Configuration.UseGameSignIn = false;
                GoogleSignIn.Configuration.RequestIdToken = true;

                ReferencesHolder.googleAlreadyConfigured = true;
            }


            AddToInformation("Calling SignIn");

            GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(OnAuthenticationFinished);
        }

        private void OnSignOut()
        {
            AddToInformation("Calling SignOut");
            GoogleSignIn.DefaultInstance.SignOut();
        }

        public void OnDisconnect()
        {
            AddToInformation("Calling Disconnect");
            GoogleSignIn.DefaultInstance.Disconnect();
        }

        internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
        {
            if (task.IsFaulted)
            {
                using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                        AddToInformation("Got Error: " + error.Status + " " + error.Message);
                    }
                    else
                    {
                        AddToInformation("Got Unexpected Exception?!?" + task.Exception);
                    }
                }
            }
            else if (task.IsCanceled)
            {
                AddToInformation("Canceled");
            }
            else
            {
                AddToInformation("Welcome: " + task.Result.DisplayName + "!");
                AddToInformation("Email = " + task.Result.Email);
                AddToInformation("Google ID Token = " + task.Result.IdToken);
                AddToInformation("Email = " + task.Result.Email);
                GoogleToken = task.Result.IdToken;
                Debug.Log(task.Result.ImageUrl);
                infoText.text = "Welcome: " + task.Result.DisplayName;
                // AuthUI.Loader.SetActive(true);
                AuthUI.ShowLoader();
                StartCoroutine(GoogleLoadImage(task.Result.ImageUrl));
            }
        }
        #endregion

        #region  FaceBook Sign Up Work
        public void Facebook_Login()
        {
            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                var permission = new List<string>() { "public_profile", "email" };
                FB.LogInWithReadPermissions(permission, AuthCallBack);
            }
            else
            {
                AuthUI.HideLoader();
            }
              
        }

        private void AuthCallBack(ILoginResult result)
        {
            if (FB.IsLoggedIn)
            {
                var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

                string[] data;
                string acc;
                string[] some;
#if UNITY_EDITOR
                Debug.Log("this is raw access " + result.RawResult);
                data = result.RawResult.Split(',');
                Debug.Log("this is access" + data[3]);
                acc = data[3];
                some = acc.Split('"');
                Debug.Log("this is access " + some[3]);
                accesstoken = some[3];
#elif UNITY_ANDROID
            Debug.Log("this is raw access "+result.RawResult);
 data = result.RawResult.Split(',');
            Debug.Log("this is access"+data[0]);
             acc = data[0];
             some = acc.Split('"');
            Debug.Log("this is access " + some[3]);


             accesstoken = some[3];
#endif

                FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);

            }
            else
            {
                Debug.Log("User Cancelled login");
            }
        }
        public void authwithfirebase(string accesstoken)
        {

            // AuthUI.Loader.SetActive(true);
            AuthUI.ShowLoader();


            auth = FirebaseAuth.DefaultInstance;
            Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(accesstoken);
            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("singin encountered error" + task.Exception);
                    // AuthUI.Loader.SetActive(false);
                    AuthUI.HideLoader();
                }
                Firebase.Auth.FirebaseUser newuser = task.Result;
                ReferencesHolder.newUserId = newuser.UserId;
                if (task.IsCompleted)
                {
                    Debug.Log("firbase fb auth done");
                    Debug.Log("thats fb new user id " + newuser.UserId);
                    Debug.Log("thats refereceholder fb new user id " + newuser.UserId);

                    db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
                    {
                        if (task.Result.Exists)
                        {
                            Debug.Log("result exist krta ha");




                            PlayerPrefs.SetString(ReferencesHolder.FBUserId, newuser.UserId);
                            PlayerPrefs.SetInt(ReferencesHolder.FBSignedUp, 1);
                            PlayerPrefs.Save();
                            PublicInfoDB Info = task.Result.ConvertTo<PublicInfoDB>();
                            ReferencesHolder.playerPublicInfo = Info;
                            FriendsDataDB Fd = task.Result.ConvertTo<FriendsDataDB>();
                            ReferencesHolder.FriendsInfo = Fd;
                            ReferencesHolder.Provider = "Facebook";
                            Debug.Log("this is photo URL" + Info.PictureURL);
                            bool check = CheckIfTextureExists(ReferencesHolder.playerPublicInfo.PictureName + ".png");
                            Debug.Log("cccccccccccccccchhheeck:" + check);


                            db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).UpdateAsync("token", PlayerPrefs.GetString("DeviceToken"));
                            db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).UpdateAsync("DeviceToken", PlayerPrefs.GetString("DeviceToken"));
                            if (check == true)
                            {
                                Debug.Log("local custom pic of FB user");
                                string path = Path.Combine(pathOfTextures, Info.PictureName + ".png");
                                StartCoroutine(LoadLocalTexture("file://" + path));
                            }
                            else
                            {
                                Debug.Log("firebase custom pic of FB user");
                                StartCoroutine(LoadImage(Info.PictureURL));

                                Debug.Log("awww");
                            }

                        }
                        else if (task.Result.Exists == false)
                        {
                            Debug.Log("Fb user not already present");

                            Texture2D copy = duplicateTexture(ReferencesHolder.playersAvatarTex);
                            byte[] textureBytes = copy.EncodeToPNG();

                            storageReference = storage.RootReference.Child(newuser.UserId + "/uploads/DisplayPic.png");
                            storageReference.PutBytesAsync(textureBytes)

                            .ContinueWithOnMainThread((Task<StorageMetadata> task) =>
                            {
                                if (task.IsFaulted || task.IsCanceled)
                                {
                                    Debug.Log(task.Exception.ToString());

                                }
                                else
                                {
                                    StorageMetadata metadata = task.Result;
                                    string md5Hash = metadata.Md5Hash;

                                    Debug.Log("Finished uploading...");
                                    Debug.Log("md5 hash = " + md5Hash);

                                    var pictureName = newuser.UserId + RemoveSpecialCharacters(md5Hash);
                                    SaveImageOnDisk(copy, pictureName);

                                    storageReference.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
                                    {
                                        if (task.IsCompleted)
                                        {
                                            DpURL = task.Result.ToString();
                                            DocumentReference docRef2 = db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);
                                            var publicinfo = new PublicInfoDB
                                            {
                                                UserId = newuser.UserId,
                                                UserName = newuser.DisplayName,
                                                Coins = 1000,
                                                AvatarUsed = false,
                                                AvatarID = "",
                                                PictureURL = DpURL,
                                                PictureName = pictureName,
                                                DeviceToken = PlayerPrefs.GetString("DeviceToken"),
                                                IsVipMember = false,
                                                Email = newuser.Email


                                            };
                                            docRef2.SetAsync(publicinfo).ContinueWithOnMainThread(task =>
                                            {

                                                Debug.Log("new user public Info added");

                                                PlayerPrefs.SetString(ReferencesHolder.FBUserId, newuser.UserId);
                                                PlayerPrefs.SetInt(ReferencesHolder.FBSignedUp, 1);
                                                PlayerPrefs.Save();

                                                GetHomeData(ReferencesHolder.FBUserId);


                                            });
                                        }
                                    });

                                }
                            });
                            Debug.Log("Image work done");
                            DocumentReference docRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_Stats_Doc);
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
                            docRef.SetAsync(stats).ContinueWithOnMainThread(task =>
                            {
                                Debug.Log("new facebook user info has been added");

                            });


                            DocumentReference docRef3 = db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_DeckSkins_Doc);

                            db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).SetAsync(new Dictionary<string, object> { { "token", PlayerPrefs.GetString("DeviceToken") }, { "Email", newuser.Email } });

                            // db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).SetAsync(new Dictionary<string, object> { { "Email", newuser.Email } });

                            DeckSkinDB deckSkinData = new DeckSkinDB()
                            {
                                Skins = defaultUnlockedSkinIDs.ToArray(),
                                CurrentSkin="000"
                            };

                            docRef3.SetAsync(deckSkinData);

                            ReferencesHolder.Provider = "Facebook";

                        }

                    });

                }
            });
        }
        #endregion

        #region Google SignUp Work
        public void SignInWithGoogleOnFirebase(string idToken)
        {
            Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                AggregateException ex = task.Exception;
                if (ex != null || task.IsCanceled)
                {
                    // AuthUI.Loader.SetActive(false);
                    AuthUI.HideLoader();
                    if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                        AddToInformation("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
                }
                else
                {
                    AddToInformation("Sign In Successful.");


                    Firebase.Auth.FirebaseUser newuser = task.Result;
                    ReferencesHolder.newUserId = newuser.UserId;
                    Debug.Log("firbase google auth done");

                    db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
                    {
                        if (task.Result.Exists)
                        {
                            Debug.Log("Google user result exist krta ha");
                            PlayerPrefs.SetString(ReferencesHolder.GoogleUserId, newuser.UserId);
                            PlayerPrefs.SetInt(ReferencesHolder.GoogleSignedUp, 1);
                            PlayerPrefs.Save();
                            PublicInfoDB Info = task.Result.ConvertTo<PublicInfoDB>();
                            ReferencesHolder.playerPublicInfo = Info;
                            FriendsDataDB Fd = task.Result.ConvertTo<FriendsDataDB>();
                            ReferencesHolder.FriendsInfo = Fd;
                            ReferencesHolder.Provider = "Google";

                            Debug.Log("this is photo URL" + Info.PictureURL);
                            bool check = CheckIfTextureExists(ReferencesHolder.playerPublicInfo.PictureName + ".png");
                            Debug.Log("cccccccccccccccchhheeck:" + check);

                            db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).UpdateAsync("token", PlayerPrefs.GetString("DeviceToken"));
                            db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).UpdateAsync("DeviceToken", PlayerPrefs.GetString("DeviceToken"));
                            db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).UpdateAsync("Email", newuser.Email);
                            db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).UpdateAsync("Email", newuser.Email);

                            if (check == true)
                            {
                                Debug.Log("local custom pic of FB user");
                                string path = Path.Combine(pathOfTextures, Info.PictureName + ".png");
                                StartCoroutine(LoadLocalTexture("file://" + path));
                            }
                            else
                            {
                                Debug.Log("firebase custom pic of FB user");
                                StartCoroutine(LoadImage(Info.PictureURL));

                                Debug.Log("awww");
                            }

                        }
                        else if (task.Result.Exists == false)
                        {
                            Debug.Log("google user not already present");

                            Texture2D copy = duplicateTexture(ReferencesHolder.playersAvatarTex);
                            byte[] textureBytes = copy.EncodeToPNG();

                            storageReference = storage.RootReference.Child(newuser.UserId + "/uploads/DisplayPic.png");
                            storageReference.PutBytesAsync(textureBytes)

                            .ContinueWithOnMainThread((Task<StorageMetadata> task) =>
                            {
                                if (task.IsFaulted || task.IsCanceled)
                                {
                                    Debug.Log(task.Exception.ToString());

                                }
                                else
                                {
                                    StorageMetadata metadata = task.Result;
                                    string md5Hash = metadata.Md5Hash;

                                    Debug.Log("Finished uploading...");
                                    Debug.Log("md5 hash = " + md5Hash);

                                    var pictureName = newuser.UserId + RemoveSpecialCharacters(md5Hash);
                                    SaveImageOnDisk(copy, pictureName);

                                    storageReference.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
                                    {
                                        if (task.IsCompleted)
                                        {
                                            DpURL = task.Result.ToString();
                                            DocumentReference docRef2 = db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);
                                            var publicinfo = new PublicInfoDB
                                            {
                                                UserId = newuser.UserId,
                                                UserName = newuser.DisplayName,
                                                Coins = 1000,
                                                AvatarUsed = false,
                                                AvatarID = "",
                                                PictureURL = DpURL,
                                                PictureName = pictureName,
                                                DeviceToken = PlayerPrefs.GetString("DeviceToken"),
                                                IsVipMember = false,
                                                Email = newuser.Email

                                            };
                                            docRef2.SetAsync(publicinfo).ContinueWithOnMainThread(task =>
                                            {

                                                Debug.Log("new user public Info added");

                                                PlayerPrefs.SetString(ReferencesHolder.GoogleUserId, newuser.UserId);
                                                PlayerPrefs.SetInt(ReferencesHolder.GoogleSignedUp, 1);
                                                PlayerPrefs.Save();

                                                GetHomeData(ReferencesHolder.GoogleUserId);


                                            });
                                        }
                                    });

                                }
                            });
                            Debug.Log("Image work done");
                            DocumentReference docRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_Stats_Doc);
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
                            docRef.SetAsync(stats).ContinueWithOnMainThread(task =>
                            {
                                Debug.Log("new google user info has been added");

                            });



                            DocumentReference docRef3 = db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_DeckSkins_Doc);

                            db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).SetAsync(new Dictionary<string, object> { { "token", PlayerPrefs.GetString("DeviceToken") }, { "Email", newuser.Email } });
                            DeckSkinDB deckSkinData = new DeckSkinDB()
                            {
                                Skins = defaultUnlockedSkinIDs.ToArray(),
                                CurrentSkin = "000"
                            };

                            docRef3.SetAsync(deckSkinData);

                            ReferencesHolder.Provider = "Google";

                        }
                    });
                }
            });
        }

        public void OnSignInSilently()
        {
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;
            AddToInformation("Calling SignIn Silently");

            GoogleSignIn.DefaultInstance.SignInSilently().ContinueWithOnMainThread(OnAuthenticationFinished);
        }

        public void OnGamesSignIn()
        {
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = true;
            GoogleSignIn.Configuration.RequestIdToken = false;

            AddToInformation("Calling Games SignIn");

            GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(OnAuthenticationFinished);
        }
        #endregion

        #region Guest SignUp
        public void GuestLogin()
        {
            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                //  AuthUI.Loader.SetActive(true);
                AuthUI.ShowLoader();
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

                        db.Collection(ReferencesHolder.FS_users_Collec).Document(newUser.UserId).SetAsync(new Dictionary<string, object> { { "token", PlayerPrefs.GetString("DeviceToken") }, { "Email", newUser.Email } });
                        //db.Collection(ReferencesHolder.FS_users_Collec).Document(newUser.UserId).SetAsync(new Dictionary<string, object> { { "Email", newUser.Email} });
                        var publicinfo = new PublicInfoDB
                        {
                            UserId = newUser.UserId,
                            UserName = "Guest" + newUser.UserId.Substring(22),
                            Coins = 1000,
                            AvatarUsed = true,
                            AvatarID = defaultAvatarID,
                            DeviceToken = PlayerPrefs.GetString("DeviceToken"),
                            IsVipMember = false,
                            Email = newUser.Email
                        };
                        docRef2.SetAsync((object)publicinfo).ContinueWithOnMainThread(task =>
                        {
                            DocumentReference docRef3 = db.Collection(ReferencesHolder.FS_users_Collec).Document(newUser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_DeckSkins_Doc);
                            DeckSkinDB deckSkinData = new DeckSkinDB()
                            {
                                Skins = defaultUnlockedSkinIDs.ToArray(),
                                CurrentSkin = "000"
                            };

                            docRef3.SetAsync(deckSkinData);
                            Debug.Log("new user public Info added");

                            PlayerPrefs.Save();
                            // AuthUI.Loader.SetActive(false);
                            AuthUI.HideLoader();
                            GetHomeData(ReferencesHolder.GuestUserId);

                        });


                    }

                }));
            }
            else
            {
                AuthUI.HideLoader();
            }

              

        }
        #endregion

        #region Email Sign UP Work

        public void SignUp()
        {
            //  AuthUI.Loader.SetActive(true);
            AuthUI.ShowLoader();
            string Pin = RandomCode.ToString();

            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                if (AuthUI.VerifyField.text == Pin)
                {
                    auth.CreateUserWithEmailAndPasswordAsync(AuthUI.SignUpemail.text, AuthUI.SignUppassword.text).ContinueWithOnMainThread((System.Action<Task<FirebaseUser>>)(task =>
                    {
                        if (task.IsCanceled)
                        {
                            Debug.Log("CreateUserWithEmailAndPasswordAsync is canceled");

                            AuthUI.SignUpconfirmPasswordwarningText.gameObject.SetActive(true);
                            AuthUI.SignUpconfirmPasswordwarningText.text = "Sign up Canceled due to weak connection";
                            return;

                        }
                        if (task.IsFaulted)
                        {

                            Debug.Log("CreateUserWithEmailAndPasswordAsync in countered error:" + task.Exception);
                            AuthUI.SignUpconfirmPasswordwarningText.gameObject.SetActive(true);
                            AuthUI.SignUpconfirmPasswordwarningText.text = "Something went wrong";
                            return;
                        }



                        if (task.IsCompleted)
                        {

                            Debug.Log("Avatar used: " + AvatarUsedValue);
                            FirebaseUser newuser = task.Result;


                            Debug.LogFormat("Firebase use created successfully :{0} ({1})", newuser.DisplayName, newuser.UserId);

                            ReferencesHolder.newUserId = newuser.UserId;


                            if (AvatarUsedValue == true)
                            {


                                DocumentReference docRef2 = db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);



                                var publicinfo = new PublicInfoDB
                                {
                                    UserId = newuser.UserId,
                                    UserName = AuthUI.SignUpUserNameField.text,
                                    Coins = 1000,
                                    AvatarUsed = AvatarUsedValue,
                                    AvatarID = defaultAvatarID,
                                    DeviceToken = PlayerPrefs.GetString("DeviceToken"),
                                    PictureURL = "",
                                    PictureName = "",
                                    IsVipMember = false,
                                    Email = newuser.Email

                                };
                                docRef2.SetAsync(publicinfo).ContinueWithOnMainThread(task =>
                                {

                                    Debug.Log("new user public Info added");
                                    Signedup = true;
                                    PlayerPrefs.SetString("UserId", newuser.UserId);
                                    PlayerPrefs.SetInt("SignedUp", 1);
                                    PlayerPrefs.SetString("Email", AuthUI.SignUpemail.text);
                                    PlayerPrefs.Save();
                                    GetHomeData(ReferencesHolder.EmailUserId);


                                });
                            }
                            else
                            {

                                Debug.Log("In else");

                                Texture2D copy = duplicateTexture(ReferencesHolder.playersAvatarTex);
                                byte[] textureBytes = copy.EncodeToPNG();
                                storageReference = storage.RootReference.Child(newuser.UserId + "/uploads/DisplayPic.png");
                                storageReference.PutBytesAsync(textureBytes)
                                .ContinueWithOnMainThread(((Task<StorageMetadata> task) =>
                                {
                                    if (task.IsFaulted || task.IsCanceled)
                                    {
                                        Debug.Log(task.Exception.ToString());

                                    }
                                    else
                                    {

                                        StorageMetadata metadata = task.Result;
                                        string md5Hash = metadata.Md5Hash;

                                        Debug.Log("Finished uploading...");
                                        Debug.Log("md5 hash = " + md5Hash);
                                        var pictureName = newuser.UserId + RemoveSpecialCharacters(md5Hash);

                                        storageReference.GetDownloadUrlAsync().ContinueWithOnMainThread((System.Action<Task<System.Uri>>)(task =>
                                        {
                                            if (task.IsCompleted)
                                            {
                                                DpURL = task.Result.ToString();
                                                DocumentReference docRef2 = db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);


                                                var publicinfo = new PublicInfoDB
                                                {
                                                    UserId = newuser.UserId,
                                                    UserName = AuthUI.SignUpUserNameField.text,
                                                    Coins = 1000,
                                                    AvatarUsed = AvatarUsedValue,
                                                    AvatarID = defaultAvatarID,
                                                    PictureURL = DpURL,
                                                    PictureName = pictureName,
                                                    DeviceToken = PlayerPrefs.GetString("DeviceToken"),
                                                    IsVipMember = false,
                                                    Email = newuser.Email
                                                };
                                                docRef2.SetAsync((object)publicinfo).ContinueWithOnMainThread(task =>
                                                {

                                                    Debug.Log("new user public Info added");
                                                    SaveInDiskCallBackFunction(pictureName);
                                                    Signedup = true;
                                                    PlayerPrefs.SetString(ReferencesHolder.EmailUserId, newuser.UserId);
                                                    PlayerPrefs.SetInt(ReferencesHolder.EmailSignedUp, 1);
                                                    PlayerPrefs.SetString("Email", AuthUI.SignUpemail.text);
                                                    PlayerPrefs.Save();
                                                    GetHomeData(ReferencesHolder.EmailUserId);


                                                });
                                            }
                                        }));



                                    }
                                }));
                            }

                            DocumentReference docRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_Stats_Doc);
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
                            docRef.SetAsync(stats).ContinueWithOnMainThread(task =>
                            {
                                Debug.Log("new email and password user info has been added");



                            });
                            DocumentReference docRef3 = db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_DeckSkins_Doc);

                            db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).SetAsync(new Dictionary<string, object> { { "token", PlayerPrefs.GetString("DeviceToken") }, { "Email", newuser.Email } });
                            //db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId+"mail").SetAsync(new Dictionary<string, object> { { "Email", newuser.Email } });

                            DeckSkinDB deckSkinData = new DeckSkinDB()
                            {
                                Skins = defaultUnlockedSkinIDs.ToArray(),
                                CurrentSkin="000"
                            };



                            docRef3.SetAsync(deckSkinData);


                            ReferencesHolder.Provider = "Email";



                        }



                    }));
                }
                else
                {
                    Debug.Log("wrong pincode");
                    if (AuthUI.VerifyField.text == "")
                    {
                        AuthUI.VerifyPinwarningText.gameObject.SetActive(true);
                        AuthUI.VerifyPinwarningText.text = "Pin code required";
                    }
                    else
                    {
                        AuthUI.VerifyPinwarningText.gameObject.SetActive(true);
                        AuthUI.VerifyPinwarningText.text = " Wrong Pin code";
                        //  AuthUI.Loader.SetActive(false);
                        AuthUI.HideLoader();
                    }
                }
            }
            

        }

        public void SendVerifyEmail()
        {
           LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                bool validEmail;
                if (Regex.IsMatch(AuthUI.SignUpemail.text, MatchEmailPattern))
                {

                    Debug.Log("valid ha");
                    validEmail = true;
                }
                else
                {
                    Debug.Log("Invalid ha");
                    validEmail = false;
                }


                if (AuthUI.SignUpConfirmPasswordField.text != AuthUI.SignUppassword.text
                    || AuthUI.SignUppassword.text.Length < 6
                    || AuthUI.SignUpUserNameField.text == ""
                    || AuthUI.SignUpemail.text == null
                    || AuthUI.SignUppassword.text == null
                    || AuthUI.SignUpConfirmPasswordField.text == null
                    || validEmail == false)

                {
                    Debug.Log("garbar");
                    if (AuthUI.SignUpUserNameField.text == "")
                    {
                        AuthUI.UsernamewarningText.gameObject.SetActive(true);
                        AuthUI.UsernamewarningText.text = "name missing";
                        Debug.Log("name missss");
                    }
                    if (AuthUI.SignUpemail.text == "")
                    {
                        AuthUI.EmailwarningText.gameObject.SetActive(true);
                        AuthUI.EmailwarningText.text = "Email missing";

                    }
                    if (validEmail == false)
                    {
                        AuthUI.EmailwarningText.gameObject.SetActive(true);
                        AuthUI.EmailwarningText.text = "Invalid email";
                    }
                    if (AuthUI.SignUppassword.text == "")
                    {
                        AuthUI.SignUpPasswordwarningText.gameObject.SetActive(true);
                        AuthUI.SignUpPasswordwarningText.text = "Password missing";
                    }
                    if (AuthUI.SignUpConfirmPasswordField.text == "")
                    {
                        AuthUI.SignUpconfirmPasswordwarningText.gameObject.SetActive(true);
                        AuthUI.SignUpconfirmPasswordwarningText.text = " Confirm Password missing";
                    }
                    if (AuthUI.SignUpConfirmPasswordField.text != AuthUI.SignUppassword.text && AuthUI.SignUpConfirmPasswordField.text != "" && AuthUI.SignUppassword.text != "")
                    {
                        AuthUI.SignUpconfirmPasswordwarningText.gameObject.SetActive(true);
                        AuthUI.SignUpconfirmPasswordwarningText.text = "Password does not match";
                    }
                    if (AuthUI.SignUppassword.text.Length < 6 && AuthUI.SignUppassword.text.Length > 0)
                    {
                        AuthUI.SignUpPasswordwarningText.gameObject.SetActive(true);
                        AuthUI.SignUpPasswordwarningText.text = "6 or more digits required";
                    }

                }
                else
                {
                    AuthUI.ShowLoader();
                    string maill = AuthUI.SignUpemail.text;
                    auth.FetchProvidersForEmailAsync(maill).ContinueWithOnMainThread(fetchTask =>
                    {
                        if (fetchTask.IsCanceled || fetchTask.IsFaulted)
                        {
                            Debug.Log("provider work canceled");
                        }

                        else
                        {
                            Debug.Log("Completed!");
                            //  Crashlytics.Log("CheckUserEmailExist Task Completed");

                            // bool isUserExist = false;

                            if (fetchTask.Result != null)
                            {
                                Debug.Log("fetchTask.Result");

                                int i = 0;

                                foreach (string provider in fetchTask.Result)
                                {
                                    Debug.Log($"thats same {provider} and I = {i}");
                                    //Debug.Log("thats same" + fetchTask.Result);
                                    EmailAlreadyExist = true;
                                }
                            }


                            if (EmailAlreadyExist)
                            {
                                // piche bhej do
                                // give error on email already exist
                                AuthUI.EmailwarningText.gameObject.SetActive(true);
                                AuthUI.EmailwarningText.text = "this email already exists";
                                Debug.Log("this email already exist");
                                EmailAlreadyExist = false;

                            }
                            else
                            {
                                //  AuthUI.Loader.SetActive(true);

                                AuthUI.RegisterCanvas.SetActive(false);
                                VerifyEmail(AuthUI.SignUpemail.text);

                            }



                            // onCheckEmailComplete.SafeInvoke(true, isUserExist);
                        }
                    });

                    //      MainUI.Loader.SetActive(false);







                }
            }
            else
            {
                AuthUI.HideLoader();
            }
              

        }

        private void VerifyEmail(string email)
        {
            Debug.Log("already exist false");

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.Timeout = 10000;
            SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Port = 587;

            mail.From = new MailAddress("abudriaz92@gmail.com");
            mail.To.Add(new MailAddress(email));

            mail.Subject = "Verification Code for Borray";
            RandomCode = UnityEngine.Random.Range(10000, 99999);
            mail.Body = $"Insert this Pin Code:{RandomCode.ToString()} in your mobile device to successfully verify your email address. ";


            SmtpServer.Credentials = new System.Net.NetworkCredential("booraydevteam@gmail.com", "loanoyxtjnaecdho") as ICredentialsByHost; SmtpServer.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };

            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            mail.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnSuccess;
            SmtpServer.Send(mail);

            AuthUI.VerifyEmailPanel.SetActive(true);

            Invoke("DelayInVerificationEmailSoundLoaderClose", 3f);

        }
        void DelayInVerificationEmailSoundLoaderClose()
        {
            // AuthUI.Loader.SetActive(false);
            AuthUI.HideLoader();
        }
        void ReVerifyEmail()
        {
            Debug.Log("ResendPinButton Pressed");
            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                RandomCode = 0;
                StartCoroutine(Countdown());
                SendVerifyEmail();
            }
            else
            {
                AuthUI.HideLoader();
            }
             
           

        }
        private IEnumerator Countdown()
        {
            float duration = 10f; // 10 seconds you can change this to
                                  //to whatever you want
            float totalTime = 0;
            while (totalTime <= duration)
            {
                AuthUI.ResendPinButton.enabled = false;
                AuthUI.ReverifyTimmerImg.fillAmount = totalTime / duration;
                totalTime += Time.deltaTime;
                var integer = (int)totalTime;
                string timer = integer.ToString();
                AuthUI.ReVerifyTimmerText.text = timer;

                yield return null;
            }
            AuthUI.ResendPinButton.enabled = true;
        }

        #endregion

        #region Email Sign In Work
        void MainLoginButtonMethod()
        {
            AuthUI.MainCanvas.SetActive(false);
            AuthUI.LoginCanvas.SetActive(true);
        }
        public void LoginValidation()
        {
            Debug.Log("Login validatiiion");
            bool validEmail;
            if (Regex.IsMatch(AuthUI.SignInemail.text, MatchEmailPattern))
            {

                Debug.Log("valid ha");
                validEmail = true;
            }
            else
            {
                Debug.Log("Invalid ha");
                validEmail = false;
            }
            if (AuthUI.SignInemail.text == "" || AuthUI.SignInpassword.text == "" || validEmail == false || AuthUI.SignInpassword.text.Length < 6)
            {
                if (AuthUI.SignInemail.text == "")
                {
                    AuthUI.SignInEmailwarningText.gameObject.SetActive(true);
                    AuthUI.SignInEmailwarningText.text = "Email missing";
                }
                if (AuthUI.SignInpassword.text == "")
                {
                    AuthUI.SignInPasswordwarningText.gameObject.SetActive(true);
                    AuthUI.SignInPasswordwarningText.text = "Password missing";
                }
                if (AuthUI.SignInemail.text != "" && validEmail == false)
                {
                    AuthUI.SignInEmailwarningText.gameObject.SetActive(true);
                    AuthUI.SignInEmailwarningText.text = "Invalid email";
                }
                if (AuthUI.SignInpassword.text.Length < 6)
                {
                    AuthUI.SignInPasswordwarningText.gameObject.SetActive(true);
                    AuthUI.SignInPasswordwarningText.text = "For Password 6 or more digits required";
                }
            }
            else
            {
                // AuthUI.Loader.SetActive(true);
                AuthUI.ShowLoader();
                SignIn();
            }
        }
        public void SignIn()
        {
            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                auth.SignInWithEmailAndPasswordAsync(AuthUI.SignInemail.text, AuthUI.SignInpassword.text).ContinueWithOnMainThread(task => {
                    if (task.IsCanceled)
                    {

                        Debug.Log("-------------->SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                        Debug.Log("Login canceled");
                        AuthUI.SignInPasswordwarningText.gameObject.SetActive(true);
                        AuthUI.SignInPasswordwarningText.text = "Login canceled";
                        //   AuthUI.Loader.SetActive(false);
                        AuthUI.HideLoader();
                        return;
                    }
                    if (task.IsFaulted)
                    {

                        Debug.Log("------------->Wrong password or Email or weak internet");
                        AuthUI.SignInPasswordwarningText.gameObject.SetActive(true);
                        AuthUI.SignInPasswordwarningText.text = "Wrong password or Email";
                        //  AuthUI.Loader.SetActive(false);
                        AuthUI.HideLoader();
                        return;
                    }
                    if (task.IsCompleted)
                    {

                        ReferencesHolder.Provider = "Email";
                        Firebase.Auth.FirebaseUser newUser = task.Result;
                        Debug.LogFormat("User signed in successfully: {0} ({1})",
                        newUser.DisplayName, newUser.UserId);
                        LoginUserId = newUser.UserId;


                        ReferencesHolder.newUserId = newUser.UserId;

                        PlayerPrefs.SetInt(ReferencesHolder.EmailSignedUp, 1);
                        PlayerPrefs.SetString(ReferencesHolder.EmailUserId, newUser.UserId);

                        PlayerPrefs.Save();
                        Debug.Log("Signed in user id >>>>>>> " + newUser.UserId);
                        Debug.Log("Signed in user Token >>>>>>> " + PlayerPrefs.GetString("DeviceToken"));
                        db.Collection(ReferencesHolder.FS_users_Collec).Document(newUser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).UpdateAsync("DeviceToken", PlayerPrefs.GetString("DeviceToken"));
                        db.Collection(ReferencesHolder.FS_users_Collec).Document(newUser.UserId).UpdateAsync("token", PlayerPrefs.GetString("DeviceToken"));
                        GetHomeData(ReferencesHolder.EmailUserId);

                    }


                });
            }
            else
            {
                AuthUI.HideLoader();
            }
               

        }
        #endregion

        #region Forget Password Work
        
        public void ForgetPasswordValidation()
        {
            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                bool validEmail;
                if (Regex.IsMatch(AuthUI.ForgetPasswordEmailField.text, MatchEmailPattern))
                {

                    Debug.Log("valid ha");
                    validEmail = true;
                }
                else
                {
                    Debug.Log("Invalid ha");
                    validEmail = false;
                }
                if (AuthUI.ForgetPasswordEmailField.text == "" || validEmail == false)
                {
                    if (AuthUI.ForgetPasswordEmailField.text == "")
                    {
                        AuthUI.ForgetPasswordEmailWarningText.gameObject.SetActive(true);
                        AuthUI.ForgetPasswordEmailWarningText.text = "Email address required";
                    }
                    else if (AuthUI.ForgetPasswordEmailField.text != "" && validEmail == false)
                    {
                        AuthUI.ForgetPasswordEmailWarningText.gameObject.SetActive(true);
                        AuthUI.ForgetPasswordEmailWarningText.text = "Invalid Email";
                    }
                }
                else
                {
                    PasswordReset();
                }
            }
               

        }
        public void PasswordReset()
        {
            string emailAddress = AuthUI.ForgetPasswordEmailField.text;
            AuthUI.ForgetPasswordGuideText.gameObject.SetActive(true);
            AuthUI.ForgetPasswordGuideText.text = "Wait Sending Email..";
            auth.SendPasswordResetEmailAsync(emailAddress).ContinueWithOnMainThread(task => {
                if (task.IsCanceled)
                {

                    AuthUI.ForgetPasswordGuideText.text = "Something went wrong please check your connection";

                }
                if (task.IsFaulted)
                {

                    AuthUI.ForgetPasswordGuideText.text = "Email could not be found";

                }
                if (task.IsCompleted)
                {
                    AuthUI.ForgetPasswordGuideText.gameObject.SetActive(true);
                    AuthUI.ForgetPasswordGuideText.text = "Email has been send press the link in email to change password";
                    Debug.Log("Password reset email sent successfully.");
                    AuthUI.ClearSignInWarnings();
                    AuthUI.ClearSignInTextFields();
                    Invoke("DelayHideForgetPanel", 4);

                }


            });

        }
        public void DelayHideForgetPanel()
        {
            AuthUI.ForgetPasswordPanel.SetActive(false);
            AuthUI.MainCanvas.SetActive(true);
            AuthUI.LoginCanvas.SetActive(false);
        }
        #endregion


        public void GetHomeData(string id)
        {
            //  AuthUI.Loader.SetActive(true);
            AuthUI.ShowLoader();
           
            //ReferencesHolder.Provider = "Facebook";

            //Debug.Log("Get home data chla ");
            db.Collection(ReferencesHolder.FS_users_Collec).Document(PlayerPrefs.GetString(id)).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_DeckSkins_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                //Debug.Log("agya skin1");
                if (task.IsCanceled || task.IsFaulted)
                {
                    // AuthUI.Loader.SetActive(false);
                    AuthUI.HideLoader();
                    MainCanvas.SetActive(true);
                    AuthUI.LoginCanvas.SetActive(false);
                    LogErrorUIHandler.instance.OpenErrorPanel("Could load data check intenet and try again");

                }
                if (task.IsCompleted)
                {
                    if (task.Result.Exists)
                    {
                        //Debug.Log("agya skin2");
                        DeckSkinDB Skins = task.Result.ConvertTo<DeckSkinDB>();
                        // Debug.Log(Skins.Skins);
                        //Debug.Log("agya skin3");
                        ReferencesHolder.AvailableSkins = Skins;
                    }
                    else
                    {
                        //Debug.Log("Wrong path for guest deck skin");
                        AuthUI.Loader.SetActive(false);
                        MainCanvas.SetActive(true);
                        AuthUI.LoginCanvas.SetActive(false);
                        LogErrorUIHandler.instance.OpenErrorPanel("Could load data check intenet and try again");
                    }
                }

            });
            db.Collection(ReferencesHolder.FS_users_Collec).Document(PlayerPrefs.GetString(id)).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    //  AuthUI.Loader.SetActive(false);
                    AuthUI.HideLoader();

                }
                if (task.IsCompleted)
                {
                    if (task.Result.Exists)
                    {
                        PublicInfoDB Info = task.Result.ConvertTo<PublicInfoDB>();
                        ReferencesHolder.playerPublicInfo = Info;
                        //Debug.Log("this is photo URL" + Info.PictureURL);
                        bool check = CheckIfTextureExists(ReferencesHolder.playerPublicInfo.PictureName + ".png");
                        //Debug.Log("cccccccccccccccchhheeck:" + check);


                        if (check == true)
                        {
                            //Debug.Log("local custom pic of FB user");
                            string path = Path.Combine(pathOfTextures, Info.PictureName + ".png");
                            StartCoroutine(LoadLocalTexture("file://" + path));
                        }
                        else
                        {
                            //Debug.Log("Pic work condtion area arrived");
                            if (Info.AvatarUsed == true)
                            {
                                //Debug.Log("<<<<<<< nae gya load image me GETHOMEDATA>>>>>>>>");
                                OpenHomeScreen();
                            }
                            else
                            {
                                //Debug.Log("<<<<<<<gya load image me GETHOMEDATA>>>>>>>>");
                                StartCoroutine(LoadImage(Info.PictureURL));
                            }
                          
                        }

                    }
                    else
                    {
                        // AuthUI.Loader.SetActive(false);
                        AuthUI.HideLoader();
                        MainCanvas.SetActive(true);
                    }

                    NotificationHandler.instance.ReAssignTopic();
                    NotificationHandler.instance.RefreshToken();

                    PhotonNetwork.NickName = ReferencesHolder.playerPublicInfo.UserId;

                }

            });
        }
        public void GetTournamentTopic()
        {
            Debug.Log("Get Tournament topic me agya");
            Query collRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_UserTournaments_Collec);
            collRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                Debug.Log("Get Tournament topic me agya 2");
                QuerySnapshot snapshot = task.Result;

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    Debug.Log("Get Tournament topic me agya 3");
                    if (!document.Exists)
                    {
                        Debug.Log("No tournament to subscribe");
                    }
                   
                        Debug.Log("Yay ha topic id"+document.Id);

                        SubscribeToTopic(document.Id);
                  
                   
                  









                }
            });
         }
        public async void SubscribeToTopic(string topic)
        {
            Debug.Log("Authentication scene topic subscribe method chla");
            await Firebase.Messaging.FirebaseMessaging.SubscribeAsync($"/topics/{topic}");
          



        }
        private void OpenHomeScreen()
        {
            //Debug.Log("open home screen method");

            SceneManager.LoadScene(1);

        }

        #region Display Image Work

        void OpenGalleryButtonMethod()
        {
            PickImage(512);
        }
        private void PickImage(int maxSize)
        {
            NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
            {
                Debug.Log("Image path: " + path);
                ReferencesHolder.AvatarUploadPath = path;
                if (path != null)
                {

                    ReferencesHolder.AvatarUsed = false;
                    AvatarUsedValue = false;
                    // Create Texture from selected image
                    Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);

                    var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                    if (texture == null)
                    {
                        Debug.Log("Couldn't load texture from " + path);
                        return;
                    }
                    AuthUI.SignUpDisplayPic.texture = texture;
                    ReferencesHolder.playersAvatarTex = texture;
                    ReferencesHolder.playersAvatarSprite = sprite;
                }
            });

            Debug.Log("Permission result: " + permission);
        }
        //below method to load image from facebook server
        void DisplayProfilePic(IGraphResult result)
        {
            if (result.Texture != null)
            {
                Debug.Log("Profile Pic");
                var SpriteImage = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
                if (PlayerPrefs.GetString(ReferencesHolder.FBUserId) == null || PlayerPrefs.GetString(ReferencesHolder.FBUserId) == "")
                {
                    ReferencesHolder.playersAvatarTex = result.Texture;
                    ReferencesHolder.playersAvatarSprite = SpriteImage;
                }

                authwithfirebase(accesstoken);
            }
            else
            {
                Debug.Log(result.Error);
            }
        }
        IEnumerator GoogleLoadImage(Uri MediaUrl)
        {
            Debug.Log("Google image load me gya");
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Create a request
            yield return request.SendWebRequest(); //Wait for the request to complete
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                var tex = ((DownloadHandlerTexture)request.downloadHandler).texture;

                var spriteImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

                if (PlayerPrefs.GetString(ReferencesHolder.GoogleUserId) == null || PlayerPrefs.GetString(ReferencesHolder.GoogleUserId) == "")
                {
                    ReferencesHolder.playersAvatarTex = tex;
                    ReferencesHolder.playersAvatarSprite = spriteImage;
                }
                SignInWithGoogleOnFirebase(GoogleToken);

            }
        }
        //below method for loading image from local persistent data path
        private IEnumerator LoadLocalTexture(string path)
        {
            Debug.Log("Load local me gaya");
            Debug.Log(path);
            Debug.Log("FILE EXITS");
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(path);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Local image load");

                var tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
                yield return new WaitForEndOfFrame();
                var spriteImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

                ReferencesHolder.playersAvatarTex = tex;
                ReferencesHolder.playersAvatarSprite = spriteImage;

                OpenHomeScreen();
            }
        }
        //below method for loading image from firebase cloud storage
        IEnumerator LoadImage(string MediaUrl)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Create a request
            yield return request.SendWebRequest(); //Wait for the request to complete
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                var tex = ((DownloadHandlerTexture)request.downloadHandler).texture;

                var spriteImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

                ReferencesHolder.playersAvatarTex = tex;
                ReferencesHolder.playersAvatarSprite = spriteImage;

                OpenHomeScreen();
            }
        }


        public void SaveImageOnDisk(Texture2D texture, string fileName)
        {

            //string path = pathOfTextures + "/" + fileName;
            Debug.Log("save image on disk me gya");

            string path = Path.Combine(pathOfTextures, fileName + ".png");
            ReferencesHolder.PersistentPath = path;

            ReferencesHolder.AvatarUploadPath = path;

            Debug.Log("yay persistent path ha:" + path);
            if (!Directory.Exists(pathOfTextures))
            {
                Debug.Log("save image on disk directory exist check");
                var folder = Directory.CreateDirectory(pathOfTextures); // returns a DirectoryInfo object

            }



            if (texture is null)
            {
                //    Logging.Log("No Image To save");
                Debug.Log("save image on disk null texture check");
                return;
            }

            Texture2D copy = duplicateTexture(texture);
            byte[] textureBytes = copy.EncodeToPNG();

            File.WriteAllBytes(path, textureBytes);

            Debug.Log("save image on disk texture saved");
            // Logging.Log(" File Written On Disk ");

        }
        public bool CheckIfTextureExists(string fileName)
        {

            string path = pathOfTextures + "/" + fileName;

            if (File.Exists(path))
            {
                return true;

            }
            return false;
        }
        Texture2D duplicateTexture(Texture2D source)
        {
            Debug.Log("duplicate texture me gya");
            RenderTexture renderTex = RenderTexture.GetTemporary(
                source.width,
                source.height,
                            0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;

        }

        //below method to generate unique name for image with no special characters
        public string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] >= '0' && str[i] <= '9')
                    || (str[i] >= 'A' && str[i] <= 'z'
                        || (str[i] == '.' || str[i] == '_')))
                {
                    sb.Append(str[i]);
                }
            }

            return sb.ToString();
        }
        void SaveInDiskCallBackFunction()
        {

            Debug.Log("SaVE Image on disk call back fuction chlgya");
            Texture2D copy = duplicateTexture(ReferencesHolder.playersAvatarTex);
            SaveImageOnDisk(copy, ReferencesHolder.playerPublicInfo.PictureName);


        }
        void SaveInDiskCallBackFunction(string pictureName)
        {

            Debug.Log("SaVE Image on disk call back fuction chlgya");
            Texture2D copy = duplicateTexture(ReferencesHolder.playersAvatarTex);
            SaveImageOnDisk(copy, pictureName);


        }


        #endregion

    }
}




