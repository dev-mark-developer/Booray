using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google;
using Firebase.Auth;
using System;
using UnityEngine.SceneManagement;
using Firebase.Firestore;
using Firebase.Extensions;

namespace Booray.Auth
{
    public class SettingUIManager : MonoBehaviour
    {

        [SerializeField] private GameObject settingsPnl;

        FirebaseFirestore db;

        //Buttons decleration
        [SerializeField] TweeningTwoStateButton music2StateBtn;


        [SerializeField] TweeningTwoStateButton sound2StateBtn;


        [SerializeField] TweeningTwoStateButton vibration2StateBtn;


       // [SerializeField] Button logoutBtn;
        public Button EditProfileButton;
        public Button LogOutButton;
        public Button QuitButton;
        public Button BackButton;


        public Button howToPlayOpenBtn;
        public Button howToPlayCancleBtn;
        public GameObject howToPlayPanel;


        [SerializeField] TextMeshProUGUI playerUserNameTxt;
        [SerializeField] Image PlayerImg;

        public Action onMusicSliderClicked;
        public Action onSoundSliderClicked;
        public Action onVibrationSliderClicked;
        string UserId;
        FirebaseAuth auth;
       // public GameObject obj;
        private void Awake()
        {

            auth = FirebaseAuth.DefaultInstance;


        }
        void Start()
        {
            Debug.Log("Vibes "+PlayerPrefs.GetInt("_vibration"));
            //SetPlayersInfo(ReferencesHolder.userName, ReferencesHolder.playersAvatarSprite);
            BackButton.onClick.AddListener(delegate { BackButtonMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
            music2StateBtn.onSwitched_Event.AddListener(OnMusicSwitchedEvent);
            sound2StateBtn.onSwitched_Event.AddListener(OnSoundSwitchedEvent);
            vibration2StateBtn.onSwitched_Event.AddListener(OnVibrationSwitchedEvent);

            howToPlayOpenBtn.onClick.AddListener(delegate { howToPlayPanel.SetActive(true); });
            howToPlayCancleBtn.onClick.AddListener(delegate { howToPlayPanel.SetActive(false); });


            music2StateBtn.SetState(MusicHandler.instance.GetMusicState());
            sound2StateBtn.SetState(SFXHandler.instance.GetSFxState());
            vibration2StateBtn.SetState(VibrationHandler.instance.GetVibrationState());
            db = FirebaseFirestore.DefaultInstance;
        }


        public void OnMusicSwitchedEvent(bool state )
        {
            MusicHandler.instance.ChangeVolumeState(state);
            SFXHandler.instance.PlayBtnClickSFX();
            Debug.Log("btn");

            onMusicSliderClicked?.Invoke();
        }
        public void OnSoundSwitchedEvent(bool state)
        {
            SFXHandler.instance.ChangeVolumeState(state);
            SFXHandler.instance.PlayBtnClickSFX();
            onSoundSliderClicked?.Invoke();



        }
        public void OnVibrationSwitchedEvent(bool state)
        {
            Debug.Log("chla vib");
            Debug.Log(state);
            VibrationHandler.instance.ChangeVibrationState(state);
            SFXHandler.instance.PlayBtnClickSFX();
            onVibrationSliderClicked?.Invoke();



        }



        public void ShowSettingPanel()
        {
            MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
            MainUIManager.Instance.HomeUI.SettingPanel.SetActive(true);
            MainUIManager.Instance.HomeUI.BackButton.gameObject.SetActive(true);


        }
        public void SetSettingsPanelActive(bool state)
        {
            settingsPnl.SetActive(state);
        }

        public void SetPlayersInfo(string username, Sprite avatar)
        {
            Debug.Log("chla Setting SetPlayersInfo");
            playerUserNameTxt.text = username;
            PlayerImg.sprite = avatar;
        }


      
        //below logout works work both guest and email user
        public void LogOut()
        {

            Debug.Log("that provider:" + ReferencesHolder.Provider);
            ReferencesHolder.joinedRoom = false;
            if (PlayerPrefs.GetInt("SignedUp")==1|| PlayerPrefs.GetInt("GSignedUp") == 1|| PlayerPrefs.GetInt("FBSignedUp") == 1 || PlayerPrefs.GetInt(ReferencesHolder.GoogleSignedUp) == 1)
            {
                PlayerPrefs.SetInt("SignedUp", 0);
                PlayerPrefs.SetInt("GSignedUp", 0);
                PlayerPrefs.SetInt("FBSignedUp", 0);
                PlayerPrefs.SetInt(ReferencesHolder.GoogleSignedUp, 0);
                PlayerPrefs.Save();
                
                switch (ReferencesHolder.Provider)
                {
                    
                    case "Email":
                        UserId = PlayerPrefs.GetString(ReferencesHolder.EmailUserId);

                        Debug.Log("Providerwaa " + ReferencesHolder.Provider);
                        Debug.Log("Providerwaa ka side effect " + UserId);
                        break;
                    case "Guest":
                        UserId = PlayerPrefs.GetString(ReferencesHolder.GuestUserId);
                        break;
                    case "Facebook":
                        UserId = PlayerPrefs.GetString(ReferencesHolder.FBUserId);
                        break;
                    case "Google":
                        //UserId = PlayerPrefs.GetString(ReferencesHolder.GoogleUserId);
                        UserId = ReferencesHolder.newUserId;
                        break;
                }
                db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).UpdateAsync("DeviceToken", null);
                db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).UpdateAsync("token", null);

            }
            else
            {
                db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc).UpdateAsync("DeviceToken", null);
                ReferencesHolder.ResetPlayerDataReferences();
                // MainUIManager.Instance.HomeUI.DeactivePanelsMethod();

                PlayerPrefs.SetInt("SignedUp", 0);
                PlayerPrefs.SetInt("GSignedUp", 0);
                PlayerPrefs.SetInt("FBSignedUp", 0);
                PlayerPrefs.SetInt(ReferencesHolder.GoogleSignedUp, 0);
                PlayerPrefs.Save();
                if (ReferencesHolder.Provider == "Google")
                {
                    Debug.Log(">>>>>>>>>>>>GoOgle signout condition");
                    GoogleSignIn.DefaultInstance.SignOut();
                   //GoogleSignIn.DefaultInstance.Disconnect();


                }
              //  PlayerPrefs.SetString("SavedProvider", ReferencesHolder.Provider);
                auth.SignOut();
                
            }
            
            SceneManager.LoadScene("LoginScene");
         


        }
        public void QuitMethod()
        {
            Application.Quit();

        }
        void BackButtonMethod()
        {
            MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
            MainUIManager.Instance.HomeUI.HomePanel.SetActive(true);
            howToPlayPanel.SetActive(false);
        }





    }
}

