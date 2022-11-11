using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Booray.Auth;

public class LobbyRoomCreationUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject lobbyRoomCreateFormPanel;

    [SerializeField] private Button backBtn;

    [SerializeField] private TextMeshProUGUI roomCreationHeadingTxt;
    [SerializeField] private TMP_InputField roomNameIF;
    [SerializeField] private TMP_Dropdown gameTypeDropDown;
    
    [SerializeField] private TMP_InputField roomAnteValueIF;
    [SerializeField] private TMP_Text roomAnteValueWarningTxt;

    [SerializeField] private Slider playerCountSlider;
    [SerializeField] private TMP_Text sliderMinValueTxt;
    [SerializeField] private TMP_Text sliderValueNotifierTxt;

    [SerializeField] private Toggle setCustomTimerToggle;
    [SerializeField] private TMP_InputField customTimerIF;
    [SerializeField] private TMP_Text CustomTimerValueWarningTxt;

    [SerializeField] private Toggle setPasswordToggle;
    [SerializeField] private TMP_InputField passwordIF;
    [SerializeField] private TMP_Text passwordValueWarningTxt;
    [Header("Speed Bet UI")]
    [SerializeField] private Slider incrementPercentSlider;
    [SerializeField] private TMP_Text sliderIncrementValueNotifierTxt;
    [SerializeField] private GameObject currentAnteIncrementToggleParent;
    [SerializeField] private Toggle currentAnteIncrementToggle;

    [Header("Scrollable UI Ref")]
    [SerializeField] private RectTransform bufferRect;
    [SerializeField] private RectTransform contentParent;
    [SerializeField] private ScrollRect scrollRect;


    [SerializeField] private Button createRoomBtn;

    [SerializeField] private Button startGameBtn;


    [SerializeField] public GameObject invitePanelGameObject;
    [SerializeField] private Canvas invitePanelCanvas;
    [SerializeField] private FriendsUIManager friendsUIManagerInstance;
    [SerializeField] private Button invitePanelBackBtn;

    public Action OnStartRoomClicked;

    public Action onBackBtnClicked;

    private void Start()
    {

        Debug.Log(" Running ");


        startGameBtn.onClick.AddListener(delegate { OnStartRoomClicked?.Invoke();  /*createRoomBtn.interactable = false;*/ });

        createRoomBtn.onClick.AddListener(OnCreateRoomClicked);
        

        playerCountSlider.onValueChanged.AddListener(SetSliderValueText);
        incrementPercentSlider.onValueChanged.AddListener(SetIncrementSliderValueText);


        setCustomTimerToggle.onValueChanged.AddListener(  SetActiveTimerOnOff);
        setPasswordToggle.onValueChanged.AddListener(SetActivePasswordOnOff);

        backBtn.onClick.AddListener(delegate { SetActiveRoomCreatePanel(false); onBackBtnClicked?.Invoke(); });

        invitePanelBackBtn.onClick.AddListener(delegate { InviteBack(); });

        roomNameIF.onSelect.AddListener(delegate { OnInFieldSelected(roomNameIF.gameObject); });
        roomNameIF.onEndEdit.AddListener(delegate { CloseKeyboard(); });
        

        roomAnteValueIF.onSelect.AddListener(delegate { OnInFieldSelected(roomAnteValueIF.gameObject); });
        roomAnteValueIF.onDeselect.AddListener(delegate { CloseKeyboard(); });
        roomAnteValueIF.onSelect.AddListener(delegate { ShowAnteFieldWarning(false); });

        customTimerIF.onSelect.AddListener(delegate { OnInFieldSelected(customTimerIF.gameObject); });
        customTimerIF.onDeselect.AddListener(delegate { CloseKeyboard(); });
        customTimerIF.onSelect.AddListener(delegate { ShowTimeDurationFieldWarning(false); });

        passwordIF.onSelect.AddListener(delegate { OnInFieldSelected(passwordIF.gameObject); });
        passwordIF.onDeselect.AddListener(delegate { CloseKeyboard(); });
        passwordIF.onSelect.AddListener(delegate { ShowPasswordFieldWarning(false); });
    }


    #region Keyboard SCrollable Menu Functions

    public void OnInFieldSelected(GameObject SelectedIF)
    {

        Debug.Log($" OnInFieldSelected -> {SelectedIF.name} ");
        OpenKeyboard(SelectedIF);

    }


    public bool IsKeyboardOpened()
    {
        return TouchScreenKeyboard.visible;
    }

    public void OpenKeyboard(GameObject selectedIF)
    {


        Debug.Log($" OpenKeyboard -> {selectedIF.name}");
        TouchScreenKeyboard.Open("");
        StartCoroutine(GetKeyboardHeight(selectedIF));
    }

    public void CloseKeyboard()
    {
        bufferRect.sizeDelta = Vector2.zero;
    }

    public IEnumerator GetKeyboardHeight(GameObject selectedGO)
    {
        Debug.Log($" GetKeyboardHeight -> {selectedGO.name}");

        yield return new WaitForSeconds(0.5f);

        float height = -1;

#if UNITY_IOS

        height = TouchScreenKeyboard.area.height;

#elif UNITY_ANDROID

        try {
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
        catch {


            Debug.Log(" Exception on keyboard open yo ");

        }

        

#endif

        bufferRect.sizeDelta = new Vector2(1, height);
        StartCoroutine(CalculateNormalizedPosition(selectedGO));
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


    #endregion


    public void InviteBack()
    {
        invitePanelGameObject.gameObject.SetActive(false);
        lobbyRoomCreateFormPanel.SetActive(true);
    }
   
    public void OnCreateRoomClicked()
    {
        // If validate forms is successfull


        Debug.Log(" OnCreateRoomClicked");

        if(ValidateFormFields())
        {

            lobbyRoomCreateFormPanel.SetActive(false);
            
            invitePanelGameObject.SetActive(true);
            //invitePanelCanvas.sortingOrder = 3;
            friendsUIManagerInstance.RefereshFriendInviteList();
            ReferencesHolder.AllInvitesList.Clear();
        }
    }


    public void SetCreateRoomButtonInteractibility(bool state)
    {
        createRoomBtn.interactable = state;
    }

    public void SetActiveRoomCreatePanel(bool state)
    {
        lobbyRoomCreateFormPanel.SetActive(state);

        if(!state)
        {
            ResetForm();
        }
    }

    public void SetActiveRoomCreatePanel(bool state,GameModeType lobby)
    {


        lobbyRoomCreateFormPanel.SetActive(state);
        if(state)
        {
            SetHeading(lobby);

            switch (lobby)
            {
                case GameModeType.ClassicBooRay:
                    {
                        sliderMinValueTxt.text = 3.ToString(); ;
                        playerCountSlider.minValue = 3;
                        break;
                    }
                case GameModeType.SpeedBet:
                    {
                        SetActiveSpeedBetOptions(true);
                        sliderMinValueTxt.text = 3.ToString(); ;
                        playerCountSlider.minValue = 3;
                        break;
                    }
                case GameModeType.FullHouse:
                    {
                        sliderMinValueTxt.text = 5.ToString(); ;
                        playerCountSlider.minValue = 5;
                        break;
                    }
            }
        }
        

        if (!state)
        {
            ResetForm();
        }
    }

    public void ResetForm()
    {
        roomNameIF.text = "";
        gameTypeDropDown.value = 0;
        roomAnteValueIF.text = "";
        playerCountSlider.value = playerCountSlider.minValue;

        setCustomTimerToggle.isOn = false;
        SetActiveTimerOnOff(false);

        setPasswordToggle.isOn = false;
        SetActivePasswordOnOff(false);

        SetActiveSpeedBetOptions(false);


        ShowAnteFieldWarning(false);
        ShowPasswordFieldWarning(false);
        ShowTimeDurationFieldWarning(false);

    }

    public void SetSliderValueText(float value)
    {
        sliderValueNotifierTxt.text = value.ToString();
    }

    public void SetIncrementSliderValueText(float value)
    {
        sliderIncrementValueNotifierTxt.text = value.ToString();
    }

    public void SetHeading(GameModeType roomtype)
    {
        string roomCreationMsg = "";
        switch (roomtype)
        {
            case GameModeType.ClassicBooRay:
            {
                    roomCreationMsg = "Create Room\n<size=30>Classic Boo Ray Lobby</size>";
                break;
            }
            case GameModeType.SpeedBet:
            {
                    roomCreationMsg = "Create Room\n<size=30>Speed Bet Lobby</size>";
                    break;
            }
            case GameModeType.FullHouse:
            {
                    roomCreationMsg = "Create Room\n<size=30>Full House Lobby</size>";
                    break;
            }
            //case GameModeType.Tournament:
            //{
            //        roomCreationMsg = "Create Room\n<size=15>Tournament Lobby</size>";
            //        break;
            //}
        }


        roomCreationHeadingTxt.text = roomCreationMsg;


    }

    public string GetRoomNameIF()
    {
        if (string.IsNullOrEmpty(roomNameIF.text))
        {
            return "Classic Booray Room - " + UnityEngine.Random.Range(0,9999);
        }
        return roomNameIF.text;
    }

    public int GetGameTypeDD()
    {
        return gameTypeDropDown.value;
    }

    public int GetRoomAnteValue()
    {
        if(string.IsNullOrEmpty(roomAnteValueIF.text))
        {
            return 100;
        }
        return int.Parse(roomAnteValueIF.text);
    }

    public void ShowAnteFieldWarning(bool state)
    {
        if(state)
            roomAnteValueWarningTxt.text = "Cannot be less than 1 and greater than 50000!";
        else
            roomAnteValueWarningTxt.text = "";
    }

    

    public int GetPlayerCountValue()
    {
        return  (int) playerCountSlider.value;
    }


    public int GetTimeDurationInSeconds()
    {
        if (string.IsNullOrEmpty(customTimerIF.text))
        {
            return 15;
        }
        return int.Parse(customTimerIF.text);
    }
    public void ShowTimeDurationFieldWarning(bool state)
    {
        if (state)
            CustomTimerValueWarningTxt.text = "Cannot be less than 5 or greater than 120 seconds";
        else
            CustomTimerValueWarningTxt.text = "";
    }




    public string GetPasswordIF()
    {
        if(string.IsNullOrEmpty(passwordIF.text))
        {
            return "";
        }
        return passwordIF.text;
    }
    public void ShowPasswordFieldWarning(bool state)
    {
        if (state)
            passwordValueWarningTxt.text = "Password cannot be empty";
        else
            passwordValueWarningTxt.text = "";
    }




    public bool ValidateFormFields()
    {

        bool result = true;

        if(string.IsNullOrEmpty(roomNameIF.text))
        {
            // display msg
            
        }

        if(!string.IsNullOrEmpty(roomAnteValueIF.text))
        {
            int a = int.Parse(roomAnteValueIF.text);

            if(a<1 || a>50000)
            {
                //display msg
                ShowAnteFieldWarning(true);
                result = false; ;
            }



        }

        if(setCustomTimerToggle.isOn && !string.IsNullOrEmpty(customTimerIF.text))
        {
            int t = int.Parse(customTimerIF.text);

            if(t<5 || t>120)
            {
                //display msg
                ShowTimeDurationFieldWarning(true);
                result = false; ;
            }
                      
        }

        if (setPasswordToggle.isOn && string.IsNullOrEmpty(passwordIF.text))
        {
            //display msg
            ShowPasswordFieldWarning(true);
            result = false; ;
        }


        return result;


    }


    public int GetAnteIncrementValue()
    {
        return (int)incrementPercentSlider.value;
    }

    public bool GetCurrentAnteIncrementToggleValue()
    {
        return currentAnteIncrementToggle.isOn;
    }

    public void SetActiveSpeedBetOptions(bool value)
    {
        if(value)
        {
            incrementPercentSlider.gameObject.SetActive(true);
            currentAnteIncrementToggleParent.SetActive(true);
        }
        else
        {
            incrementPercentSlider.value = 50;
            currentAnteIncrementToggle.isOn = false;
            incrementPercentSlider.gameObject.SetActive(false);
            currentAnteIncrementToggleParent.SetActive(false);
        }
    }


    public void SetActiveTimerOnOff(bool Value)
    {
        if(Value)
        {

            customTimerIF.gameObject.SetActive(true);
        }
        else
        {
            customTimerIF.text = "";
            customTimerIF.gameObject.SetActive(false);
        }

    }

    public void SetActivePasswordOnOff(bool Value)
    {
        if (Value)
        {

            passwordIF.gameObject.SetActive(true);
        }
        else
        {
            passwordIF.text = "";
            passwordIF.gameObject.SetActive(false);
        }

    }


    
   

}