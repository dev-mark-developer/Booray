using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Firebase.Firestore;
using Firebase.Extensions;
using Booray.Auth;
using UCharts;
using UnityEngine.U2D;
using System;

public class ViewProfileManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas ProfileCanvas;
    public Canvas DeckSkinCanvas;

    public TextMeshProUGUI FriendUserNameTxt;
    public Image FriendImg;
    public Image ProfileDeckSkinImage;
    public GameObject[] ProfileDeckskinButtons;
    public TextMeshProUGUI FriendCoinTxt;
    [SerializeField] Sprite DefaultSprite;
    [SerializeField] private Button backBtn;


    [SerializeField] private Button ViewSkinsBtn;
    [SerializeField] private Button ViewSkinsBackBtn;
    [SerializeField] private Button ViewSkinsCutBtn;

    public Button AddFriendButton;

    [SerializeField] Image ClassicBar;
    [SerializeField] Image SpeedBar;
    [SerializeField] Image FullHouseBar;
    [SerializeField] Image TournamentBar;


    [SerializeField] TextMeshProUGUI ClassicWinText;
    [SerializeField] TextMeshProUGUI ClassicLoseText;
    [SerializeField] TextMeshProUGUI ClassicPlayedText;

    [SerializeField] TextMeshProUGUI SpeedBetWinText;
    [SerializeField] TextMeshProUGUI SpeedBetLoseText;
    [SerializeField] TextMeshProUGUI SpeedBetPlayedText;

    [SerializeField] TextMeshProUGUI FullHouseWinText;
    [SerializeField] TextMeshProUGUI FullHouseLoseText;
    [SerializeField] TextMeshProUGUI FullHousePlayedText;

    [SerializeField] TextMeshProUGUI TournamentWinText;
    [SerializeField] TextMeshProUGUI TournamentLoseText;
    [SerializeField] TextMeshProUGUI TournamentPlayedText;

    FirebaseFirestore db;


    //string UserId;
    //public GameObject FriendsWinGraph;
    //public GameObject FriendsPlayedGraph;


    //[SerializeField] PieChart gamesPlayedPieChartInstance;
    //[SerializeField] PieChart gamesWonPieChartInstance;


    public Image[] GPImagesPiechart;
    public float[] GPPievalues;

    public Image[] GWImagesPiechart;
    public float[] GWPievalues;

    public GameObject EmptyGamePlayedPieObj;
    public GameObject EmptyGameWonPieObj;
    public GameObject Loader;
    public Action onAddfriendClicked_Event;
    public SpriteAtlas avatarAtlus;

    private void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;
    }
    void Start()
    {

        backBtn.onClick.AddListener(delegate { BackMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
        ViewSkinsBtn.onClick.AddListener(ViewSkinsBtnMethod);
        ViewSkinsBackBtn.onClick.AddListener(ViewSkinsBackBtnMethod);
        ViewSkinsCutBtn.onClick.AddListener(ViewSkinsCutBtnMethod);

        AddFriendButton.onClick.AddListener(delegate { onAddfriendClicked_Event?.Invoke(); });

    }
    public void BackMethod()
    {
        ProfileCanvas.sortingOrder = -1;
   
    }
    public void ViewSkinsBtnMethod()
    {
        ProfileCanvas.sortingOrder = -1;
        DeckSkinCanvas.sortingOrder = 10;
    }
    public void ViewSkinsCutBtnMethod()
    {
   
        DeckSkinCanvas.sortingOrder = -1;
    }
    public void ViewSkinsBackBtnMethod()
    {
        ProfileCanvas.sortingOrder = 10;
        DeckSkinCanvas.sortingOrder = -1;
    }
    public void GetBarGraphStats(string PlayerId)
    {
        Debug.Log("Stats load Method inside.."+ PlayerId);
        ReferencesHolder.sendid= PlayerId;
        ReferencesHolder.allowsend = true;
        Loader.SetActive(true);
        db.Collection(ReferencesHolder.FS_users_Collec).Document(PlayerId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_Stats_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Stats load ka issue faulted me gya..");

            }
            if (task.IsCompleted)
            {
                Debug.Log("Stats load ka in complete condition");
                StatsDB stats = task.Result.ConvertTo<StatsDB>();
                // ReferencesHolder.playerStats = stats;

                ClassicWinText.text = "Win " + stats.ClassicBorrayWin.ToString();
                ClassicLoseText.text = "Lose " + stats.ClassicBorrayLoss.ToString();
                float classicplayed = stats.ClassicBorrayWin + stats.ClassicBorrayLoss;
                //ClassicPlayedText.text = $"Played\n<size=70><b>{classicplayed}"; 
                ClassicPlayedText.text = $"Played\n<size=70><b>{classicplayed}";
                float classwinpercent = (stats.ClassicBorrayWin / classicplayed) * 100;
                ClassicBar.fillAmount = classwinpercent * 0.01f;


                SpeedBetWinText.text = "Win " + stats.SpeedBetWin.ToString();
                SpeedBetLoseText.text = "Lose " + stats.SpeedBetLoss.ToString();
                float speedbetplayed = stats.SpeedBetWin + stats.SpeedBetLoss;
                SpeedBetPlayedText.text = $"Played\n<size=70><b>{speedbetplayed}";
                float speedwinpercent = (stats.SpeedBetWin / speedbetplayed) * 100;
                SpeedBar.fillAmount = speedwinpercent * 0.01f;


                FullHouseWinText.text = "Win " + stats.FullHouseWin.ToString();
                FullHouseLoseText.text = "Lose " + stats.FullHouseLoss.ToString();
                float fullhouseplayed = stats.FullHouseWin + stats.FullHouseLoss;
                FullHousePlayedText.text = $"Played\n<size=70><b>{fullhouseplayed}";
                float fullhousewinpercent = (stats.FullHouseWin / fullhouseplayed) * 100;
                FullHouseBar.fillAmount = fullhousewinpercent * 0.01f;


                TournamentWinText.text = "Win " + stats.TournamentWin.ToString();
                TournamentLoseText.text = "Lose " + stats.TournamentLoss.ToString();
                float tournamentplayed = stats.TournamentWin + stats.TournamentLoss;
                TournamentPlayedText.text = $"Played\n<size=70><b>{tournamentplayed}";
                float tournamentwinpercent = (stats.TournamentWin / tournamentplayed) * 100;
                TournamentBar.fillAmount = tournamentwinpercent * 0.01f;

                GPPievalues[0] = stats.ClassicBorrayLoss + stats.ClassicBorrayWin;
                GPPievalues[1] = stats.SpeedBetLoss + stats.SpeedBetWin;
                GPPievalues[2] = stats.FullHouseLoss + stats.FullHouseWin;
                GPPievalues[3] = stats.TournamentLoss + stats.TournamentWin;

                GWPievalues[0] = stats.ClassicBorrayWin;
                GWPievalues[1] = stats.SpeedBetWin;
                GWPievalues[2] = stats.FullHouseWin;
                GWPievalues[3] = stats.TournamentWin;

                GPSetValues(GPPievalues);
                GWSetValues(GWPievalues);

                if (stats.ClassicBorrayLoss + stats.ClassicBorrayWin == 0 && stats.SpeedBetLoss + stats.SpeedBetWin == 0 && stats.FullHouseLoss + stats.FullHouseWin == 0 && stats.TournamentLoss + stats.TournamentWin == 0)
                {
                    EmptyGamePlayedPieObj.SetActive(true);
                }
                else
                {
                    EmptyGamePlayedPieObj.SetActive(false);
                }
                if (stats.ClassicBorrayWin == 0 && stats.SpeedBetWin == 0 && stats.FullHouseWin == 0 && stats.TournamentWin == 0)
                {
                    EmptyGameWonPieObj.SetActive(true);
                }
                else
                {
                    EmptyGameWonPieObj.SetActive(false);
                }
                //StartCoroutine(gamesWonPieChartInstance.DrawPieChart(stats));
                //StartCoroutine(gamesPlayedPieChartInstance.DrawPieChart(stats));
            }

            Debug.Log("Stats load complete ka end");




        });



        
    }




    public IEnumerator LoadImage(string MediaUrl)
    {
        Debug.Log("Load img me gya..");
        FriendImg.gameObject.SetActive(false);
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Create a request
        yield return request.SendWebRequest(); //Wait for the request to complete
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("texture mil gya.......!!!!");

            var tex = ((DownloadHandlerTexture)request.downloadHandler).texture;


            var spriteImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            FriendImg.gameObject.SetActive(true);
            FriendImg.sprite = spriteImage;

            Loader.SetActive(false);




        }
    }
    public void GPSetValues(float[] valuesToSet)
    {
        float totalValues = 0;
        for (int i = 0; i < GPImagesPiechart.Length; i++)
        {
            totalValues += GPFindPercentage(valuesToSet, i);
            GPImagesPiechart[i].fillAmount = totalValues;
        }
    }
    private float GPFindPercentage(float[] valuesToSet, int index)
    {
        float totalAmount = 0;
        for (int i = 0; i < valuesToSet.Length; i++)
        {
            totalAmount += valuesToSet[i];

        }
        return valuesToSet[index] / totalAmount;
    }
    public void GPGeneratePieChart()
    {
        GPSetValues(GPPievalues);
    }

    public void GWSetValues(float[] valuesToSet)
    {
        float totalValues = 0;
        for (int i = 0; i < GWImagesPiechart.Length; i++)
        {
            totalValues += GWFindPercentage(valuesToSet, i);
            GWImagesPiechart[i].fillAmount = totalValues;
        }
    }
    private float GWFindPercentage(float[] valuesToSet, int index)
    {
        float totalAmount = 0;
        for (int i = 0; i < valuesToSet.Length; i++)
        {
            totalAmount += valuesToSet[i];

        }
        return valuesToSet[index] / totalAmount;
    }
    public void GWGeneratePieChart()
    {
        GWSetValues(GWPievalues);
    }
    public void SendFriendRequestToProfile(string toUserID)
    {
        Debug.Log($"Interacting Firebase -> {ReferencesHolder.playerPublicInfo.UserId} Sending Friend Request -> {toUserID}");

        db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId)
            .Collection(ReferencesHolder.FS_FriendReqSent_Collec).Document(toUserID).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result.Exists)
                {
                    // CallBackSent?.Invoke();
                }
                else
                {
                    var fromUserID = ReferencesHolder.playerPublicInfo.UserId;

                    var FriendReqSentData = new FriendReqDataDB
                    {
                        To = toUserID,
                        From = ReferencesHolder.playerPublicInfo.UserId,
                        RequestStatus = true

                    };

                    db.Collection(ReferencesHolder.FS_users_Collec).Document(fromUserID)
                        .Collection(ReferencesHolder.FS_FriendReqSent_Collec).Document(toUserID)
                        .SetAsync(FriendReqSentData).ContinueWithOnMainThread(tast =>
                        {
                            Debug.Log("RequestSent");
                            AddFriendButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Request Sent";
                            //  CallBackSent?.Invoke();

                        });

                        db.Collection(ReferencesHolder.FS_users_Collec).Document(toUserID)
                        .Collection(ReferencesHolder.FS_FriendReqRecieve_Collec).Document(fromUserID)
                        .SetAsync(FriendReqSentData).ContinueWithOnMainThread(tast =>
                        {
                            Debug.Log("RequestReceived");

                            //CallBackRecieve?.Invoke();
                        });
                }
            });
    }
}
