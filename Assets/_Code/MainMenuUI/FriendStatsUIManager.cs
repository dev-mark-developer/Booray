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

public class FriendStatsUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject friendsStatPanel;
    
    public TextMeshProUGUI FriendUserNameTxt;
    public Image FriendImg;
    public TextMeshProUGUI FriendCoinTxt;
    [SerializeField] Sprite DefaultSprite;
    [SerializeField] private Button backBtn;

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
    public GameObject FriendsWinGraph;
    public GameObject FriendsPlayedGraph;


    [SerializeField] PieChart gamesPlayedPieChartInstance;
    [SerializeField] PieChart gamesWonPieChartInstance;


    public Image[] GPImagesPiechart;
    public float[] GPPievalues;

    public Image[] GWImagesPiechart;
    public float[] GWPievalues;

    private void OnEnable()
    {
        FriendUserNameTxt.text = ReferencesHolder.FriendStatsName;
        if (ReferencesHolder.AvatarUsed == true)
        {
            Debug.Log("friend avatar used true me gya........");
            FriendImg.sprite = ReferencesHolder.playersAvatarSprite;
        }
        else if (ReferencesHolder.FriendSpriteUrl != null || ReferencesHolder.FriendSpriteUrl == "")
        {
            Debug.Log("friend avatar not used true me gya........");
            StartCoroutine(LoadImage(ReferencesHolder.FriendSpriteUrl));
        }



       // MainUIManager.Instance.HomeUI.BackButton.gameObject.SetActive(false);
        //FriendImg.sprite = ReferencesHolder.FriendStatsSprite;
    }
  
    private void OnDisable()
    {
        
       // MainUIManager.Instance.HomeUI.BackButton.gameObject.SetActive(true);
    }
    private void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;
    }
    void Start()
    {

        backBtn.onClick.AddListener(delegate { BackMethod(); SFXHandler.instance.PlayBtnClickSFX(); });
        //FriendUserNameTxt.text = ReferencesHolder.FriendStatsName;
        //if (ReferencesHolder.AvatarUsed == true)
        //{
        //    FriendImg.sprite = ReferencesHolder.playersAvatarSprite;
        //}
        //else if (ReferencesHolder.FriendSpriteUrl != null || ReferencesHolder.FriendSpriteUrl == "")
        //{
        //    StartCoroutine(LoadImage(ReferencesHolder.FriendSpriteUrl));
        //}
    }
    public void BackMethod()
    {
        friendsStatPanel.SetActive(false);
        MainUIManager.Instance.HomeUI.FriendsPanel.SetActive(true);
    }
    public void GetBarGraphStats()
    {

        db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.FriendId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_Stats_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                /*Loader.SetActive(false)*/

            }
            if (task.IsCompleted)
            {

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
                SpeedBetPlayedText.text = $"Played\n<size=70><b>{speedbetplayed}" ;
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
                //StartCoroutine(gamesWonPieChartInstance.DrawPieChart(stats));
                //StartCoroutine(gamesPlayedPieChartInstance.DrawPieChart(stats));
            }






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
}
