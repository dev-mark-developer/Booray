using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Google;
using UnityEngine;
using TMPro;
using Firebase.Extensions;
using Firebase.Storage;
using Firebase.Firestore;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using System.Collections;
using UnityEngine.UI;
using Firebase.Messaging;

namespace Booray.Auth
{
    public class GoogleAuth : MonoBehaviour
    {
        public TextMeshProUGUI infoText;
        public string webClientId = "949171170208-pevs6ts14fmled463qjha2g6hfobiang.apps.googleusercontent.com";

        private FirebaseAuth auth;
        FirebaseFirestore db;
        FirebaseStorage storage;
        StorageReference storageReference;
        public GameObject Loader;
        public GameObject MainCanvas;
        private string pathOfTextures;
        string DpURL;
        string GoogleToken;
        private GoogleSignInConfiguration configuration;
        private string defaultAvatarID = "panda";
        [SerializeField] List<string> defaultUnlockedSkinIDs;
        public Button GoogleButton;
        private void Awake()
            
        {
            Debug.Log("Here google");
            pathOfTextures = Application.persistentDataPath + "/WebTexturesFolder";
            configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
            //auth = FirebaseAuth.DefaultInstance;
         

        }
        private void Start()
        {
            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseFirestore.DefaultInstance;
            storage = FirebaseStorage.DefaultInstance;
            storageReference = storage.GetReferenceFromUrl("gs://ultimate-booray.appspot.com");
        //    FirebaseMessaging.TokenReceived += TokenReceived;
            Firebase.Messaging.FirebaseMessaging.SubscribeAsync("/topics/Tournament");
            GoogleButton.onClick.AddListener(delegate { SignInWithGoogle(); SFXHandler.instance.PlayBtnClickSFX(); });
            if (PlayerPrefs.GetInt(ReferencesHolder.GoogleSignedUp) == 1)
            {
                Debug.Log("google ka auto chla");

                //  GetHomeData();
                SignInWithGoogle();
        
                
                    
            }
        }
        //private void TokenReceived(object sender, TokenReceivedEventArgs e)
        //{
        //    Debug.Log("Token Received" + e.Token);
        //    ReferencesHolder.RecievedDeviceToken = e.Token;
        //    PlayerPrefs.SetString("DeviceToken", e.Token);
        //    PlayerPrefs.Save();
          
        //}

        public void SignInWithGoogle() { OnSignIn(); }
        public void SignOutFromGoogle() { OnSignOut(); }

        private void OnSignIn()
        {
            

            if(ReferencesHolder.googleAlreadyConfigured==false)
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
               // SignInWithGoogleOnFirebase(task.Result.IdToken);
                Debug.Log(task.Result.ImageUrl);
                infoText.text = "Welcome: " + task.Result.DisplayName;
                Loader.SetActive(true);
                StartCoroutine(GoogleLoadImage(task.Result.ImageUrl));
            }

        }
     

        public void SignInWithGoogleOnFirebase(string idToken)
        {
            Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                AggregateException ex = task.Exception;
                if (ex != null||task.IsCanceled)
                {
                    Loader.SetActive(false);
                    if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                        AddToInformation("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
                }
                else
                {
                    AddToInformation("Sign In Successful.");

                   // infoText.text = "Welcome: " + task.Result.DisplayName;
                    Firebase.Auth.FirebaseUser newuser = task.Result;
                    ReferencesHolder.newUserId = newuser.UserId;
                    ///SceneManager.LoadScene("MainScene");
                    Debug.Log("firbase google auth done");
                
                    db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
                    {
                        if (task.Result.Exists)
                        {
                            Debug.Log("Google user result exist krta ha");
                            //if(task.Result.Id==ReferencesHolder.newUserId|| task.Result.Id == newuser.UserId)
                            //{
                            //    Debug.Log("Fb user already present");


                            //}
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
                            //  Debug.Log(ReferencesHolder.playerPublicInfo.PictureName);

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
                            //FriendReqData frd = task.Result.ConvertTo<FriendReqData>();

                            //  OpenHomeScreen();
                            // GetHomeData();
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
                                    //string pictureName = newuser.UserId + RemoveSpecialCharacters(md5Hash);
                                    var pictureName = newuser.UserId + RemoveSpecialCharacters(md5Hash);
                                    SaveImageOnDisk(copy, pictureName);
                                    // Debug.Log("the name of picha" + ReferencesHolder.playerPublicInfo.PictureName);
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
                                                IsVipMember = false
                                                // PictureName = ReferencesHolder.playerPublicInfo.PictureName,
                                            };
                                            docRef2.SetAsync(publicinfo).ContinueWithOnMainThread(task =>
                                            {

                                                Debug.Log("new user public Info added");
                                                // SaveInDiskCallBackFunction(pictureName);
                                                PlayerPrefs.SetString(ReferencesHolder.GoogleUserId, newuser.UserId);
                                                PlayerPrefs.SetInt(ReferencesHolder.GoogleSignedUp, 1);
                                                PlayerPrefs.Save();
                                                //  ReferencesHolder.playerPublicInfo = publicinfo;
                                                //  SaveInDiskCallBackFunction(pictureName);
                                                //PlayerPrefs.Save();
                                                //SaveImageOnDisk(copy, pictureName);
                                                GetHomeData();


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

                            //DocumentReference docRef3 = db.Collection("Users").Document(newuser.UserId).Collection("UserData").Document("DeckSkins");
                            //DeckSkinDB deckSkinData = new DeckSkinDB()
                            //{
                            //    Skins = defaultUnlockedSkinIDs.ToArray()
                            //};

                            //docRef3.SetAsync(deckSkinData);

                            DocumentReference docRef3 = db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_DeckSkins_Doc);
                            //Dictionary<string, object> docData = new Dictionary<string, object>
                            //    {

                            //       { "Skins", new List<object>() { "000","001" } },


                            //    };

                            db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).SetAsync(new Dictionary<string, object> { { "token", PlayerPrefs.GetString("DeviceToken") } });
                            DeckSkinDB deckSkinData = new DeckSkinDB()
                            {
                                Skins = defaultUnlockedSkinIDs.ToArray()
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

        public void AddToInformation(string str) { infoText.text += "\n" + str; }

        public void GetHomeData()
        {
            Loader.SetActive(true);
            ReferencesHolder.Provider = "Google";
            Debug.Log("Google Get home data chla ");
            db.Collection(ReferencesHolder.FS_users_Collec).Document(PlayerPrefs.GetString(ReferencesHolder.GoogleUserId)).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_DeckSkins_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
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


                    }


                }

            });
            db.Collection(ReferencesHolder.FS_users_Collec).Document(PlayerPrefs.GetString(ReferencesHolder.GoogleUserId)).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Loader.SetActive(false);

                }
                if (task.IsCompleted)
                {
                    if (task.Result.Exists)
                    {
                        PublicInfoDB Info = task.Result.ConvertTo<PublicInfoDB>();
                        ReferencesHolder.playerPublicInfo = Info;
                        //FriendsDataDB Fd = task.Result.ConvertTo<FriendsDataDB>();
                        //ReferencesHolder.FriendsInfo = Fd;
                        //PublicInfoDB Info = task.Result.ConvertTo<PublicInfoDB>();
                        //Debug.Log("yay ha naam" + Info.UserName);
                        //ReferencesHolder.userName = Info.UserName;
                        //ReferencesHolder.Coins = Info.Coins.ToString();
                        //ReferencesHolder.userID = Info.UserId;
                        //ReferencesHolder.AvatarUsed = Info.AvatarUsed;

                        Debug.Log("this is photo URL" + Info.PictureURL);
                        bool check = CheckIfTextureExists(ReferencesHolder.playerPublicInfo.PictureName + ".png");
                        Debug.Log("cccccccccccccccchhheeck:" + check);
                        Debug.Log(ReferencesHolder.playerPublicInfo.PictureName);

                        if (check == true)
                        {
                            string path = Path.Combine(pathOfTextures, Info.PictureName + ".png");
                            StartCoroutine(LoadLocalTexture("file://" + path));
                        }
                        else
                        {
                            if (Info.PictureURL == "" || Info.PictureURL == null)
                            {


                                db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).UpdateAsync("AvatarUsed",true);
                                db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).UpdateAsync("AvatarID",defaultAvatarID);
                                ReferencesHolder.AvatarUsed = true;
                                OpenHomeScreen();
                            }
                            else
                            {
                                StartCoroutine(LoadImage(Info.PictureURL));
                            }
                          //  StartCoroutine(LoadImage(Info.PictureURL));

                            Debug.Log("awww");
                        }







                    }
                    else
                    {
                        Loader.SetActive(false);
                        MainCanvas.SetActive(true);
                        //PlayerPrefs.SetString(ReferencesHolder.GoogleUserId, "");
                        //PlayerPrefs.Save();
                    }

                }






            });



        }
        private void OpenHomeScreen()
        {
            Debug.Log("open home screen method");

            SceneManager.LoadScene("MainScene");

        }

        #region Display Image Work

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

                //ReferencesHolder.AvatarUsed = false;
                //  AvatarUsedValue = false;

                OpenHomeScreen();

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
                //  ReferencesHolder.AvatarUsed = false;
                //  AvatarUsedValue = false;
                // SaveInDiskCallBackFunction();
                // OpenHomeScreen();


            }
        }
        //below method for loading image from firebase cloud storage
        IEnumerator LoadImage(string MediaUrl)
        {
            Debug.Log("Firebase image load me gya");
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


                //  ReferencesHolder.AvatarUsed = false;
                //  AvatarUsedValue = false;
                SaveInDiskCallBackFunction();
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


            Texture2D copy = duplicateTexture(ReferencesHolder.playersAvatarTex);
            SaveImageOnDisk(copy, ReferencesHolder.playerPublicInfo.PictureName);


        }
        void SaveInDiskCallBackFunction(string pictureName)
        {


            Texture2D copy = duplicateTexture(ReferencesHolder.playersAvatarTex);
            SaveImageOnDisk(copy, pictureName);


        }


        #endregion

        


    }

}
