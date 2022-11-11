using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Storage;
using TMPro;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

namespace Booray.Auth
{

    public class EditProfileUIManager : MonoBehaviour
    {




        FirebaseAuth auth;
        FirebaseFirestore db;
        FirebaseStorage storage;
        StorageReference storageReference;

        [SerializeField] GameObject editProfilePanel;
        public GameObject ChangePasswordAlert;
        public GameObject ConnectionAlertPanel;

        [SerializeField] Button editProfileBtn;
        [SerializeField] Button backBtn;

        [Header("Change Password")]

        [SerializeField] GameObject changePassPanel;

        [SerializeField] TMP_InputField oldPassIF;
        [SerializeField] TextMeshProUGUI oldWarningTxt;

        [SerializeField] TMP_InputField newPassIF;
        [SerializeField] TextMeshProUGUI newPassWarningTxt;

        [SerializeField] TMP_InputField confPassIF;
        [SerializeField] TextMeshProUGUI confPassWarningTxt;

        [SerializeField] Button changePasswordBtn;

        [SerializeField] Button saveBtn;

        [SerializeField] TextMeshProUGUI changePassStateTxt;

        [Header("Avatar")]

        [SerializeField] GameObject changeAvatarPanel;

        [SerializeField] Button useAvatarBtn;

        
        [SerializeField] List<Button> avatarButton_List;

        [SerializeField] SpriteAtlas avatarSpriteAtlus;

        [Header("Profile Image")]



        [SerializeField] Button changePicBtn;

       // public RawImage DisplayPic;
        public Image spriteDisplayPic;

        private string DpURL;
        private string pathOfTextures;

        


        private void Awake()
        {
            auth = FirebaseAuth.DefaultInstance;
            
            db = FirebaseFirestore.DefaultInstance;
            storage = FirebaseStorage.DefaultInstance;
            storageReference = storage.GetReferenceFromUrl("gs://ultimate-booray.appspot.com");

            pathOfTextures = Application.persistentDataPath + "/WebTexturesFolder";
        }


        private void Start()
        {
            MethodSubscriptions();
           
            if (ReferencesHolder.Provider == "Email")
            {
                changePasswordBtn.gameObject.SetActive(true);
                changePicBtn.gameObject.SetActive(true);

            }
            else if(ReferencesHolder.Provider == "Guest")
            {
                changePasswordBtn.gameObject.SetActive(false);
                changePicBtn.gameObject.SetActive(false);
            }
            else
            {
                changePicBtn.gameObject.SetActive(true);
                changePasswordBtn.gameObject.SetActive(false);
            }


            //if (ReferencesHolder.Provider != "Email")
            //{
            //    changePasswordBtn.gameObject.SetActive(false);
            //}
            //else
            //{
            //    changePasswordBtn.gameObject.SetActive(true);
            //}
            //if (ReferencesHolder.Provider == "Guest")
            //{
            //    changePicBtn.gameObject.SetActive(false);
            //}
            //else
            //{
            //    changePicBtn.gameObject.SetActive(true);
            //}
        }

        private void MethodSubscriptions()
        {
            changePasswordBtn.onClick.AddListener(delegate { ShowChangPasswordPanel(); SFXHandler.instance.PlayBtnClickSFX(); });
            saveBtn.onClick.AddListener(delegate { EditPassword(); SFXHandler.instance.PlayBtnClickSFX(); });
            changePicBtn.onClick.AddListener(delegate { OpenGalleryButtonMethod(); ClearChangePasswordTextFields(); ClearChangePasswordWarnings(); SFXHandler.instance.PlayBtnClickSFX(); });
            useAvatarBtn.onClick.AddListener(delegate { OpenAvatarPanel(); SFXHandler.instance.PlayBtnClickSFX(); });

            editProfileBtn.onClick.AddListener(delegate { ShowEditProfilePanel(); });
            backBtn.onClick.AddListener(delegate { ClosePanel(); });
            //AvatarBackButton.onClick.AddListener(delegate { AvatarPanelBackMethod(); SFXHandler.instance.PlayBtnClickSFX(); });

            oldPassIF.onSelect.AddListener(delegate { oldWarningTxt.gameObject.SetActive(false); });
            newPassIF.onSelect.AddListener(delegate { newPassWarningTxt.gameObject.SetActive(false); });
            confPassIF.onSelect.AddListener(delegate { confPassWarningTxt.gameObject.SetActive(false); });

        }

        public void ShowEditProfilePanel()
        {
            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
                MainUIManager.Instance.HomeUI.DeactiveHomePanel();
                MainUIManager.Instance.HomeUI.ChangePasswordPanel.SetActive(true);
                // MainUIManager.Instance.HomeUI.BackButton.gameObject.SetActive(true);
                editProfilePanel.SetActive(true);
            }
            else
            {
               // LogErrorUIHandler.instance.OpenErrorPanel("Internet not reachable or connected");
            }
           

        }

        public void DeactivatePanel()
        {
            editProfilePanel.SetActive(false);
           
            ResetPanel();

        }

        public void ClosePanel()
        {
            editProfilePanel.SetActive(false);
            MainUIManager.Instance.HomeUI.HomePanel.SetActive(true);
            ResetPanel();
        }

        public void ResetPanel()
        {
            changeAvatarPanel.SetActive(false);
            changePassPanel.SetActive(false);
        }

        #region Change Password Functions

        public void ShowChangPasswordPanel()
        {

            changeAvatarPanel.SetActive(false);
            changePassPanel.SetActive(true);

        }

        bool ValidateFields()
        {
            bool allOk = true;

            if(string.IsNullOrEmpty(oldPassIF.text))
            {
                oldWarningTxt.gameObject.SetActive(true);
                oldWarningTxt.text = "Please Enter your Old Password";
                allOk = false;
            }

            if(string.IsNullOrEmpty(newPassIF.text))
            {
                newPassWarningTxt.gameObject.SetActive(true);
                newPassWarningTxt.text = "Please Enter your New Password";
                allOk = false;
            }
            else if(newPassIF.text.Length < 6)
            {
                newPassWarningTxt.gameObject.SetActive(true);
                newPassWarningTxt.text = "password should be of 6 or more characters";
                allOk = false;
            }

            if (string.IsNullOrEmpty(confPassIF.text))
            {
                confPassWarningTxt.gameObject.SetActive(true);
                confPassWarningTxt.text = "Please Confirm your New Password";
                allOk = false;
            }

            else if(!newPassIF.text.Equals(confPassIF.text))
            {


                confPassWarningTxt.gameObject.SetActive(true);
                confPassWarningTxt.text = "Does Not Match your new Password";
                allOk = false;
            }

            return allOk;
        }

        void EditPassword()
        {
            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                if (!ValidateFields())
                {
                    return;
                }


                Debug.Log("entered edit password");
                FirebaseUser user = auth.CurrentUser;
                //Debug.Log("yay ha bhai email ager ha tooo "PlayerPrefs.GetString("Email"));
                // Get auth credentials from the user for re-authentication. The example below shows
                // email and password credentials but there are multiple possible providers,
                // such as GoogleAuthProvider or FacebookAuthProvider.
                Credential credential =
                EmailAuthProvider.GetCredential(auth.CurrentUser.Email, oldPassIF.text);

                if (user != null)
                {
                    user.ReauthenticateAsync(credential).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCanceled)
                        {
                            // Debug.LogError("ReauthenticateAsync was canceled.");
                            return;
                        }
                        if (task.IsFaulted)
                        {
                            oldWarningTxt.gameObject.SetActive(true);
                            oldWarningTxt.text = "wrong password";
                            //  Debug.LogError("ReauthenticateAsync encountered an error: " + task.Exception);
                            return;
                        }
                        else
                        {
                            Debug.Log("User reauthenticated successfully.");
                            PasswordChange();


                        }


                    });
                }
            }
            else
            {
              //  LogErrorUIHandler.instance.OpenErrorPanel("Internet not reachable or connected");
            
            }
           
        }
        // password change work comment below
        public void ChangePasswordClearWarning()
        {
            oldWarningTxt.gameObject.SetActive(false);
            newPassWarningTxt.gameObject.SetActive(false);
            confPassWarningTxt.gameObject.SetActive(false);

            oldWarningTxt.text = "";
            newPassWarningTxt.text = "";
            confPassWarningTxt.text = "";
            changePassStateTxt.text = "";

        }

        public void PasswordChange()
        {
            Firebase.Auth.FirebaseUser user = auth.CurrentUser;
            string newPassword = newPassIF.text;
            if (user != null)
            {
                user.UpdatePasswordAsync(newPassword).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogFormat("UpdatePasswordAsync was canceled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogFormat("UpdatePasswordAsync encountered an error: " + task.Exception);
                        return;
                    }




                }).ContinueWithOnMainThread(task =>
                {




                    if (task.IsFaulted || task.IsCanceled)
                    {
                        changePassStateTxt.text = "Something went wrong... Please Try Again!";
                    }
                    else
                    {
                       // MainUIManager.Instance.HomeUI.ChangePasswordPanel.SetActive(false);

                        oldPassIF.text = "";


                        newPassIF.text = "";


                        confPassIF.text = "";

                        changePassStateTxt.text = "";
                        Debug.Log("Password changed with success");
                        ChangePasswordAlert.SetActive(true);
                        
                        Invoke("AfterPasswordChanged", 3.5f);

                    }



                });
            }
        }
        public void ClearChangePasswordTextFields()
        {
            oldPassIF.text = "";
            newPassIF.text = "";
            confPassIF.text = "";
            changePassStateTxt.text = "";
        }
        public void ClearChangePasswordWarnings()
        {
            oldWarningTxt.text = "";
            newPassWarningTxt.text = "";
            confPassWarningTxt.text = "";
        }

        public void AfterPasswordChanged()
        {
          //  MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
            ChangePasswordAlert.SetActive(false);
            MainUIManager.Instance.SettingUI.LogOut();
        }
        #endregion

        #region Change Image Functions && AVATAR

        public void OpenAvatarPanel()
        {
            changeAvatarPanel.SetActive(true);
            ClearChangePasswordTextFields();
            ClearChangePasswordWarnings();
            changePassPanel.SetActive(false);

        }

        public void AvatarPanelBackMethod()
        {
            changeAvatarPanel.SetActive(false);
        }

        void OpenGalleryButtonMethod()
        {
           LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                PickImage(512);
              
            }
            else
            {
                Debug.Log("WIFI NOT CONNECTED!");
                ConnectionAlertPanel.SetActive(true);
                ConnectionAlertPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Please connect to internet first!";
              //  LogErrorUIHandler.instance.OpenErrorPanel("Connection not reachable or connected");
            }
           
        }

        Texture2D duplicateTexture(Texture2D source)
        {
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


        private void PickImage(int maxSize)
        {
            NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
            {
                Debug.Log("Image path: " + path);
                ReferencesHolder.AvatarUploadPath = path;
                if (path != null)
                {

                    ReferencesHolder.AvatarUsed = false;
                    //  AvatarUsedValue = false;
                    // Create Texture from selected image
                    Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);

                    var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                    if (texture == null)
                    {
                        Debug.Log("Couldn't load texture from " + path);
                        return;
                    }
                    //  DisplayPic.texture = texture;
                    ReferencesHolder.playerPublicInfo.AvatarUsed = false;
                    //ReferencesHolder.playersAvatarTex = texture;
                    //ReferencesHolder.playersAvatarSprite = sprite;
                    //MainUIManager.Instance.SetDisplayImagesInMenus();
                    DocumentReference docRef2 = db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);

                    docRef2.UpdateAsync("AvatarUsed", false).ContinueWithOnMainThread(task =>
                    {
                        ReferencesHolder.playersAvatarTex = texture;
                        ReferencesHolder.playersAvatarSprite = sprite;
                        MainUIManager.Instance.SetDisplayImagesInMenus();
                        spriteDisplayPic.sprite = ReferencesHolder.playersAvatarSprite;
                        UpLoadingLocalAndServer();
                        Debug.Log("updated the used avatar bool");
                    });

                }
            });

            Debug.Log("Permission result: " + permission);
        }


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

        public void DeleteImagesFolder()
        {
            if (Directory.Exists(pathOfTextures))
            {
                Directory.Delete(pathOfTextures, true);

                //    Logging.Log(" Texture Folder Deleted ");
            }
        }


        public void UpLoadingLocalAndServer()
        {
            // MainUIManager.Instance.Loader.SetActive(true);

            Texture2D copy = duplicateTexture(ReferencesHolder.playersAvatarTex);

            byte[] textureBytes = copy.EncodeToPNG();
            storageReference = storage.RootReference.Child(ReferencesHolder.playerPublicInfo.UserId + "/uploads/DisplayPic.png");
            storageReference.PutBytesAsync(textureBytes)
            .ContinueWithOnMainThread((Task<StorageMetadata> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception.ToString());

                }
                else
                {
                    // Metadata contains file metadata such as size, content-type, and md5hash.
                    StorageMetadata metadata = task.Result;
                    string md5Hash = metadata.Md5Hash;

                    Debug.Log("Finished uploading...");
                    Debug.Log("md5 hash = " + md5Hash);

                    var pictureName = ReferencesHolder.playerPublicInfo.UserId + RemoveSpecialCharacters(md5Hash);
                    Debug.Log("Delete image on disk se pahly");
                    DeleteImagesFolder();
                    Debug.Log("save image on disk se pahly");
                    SaveImageOnDisk(copy, pictureName);
                    storageReference.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCompleted)
                        {
                            DpURL = task.Result.ToString();
                            //  db.Collection(ReferencesHolder.FS_users_Collec).Document(newUser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).UpdateAsync("DeviceToken", PlayerPrefs.GetString("DeviceToken"));
                            db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).UpdateAsync("PictureURL", DpURL);
                            db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).UpdateAsync("PictureName", pictureName);


                            //var publicinfo = new PublicInfoDB
                            //{

                            //    PictureURL = DpURL,
                            //    PictureName = pictureName
                            //};
                            //docRef2.UpdateAsync((IDictionary<string, object>)publicinfo).ContinueWithOnMainThread(task =>
                            //{


                            //    // MainUIManager.Instance.Loader.SetActive(false);


                            //});
                        }
                    });



                }
            });
        }

        public void AvartarButtonsMethod()
        {

            switch (EventSystem.current.currentSelectedGameObject.name)
            {
                case "Panda":
                    ReferencesHolder.playerPublicInfo.AvatarID = "panda";

                    break;
                case "Mouse":
                    ReferencesHolder.playerPublicInfo.AvatarID = "mouse";
                    break;
                case "Dog":
                    ReferencesHolder.playerPublicInfo.AvatarID = "dog";
                    break;
                case "Lion":
                    ReferencesHolder.playerPublicInfo.AvatarID = "lion";
                    break;
                case "Frog":
                    ReferencesHolder.playerPublicInfo.AvatarID = "frog";
                    break;
                case "Cat":
                    ReferencesHolder.playerPublicInfo.AvatarID = "cat_01";
                    break;
            }

            Debug.Log("Avatar value was" + ReferencesHolder.playerPublicInfo.AvatarID);
            //AvatarPanel.SetActive(false);
            ReferencesHolder.playerPublicInfo.AvatarUsed = true;
            MainUIManager.Instance.SetDisplayImagesInMenus();
            spriteDisplayPic.sprite = ReferencesHolder.playersAvatarSprite;
            DocumentReference docRef2 = db.Collection(ReferencesHolder.FS_users_Collec).Document(auth.CurrentUser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);

            docRef2.UpdateAsync("AvatarUsed", true).ContinueWithOnMainThread(task =>
            {

                MainUIManager.Instance.SetDisplayImagesInMenus();
                spriteDisplayPic.sprite = ReferencesHolder.playersAvatarSprite;
                Debug.Log("updated the used avatar bool");
            });


            docRef2.UpdateAsync("AvatarID", ReferencesHolder.playerPublicInfo.AvatarID).ContinueWithOnMainThread(task =>
            {

                Debug.Log("updated the used avatar id");



            });



        }

        public void AvartarButtonsMethod(string avatarID)
        {

            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus == true)
            {
                switch (avatarID)
                {
                    case "Panda":
                        ReferencesHolder.playerPublicInfo.AvatarID = "panda";

                        break;
                    case "Mouse":
                        ReferencesHolder.playerPublicInfo.AvatarID = "mouse";
                        break;
                    case "Dog":
                        ReferencesHolder.playerPublicInfo.AvatarID = "dog";
                        break;
                    case "Lion":
                        ReferencesHolder.playerPublicInfo.AvatarID = "lion";
                        break;
                    case "Frog":
                        ReferencesHolder.playerPublicInfo.AvatarID = "frog";
                        break;
                    case "Cat":
                        ReferencesHolder.playerPublicInfo.AvatarID = "cat_01";
                        break;
                }

                Debug.Log("Avatar value was" + ReferencesHolder.playerPublicInfo.AvatarID);
                //AvatarPanel.SetActive(false);
                ReferencesHolder.playerPublicInfo.AvatarUsed = true;
                MainUIManager.Instance.SetDisplayImagesInMenus();
                spriteDisplayPic.sprite = ReferencesHolder.playersAvatarSprite;
                DocumentReference docRef2 = db.Collection(ReferencesHolder.FS_users_Collec).Document(auth.CurrentUser.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);

                docRef2.UpdateAsync("AvatarUsed", true).ContinueWithOnMainThread(task =>
                {

                    MainUIManager.Instance.SetDisplayImagesInMenus();
                    spriteDisplayPic.sprite = ReferencesHolder.playersAvatarSprite;
                    Debug.Log("updated the used avatar bool");
                });


                docRef2.UpdateAsync("AvatarID", ReferencesHolder.playerPublicInfo.AvatarID).ContinueWithOnMainThread(task =>
                {

                    Debug.Log("updated the used avatar id");



                });
            }
            else
            {
              //  LogErrorUIHandler.instance.OpenErrorPanel("Internet not reachable or connected");
                
            }

           



        }

        #endregion


    }

}
