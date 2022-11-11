using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
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

namespace Booray.Auth
{
    public class Facebookauth : MonoBehaviour
    {

        [SerializeField] List<string> defaultUnlockedSkinIDs;

        FirebaseAuth auth;
        FirebaseFirestore db;
        FirebaseStorage storage;
        StorageReference storageReference;
        public GameObject Loader;
        public GameObject MainCanvas;
        private string pathOfTextures;
        string DpURL;
        private bool Signedup = false;
        public string accesstoken;
        public Button FacbookButton;
        

        private void Awake()
        {

            //auth = FirebaseAuth.DefaultInstance;
            //db = FirebaseFirestore.DefaultInstance;
            pathOfTextures = Application.persistentDataPath + "/WebTexturesFolder";
            //storage = FirebaseStorage.DefaultInstance;
            //storageReference = storage.GetReferenceFromUrl("gs://accountmanger-ff8e7.appspot.com");
            if (!FB.IsInitialized)
            {
                FB.Init(InitCallBack, OnHideUnity);
            }
            else
            {
                FB.ActivateApp();
            }
            Debug.Log("yay FBauto value:" + PlayerPrefs.GetInt(ReferencesHolder.FBSignedUp));
            Debug.Log("yay current FBuserid ha" + PlayerPrefs.GetString(ReferencesHolder.FBUserId));
            //if (PlayerPrefs.GetInt(ReferencesHolder.FBSignedUp) == 1)
            //{
            //    GetHomeData();
            //}
        }

        private void Start()
        {
            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseFirestore.DefaultInstance;
            storage = FirebaseStorage.DefaultInstance;
            // storageReference = storage.GetReferenceFromUrl("gs://accountmanger-ff8e7.appspot.com");
            storageReference = storage.GetReferenceFromUrl("gs://ultimate-booray.appspot.com");
          //  FirebaseMessaging.TokenReceived += TokenReceived;
            Firebase.Messaging.FirebaseMessaging.SubscribeAsync("/topics/Tournament");
            FacbookButton.onClick.AddListener(delegate { Facebook_Login(); SFXHandler.instance.PlayBtnClickSFX(); });
            if (PlayerPrefs.GetInt(ReferencesHolder.FBSignedUp) == 1)
            {
                // GetHomeData();
                Facebook_Login();
            }
        }
        //private void TokenReceived(object sender, TokenReceivedEventArgs e)
        //{
        //    Debug.Log("Token Received" + e.Token);
        //    ReferencesHolder.RecievedDeviceToken = e.Token;
        //    PlayerPrefs.SetString("DeviceToken", e.Token);
        //    PlayerPrefs.Save();
           
        //}
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

        
        #region Sign Up Work
        public void Facebook_Login()
        {
            var permission = new List<string>() { "public_profile", "email" };
            FB.LogInWithReadPermissions(permission, AuthCallBack);
        }

        private void AuthCallBack(ILoginResult result)
        {
            if (FB.IsLoggedIn)
            {
                var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                //  debug.text = (aToken.UserId);

                //string accesstoken;
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
                //authwithfirebase(accesstoken);






            }
            else
            {
                Debug.Log("User Cancelled login");
            }
        }
        public void authwithfirebase(string accesstoken)
        {

            Loader.SetActive(true);


            auth = FirebaseAuth.DefaultInstance;
            Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(accesstoken);
            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("singin encountered error" + task.Exception);
                    Loader.SetActive(false);
                }
                Firebase.Auth.FirebaseUser newuser = task.Result;
                ReferencesHolder.newUserId = newuser.UserId;
                if (task.IsCompleted)
                {
                    Debug.Log("firbase fb auth done");
                    Debug.Log("thats fb new user id " + newuser.UserId);
                    Debug.Log("thats refereceholder fb new user id " + newuser.UserId);
                    //    db.Collection(ReferencesHolder.FS_users_Collec).Document(PlayerPrefs.GetString(ReferencesHolder.EmailUserId)).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc)
                    db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
                    {
                        if (task.Result.Exists)
                        {
                            Debug.Log("result exist krta ha");
                            //if(task.Result.Id==ReferencesHolder.newUserId|| task.Result.Id == newuser.UserId)
                            //{
                            //    Debug.Log("Fb user already present");


                            //}
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
                        else if(task.Result.Exists==false)
                        {
                            Debug.Log("Fb user not already present");
                            // ReferencesHolder.newUserId = newuser.UserId;
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
                                                
                                            };
                                            docRef2.SetAsync(publicinfo).ContinueWithOnMainThread(task =>
                                            {

                                                Debug.Log("new user public Info added");
                                                // SaveInDiskCallBackFunction(pictureName);
                                                PlayerPrefs.SetString(ReferencesHolder.FBUserId, newuser.UserId);
                                                PlayerPrefs.SetInt(ReferencesHolder.FBSignedUp, 1);
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
                                Debug.Log("new facebook user info has been added");



                            });

                            //DocumentReference docRef3 = db.Collection("Users").Document(newuser.UserId).Collection("UserData").Document("DeckSkins");
                            //DeckSkinDB deckSkinData = new DeckSkinDB()
                            //{
                            //    Skins = defaultUnlockedSkinIDs.ToArray()
                            //};

                            //docRef3.SetAsync(deckSkinData);

                            DocumentReference docRef3 = db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_DeckSkins_Doc);

                            db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).SetAsync(new Dictionary<string, object> { { "token", PlayerPrefs.GetString("DeviceToken") } });
                            //Dictionary<string, object> docData = new Dictionary<string, object>
                            //    {

                            //       { "Skins", new List<object>() { "000","001" } },


                            //    };
                            DeckSkinDB deckSkinData = new DeckSkinDB()
                            {
                                Skins = defaultUnlockedSkinIDs.ToArray()
                            };

                            docRef3.SetAsync(deckSkinData);


                            ReferencesHolder.Provider = "Facebook";

                        }

                    });

                }
               





            });
        }
        #endregion
        public void GetHomeData()
        {
            Loader.SetActive(true);
            ReferencesHolder.Provider = "Facebook";
       
            Debug.Log("Get home data chla ");
            db.Collection(ReferencesHolder.FS_users_Collec).Document(PlayerPrefs.GetString(ReferencesHolder.FBUserId)).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_DeckSkins_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
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
            db.Collection(ReferencesHolder.FS_users_Collec).Document(PlayerPrefs.GetString(ReferencesHolder.FBUserId)).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
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
                        //PublicInfoDB Info = task.Result.ConvertTo<PublicInfoDB>();
                        //Debug.Log("yay ha naam" + Info.UserName);
                        //ReferencesHolder.userName = Info.UserName;
                        //ReferencesHolder.Coins = Info.Coins.ToString();
                        //ReferencesHolder.userID = Info.UserId;
                        //ReferencesHolder.AvatarUsed = Info.AvatarUsed;

                        Debug.Log("this is photo URL" + Info.PictureURL);
                        bool check = CheckIfTextureExists(ReferencesHolder.playerPublicInfo.PictureName + ".png");
                        Debug.Log("cccccccccccccccchhheeck:" + check);
                      //  Debug.Log(ReferencesHolder.playerPublicInfo.PictureName);

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
                    else
                    {
                        Loader.SetActive(false);
                        MainCanvas.SetActive(true);
                        //PlayerPrefs.SetString(ReferencesHolder.GuestUserId, "");
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
        //below method to load image from facebook server
        void DisplayProfilePic(IGraphResult result)
        {
            if (result.Texture != null)
            {
                Debug.Log("Profile Pic");
                var SpriteImage = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
                if(PlayerPrefs.GetString(ReferencesHolder.FBUserId)==null|| PlayerPrefs.GetString(ReferencesHolder.FBUserId) == "")
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
               

                //  ReferencesHolder.AvatarUsed = false;
                //  AvatarUsedValue = false;

               


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
           // OpenHomeScreen();

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




