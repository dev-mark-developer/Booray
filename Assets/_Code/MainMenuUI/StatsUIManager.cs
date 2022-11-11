using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UCharts;
using Firebase.Firestore;
using Firebase.Extensions;
using DG.Tweening;

namespace Booray.Auth
{
    public class StatsUIManager : MonoBehaviour
    {
        FirebaseFirestore db;
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

        [SerializeField] TextMeshProUGUI playerUserNameTxt;
        [SerializeField] Image PlayerImg;
        public TextMeshProUGUI playerCointxt;
        //Buttons References
        //public Button StatsButton;
        public Button StatsFriendButton;
        [SerializeField] GameObject WinGraph;
        [SerializeField] GameObject PlayedGraph;
        string UserId;

        //[SerializeField] PieChart gamesWonPieChartInstance;
        //[SerializeField] PieChart gamesPlayedPieChartInstance;

        public Image[] GPImagesPiechart;
        public float[] GPPievalues;

        public Image[] GWImagesPiechart;
        public float[] GWPievalues;


        [SerializeField] Button backBtn;

        public GameObject EmptyGamePlayedPieObj;
        public GameObject EmptyGameWonPieObj;

        public void Start()
        {
            //PlayerImg.sprite = ReferencesHolder.playersAvatarSprite;
            db = FirebaseFirestore.DefaultInstance;
            //WinGraph.GetComponent<PieChart>().enabled = true;
            //PlayedGraph.GetComponent<PieChart>().enabled = true;
            playerCointxt.text = ReferencesHolder.playerPublicInfo.Coins.ToString();
            //SetPlayersInfo(ReferencesHolder.userName, ReferencesHolder.playersAvatarSprite);
          //  StartCoroutine(LogErrorUIHandler.instance.CheckForInternet());

        }

        public void ShowStatsPanel()
        {
            LogErrorUIHandler.instance.CheckForInternet();
            if (ReferencesHolder.InternetStatus==true)
            {
                Debug.Log("Show stats chalaaa!!!!!!!!!");
                GetBarGraphStats();
                MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
                MainUIManager.Instance.HomeUI.StatsPanel.SetActive(true);
                //  MainUIManager.Instance.StatsUI.playerCointxt.text = ReferencesHolder.playerPublicInfo.Coins.ToString();
                // MainUIManager.Instance.HomeUI.BackButton.gameObject.SetActive(true);

                backBtn.onClick.AddListener(DeactiveStatsPanel);
                //LogErrorUIHandler.instance.OpenErrorPanel("Internet not reachable");
            }
            else
            {
                MainUIManager.Instance.HomeUI.HomePanel.SetActive(true);
               // LogErrorUIHandler.instance.OpenErrorPanel("Internet not reachable or connected");
            }
                

        }

        public void DeactiveStatsPanel()
        {
            MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
            MainUIManager.Instance.HomeUI.HomePanel.SetActive(true);
        }
        public void SetPlayersInfo(string username, Sprite avatar)
        {


            Debug.Log("chla Setting SetPlayersInfo");
            playerUserNameTxt.text = username;
            PlayerImg.sprite = avatar;
        }
        public void GetBarGraphStats()
        {
            MainUIManager.Instance.SetLoaderState(true);

            //switch (ReferencesHolder.Provider)
            //{
            //    case "Email":
            //        UserId = PlayerPrefs.GetString(ReferencesHolder.EmailUserId);

            //        Debug.Log("Providerwaa " + ReferencesHolder.Provider);
            //        Debug.Log("Providerwaa ka side effect " + UserId);
            //        break;
            //    case "Guest":
            //        UserId = PlayerPrefs.GetString(ReferencesHolder.GuestUserId);
            //        break;
            //    case "Facebook":
            //        UserId = PlayerPrefs.GetString(ReferencesHolder.FBUserId);
            //        break;
            //    case "Google":
            //        UserId = ReferencesHolder.newUserId;
            //        break;
            //}
            db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_Stats_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    /*Loader.SetActive(false)*/
                    MainUIManager.Instance.SetLoaderState(false);
                }
                if (task.IsCompleted)
                {
                    MainUIManager.Instance.SetLoaderState(false);

                    StatsDB stats = task.Result.ConvertTo<StatsDB>();
                     ReferencesHolder.playerStats = stats;

                    ClassicWinText.text = "Win " + stats.ClassicBorrayWin.ToString();
                    ClassicLoseText.text = "Lose " + stats.ClassicBorrayLoss.ToString();
                    float classicplayed = stats.ClassicBorrayWin + stats.ClassicBorrayLoss;
                    ClassicPlayedText.text = $"Played\n<size=70><b>{classicplayed}";
                    float classwinpercent = (stats.ClassicBorrayWin / classicplayed) * 100;
                    ClassicBar.fillAmount = classwinpercent * 0.01f;


                    SpeedBetWinText.text = "Win " +stats.SpeedBetWin.ToString();
                    SpeedBetLoseText.text = "Lose " + stats.SpeedBetLoss.ToString();
                    float speedbetplayed = stats.SpeedBetWin + stats.SpeedBetLoss;
                    SpeedBetPlayedText.text = $"Played\n<size=70><b>{speedbetplayed}";
                    float speedwinpercent = (stats.SpeedBetWin / speedbetplayed) * 100;
                    SpeedBar.fillAmount = speedwinpercent * 0.01f;


                    FullHouseWinText.text = "Win " +stats.FullHouseWin.ToString();
                    FullHouseLoseText.text = "Lose " +stats.FullHouseLoss.ToString();
                    float fullhouseplayed = stats.FullHouseWin + stats.FullHouseLoss;
                    FullHousePlayedText.text = $"Played\n<size=70><b>{fullhouseplayed}";
                    float fullhousewinpercent = (stats.FullHouseWin / fullhouseplayed) * 100;
                    FullHouseBar.fillAmount = fullhousewinpercent * 0.01f;


                    TournamentWinText.text = "Win " +stats.TournamentWin.ToString();
                    TournamentLoseText.text = "Lose " +stats.TournamentLoss.ToString();
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
                    if(stats.ClassicBorrayLoss+ stats.ClassicBorrayWin==0&& stats.SpeedBetLoss + stats.SpeedBetWin==0 && stats.FullHouseLoss + stats.FullHouseWin==0&& stats.TournamentLoss + stats.TournamentWin == 0)
                    {
                        EmptyGamePlayedPieObj.SetActive(true);
                    }
                    else
                    {
                        EmptyGamePlayedPieObj.SetActive(false);
                    }
                    if(stats.ClassicBorrayWin==0&& stats.SpeedBetWin==0&& stats.FullHouseWin==0&& stats.TournamentWin == 0)
                    {
                        EmptyGameWonPieObj.SetActive(true);
                    }
                    else
                    {
                        EmptyGameWonPieObj.SetActive(false);
                    }
                    Debug.Log("classic booray wins of friend: " +stats.ClassicBorrayWin);
                    //StartCoroutine( gamesWonPieChartInstance.DrawPieChart(stats));
                    //StartCoroutine(gamesPlayedPieChartInstance.DrawPieChart(stats));

                }


                



            });



            //MainUIManager.Instance.SetLoaderState(false);



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
}

