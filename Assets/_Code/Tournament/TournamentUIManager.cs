using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using PullToRefresh;
using Booray.Auth;

#region Tournament Status Enum & Extension
public enum TournamentStatus
{
    Draft,
    Announced,
    ApplicationStart,
    ApplicationEnd,
    TournamentStart,
    TournamentEnd,
    WinnerDay,
    Cancelled
}
public static class TournamentStatusExtensions
{
    public static string GetString(this TournamentStatus me)
    {
        switch (me)
        {
            case TournamentStatus.Draft:
                {
                    return "Draft";

                }
            case TournamentStatus.Announced:
                {

                    return "Announced";
                }
            case TournamentStatus.ApplicationStart:
                {

                    return "Application Start";
                }
            case TournamentStatus.ApplicationEnd:
                {

                    return "Application Closed";
                }
            case TournamentStatus.TournamentStart:
                {
                    return "Tournament Start";
                }
            case TournamentStatus.TournamentEnd:
                {

                    return "Tournament End";
                }
            case TournamentStatus.WinnerDay:
                {

                    return "Winner Rewards Day";
                }
            case TournamentStatus.Cancelled:
                {

                    return "Cancelled";
                }

            default:
                {
                    return "Draft";
                }


        }
    }
}
#endregion


public class TournamentUIManager : MonoBehaviour
{

    List<string> tournamentRoomNames = new List<string> 
    { "Match-1","Match-2","Match-3","Match-4","Match-5","Match-6","Match-7","Final"};
    


    public TournamentDetailCardUIHandler tDetailUIControllerInstance;
    public TournamentLeaderBoardUIManager tLeaderBoardUIControllerInstance;
    public TournamentResultUIHandler tResultUIControllerInstance;

    public TournamentUISubMenuHandler tSubMenuUIControllerInstance;
    public TournamentFirebaseInteractionManager tFSInteractionManagerInstance;
    public TournamentPhotonManager tPhotonManagerInstance;

    public UIRefreshControl pullToRefreshControlInstance;

    [Space]

    [Header("UI References")]

    [SerializeField] private GameObject tCardPrefab;

    [Space]

    [SerializeField] private Button backBtn;
    [SerializeField] private Button tournamentMainMenuBtn;

    [Space]

    [SerializeField] private GameObject tournamentMainPanel;
    [SerializeField] private GameObject cardHolderPanel;
    [SerializeField] private Transform tournamentListContentParent;

    private List<TournamentCardItemUIHandler> tournamentCardsList;

    [SerializeField] private GameObject emptyTorunamentUIItem;


    [Space]

    [Header("Tween Controls")]
    [SerializeField] private RectTransform onscreenPoint;
    [SerializeField] private RectTransform leftPoint;
    [SerializeField] private RectTransform rightPoint;

    [SerializeField] private float tweenDuration;
    [SerializeField] private Ease easeTypeIn;
    [SerializeField] private Ease easeTypeOut;

    private Tween cardListPanelTween;
    private Tween cardDetailPanelTween;
    private Tween cardLeaderBoardPanelTween;
    private Tween cardResultPanelTween;

    [SerializeField] private RectTransform cardListRect;
    [SerializeField] private RectTransform cardDetailRect;
    [SerializeField] private RectTransform cardLeaderBoardRect;
    [SerializeField] private RectTransform cardResultRect;


    [Header("Testing Parameters")]
    [SerializeField] private Sprite tsprite;
    TournamentDB testInfo;

    private int menuLvl = 0;



    private TournamentPassDB winnerPass;

    private TournamentDB selectedTournament;
    private bool loadingTournaments;

    private void Start()
    {
        MethodSubscriber();

        tournamentCardsList = new List<TournamentCardItemUIHandler>();

        


        // CreateTournamentCard();

        StartCoroutine(InitializeTournaments());
    }



    public void SetActiveEmptyTournamentUI(bool state)
    {
        emptyTorunamentUIItem.SetActive(state);
    }

    public void MethodSubscriber()
    {
        tournamentMainMenuBtn.onClick.AddListener(delegate { OpenTournamentPanel();  SFXHandler.instance.PlayBtnClickSFX(); });
        backBtn.onClick.AddListener(delegate { OnBackBtnPressedEvent(); });

        tDetailUIControllerInstance.onParticipationBtnClicked = OnParticipateClickEvent;
        tDetailUIControllerInstance.onLeaderBoardBtnClicked = OnLeaderBoardClickedEvent;


        tResultUIControllerInstance.onAvailPrizeClicked = OnPrizeAvailClicked;
        tResultUIControllerInstance.onBackToHomeClick = OnBackBtnPressedEvent;

        tSubMenuUIControllerInstance.onConfirmYesBtnClicked = OnParticipationConfirmationClickEvent;

        pullToRefreshControlInstance.OnRefresh.AddListener(delegate { LoadTournamentCards(); });

    }

    public void OpenTournamentPanel()
    {
       LogErrorUIHandler.instance.CheckForInternet();
        if (ReferencesHolder.InternetStatus == true)
        {
            SetActiveTournamentPanel(true);
        }
            
    }
    IEnumerator InitializeTournaments()
    {
        foreach (var item in tournamentCardsList)
        {
            Destroy(item.gameObject);
        }

        tournamentCardsList.Clear();

        yield return null;

        if(loadingTournaments)
        {
            yield break ;
        }

        loadingTournaments = true;


        yield return new WaitForSeconds(2);

        tFSInteractionManagerInstance
            .GetAllTournamentInfoFromFS(delegate { loadingTournaments = false; Debug.Log(" Failure "); pullToRefreshControlInstance.EndRefreshing(); ; }, CreateTournamentCards);
    }


    //public void LoadTournamentCards()
    //{
    //    if (loadingTournaments)
    //    {
    //        return;
    //    }

    //    loadingTournaments = true;

    //    foreach (var item in tournamentCardsList)
    //    {
    //        Destroy(item.gameObject);
    //    }

    //    tournamentCardsList.Clear();


    //    tFSInteractionManagerInstance
    //        .GetAllTournamentInfoFromFS
    //        (delegate { loadingTournaments = false; Debug.Log(" Failure "); pullToRefreshControlInstance.EndRefreshing(); ; }, CreateTournamentCards);
    //}

    public void LoadTournamentCards()
    {
        LogErrorUIHandler.instance.CheckForInternet();
        if (ReferencesHolder.InternetStatus == true)
        {
            StartCoroutine(InitializeTournaments());
        }
        else
        {
            pullToRefreshControlInstance.EndRefreshing();
        }
            
    }

    public void SetActiveTournamentPanel(bool state)
    {
        if (state)
        {
            MainUIManager.Instance.HomeUI.DeactivePanelsMethod();
            MainUIManager.Instance.HomeUI.DeactiveHomePanel();
        }
        

        tournamentMainPanel.SetActive(state);

    }

    public void OnBackBtnPressedEvent()
    {
        switch(menuLvl)
        {
            case 0:
                {
                    MainUIManager.Instance.HomeUI.HomePanel.SetActive(true);
                    SetActiveTournamentPanel(false);
                    break;
                }
            case 1:
                {
                    OpenCardListPanel();
                    menuLvl -= 1;
                    break;
                }
            case 2:
                {
                    tDetailUIControllerInstance.SetParticipationInteractable(true);
                    tDetailUIControllerInstance.SetLeaderBoardInteractable(true);
                    OpenCardDetailPanel();
                    menuLvl -= 1;
                    break;
                }
        }
    }

    public void CreateTournamentCard()
    {


        GameObject tCardObject = Instantiate(tCardPrefab, tournamentListContentParent);

        TournamentCardItemUIHandler cardUIHandler = tCardObject.GetComponent<TournamentCardItemUIHandler>();

        cardUIHandler.SetUpCardObject(testInfo);

        cardUIHandler.onParticipateBtnClicked += OpenTournamentDetails;
        

        
    }

    public void CreateTournamentCards(List<TournamentDB> infoList)
    {


        Debug.Log("  Creating Tournament Cards");

        foreach(var info in infoList )
        {

            //Debug.Log($" Tournament Id = {info.Id} ");

            GameObject tCardObject = Instantiate(tCardPrefab, tournamentListContentParent);

            TournamentCardItemUIHandler cardUIHandler = tCardObject.GetComponent<TournamentCardItemUIHandler>();

            cardUIHandler.SetUpCardObject(info);
            cardUIHandler.onParticipateBtnClicked += OpenTournamentDetails;


            tournamentCardsList.Add(cardUIHandler);


        }

        if(tournamentCardsList.Count==0)
        {
            SetActiveEmptyTournamentUI(true);
        }
        else
        {
            SetActiveEmptyTournamentUI(false);
        }

        loadingTournaments = false;
        pullToRefreshControlInstance.EndRefreshing();

    }




    public void OnPlayTournament()
    {

    }






    public void OpenTournamentDetails(TournamentDB tInfo)
    {
        tDetailUIControllerInstance.SetParticipationInteractable(true);
        tDetailUIControllerInstance.SetLeaderBoardInteractable(true);

        tDetailUIControllerInstance.SetUpDetailUI(tInfo);

        selectedTournament = tInfo;

        OpenCardDetailPanel();
        menuLvl += 1;
        
    }


    public void OnLeaderBoardClickedEvent()
    {

        if (Input.touchCount > 1)
            return;

        tDetailUIControllerInstance.SetParticipationInteractable(false);
        
        MainUIManager.Instance.SetLoaderState(true);
        LogErrorUIHandler.instance.CheckForInternet();
        if (ReferencesHolder.InternetStatus == true)
        {
            MainUIManager.Instance.SetLoaderState(true);


            tFSInteractionManagerInstance.GetBestPlayersInTournament(selectedTournament.Id,
                delegate { Debug.Log("Interaction Failed -> GetBestPlayersInTournament"); MainUIManager.Instance.SetLoaderState(false); },
                delegate (List<TournamentPassDB> passList) { tLeaderBoardUIControllerInstance.SetUpTop15ResultsOfTournament(passList); MainUIManager.Instance.SetLoaderState(false); });


            //tFSInteractionManagerInstance.GetMyPassInTournament(selectedTournament.Id, ReferencesHolder.playerPublicInfo.UserId,
            //    delegate { Debug.Log("Interaction Failed -> GetBestPlayersInTournament"); },
            //    delegate(TournamentPassDB pass) { tLeaderBoardUIControllerInstance.SetUpMyResult(pass); });

            OpenCardLeaderBoardPanel();


            //tFSInteractionManagerInstance.Get

            //OpenCardLeaderBoardPanel();
            menuLvl += 1;
        }
            
    }

    public void OnResultPanelOpenEvent(TournamentPassDB tpass)
    {
       
        winnerPass = tpass;
        OpenCardResultPanel();


        

        tResultUIControllerInstance.SetUpResultUI( tpass.userName, selectedTournament.PrizeAmount,
            tpass.avatarUsed,tpass.imageURL,
            tpass.avatarId,tpass.userId.Equals(ReferencesHolder.playerPublicInfo.UserId));

        menuLvl += 1;
    }

    public void OnPrizeAvailClicked()
    {
        if(ReferencesHolder.playerPublicInfo.UserId.Equals(winnerPass.userId))
        {
            tFSInteractionManagerInstance.AddPlayerCoins(ReferencesHolder.playerPublicInfo.UserId,selectedTournament.PrizeAmount
                , delegate { Debug.Log(" Interaction Failed... "); },
                delegate{ MainUIManager.Instance.HomeUI.UpdateCoins(ReferencesHolder.playerPublicInfo.Coins + selectedTournament.PrizeAmount); } );
        }
        else
        {
            LogErrorUIHandler.instance.OpenErrorPanel("Sorry You are not the winner!");
        }

    }






    public void OnParticipateClickEvent()
    {
        if (Input.touchCount > 1)
            return;

        

        // First need to check which status is the tournament in then do other things

        if (selectedTournament == null)
        {
            return;
        }


        if(selectedTournament.Status.Equals("Application Start"))
        {
            tSubMenuUIControllerInstance.SetPartiFeeText(selectedTournament.ParticipationFees);
            tSubMenuUIControllerInstance.SetActiveConfirmationSubMenu(true);
        }
        else if(selectedTournament.Status.Equals("Tournament Start"))
        {
            MainUIManager.Instance.SetLoaderState(true);

            tFSInteractionManagerInstance.ValidatingPlay
                (ReferencesHolder.playerPublicInfo.UserId, selectedTournament,
                delegate { Debug.Log(" Failed"); MainUIManager.Instance.SetLoaderState(false); }, 
                OnPlayEvent);

            //ReferencesHolder.selectedLobby = GameModeType.Tournament;
            ReferencesHolder.selectTournament = selectedTournament;
            ReferencesHolder.isPlayingTournament = true;
        }
        else if(selectedTournament.Status.Equals("Winner Rewards Day"))
        {
            tDetailUIControllerInstance.SetParticipationInteractable(false);
            tDetailUIControllerInstance.SetLeaderBoardInteractable(false);

            tFSInteractionManagerInstance.GetWinnerOfTournamentFromFS(selectedTournament.Id,
                delegate { Debug.Log("Interaction Failed");  
                    LogErrorUIHandler.instance.OpenErrorPanel("Cannot Connect to the server... Please try again!"); 
                    tDetailUIControllerInstance.SetParticipationInteractable(false); },
                delegate(TournamentPassDB tpass) { Debug.Log("Interaction Success"); OnResultPanelOpenEvent(tpass); });
        }
        else
        {
            tFSInteractionManagerInstance
                .CheckStatus(selectedTournament.Id, selectedTournament.Status, delegate { Debug.Log("Failed"); }, tSubMenuUIControllerInstance.OpenMsg);
        }

 
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tournament"></param>
    /// <param name="tpass"></param>
    /// <param name="status">  |\ -1 = Not Participatn /|  |\ 0 = SUCCESS /|  |\ 1 = Not Enough Money /| </param>
    public void OnPlayEvent(TournamentDB tournament, TournamentPassDB tpass, int status)
    {
        LogErrorUIHandler.instance.CheckForInternet();
        if (ReferencesHolder.InternetStatus == true)
        {
            if (!tournament.Status.Equals("Tournament Start"))
            {
                MainUIManager.Instance.SetLoaderState(false);
                tSubMenuUIControllerInstance.SetNoCapacityMessage(TournamentUISubMenuHandler.MsgType.StatusChange);
                tSubMenuUIControllerInstance.SetActiveMsgPanel(true);
                return;
            }

            switch (status)
            {
                case 0:
                    {
                        if (tpass != null && tournament != null)
                        {
                            tPhotonManagerInstance.JoinTournamentRoom(tournament);
                            ReferencesHolder.myTournamentPass = tpass;

                        }

                        return;

                    }

                case 1:
                    {
                        MainUIManager.Instance.SetLoaderState(false);
                        tSubMenuUIControllerInstance.SetNoCapacityMessage(TournamentUISubMenuHandler.MsgType.NotEnoughMoney);
                        tSubMenuUIControllerInstance.SetActiveMsgPanel(true);

                        return;

                    }
                case -1:
                    {
                        MainUIManager.Instance.SetLoaderState(false);
                        tSubMenuUIControllerInstance.SetNoCapacityMessage(TournamentUISubMenuHandler.MsgType.NotParticipated);
                        tSubMenuUIControllerInstance.SetActiveMsgPanel(true);

                        return;

                    }
                case -2:
                    {
                        // Tournament Status Change Status.

                        return;

                    }
            }
        }
          

    }

    public void OnPlayEvent(TournamentDB tournament,TournamentPassDB tPass)
    {
        if(!tournament.Status.Equals("Tournament Start"))
        {
            MainUIManager.Instance.SetLoaderState(false);
            tSubMenuUIControllerInstance.SetNoCapacityMessage(TournamentUISubMenuHandler.MsgType.StatusChange);
            tSubMenuUIControllerInstance.SetActiveMsgPanel(true);
        }
        

        else if(tournament==null)
        {
            Debug.Log("Cant Join");

             MainUIManager.Instance.SetLoaderState(false);
            tSubMenuUIControllerInstance.SetNoCapacityMessage(TournamentUISubMenuHandler.MsgType.NotParticipated);
            tSubMenuUIControllerInstance.SetActiveMsgPanel(true);
        }
        else
        {
            if(tPass==null)
            {
                MainUIManager.Instance.SetLoaderState(false);
                tSubMenuUIControllerInstance.SetNoCapacityMessage(TournamentUISubMenuHandler.MsgType.NotEnoughMoney);
                tSubMenuUIControllerInstance.SetActiveMsgPanel(true);
            }
            else
            {
                tPhotonManagerInstance.JoinTournamentRoom(tournament);
                ReferencesHolder.myTournamentPass = tPass;
            }

            //tPhotonManagerInstance.JoinTournamentRoom(tournament);
           // ReferencesHolder.myTournamentPass = tPass;
        }
    }


    public void OnParticipationConfirmationClickEvent()
    {

        Debug.Log(" OnParticipationConfirmationClickEvent() ");

       
        var tId = selectedTournament.Id;

        if(tDetailUIControllerInstance.GetTACToggleStatus())
        {
            tSubMenuUIControllerInstance.SetInteractibilityOfYes(false);
            tFSInteractionManagerInstance.ParticipateOnTournament(ReferencesHolder.playerPublicInfo, tId, delegate { Debug.Log("Participation Failed"); tSubMenuUIControllerInstance.SetInteractibilityOfYes(true); }, tSubMenuUIControllerInstance.OpenMsg);
            tDetailUIControllerInstance.ResetToggle();
        }
        else
        {
            tSubMenuUIControllerInstance.OpenMsg(TournamentUISubMenuHandler.MsgType.AgreeToTnC);
        }

    }




    #region Tween Controls
    public void OpenCardDetailPanel()
    {
        if (cardListPanelTween != null)
            cardListPanelTween.Kill();

        if (cardDetailPanelTween != null)
            cardDetailPanelTween.Kill();

        if (cardLeaderBoardPanelTween != null )
            cardLeaderBoardPanelTween.Kill();

        if (cardResultPanelTween != null)
            cardResultPanelTween.Kill();

        cardListPanelTween = cardListRect.DOAnchorPosX(leftPoint.anchoredPosition.x, tweenDuration).SetEase(easeTypeIn);
        cardDetailPanelTween = cardDetailRect.DOAnchorPosX(onscreenPoint.anchoredPosition.x, tweenDuration).SetEase(easeTypeOut);
        cardLeaderBoardPanelTween = cardLeaderBoardRect.DOAnchorPosX(rightPoint.anchoredPosition.x, tweenDuration).SetEase(easeTypeOut);
        cardResultPanelTween = cardResultRect.DOAnchorPosX(rightPoint.anchoredPosition.x, tweenDuration).SetEase(easeTypeOut);
    }

    public void OpenCardListPanel()
    {
        if (cardListPanelTween != null)
            cardListPanelTween.Kill();

        if (cardDetailPanelTween != null)
            cardDetailPanelTween.Kill();

        cardListPanelTween   = cardListRect.DOAnchorPosX(onscreenPoint.anchoredPosition.x, tweenDuration).SetEase(easeTypeIn);
        cardDetailPanelTween = cardDetailRect.DOAnchorPosX(rightPoint.anchoredPosition.x, tweenDuration).SetEase(easeTypeOut);
    }

    public void OpenCardLeaderBoardPanel()
    {
       

        if (cardDetailPanelTween != null)
            cardDetailPanelTween.Kill();
        if (cardLeaderBoardPanelTween != null)
            cardLeaderBoardPanelTween.Kill();

        cardLeaderBoardPanelTween = cardLeaderBoardRect.DOAnchorPosX(onscreenPoint.anchoredPosition.x, tweenDuration).SetEase(easeTypeIn);
        cardDetailPanelTween = cardDetailRect.DOAnchorPosX(leftPoint.anchoredPosition.x, tweenDuration).SetEase(easeTypeIn);


    }

    public void OpenCardResultPanel()
    {
        if (cardDetailPanelTween != null)
            cardDetailPanelTween.Kill();
        if (cardResultPanelTween != null)
            cardResultPanelTween.Kill();

        cardResultPanelTween = cardResultRect.DOAnchorPosX(onscreenPoint.anchoredPosition.x, tweenDuration).SetEase(easeTypeIn).OnComplete(delegate { tResultUIControllerInstance.PlayConfettiParticle(); ResetDetailButtons(); });
        cardDetailPanelTween = cardDetailRect.DOAnchorPosX(leftPoint.anchoredPosition.x, tweenDuration).SetEase(easeTypeIn);
    }

    public void ResetDetailButtons()
    {
        tDetailUIControllerInstance.SetLeaderBoardInteractable(true);
        tDetailUIControllerInstance.SetParticipationInteractable(true);
    }
    #endregion





}
