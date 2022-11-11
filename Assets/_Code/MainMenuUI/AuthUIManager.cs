using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Booray.Auth
{
    public class AuthUIManager : MonoBehaviour
    {

        //Canvas references
        public Canvas Title_Canvas;
        public Canvas Login_Canvas;
        public Canvas Register_Canvas;
        public Canvas Forget_Canvas;
        public Canvas Verify_Canvas;



        // Screens references
        public GameObject MainCanvas;
        public GameObject LoginCanvas;
        public GameObject RegisterCanvas;
        public GameObject VerifyEmailPanel;
        public GameObject ForgetPasswordPanel;


        //sign up panel variables
        public RawImage SignUpDisplayPic;
        public TMP_InputField SignUpUserNameField;
        public TextMeshProUGUI UsernamewarningText;
        public TMP_InputField SignUpemail;
        public TextMeshProUGUI EmailwarningText;
        public TMP_InputField SignUppassword;
        public TextMeshProUGUI SignUpPasswordwarningText;
        public TMP_InputField SignUpConfirmPasswordField;
        public TextMeshProUGUI SignUpconfirmPasswordwarningText;



        //Sign in Panel variables
        public TMP_InputField SignInemail;
        public TextMeshProUGUI SignInEmailwarningText;
        public TMP_InputField SignInpassword;
        public TextMeshProUGUI SignInPasswordwarningText;

        //verify email panel variable
        public TMP_InputField VerifyField;
        public TextMeshProUGUI VerifyPinwarningText;
        public TextMeshProUGUI ReVerifyTimmerText;
        public Image ReverifyTimmerImg;



        //Forget Password panel variable

        public TMP_InputField ForgetPasswordEmailField;
        public TextMeshProUGUI ForgetPasswordEmailWarningText;
        public TextMeshProUGUI ForgetPasswordGuideText;

        // Buttons Delarations
        public Button LoginBackButton;
        public Button ForgetPasswordBackButton;
        public Button LoginForgetPasswordButton;
        public Button LoginRegisterButton;
        public Button RegisterLoginButton;
        public Button VerifyPanelBackButton;
        public Button QuitButton;


        public Button RegisterButton;
        public Button ForgetPasswordChangeButton;
        public Button VerifyEmailButton;
        public Button ResendPinButton;
        public Button OpenGalleryButton;
        public Button MainLoginButton;
        public Button LoginButton;

        public Button GoogleButton;
        public Button GuestLoginButton;
        public Button FacbookButton;

        public GameObject Loader;


        public RectTransform bufferImage;
        public ScrollRect scrollRect;
        public RectTransform contentParent;
        private Ping ping;

        void Start()
        {

            LoginBackButton.onClick.AddListener(delegate { LoginBackButtonMethod(); SFXHandler.instance.PlayBtnClickSFX(); });

            ForgetPasswordBackButton.onClick.AddListener(delegate { ForgetPasswordBackButtonMethod(); SFXHandler.instance.PlayBtnClickSFX(); });

            LoginForgetPasswordButton.onClick.AddListener(delegate { OpenForgetPasswordPanel(); SFXHandler.instance.PlayBtnClickSFX(); });



            LoginRegisterButton.onClick.AddListener(delegate { ShowRegisterPanel(); SFXHandler.instance.PlayBtnClickSFX(); });

            RegisterLoginButton.onClick.AddListener(delegate { ShowLoginPanel(); SFXHandler.instance.PlayBtnClickSFX(); });

            VerifyPanelBackButton.onClick.AddListener(delegate { VerifyPanelBackButtonMethod(); SFXHandler.instance.PlayBtnClickSFX(); });

            QuitButton.onClick.AddListener(delegate { QuitMethod(); SFXHandler.instance.PlayBtnClickSFX(); });

            SignInemail.onSelect.AddListener(delegate { ClearSignInWarnings(); });
            SignInpassword.onSelect.AddListener(delegate { ClearSignInWarnings(); });

            SignUpUserNameField.onSelect.AddListener(delegate { ClearSignUpWarnings(); OnOpenKeyboard(SignUpUserNameField.gameObject); });
            SignUpUserNameField.onDeselect.AddListener(delegate { OnCloseKeyboard(); });

            SignUpemail.onSelect.AddListener(delegate { ClearSignUpWarnings(); OnOpenKeyboard(SignUpemail.gameObject); });
            SignUpemail.onDeselect.AddListener(delegate { OnCloseKeyboard(); });


            SignUppassword.onSelect.AddListener(delegate { ClearSignUpWarnings(); OnOpenKeyboard(SignUppassword.gameObject); });
            SignUppassword.onDeselect.AddListener(delegate { OnCloseKeyboard(); });

            SignUpConfirmPasswordField.onSelect.AddListener(delegate { ClearSignUpWarnings(); OnOpenKeyboard(SignUpConfirmPasswordField.gameObject); });
            SignUpConfirmPasswordField.onDeselect.AddListener(delegate { OnCloseKeyboard(); });


            ForgetPasswordEmailField.onSelect.AddListener(delegate { ClearForgetPasswordGuides(); });

            VerifyField.onSelect.AddListener(delegate { ClearVerifyPinWarnings(); });


          

       

        }


        public void OnOpenKeyboard(GameObject selectedIF)
        {
            Debug.Log("open keyboard");
            Debug.Log($" OpenKeyboard -> {selectedIF.name}");
            TouchScreenKeyboard.Open("");
            StartCoroutine(GetKeyboardHeight(selectedIF));
        }

        public void OnCloseKeyboard()
        {
            bufferImage.sizeDelta = Vector2.zero;
        }

        public IEnumerator GetKeyboardHeight(GameObject selectedIF)
        {
            Debug.Log($" GetKeyboardHeight -> ola mama");

            yield return new WaitForSeconds(0.5f);

            float height = -1;

#if UNITY_IOS

        height = TouchScreenKeyboard.area.height;

#elif UNITY_ANDROID

            try
            {
                using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");

                    using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
                    {
                        View.Call("getWindowVisibleDisplayFrame", Rct);

                        height = Screen.height - Rct.Call<int>("height");
                    }
                }
            }
            catch
            {


                Debug.Log(" Exception on keyboard open yo ");

            }



#endif



            // var sd = new Vector2(scrollRect.sizeDelta.x, scrollRect.sizeDelta.y - height);



            //scrollRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,  height);

            bufferImage.sizeDelta = new Vector2(1, height);
            Debug.Log($"Buffer Image = { bufferImage.sizeDelta}  | {new Vector2(1, height)}");
            StartCoroutine(CalculateNormalizedPosition(selectedIF));
        }
        private IEnumerator CalculateNormalizedPosition(GameObject selectedInputField)
        {
            yield return new WaitForEndOfFrame();

            //Get the new total height of the content gameobject
            var newContentHeight = contentParent.sizeDelta.y;
            //Get the local y position of the selected input
            var selectedInputHeight = selectedInputField.transform.localPosition.y;
            //Get the normalized position of the selected input
            var selectedInputfieldHeightNormalized = 1 - selectedInputHeight / -newContentHeight;
            //Assign the button's normalized position to the scroll rect's normalized position
            scrollRect.verticalNormalizedPosition = selectedInputfieldHeightNormalized;
        }

        //public void AfterEmailVerified()
        //{
        //   // HideLoader();
        //   //// Loader.SetActive(false);
        //   // VerifyEmailPanel.SetActive(true);
        //   // RegisterCanvas.SetActive(false);
        //}

        void ShowRegisterPanel()
        {
            RegisterCanvas.SetActive(true);
            LoginCanvas.SetActive(false);
            ClearSignInWarnings();
            ClearSignInTextFields();
        }
        void ShowLoginPanel()
        {
            RegisterCanvas.SetActive(false);
            LoginCanvas.SetActive(true);
            ClearSignUpTextFields();
            ClearSignUpWarnings();
        }
        void LoginBackButtonMethod()
        {
            LoginCanvas.SetActive(false);
            MainCanvas.SetActive(true);
            ClearSignInTextFields();
            ClearSignInWarnings();
        }
        void ClearSignUpWarnings()
        {
            UsernamewarningText.text = "";
            UsernamewarningText.gameObject.SetActive(false);

            EmailwarningText.text = "";
            EmailwarningText.gameObject.SetActive(false);

            SignUpPasswordwarningText.text = "";
            SignUpPasswordwarningText.gameObject.SetActive(false);

            SignUpconfirmPasswordwarningText.text = "";
            SignUpconfirmPasswordwarningText.gameObject.SetActive(false);

        }
        public void ClearSignUpTextFields()
        {
            SignUpemail.text = "";
        

            SignUppassword.text = "";
         

            SignUpUserNameField.text = "";
        

            SignUpConfirmPasswordField.text = "";
           
           
        }
        public void ClearSignInWarnings()
        {
            SignInEmailwarningText.text = "";
            SignInEmailwarningText.gameObject.SetActive(false);

            SignInPasswordwarningText.text = "";
            SignInPasswordwarningText.gameObject.SetActive(false);
            // SignInPasswordwarningText.text = "";
        }
        public void ClearSignInTextFields()
        {
            SignInemail.text = "";
            SignInpassword.text = "";
        }
        void ClearForgetPasswordGuides()
        {
            ForgetPasswordEmailWarningText.text = "";
            ForgetPasswordEmailWarningText.gameObject.SetActive(false);
            ForgetPasswordGuideText.text = "";
            ForgetPasswordGuideText.gameObject.SetActive(false);
        }
        void ClearForgetPasswordTextfield()
        {
            ForgetPasswordEmailField.text = "";
        }
        void ForgetPasswordBackButtonMethod()
        {
            Debug.Log("forget password...chla...");
            ForgetPasswordPanel.SetActive(false);
            LoginCanvas.SetActive(true);
            ClearForgetPasswordTextfield();
            ClearForgetPasswordGuides();
            ClearSignInTextFields();
            ClearSignInWarnings();
        }
        void ClearVerifyPinWarnings()
        {
            VerifyPinwarningText.text = "";
            VerifyPinwarningText.gameObject.SetActive(false);
        }
        void VerifyPanelBackButtonMethod()
        {
            RegisterCanvas.SetActive(true);
            VerifyEmailPanel.SetActive(false);
            ClearSignUpTextFields();
            ClearSignUpWarnings();
        }
        void OpenForgetPasswordPanel()
        {
            ForgetPasswordPanel.SetActive(true);
            LoginCanvas.SetActive(false);
            ForgetPasswordGuideText.gameObject.SetActive(true);
            ForgetPasswordGuideText.text = "Please write email for verification";
        }
        void QuitMethod()
        {
            Debug.Log("daba");
            Application.Quit();
        }
        public void ResetVerifyPanel()
        {
            VerifyField.text = "";
            VerifyPinwarningText.text = "";
            ReVerifyTimmerText.text = "";
            ReverifyTimmerImg.fillAmount = 0;

        }
        public void ShowLoader()
        {
            Loader.SetActive(true);
            Title_Canvas.enabled = false;
            Login_Canvas.enabled = false;
            Register_Canvas.enabled = false;
            Forget_Canvas.enabled = false;
            Verify_Canvas.enabled = false;
        }
        public void HideLoader()
        {
            Loader.SetActive(false);
            Title_Canvas.enabled = true;
            Login_Canvas.enabled = true;
            Register_Canvas.enabled = true;
            Forget_Canvas.enabled = true;
            Verify_Canvas.enabled = true;
        }


    }
}

