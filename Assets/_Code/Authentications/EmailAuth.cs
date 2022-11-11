using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Storage;
//using TMPro;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Photon.Pun;
using UnityEngine.Networking;
//using DG.Tweening;
using UnityEngine.SceneManagement;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using Firebase.Messaging;



namespace Booray.Auth
{

    public class EmailAuth : MonoBehaviour

    {

        [SerializeField] List<string> defaultUnlockedSkinIDs;



        [SerializeField] Button LoginButton;
        [SerializeField] Button RegisterButton;
        [SerializeField] Button ForgetPasswordChangeButton;
        [SerializeField] Button VerifyEmailButton;
        [SerializeField] Button ResendPinButton;
        [SerializeField] Button OpenGalleryButton;
        [SerializeField] Button MainLoginButton;
        //[SerializeField] Button ChangePasswordButton;






      //  [SerializeField] EmailAuthUIManager EmailUI;
        public bool Login;
        public string LoginUserId;
        public int RandomCode;
        private string defaultAvatarID = "panda";
        FirebaseAuth auth;
        FirebaseFirestore db;
        FirebaseStorage storage;
        StorageReference storageReference;
        public const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
            + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
            + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
            + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

      //  public GameObject HomeUIScriptObj;
        private bool AvatarUsedValue=true;
      
        private string pathOfTextures;
        string DpURL;
        private bool Signedup = false;
        private bool Security=false;
        bool EmailAlreadyExist = false;
      //  public void Awake()
      //  {
           
            //pathOfTextures = Application.persistentDataPath + "/WebTexturesFolder";
            //Debug.Log("Here Email");
            //auth = FirebaseAuth.DefaultInstance;
            //db = FirebaseFirestore.DefaultInstance;
            //storage = FirebaseStorage.DefaultInstance;
            //storageReference = storage.GetReferenceFromUrl("gs://accountmanger-ff8e7.appspot.com");

      //  }

    //    private void Start()
    //    {
    //        auth = FirebaseAuth.DefaultInstance;
    //        db = FirebaseFirestore.DefaultInstance;
    //        storage = FirebaseStorage.DefaultInstance;
    //        // storageReference = storage.GetReferenceFromUrl("gs://accountmanger-ff8e7.appspot.com");
    //        storageReference = storage.GetReferenceFromUrl("gs://ultimate-booray.appspot.com");
    //       // FirebaseMessaging.TokenReceived += TokenReceived;
    //        Firebase.Messaging.FirebaseMessaging.SubscribeAsync("/topics/Tournament");
    //        Screen.sleepTimeout = SleepTimeout.NeverSleep;

    //        var autoLogin = PlayerPrefs.GetInt(ReferencesHolder.EmailSignedUp);

    //        Debug.Log($" Auto Login ->  {autoLogin} -> {PlayerPrefs.GetString(ReferencesHolder.EmailUserId,"-")} ");

    //        //Debug.Log("yay auto value:" + PlayerPrefs.GetInt(ReferencesHolder.EmailSignedUp)); 
    //       // Debug.Log("yay current userid ha" + PlayerPrefs.GetString(ReferencesHolder.EmailUserId));
    //     //   LoginButton.onClick.AddListener(LoginValidation);
    //        LoginButton.onClick.AddListener(delegate { LoginValidation();SFXHandler.instance.PlayBtnClickSFX(); });
    //        RegisterButton.onClick.AddListener(delegate { SendVerifyEmail();EmailUI.ResetVerifyPanel();SFXHandler.instance.PlayBtnClickSFX(); });
    //        MainLoginButton.onClick.AddListener(delegate { MainLoginButtonMethod();SFXHandler.instance.PlayBtnClickSFX(); });
    //        VerifyEmailButton.onClick.AddListener(delegate { SignUp(); SFXHandler.instance.PlayBtnClickSFX(); });
    //        ForgetPasswordChangeButton.onClick.AddListener(delegate { ForgetPasswordValidation(); SFXHandler.instance.PlayBtnClickSFX(); });
    //        ResendPinButton.onClick.AddListener(delegate { ReVerifyEmail(); SFXHandler.instance.PlayBtnClickSFX(); });
    //        OpenGalleryButton.onClick.AddListener(delegate { OpenGalleryButtonMethod(); SFXHandler.instance.PlayBtnClickSFX(); });

            
         
    //        // ChangePasswordButton.onClick.AddListener(ValidateChangePassword);

    //        if (autoLogin == 1)
    //        {
    //            Security = true;
    //            GetHomeData();

    //        }


    //    }


    //    //private void TokenReceived(object sender, TokenReceivedEventArgs e)
    //    //{
    //    //    Debug.Log("Token Received" + e.Token);
    //    //    ReferencesHolder.RecievedDeviceToken = e.Token;
    //    //    PlayerPrefs.SetString("DeviceToken", e.Token);
    //    //    PlayerPrefs.Save();
           
    //    //}



    //    #region Sign UP Work

    //    //        void DisplayIdentityProviders(Firebase.Auth.FirebaseAuth auth,   "riazyabood@gmail.com")

    //    //        {
    //    //            System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<string>> FetchProvidersForEmailAsync(
    //    // string email
    //    //)
    //    //            auth.FetchProvidersForEmailAsync().ContinueWith((authTask) =>
    //    //            {
    //    //                if (authTask.IsCanceled)
    //    //                {
    //    //                    Debug.Log("Provider fetch canceled.");
    //    //                }
    //    //                else if (authTask.IsFaulted)
    //    //                {
    //    //                    Debug.Log("Provider fetch encountered an error.");
    //    //                    Debug.Log(authTask.Exception.ToString());
    //    //                }
    //    //                else if (authTask.IsCompleted)
    //    //                {
    //    //                    Debug.Log("Email Providers:");
    //    //                    foreach (string provider in authTask.result)
    //    //                    {
    //    //                        Debug.Log(provider);
    //    //                    }
    //    //                }
    //    //            });
    //    //        }
    //    public void SignUp()
    //    {
    //        EmailUI.Loader.SetActive(true);
    //        string Pin = RandomCode.ToString();
           

    //        if (EmailUI.VerifyField.text == Pin)
    //        {
    //            auth.CreateUserWithEmailAndPasswordAsync(EmailUI.SignUpemail.text, EmailUI.SignUppassword.text).ContinueWithOnMainThread((System.Action<Task<FirebaseUser>>)(task =>
    //            {
    //                if (task.IsCanceled)
    //                {
    //                    Debug.Log("CreateUserWithEmailAndPasswordAsync is canceled");

    //                    EmailUI.SignUpconfirmPasswordwarningText.text = "Sign up Canceled due to weak connection";
    //                    return;

    //                }
    //                if (task.IsFaulted)
    //                {

    //                    Debug.Log("CreateUserWithEmailAndPasswordAsync in countered error:" + task.Exception);
    //                    EmailUI.SignUpconfirmPasswordwarningText.text = "Something went wrong";
    //                    return;
    //                }



    //                if (task.IsCompleted)
    //                {
                     
    //                    Debug.Log("Avatar used: " + AvatarUsedValue);
    //                    FirebaseUser newuser = task.Result;
    //                 //   Firebase.Auth.FirebaseUser newUser = task.Result;
                      
    //                    Debug.LogFormat("Firebase use created successfully :{0} ({1})", newuser.DisplayName, newuser.UserId);
    //                    // ReferencesHolder.playerPublicInfo.UserId = newuser.UserId;
    //                    ReferencesHolder.newUserId = newuser.UserId;
                       

    //                    if (AvatarUsedValue == true)
    //                    {
                           

    //                        DocumentReference docRef2 = db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);



    //                        var publicinfo = new PublicInfoDB
    //                        {
    //                            UserId = newuser.UserId,
    //                            UserName = EmailUI.SignUpUserNameField.text,
    //                            Coins = 1000,
    //                            AvatarUsed = AvatarUsedValue,
    //                            AvatarID = defaultAvatarID,
    //                            DeviceToken = PlayerPrefs.GetString("DeviceToken"),
    //                            PictureURL = "",
    //                            PictureName = "",
    //                            IsVipMember = false

    //                        };
    //                        docRef2.SetAsync(publicinfo).ContinueWithOnMainThread(task =>
    //                        {

    //                            Debug.Log("new user public Info added");
    //                            Signedup = true;
    //                            PlayerPrefs.SetString("UserId", newuser.UserId);
    //                            PlayerPrefs.SetInt("SignedUp", 1);
    //                            PlayerPrefs.SetString("Email", EmailUI.SignUpemail.text);
    //                            PlayerPrefs.Save();
    //                            GetHomeData();


    //                        });
    //                    }
    //                    else
    //                    {

    //                        Debug.Log("In else");

    //                        Texture2D copy = duplicateTexture(ReferencesHolder.playersAvatarTex);
    //                        byte[] textureBytes = copy.EncodeToPNG();
    //                        storageReference = storage.RootReference.Child(newuser.UserId + "/uploads/DisplayPic.png");
    //                        storageReference.PutBytesAsync(textureBytes)
    //                        .ContinueWithOnMainThread(((Task<StorageMetadata> task) =>
    //                        {
    //                            if (task.IsFaulted || task.IsCanceled)
    //                            {
    //                                Debug.Log(task.Exception.ToString());

    //                            }
    //                            else
    //                            {
    //                                // Metadata contains file metadata such as size, content-type, and md5hash.
    //                                StorageMetadata metadata = task.Result;
    //                                string md5Hash = metadata.Md5Hash;

    //                                Debug.Log("Finished uploading...");
    //                                Debug.Log("md5 hash = " + md5Hash);
    //                                var pictureName = newuser.UserId + RemoveSpecialCharacters(md5Hash);

    //                                storageReference.GetDownloadUrlAsync().ContinueWithOnMainThread((System.Action<Task<System.Uri>>)(task =>
    //                                {
    //                                    if (task.IsCompleted)
    //                                    {
    //                                        DpURL = task.Result.ToString();
    //                                        DocumentReference docRef2 = db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);


    //                                        var publicinfo = new PublicInfoDB
    //                                        {
    //                                            UserId = newuser.UserId,
    //                                            UserName = EmailUI.SignUpUserNameField.text,
    //                                            Coins = 1000,
    //                                            AvatarUsed = AvatarUsedValue,
    //                                            AvatarID = defaultAvatarID,
    //                                            PictureURL = DpURL,
    //                                            PictureName = pictureName,
    //                                            DeviceToken = PlayerPrefs.GetString("DeviceToken"),
    //                                            IsVipMember = false
    //                                        };
    //                                        docRef2.SetAsync((object)publicinfo).ContinueWithOnMainThread(task =>
    //                                        {

    //                                            Debug.Log("new user public Info added");
    //                                            SaveInDiskCallBackFunction(pictureName);
    //                                            Signedup = true;
    //                                            PlayerPrefs.SetString(ReferencesHolder.EmailUserId, newuser.UserId);
    //                                            PlayerPrefs.SetInt(ReferencesHolder.EmailSignedUp, 1);
    //                                            PlayerPrefs.SetString("Email", EmailUI.SignUpemail.text);
    //                                            PlayerPrefs.Save();
    //                                            GetHomeData();


    //                                        });
    //                                    }
    //                                }));



    //                            }
    //                        }));
    //                    }

    //                    DocumentReference docRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_Stats_Doc);
    //                    var stats = new StatsDB
    //                    {
    //                        ClassicBorrayWin = 0,
    //                        ClassicBorrayLoss = 0,
    //                        SpeedBetWin = 0,
    //                        SpeedBetLoss = 0,
    //                        FullHouseWin = 0,
    //                        FullHouseLoss = 0,
    //                        TournamentWin = 0,
    //                        TournamentLoss = 0

    //                    };
    //                    docRef.SetAsync(stats).ContinueWithOnMainThread(task =>
    //                    {
    //                        Debug.Log("new email and password user info has been added");
                            


    //                    });
    //                    DocumentReference docRef3 = db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_DeckSkins_Doc);

    //                     db.Collection(ReferencesHolder.FS_users_Collec).Document(newuser.UserId).SetAsync(new Dictionary<string, object> { { "token", PlayerPrefs.GetString("DeviceToken") } });

    //                    DeckSkinDB deckSkinData = new DeckSkinDB()
    //                    {
    //                        Skins = defaultUnlockedSkinIDs.ToArray()
    //                    };
                        
    //                    //Dictionary<string, object> docData = new Dictionary<string, object>
    //                    //{

    //                    //   { "Skins", new List<object>() { "000","001" } },


    //                    //};

    //                    docRef3.SetAsync(deckSkinData);


    //                    ReferencesHolder.Provider = "Email";



    //                }



    //            }));
    //        }
    //        else
    //        {
    //            Debug.Log("wrong pincode");
    //            if (EmailUI.VerifyField.text =="")
    //            {
    //                EmailUI.VerifyPinwarningText.text = "Pin code required";
    //            }
    //            else 
    //            {
    //                EmailUI.VerifyPinwarningText.text = " Wrong Pin code";
    //                EmailUI.Loader.SetActive(false);
    //            }
    //        }

    //    }
      
    //    public void SendVerifyEmail()
    //    {
          
    //        bool validEmail;
    //        if (Regex.IsMatch(EmailUI.SignUpemail.text, MatchEmailPattern))
    //        {

    //            Debug.Log("valid ha");
    //            validEmail = true;
    //        }
    //        else
    //        {
    //            Debug.Log("Invalid ha");
    //            validEmail = false;
    //        }


    //        if (EmailUI.SignUpConfirmPasswordField.text != EmailUI.SignUppassword.text 
    //            || EmailUI.SignUppassword.text.Length < 6 
    //            || EmailUI.SignUpUserNameField.text == "" 
    //            || EmailUI.SignUpemail.text == null  
    //            || EmailUI.SignUppassword.text == null 
    //            || EmailUI.SignUpConfirmPasswordField.text == null 
    //            || validEmail == false)

    //        {
    //            Debug.Log("garbar");
    //            if (EmailUI.SignUpUserNameField.text == "")
    //            {
    //                EmailUI.UsernamewarningText.text = "name missing";
    //                Debug.Log("name missss");
    //            }
    //            if (EmailUI.SignUpemail.text == "")
    //            {
    //                EmailUI.EmailwarningText.text = "Email missing";
    //            }
    //            if (validEmail == false)
    //            {
    //                EmailUI.EmailwarningText.text = "Invalid email";
    //            }
    //            if (EmailUI.SignUppassword.text == "")
    //            {
    //                EmailUI.SignUpPasswordwarningText.text = "Password missing";
    //            }
    //            if (EmailUI.SignUpConfirmPasswordField.text == "")
    //            {
    //                EmailUI.SignUpconfirmPasswordwarningText.text = " Confirm Password missing";
    //            }
    //            if (EmailUI.SignUpConfirmPasswordField.text != EmailUI.SignUppassword.text && EmailUI.SignUpConfirmPasswordField.text != "" && EmailUI.SignUppassword.text != "")
    //            {

    //                EmailUI.SignUpconfirmPasswordwarningText.text = "Password does not match";
    //            }
    //            if(EmailUI.SignUppassword.text.Length < 6 && EmailUI.SignUppassword.text.Length > 0)
    //            {
    //                EmailUI.SignUpPasswordwarningText.text = "6 or more digits required";
    //            }

    //        }
    //        else
    //        {
    //           // Debug.Log("vaaaaaalue: " + EmailUI.SignUpemail.text);
    //            string maill = EmailUI.SignUpemail.text;
    //            auth.FetchProvidersForEmailAsync(maill).ContinueWithOnMainThread(fetchTask =>
    //            {
    //                if (fetchTask.IsCanceled|| fetchTask.IsFaulted)
    //                {
    //                    Debug.Log("provider work canceled");
    //                }
                    
    //                else 
    //                {
    //                    Debug.Log("Completed!");
    //                    //  Crashlytics.Log("CheckUserEmailExist Task Completed");

    //                    // bool isUserExist = false;

    //                    if (fetchTask.Result != null)
    //                    {
    //                        Debug.Log("fetchTask.Result");

    //                        int i = 0;

    //                        foreach (string provider in fetchTask.Result)
    //                        {
    //                            Debug.Log($"thats same {provider} and I = {i}" );
    //                            //Debug.Log("thats same" + fetchTask.Result);
    //                            EmailAlreadyExist = true;
    //                        }
    //                    }


    //                    if(EmailAlreadyExist)
    //                    {
    //                        // piche bhej do
    //                        // give error on email already exist
    //                        EmailUI.EmailwarningText.text = "this email already exists";
    //                        Debug.Log("this email already exist");
    //                        EmailAlreadyExist = false;

    //                    }
    //                    else
    //                    {
    //                        EmailUI.Loader.SetActive(true);
    //                        VerifyEmail(EmailUI.SignUpemail.text);
                            
    //                    }

                        

    //                    // onCheckEmailComplete.SafeInvoke(true, isUserExist);
    //                }
    //            });
                  
    //                //      MainUI.Loader.SetActive(false);


               




    //        }

    //    }

    //    private void VerifyEmail(string email)
    //    {
    //        Debug.Log("already exist false");
           
    //        MailMessage mail = new MailMessage();
    //        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
    //        SmtpServer.Timeout = 10000;
    //        SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
    //        SmtpServer.UseDefaultCredentials = false;
    //        SmtpServer.Port = 587;

    //        mail.From = new MailAddress("abudriaz92@gmail.com");
    //        mail.To.Add(new MailAddress(email));

    //        mail.Subject = "Verification Code for Borray";
    //        RandomCode = Random.Range(10000, 99999);
    //        mail.Body = $"Insert this Pin Code:{RandomCode.ToString()} in your mobile device to successfully verify your email address. ";


    //        SmtpServer.Credentials = new System.Net.NetworkCredential("booraydevteam@gmail.com", "loanoyxtjnaecdho") as ICredentialsByHost; SmtpServer.EnableSsl = true;
    //        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    //        {
    //            return true;
    //        };

    //        mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
    //        mail.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnSuccess;
    //        SmtpServer.Send(mail);

    //         EmailUI.VerifyEmailPanel.SetActive(true);
       
    //        Invoke("DelayInVerificationEmailSoundLoaderClose", 3f);
          
    //    }
    //    void DelayInVerificationEmailSoundLoaderClose()
    //    {
    //        EmailUI.Loader.SetActive(false);
    //    }
    //    void ReVerifyEmail()
    //    {
    //        Debug.Log("ResendPinButton Pressed");
    //        RandomCode = 0;
    //        StartCoroutine(Countdown());
    //        SendVerifyEmail();
            
    //    }
    //    private IEnumerator Countdown()
    //    {
    //        float duration = 10f; // 10 seconds you can change this to
    //                             //to whatever you want
    //        float totalTime = 0;
    //        while (totalTime <= duration)
    //        {
    //            ResendPinButton.enabled = false;
    //            EmailUI.ReverifyTimmerImg.fillAmount = totalTime / duration;
    //            totalTime += Time.deltaTime;
    //            var integer = (int)totalTime; 
    //            string timer = integer.ToString();
    //            EmailUI.ReVerifyTimmerText.text=timer;
               
    //            yield return null;
    //        }
    //        ResendPinButton.enabled = true;
    //    }

    //    #endregion

    //    #region Sign In Work
    //    void MainLoginButtonMethod()
    //    {
    //        EmailUI.MainCanvas.SetActive(false);
    //        EmailUI.LoginCanvas.SetActive(true);
    //    }
    //    public void LoginValidation()
    //    {
    //        Debug.Log("Login validatiiion");
    //        bool validEmail;
    //        if (Regex.IsMatch(EmailUI.SignInemail.text, MatchEmailPattern))
    //        {

    //            Debug.Log("valid ha");
    //            validEmail = true;
    //        }
    //        else
    //        {
    //            Debug.Log("Invalid ha");
    //            validEmail = false;
    //        }
    //        if (EmailUI.SignInemail.text == "" || EmailUI.SignInpassword.text == "" || validEmail == false || EmailUI.SignInpassword.text.Length < 6)
    //        {
    //            if (EmailUI.SignInemail.text == "")
    //            {
    //                EmailUI.SignInEmailwarningText.text = "Email missing";
    //            }
    //            if (EmailUI.SignInpassword.text == "")
    //            {
    //                EmailUI.SignInPasswordwarningText.text = "Password missing";
    //            }
    //            if (EmailUI.SignInemail.text != "" && validEmail == false)
    //            {
    //                EmailUI.SignInEmailwarningText.text = "Invalid email";
    //            }
    //            if (EmailUI.SignInpassword.text.Length < 6)
    //            {
    //                EmailUI.SignInPasswordwarningText.text = "For Password 6 or more digits required";
    //            }
    //        }
    //        else
    //        {
    //            EmailUI.Loader.SetActive(true);
    //            SignIn();
    //        }
    //    }
    //    public void SignIn()
    //    {
          
    //        auth.SignInWithEmailAndPasswordAsync(EmailUI.SignInemail.text, EmailUI.SignInpassword.text).ContinueWithOnMainThread(task => {
    //            if (task.IsCanceled)
    //            {
    //                // Debug.Log("SignInWithEmailAndPasswordAsync was canceled.");
    //                Debug.Log("-------------->SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
    //                Debug.Log("Login canceled");
    //                EmailUI.SignInPasswordwarningText.text = "Login canceled";
    //                EmailUI.Loader.SetActive(false);
    //                return;
    //            }
    //            if (task.IsFaulted)
    //            {
    //                //  Debug.Log("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
    //                Debug.Log("------------->Wrong password or Email or weak internet");
    //                EmailUI.SignInPasswordwarningText.text = "Wrong password or Email";
    //                EmailUI.Loader.SetActive(false);
    //                return;
    //            }
    //            if (task.IsCompleted)
    //            {

    //                ReferencesHolder.Provider = "Email";
    //                Firebase.Auth.FirebaseUser newUser = task.Result;
    //                Debug.LogFormat("User signed in successfully: {0} ({1})",
    //                newUser.DisplayName, newUser.UserId);
    //                LoginUserId = newUser.UserId;
                   
    //                //ReferencesHolder.playerPublicInfo.UserId = newUser.UserId;
    //                ReferencesHolder.newUserId = newUser.UserId;
    //                // EmailUI.LoginCanvas.SetActive(false);
    //                // EmailUI.HomeCanvas.SetActive(true);
    //                PlayerPrefs.SetInt(ReferencesHolder.EmailSignedUp, 1);
    //                PlayerPrefs.SetString(ReferencesHolder.EmailUserId, newUser.UserId);
    //                //PlayerPrefs.SetString("Email", EmailUI.SignInemail.text);
    //                PlayerPrefs.Save();
    //                Debug.Log("Signed in user id >>>>>>> " + newUser.UserId);
    //                Debug.Log("Signed in user Token >>>>>>> " + PlayerPrefs.GetString("DeviceToken"));
    //                db.Collection(ReferencesHolder.FS_users_Collec).Document(newUser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).UpdateAsync("DeviceToken", PlayerPrefs.GetString("DeviceToken"));
    //                db.Collection(ReferencesHolder.FS_users_Collec).Document(newUser.UserId).UpdateAsync("token", PlayerPrefs.GetString("DeviceToken"));
    //                GetHomeData();

    //            }


    //        });

    //    }
    //    #endregion

    //    #region Change Password Work
    //    //password change work comment below
    //    //public void ChangePasswordClearWarning()
    //    //{
    //    //    MainUI.HomeUI.OldwarningText.text = "";
    //    //    MainUI.HomeUI.NewwarningText.text = "";
    //    //    MainUI.HomeUI.ConfirmwarningText.text = "";
    //    //    MainUI.HomeUI.ChangePasswordFailedText.text = "";

    //    //}
    //    //public void ValidateChangePassword()
    //    //{


    //    //    auth.SignInWithEmailAndPasswordAsync(PlayerPrefs.GetString("Email"), MainUI.HomeUI.ChangePanelOldPasswordField.text).ContinueWithOnMainThread(task =>
    //    //    {
    //    //        if (task.IsCanceled)
    //    //        {
    //    //            // Debug.Log("SignInWithEmailAndPasswordAsync was canceled.");
    //    //            Debug.Log("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
    //    //            Debug.Log("Login canceled");

    //    //            return;
    //    //        }
    //    //        if (task.IsFaulted)
    //    //        {

    //    //            Debug.Log("Wrong password or Email or weak internet");
    //    //            MainUI.HomeUI.OldwarningText.text = "Wrong old password";
    //    //            return;
    //    //        }
    //    //        if (task.IsCompleted)
    //    //        {

    //    //            if (MainUI.HomeUI.ChangePanelOldPasswordField.text == "" || MainUI.HomeUI.ChangePanelNewPasswordField.text == "" || MainUI.HomeUI.ChangePanelConfirmPasswordField.text == "" || MainUI.HomeUI.ChangePanelNewPasswordField.text.Length < 6 || MainUI.HomeUI.ChangePanelNewPasswordField.text != MainUI.HomeUI.ChangePanelConfirmPasswordField.text)
    //    //            {
    //    //                if (MainUI.HomeUI.ChangePanelOldPasswordField.text == "")
    //    //                {
    //    //                    MainUI.HomeUI.OldwarningText.text = "Old password missing";
    //    //                }
    //    //                if (MainUI.HomeUI.ChangePanelNewPasswordField.text == "")
    //    //                {
    //    //                    MainUI.HomeUI.NewwarningText.text = "New password missing";
    //    //                }
    //    //                if (MainUI.HomeUI.ChangePanelConfirmPasswordField.text == "")
    //    //                {
    //    //                    MainUI.HomeUI.ConfirmwarningText.text = "Confirm password missing";
    //    //                }
    //    //                if (MainUI.HomeUI.ChangePanelNewPasswordField.text.Length < 6)
    //    //                {
    //    //                    MainUI.HomeUI.NewwarningText.text = "The password should be of 6 or more digit";
    //    //                }
    //    //                if (MainUI.HomeUI.ChangePanelNewPasswordField.text != MainUI.HomeUI.ChangePanelConfirmPasswordField.text)
    //    //                {
    //    //                    MainUI.HomeUI.ConfirmwarningText.text = "Value not match with new password";
    //    //                }






    //    //            }
    //    //            else
    //    //            {
    //    //                PasswordChange();
    //    //            }

    //    //        }


    //    //    });





    //    // }
    //    //public void PasswordChange()
    //    //{
    //    //    Firebase.Auth.FirebaseUser user = auth.CurrentUser;
    //    //    string newPassword = MainUI.HomeUI.ChangePanelNewPasswordField.text;
    //    //    if (user != null)
    //    //    {
    //    //        user.UpdatePasswordAsync(newPassword).ContinueWith(task =>
    //    //        {
    //    //            if (task.IsCanceled)
    //    //            {
    //    //                Debug.LogFormat("UpdatePasswordAsync was canceled.");
    //    //                return;
    //    //            }
    //    //            if (task.IsFaulted)
    //    //            {
    //    //                Debug.LogFormat("UpdatePasswordAsync encountered an error: " + task.Exception);
    //    //                return;
    //    //            }



    //    //        }).ContinueWithOnMainThread(task =>
    //    //        {
    //    //            DocumentReference docRef = db.Collection("users").Document(PlayerPrefs.GetString("UserId")).Collection("PrivateData").Document(PlayerPrefs.GetString("UserId"));
    //    //            Dictionary<string, object> user = new Dictionary<string, object>
    //    //                {
    //    //                          { "Password", MainUI.HomeUI.ChangePanelNewPasswordField.text },

    //    //                 };
    //    //            docRef.SetAsync(user).ContinueWithOnMainThread(task =>
    //    //            {

    //    //                if (task.IsFaulted)
    //    //                {
    //    //                    MainUI.HomeUI.ChangePasswordFailedText.text = "Something went wrong password could be changed";
    //    //                }
    //    //                if (task.IsCompleted)
    //    //                {
    //    //                    MainUI.HomeUI.ChangePasswordPanel.SetActive(false);
    //    //                   // LogOut();
    //    //                    MainUI.HomeUI.ChangePanelOldPasswordField.text = "";


    //    //                    MainUI.HomeUI.ChangePanelNewPasswordField.text = "";


    //    //                    MainUI.HomeUI.ChangePanelConfirmPasswordField.text = "";

    //    //                }

    //    //                Debug.Log("Added data to the aturing document in the users collection.");
    //    //            });
    //    //        });
    //    //    }
    //    //}
    //    #endregion

    //    #region Forget Password Work
    //    public void ForgetPasswordValidation()
    //    {
    //        bool validEmail;
    //        if (Regex.IsMatch(EmailUI.ForgetPasswordEmailField.text, MatchEmailPattern))
    //        {

    //            Debug.Log("valid ha");
    //            validEmail = true;
    //        }
    //        else
    //        {
    //            Debug.Log("Invalid ha");
    //            validEmail = false;
    //        }
    //        if (EmailUI.ForgetPasswordEmailField.text == "" || validEmail == false)
    //        {
    //            if (EmailUI.ForgetPasswordEmailField.text == "")
    //            {
    //                EmailUI.ForgetPasswordEmailWarningText.text = "Email address required";
    //            }
    //            else if (EmailUI.ForgetPasswordEmailField.text != "" && validEmail == false)
    //            {
    //                EmailUI.ForgetPasswordEmailWarningText.text = "Invalid Email";
    //            }
    //        }
    //        else
    //        {
    //            PasswordReset();
    //        }
            
    //    }
    //    public void PasswordReset()
    //    {
    //        string emailAddress = EmailUI.ForgetPasswordEmailField.text;
    //        EmailUI.ForgetPasswordGuideText.text = "Wait Sending Email..";
    //        auth.SendPasswordResetEmailAsync(emailAddress).ContinueWithOnMainThread(task => {
    //            if (task.IsCanceled)
    //            {
                    
    //                EmailUI.ForgetPasswordGuideText.text = "Something went wrong please check your connection";
                    
    //            }
    //            if (task.IsFaulted)
    //            {
                   
    //                EmailUI.ForgetPasswordGuideText.text = "Email could not be found";
                    
    //            }
    //            if (task.IsCompleted)
    //            {
    //                EmailUI.ForgetPasswordGuideText.text = "Email has been send press the link in email to change password";
    //                Debug.Log("Password reset email sent successfully.");
    //                EmailUI.ClearSignInWarnings();
    //                EmailUI.ClearSignInTextFields();
    //                Invoke("DelayHideForgetPanel", 4);
                   
    //            }

                
    //        });

    //    }
    //    public void DelayHideForgetPanel()
    //    {
    //        EmailUI.ForgetPasswordPanel.SetActive(false);
    //    }
    //    #endregion
    //    public void GetHomeData()
    //    {
    //        ReferencesHolder.Provider = "Email";
    //        EmailUI.Loader.SetActive(true);
    //        db.Collection(ReferencesHolder.FS_users_Collec).Document(PlayerPrefs.GetString(ReferencesHolder.EmailUserId)).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_DeckSkins_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
    //        {
    //            Debug.Log("agya skin1");
    //            if (task.IsCanceled || task.IsFaulted)
    //            {
    //                EmailUI.Loader.SetActive(false);
    //                EmailUI.MainCanvas.SetActive(true);
    //                EmailUI.LoginCanvas.SetActive(false);
    //                EmailUI.RegisterCanvas.SetActive(false);

    //            }
    //            if (task.IsCompleted)
    //            {
    //                if (task.Result.Exists)
    //                {
    //                    Debug.Log("agya skin2");
    //                    DeckSkinDB Skins = task.Result.ConvertTo<DeckSkinDB>();
    //                 // Debug.Log(Skins.Skins);
    //                    Debug.Log("agya skin3");
    //                    ReferencesHolder.AvailableSkins = Skins;
    //                }
    //                else
    //                {
    //                    Debug.Log("Wrong path");
    //                }


    //            }

    //        });


    //        db.Collection(ReferencesHolder.FS_users_Collec).Document(PlayerPrefs.GetString(ReferencesHolder.EmailUserId)).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
    //        {
    //            if (task.IsCanceled || task.IsFaulted)
    //            {
    //                EmailUI.Loader.SetActive(false);
    //                EmailUI.MainCanvas.SetActive(true);
    //                EmailUI.LoginCanvas.SetActive(false);
    //                EmailUI.RegisterCanvas.SetActive(false);

    //            }
               
    //            if (task.IsCompleted)
    //            {
    //                if (task.Result.Exists)
    //                {

    //                    PublicInfoDB Info = task.Result.ConvertTo<PublicInfoDB>();
    //                    ReferencesHolder.playerPublicInfo = Info;
    //                    //Debug.Log("yay ha naam" + Info.UserName);
    //                    //ReferencesHolder.userName = Info.UserName;
    //                    //ReferencesHolder.Coins = Info.Coins.ToString();
    //                    //ReferencesHolder.AvatarUsed = Info.AvatarUsed;
    //                    //ReferencesHolder.AvatarID = Info.AvatarID;
    //                    //ReferencesHolder.AvatarUniqueName = Info.PictureName;
    //                    //ReferencesHolder.AvatarUploadPath = Info.PictureURL;
    //                    //ReferencesHolder.userID = Info.UserId;

    //                    Debug.Log($"");

    //                    if (Signedup == false&&Info.AvatarUsed==false)
    //                    {


    //                        bool check = CheckIfTextureExists(ReferencesHolder.playerPublicInfo.PictureName + ".png");
    //                        Debug.Log("cccccccccccccccchhheeck:" + check);
    //                        Debug.Log(ReferencesHolder.playerPublicInfo.PictureName);

    //                        if (check == true)
    //                        {
    //                            string path = Path.Combine(pathOfTextures, Info.PictureName + ".png");
    //                            StartCoroutine(LoadLocalTexture("file://" + path));
    //                        }
    //                        else
    //                        {
    //                            if(Info.PictureURL==""|| Info.PictureURL == null)
    //                            {
                                    
                                               
    //                                db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).UpdateAsync("AvatarUsed",true);
    //                                db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).UpdateAsync("AvatarID",defaultAvatarID);
    //                                ReferencesHolder.AvatarUsed = true;
    //                                OpenHomeScreen();
    //                            }
    //                            else
    //                            {
    //                                StartCoroutine(LoadImage(Info.PictureURL));
    //                            }
                               

    //                            Debug.Log("awww");
    //                        }


    //                    }
    //                    else
    //                    {
    //                        OpenHomeScreen();
    //                    }





    //                    }
    //                    else
    //                    {
    //                       if (Security == true)
    //                       {
    //                         EmailUI.MainCanvas.SetActive(true);
    //                         EmailUI.Loader.SetActive(false);
    //                         EmailUI.LoginCanvas.SetActive(false);
    //                         EmailUI.RegisterCanvas.SetActive(false);
    //                       }
    //                       else
    //                       {
    //                        DocumentReference docRef2 = db.Collection(ReferencesHolder.FS_users_Collec).Document(PlayerPrefs.GetString(ReferencesHolder.EmailUserId)).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);
    //                        var publicinfo = new PublicInfoDB
    //                        {
    //                            UserId = PlayerPrefs.GetString(ReferencesHolder.EmailUserId),
    //                            UserName = "DummyUser",
    //                            Coins = 1000,
    //                            AvatarUsed = AvatarUsedValue,
    //                            AvatarID = defaultAvatarID,
    //                            PictureURL = "",
    //                            PictureName ="",
                               
    //                        };
    //                        docRef2.SetAsync(publicinfo).ContinueWithOnMainThread(task =>
    //                        {
    //                            Debug.Log("new email and password user info has been added");
    //                            ReferencesHolder.playerPublicInfo = publicinfo;
                             

    //                        });
    //                        DocumentReference docRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(PlayerPrefs.GetString(ReferencesHolder.EmailUserId)).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_Stats_Doc);
    //                        var stats = new StatsDB
    //                        {
    //                            ClassicBorrayWin = 0,
    //                            ClassicBorrayLoss = 0,
    //                            SpeedBetWin = 0,
    //                            SpeedBetLoss = 0,
    //                            FullHouseWin = 0,
    //                            FullHouseLoss = 0,
    //                            TournamentWin = 0,
    //                            TournamentLoss = 0

    //                        };
    //                        docRef.SetAsync(stats).ContinueWithOnMainThread(task =>
    //                        {
    //                            Debug.Log("new email and password user info has been added");

    //                            OpenHomeScreen();

    //                        });
    //                    }



    //                }

    //                }
                
               
               




    //        });
           
         

    //    }
     

    //    private void OpenHomeScreen()
    //    {


    //        Debug.Log("Open Home scene...");
    //       // UpdateStuff();
    //        SceneManager.LoadScene(ReferencesHolder.mainMenuSceneIndex);
           
           

    //    }

    //    #region DeckSkinUpdate
    //    async void UpdateStuff()
    //    {
    //        DocumentReference docref = db.Collection(ReferencesHolder.FS_users_Collec).Document(PlayerPrefs.GetString(ReferencesHolder.EmailUserId)).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_DeckSkins_Doc);
    //        await docref.UpdateAsync("Skins", FieldValue.ArrayUnion("001"));

            
    //    }
    //    #endregion


    //    #region Display Image Work
    //    //below method for loading image from local persistent data path
    //    private IEnumerator LoadLocalTexture(string path)
    //    {
    //        Debug.Log("Load local Texture");
           
            
    //        Debug.Log(path);
            
                
    //            Debug.Log("FILE EXITS");
    //            UnityWebRequest request = UnityWebRequestTexture.GetTexture(path);
    //            yield return request.SendWebRequest();
                
    //            if (request.result == UnityWebRequest.Result.ConnectionError)
    //            {
    //                Debug.Log(request.error);
    //            }
    //            else
    //            {
    //                Debug.Log("Local image load");

    //            var tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
    //            yield return new WaitForEndOfFrame();
    //            var spriteImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

    //            ReferencesHolder.playersAvatarTex = tex;
    //            ReferencesHolder.playersAvatarSprite = spriteImage;

    //            AvatarUsedValue = false;

    //                OpenHomeScreen();
                    
    //            }

    //    }
    //    //below method for loading image from firebase cloud storage
    //    IEnumerator LoadImage(string MediaUrl)
    //    {
    //        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Create a request
    //        yield return request.SendWebRequest(); //Wait for the request to complete
    //        if (request.result == UnityWebRequest.Result.ConnectionError)
    //        {
    //            Debug.Log(request.error);
    //        }
    //        else
    //        {


    //            var tex = ((DownloadHandlerTexture)request.downloadHandler).texture;

    //            var spriteImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

    //            ReferencesHolder.playersAvatarTex = tex;
    //            ReferencesHolder.playersAvatarSprite = spriteImage;



    //            AvatarUsedValue = false;
    //            SaveInDiskCallBackFunction();
    //            OpenHomeScreen();


    //        }
    //    }
    //    private void PickImage(int maxSize)
    //    {
    //        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
    //        {
    //            Debug.Log("Image path: " + path);
    //            ReferencesHolder.AvatarUploadPath = path;
    //            if (path != null)
    //            {

    //                ReferencesHolder.AvatarUsed = false;
    //                AvatarUsedValue = false;
    //                // Create Texture from selected image
    //                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
                    
    //                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

    //                if (texture == null)
    //                {
    //                    Debug.Log("Couldn't load texture from " + path);
    //                    return;
    //                }
    //                EmailUI.SignUpDisplayPic.texture = texture;
    //                ReferencesHolder.playersAvatarTex = texture;
    //                ReferencesHolder.playersAvatarSprite = sprite;
    //            }
    //        });

    //        Debug.Log("Permission result: " + permission);
    //    }



    //    public void SaveImageOnDisk(Texture2D texture, string fileName)
    //    {

    //        //string path = pathOfTextures + "/" + fileName;
    //        Debug.Log("save image on disk ");
    //        string path = Path.Combine(pathOfTextures, fileName + ".png");
    //        ReferencesHolder.PersistentPath = path;
    //        ReferencesHolder.AvatarUploadPath = path;
    //        Debug.Log(" persistent path of Image:" + path);
    //        if (!Directory.Exists(pathOfTextures))
    //        {
    //            Debug.Log("save image on disk directory exist check");
    //            var folder = Directory.CreateDirectory(pathOfTextures); // returns a DirectoryInfo object

    //        }



    //        if (texture is null)
    //        {
    //            //    Logging.Log("No Image To save");
    //            Debug.Log("save image on disk null texture check");
    //            return;
    //        }

    //        Texture2D copy = duplicateTexture(texture);
    //        byte[] textureBytes = copy.EncodeToPNG();

    //        File.WriteAllBytes(path, textureBytes);

    //        Debug.Log("save image on disk texture saved");
    //        // Logging.Log(" File Written On Disk ");

    //    }
    //    public bool CheckIfTextureExists(string fileName)
    //    {

    //        string path = pathOfTextures + "/" + fileName;

    //        if (File.Exists(path))
    //        {
    //            return true;

    //        }
    //        return false;
    //    }
    //    Texture2D duplicateTexture(Texture2D source)
    //    {
    //        RenderTexture renderTex = RenderTexture.GetTemporary(
    //            source.width,
    //            source.height,
    //                        0,
    //            RenderTextureFormat.Default,
    //            RenderTextureReadWrite.Linear);

    //        Graphics.Blit(source, renderTex);
    //        RenderTexture previous = RenderTexture.active;
    //        RenderTexture.active = renderTex;
    //        Texture2D readableText = new Texture2D(source.width, source.height);
    //        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
    //        readableText.Apply();
    //        RenderTexture.active = previous;
    //        RenderTexture.ReleaseTemporary(renderTex);
    //        return readableText;

    //    }
    //    void OpenGalleryButtonMethod()
    //    {
    //        PickImage(512);
    //    }
    //    //below method to generate unique name for image with no special characters
    //    public string RemoveSpecialCharacters(string str)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        for (int i = 0; i < str.Length; i++)
    //        {
    //            if ((str[i] >= '0' && str[i] <= '9')
    //                || (str[i] >= 'A' && str[i] <= 'z'
    //                    || (str[i] == '.' || str[i] == '_')))
    //            {
    //                sb.Append(str[i]);
    //            }
    //        }

    //        return sb.ToString();
    //    }
    //    void SaveInDiskCallBackFunction()
    //    {
            
            
    //            Texture2D copy = duplicateTexture(ReferencesHolder.playersAvatarTex);
    //            SaveImageOnDisk(copy, ReferencesHolder.playerPublicInfo.PictureName);
            
           
    //    }
    //    void SaveInDiskCallBackFunction(string pictureName)
    //    {


    //        Texture2D copy = duplicateTexture(ReferencesHolder.playersAvatarTex);
    //        SaveImageOnDisk(copy, pictureName);



    //    }
    }

    //#endregion

}




