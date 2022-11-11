using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Auth;
using QFSW.MOP2;
using TMPro;
using UnityEngine.EventSystems;
using Booray.Game;
using System.Linq;
using UnityEngine.Networking;

public class ChatHandler : MonoBehaviour


{
    public GameObject ChatContentParent;

    [SerializeField] GameManager gameManagerInstance;


    [SerializeField] ChatDatabase chatFirebaseInstance; 


    FirebaseAuth auth;
    //[SerializeField] ChatDatabase database;

    [SerializeField] GameObject messagePrefab_Theirs;
    [SerializeField] GameObject messagePrefab_mine; 

    [SerializeField] Transform messagecontainerContent;
    [SerializeField] TMP_InputField chatInputField;

    [SerializeField] GameObject ChatPanel;
    


    [SerializeField] Transform emojiParentTransform;

    
     public Button OpenChatButton;
    [SerializeField]
    Button CloseChatButton;

    [SerializeField]
    Button sendBtn;


    [SerializeField]
    Button EmojiButton;

    [SerializeField]
    Button[] SpriteButtons;


    [SerializeField] private TextMeshProUGUI msgCountTxt;
    [SerializeField] private Image msgCounterImg;


    private int msgCounter = 0;


    // Start is called before the first frame update
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;


        Invoke("DelayInDestroyChat", 1.5f);
        OpenChatButton.onClick.AddListener(OpenChat);
  
        CloseChatButton.onClick.AddListener(CloseChatPanel);
        sendBtn.onClick.AddListener(sendMessage);


      

        Debug.Log(ReferencesHolder.playerPublicInfo.UserName + 2);

       



    }

    public void SetChatButtonInteractibility(bool state)
    {
        OpenChatButton.gameObject.SetActive(state);
    }

    private void OnEnable()
    {
        chatFirebaseInstance.OnListen += InstantiateMessage;
        chatFirebaseInstance.OnListenFail += Debug.Log;
        
    }

    private void OnDisable()
    {
        chatFirebaseInstance.OnListen -= InstantiateMessage;
        chatFirebaseInstance.OnListenFail -= Debug.Log;

    }

    


    public void DisplayMsgCount()
    {

    }

    

    void OpenChat()
    {
        ResetMsgCounter();

        Debug.Log(" Opening panel ");
        //if (ReferencesHolder.joinedRoom == true)
        //{
        //    DestroyPreviousChat();
        //    ReferencesHolder.joinedRoom = false;
        //}
        ChatPanel.SetActive(true);
    }
    void CloseChatPanel()
    {

        Debug.Log(" Closing Panel ");
        ChatPanel.SetActive(false);
    }



    private void UpdateMsgCounter()
    {
        msgCounter += 1;
        
        if(msgCounterImg.gameObject.activeSelf == false)
        {
            msgCounterImg.gameObject.SetActive(true);
            
        }

        if(msgCounter>100)
        {
            msgCountTxt.text = "100+";
        }
        else
        {
            msgCountTxt.text = msgCounter.ToString();

        }
        
    }

    private void ResetMsgCounter()
    {
        msgCounter = 0;

        msgCounterImg.gameObject.SetActive(false);
    }








    public void sendMessage()
    {
        if(string.IsNullOrEmpty( chatInputField.text))
        {
            return;
        }


        Message message = new Message();
       
        message.Time = Timestamp.GetCurrentTimestamp();
        message.User = ReferencesHolder.playerPublicInfo.UserId;
        message.UserName = ReferencesHolder.playerPublicInfo.UserName;
        message.PictureName = ReferencesHolder.playerPublicInfo.PictureName;
        message.PictureURL = ReferencesHolder.playerPublicInfo.PictureURL;
        message.AvatarUsed = ReferencesHolder.playerPublicInfo.AvatarUsed;
        message.AvatarID = ReferencesHolder.playerPublicInfo.AvatarID;
        message.ChatItem = "chat";
        message.message = $"{chatInputField.text}";
        
       

        chatInputField.text = "";
        
        chatFirebaseInstance.MessageItemList.Add(message.message);
      
        
        chatFirebaseInstance.Postmessage(message, () => Debug.Log("Message sent"), Debug.Log);


    }
    private void InstantiateMessage(Message message)
    {

        if (!ChatPanel.activeSelf)
        {
            if (ReferencesHolder.joinedRoom == false)
            {
                UpdateMsgCounter();
            }
            
        }


        try {

            GameObject newMessage;
            
            if(message.User.Equals(ReferencesHolder.playerPublicInfo.UserId))
            {
              
                newMessage = Instantiate(messagePrefab_mine, transform.position, Quaternion.identity);
                CheckAndLoadChatImages(newMessage, message);


            }
            else
            {

                messagePrefab_Theirs.GetComponent<ChatItemUIHandler>().Player_Name_TMP.text = message.UserName;
                
                newMessage = Instantiate(messagePrefab_Theirs, transform.position, Quaternion.identity);
                //StartCoroutine(newMessage.GetComponent<ChatItemUIHandler>().LoadMessageImage(message.PictureURL, message.AvatarUsed, message.AvatarID));
                CheckAndLoadChatImages(newMessage, message);
            }


            var textCont = newMessage.GetComponent<ChatItemUIHandler>();
          

            textCont.SetChatText(message.message);
           
    
           var playerList = gameManagerInstance.GetAllPlayersInsideRoom();

            Debug.Log($" Player List COunt -> {playerList.Count} ");

            Debug.Log($"message user ID = {message.User} -- {playerList[0].photonPlayer.NickName} -- {ReferencesHolder.playerPublicInfo.UserId}");

            var senderPlayer = playerList.FirstOrDefault(x => x.photonPlayer.NickName.Equals(message.User));
            //StartCoroutine(newMessage.GetComponent<ChatItemUIHandler>().LoadMessageImage(message.PictureURL, message.AvatarUsed));
            //if (senderPlayer == null)
            //{
            //    Debug.Log("SenderIs null");
            //}
            //else
            //{
            //    var displayimfOfSender = senderPlayer.playerUIControllerInstance.GetPlayerAvatar();

            //    textCont.SetDisplayImg(displayimfOfSender);
            //}








            newMessage.transform.SetParent(messagecontainerContent, false);
        }
        catch
        {
            Debug.Log("There is some exceptrion");
        }
     
       
    }

    void EmojiMethod()
    {
        chatInputField.text = chatInputField.text + EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
    }
  
    public void DestroyPreviousChat()
    {
        Debug.Log("Chat destroy function ka strat");
        for (var i = ChatContentParent.transform.childCount - 1; i >= 0; i--)
        {
            //  Destroy(ChatContentParent.transform.GetChild(i));
         ChatContentParent.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void DelayInDestroyChat()
    {
        if (ReferencesHolder.joinedRoom == true)
        {
            DestroyPreviousChat();
            ReferencesHolder.joinedRoom = false;
        }
    }

    public void CheckAndLoadChatImages(GameObject newMessage,Message message)
    {
        var textCont = newMessage.GetComponent<ChatItemUIHandler>();


        textCont.SetChatText(message.message);


        var playerList = gameManagerInstance.GetAllPlayersInsideRoom();

        Debug.Log($" Player List COunt -> {playerList.Count} ");

        Debug.Log($"message user ID = {message.User} -- {playerList[0].photonPlayer.NickName} -- {ReferencesHolder.playerPublicInfo.UserId}");

        var senderPlayer = playerList.FirstOrDefault(x => x.photonPlayer.NickName.Equals(message.User));
      
        if (senderPlayer == null)
        {
            Debug.Log("SenderIs null");
            StartCoroutine(newMessage.GetComponent<ChatItemUIHandler>().LoadMessageImage(message.PictureURL, message.AvatarUsed, message.AvatarID));

        }
        else
        {
            var displayimfOfSender = senderPlayer.playerUIControllerInstance.GetPlayerAvatar();

            textCont.SetDisplayImg(displayimfOfSender);
        }

    }



}
