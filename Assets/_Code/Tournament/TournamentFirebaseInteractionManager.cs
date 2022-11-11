using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using Booray.Auth;

public class TournamentFirebaseInteractionManager : MonoBehaviour
{
    FirebaseFirestore db;

    private CollectionReference tournamentCollection;

    private void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;

        tournamentCollection = db.Collection(ReferencesHolder.FS_Tournament_Collec);
    }




    private enum ParticipationStatus
    {
        Failed,
        Participated,
        NotEnoughMoney,
        AlreadyExist,
        StatusChange,
        NotEnoughCapacity
    }

    //----------------------==============---------------------------------------------------------==========================================================
    //----------------------==============---------------------------------------------------------==========================================================
    //----------------------==============---------------------------------------------------------==========================================================

    public void AddPlayerCoins(string userID, int amount, Action onFailCallback, Action onSuccessCallback)
    {
        DocumentReference userInfoDocRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(userID)
            .Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);

        userInfoDocRef.UpdateAsync(new Dictionary<string, object> { { "Coins", FieldValue.Increment(amount) } }).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {


                Debug.Log($" Added Coins to Player ->  {amount} ");
                onSuccessCallback?.Invoke();

            }
            else
            {
                onFailCallback?.Invoke();
            }
        });
    }
    
    
    public void SyncPlayerCoins(string userId)
    {
        db.Collection(ReferencesHolder.FS_users_Collec).Document(userId)
            .Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc)
            .GetSnapshotAsync().ContinueWithOnMainThread(Snap => 
            {
                if(Snap.IsFaulted || Snap.IsCanceled)
                {
                    Debug.Log("Syncing money failed");
                    return;
                }

                if(!Snap.Result.Exists)
                {

                    Debug.Log(" Not exist ");
                    return;
                }
                var userinfo = Snap.Result.ConvertTo<PublicInfoDB>();


                MainUIManager.Instance.HomeUI.UpdateCoins(userinfo.Coins);
               


            });
    }

    #region General

    public void GetAllTournamentInfoFromFS(System.Action onFailedCallback, System.Action<List<TournamentDB>> onSuccessCallback)
    {

        Debug.Log(" Interacting Firebase -> Tournament Info ");

        Query collecRef = db.Collection(ReferencesHolder.FS_Tournament_Collec).WhereNotIn("Status",new List<object> { "Draft","Cancelled"});/* WhereNotEqualTo("Status", "Draft").WhereNotEqualTo("Status", "Cancelled");*/

        collecRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {

                QuerySnapshot snapshot = task.Result;

                List<TournamentDB> tournamentInfoList = new List<TournamentDB>();
                //On Success
                Debug.Log(" Interacting Firebase -> Tournament Info Get  -> Success");


                foreach (DocumentSnapshot document in snapshot.Documents)
                {

                    //Debug.Log($"Document {document.Id}  ");

                    Dictionary<string, object> tt = document.ToDictionary();



                    TournamentDB info = document.ConvertTo<TournamentDB>();
                    //TournamentDB info = 

                    //Debug.Log($" tournament -> {tt["Id"]} | {tt["Name"]} | {tt["Status"]} ");


                    tournamentInfoList.Add(info);




                }

                onSuccessCallback?.Invoke(tournamentInfoList);
                // Invoke success and send the list to the callback




            }
            else
            {
                //On failure
                Debug.Log(" Interacting Firebase -> Tournament Info Get  -> Failure");
                onFailedCallback?.Invoke();
            }


        });


    }



    public void CheckStatus(string tournamentId, string currentStatus, System.Action onFailedCallback, System.Action<TournamentUISubMenuHandler.MsgType> onSuccessCallback)
    {
        tournamentCollection.Document(tournamentId).GetSnapshotAsync().ContinueWithOnMainThread(snapshot =>
        {
            if (snapshot.IsCompleted)
            {
                TournamentDB tInfo = snapshot.Result.ConvertTo<TournamentDB>();

                if (tInfo.Status == currentStatus)
                {
                    switch (currentStatus)
                    {
                        case "Announced":
                            {
                                Debug.Log("Coming soon...");

                                onSuccessCallback?.Invoke(TournamentUISubMenuHandler.MsgType.ComingSoon);
                                break;
                            }
                        case "Application Closed":
                            {
                                Debug.Log("Applications have been Closed...");
                                onSuccessCallback?.Invoke(TournamentUISubMenuHandler.MsgType.ApplicationEnded);
                                break;
                            }
                        case "Tournament End":
                            {

                                Debug.Log("Tournament Has Ended...");
                                onSuccessCallback?.Invoke(TournamentUISubMenuHandler.MsgType.TournamentEnded);
                                break;
                            }
                    }
                }
                else
                {
                    Debug.Log("Status Changed : Please Refresh Your Tournament List ");

                    onSuccessCallback?.Invoke(TournamentUISubMenuHandler.MsgType.StatusChange);

                }

            }
            else
            {

            }

        });



    }

    #endregion


    //----------------------==============---------------------------------------------------------==========================================================
    //----------------------==============---------------------------------------------------------==========================================================
    //----------------------==============---------------------------------------------------------==========================================================


    #region Announcement

    #endregion


    //----------------------==============---------------------------------------------------------==========================================================
    //----------------------==============---------------------------------------------------------==========================================================
    //----------------------==============---------------------------------------------------------==========================================================



    #region Participation
    public void checkIfPlayerExistAsParticipant(string userId, TournamentDB tInfo, System.Action onFailCallback, System.Action<TournamentDB, TournamentPassDB> onSuccessCallback)
    {

        Debug.Log(" Interacting Firebase -> Check if player exists as participant ");


        db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tInfo.Id)
            .Collection(ReferencesHolder.FS_TournamentParticipants_Collec).Document(userId).GetSnapshotAsync().ContinueWithOnMainThread(snapshot =>
            {
                if (snapshot.IsCompleted)
                {
                    if (snapshot.Result.Exists)
                    {




                        var pass = snapshot.Result.ConvertTo<TournamentPassDB>();

                        if (pass.tournamentCoins >= tInfo.AnteAmount)
                        {

                            onSuccessCallback.Invoke(tInfo, pass);
                        }
                        else
                        {

                            Debug.Log("Not Enough Tournament Points");


                            onSuccessCallback.Invoke(tInfo, null);
                        }


                        Debug.Log(" Player Exists ");

                    }

                    else
                    {
                        Debug.Log(" Player does not exist as participant");
                        onSuccessCallback?.Invoke(null, null);
                    }
                }
                else
                {


                    onFailCallback?.Invoke();



                }






            });


    }

    public async void SubscribeToTopic(string topic)
    {
        await Firebase.Messaging.FirebaseMessaging.SubscribeAsync($"/topics/{topic}");
        SaveTopic(topic);



    }
    public void SaveTopic(string topic)
    {
        DocumentReference docRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId).Collection(ReferencesHolder.FS_UserTournaments_Collec).Document(topic);
        var Topics = new TopicTournamentDB
        {
            TournamentID = topic

        };
        docRef.SetAsync(Topics).ContinueWithOnMainThread(task => {
            Debug.Log("Tournament Id have been added");


          
        });
    }


    public void ParticipateOnTournament(PublicInfoDB publicInfo, string tournamentId, 
        Action onFailCallback, Action<TournamentUISubMenuHandler.MsgType> onsuccessCallback)
    {

        TournamentPassDB pass = new TournamentPassDB();
        pass.userId = publicInfo.UserId;
        pass.userName = publicInfo.UserName;
        pass.imageURL = publicInfo.PictureURL;
        pass.avatarId = publicInfo.AvatarID;
        pass.avatarUsed = publicInfo.AvatarUsed;
        pass.points = 0;
        pass.tournamentCoins = 1000;



        DocumentReference tournamentDocRef = db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentId);

        DocumentReference passDocRef = db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentId)
            .Collection(ReferencesHolder.FS_TournamentParticipants_Collec).Document(publicInfo.UserId);

        DocumentReference userInfoDocRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(publicInfo.UserId)
            .Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);


        TournamentDB temp;

        passDocRef.GetSnapshotAsync().ContinueWithOnMainThread(task => 
        {
            if(task.IsFaulted || task.IsCanceled)
            {
                onFailCallback?.Invoke();
            }

            if(task.Result.Exists)
            {
                onsuccessCallback?.Invoke(TournamentUISubMenuHandler.MsgType.AlreadyExist);
            }
            else
            {
                db.RunTransactionAsync(trans => 
                {
                    return trans.GetSnapshotAsync(tournamentDocRef).ContinueWith(snap => 
                    {
                        TournamentDB tournamentObject = snap.Result.ConvertTo<TournamentDB>();

                        if (tournamentObject.Status.Equals("Application Start") 
                        && tournamentObject.CurrentParticipants < tournamentObject.MaximumParticipants 
                        && publicInfo.Coins >= tournamentObject.ParticipationFees)

                        {
                            //return ParticipationStatus.Participated;
                            // subscribe 

                            

                            string topic = $"Topic_{tournamentObject.Id}";
                            //Firebase.Messaging.FirebaseMessaging.SubscribeAsync($"/topics/{topic}");

                            //NotificationHandler.instance.RefreshToken();
                            //NotificationHandler.instance.UpdateToken();

                            SubscribeToTopic(topic);

                            trans.Set(passDocRef, pass);

                            trans.Update(tournamentDocRef, new Dictionary<string, object> { { "CurrentParticipants", FieldValue.Increment(1) } });

                            trans.Update(userInfoDocRef, new Dictionary<string, object> { { "Coins", FieldValue.Increment(-tournamentObject.ParticipationFees) } });

                            temp = tournamentObject;

                            return ParticipationStatus.Participated;
                            //else
                                //{
                                //    transaction.Set(passDocRef, pass);
                                //    return ParticipationStatus.Participated;
                                //}
                            

                        }
                        else
                        {
                            if(!tournamentObject.Status.Equals("Application Start"))
                            {
                                return ParticipationStatus.StatusChange;
                            }
                            else if(tournamentObject.CurrentParticipants >= tournamentObject.MaximumParticipants)
                            {
                                return ParticipationStatus.NotEnoughCapacity;
                            }
                            else if(publicInfo.Coins < tournamentObject.ParticipationFees)
                            {
                                return ParticipationStatus.NotEnoughMoney;
                            }

                            return ParticipationStatus.Failed;
                        }



                    });
                }).ContinueWithOnMainThread(task => 
                {
                    if(task.IsCompleted)
                    {

                        switch (task.Result)
                        {
                            case ParticipationStatus.Participated:
                                {
                                    onsuccessCallback?.Invoke(TournamentUISubMenuHandler.MsgType.Participated);

                                    SyncPlayerCoins(publicInfo.UserId);

                                    Debug.Log("Participated");
                                    break;
                                }
                            case ParticipationStatus.NotEnoughMoney:
                                {
                                    onsuccessCallback?.Invoke(TournamentUISubMenuHandler.MsgType.NotEnoughMoney);
                                    Debug.Log("No money");
                                    break;
                                }
                            case ParticipationStatus.AlreadyExist:
                                {
                                    onsuccessCallback?.Invoke(TournamentUISubMenuHandler.MsgType.AlreadyExist);
                                    Debug.Log("Already exist");
                                    break;
                                }
                            case ParticipationStatus.StatusChange:
                                {
                                    onsuccessCallback?.Invoke(TournamentUISubMenuHandler.MsgType.StatusChange);
                                    Debug.Log("Status Change");
                                    break;
                                }
                            case ParticipationStatus.Failed:
                                {
                                    onsuccessCallback?.Invoke(TournamentUISubMenuHandler.MsgType.ParticipationFailed);
                                    Debug.Log("Failed");
                                    break;
                                }
                            case ParticipationStatus.NotEnoughCapacity:
                                {
                                    onsuccessCallback?.Invoke(TournamentUISubMenuHandler.MsgType.NoCapacity);
                                    Debug.Log("Not Enough Capacity");
                                    break;
                                }

                            default:
                                {
                                    Debug.Log("Fail");
                                    break;
                                }
                        }

                        Debug.Log(" boo ");
                    }
                    else if(task.IsFaulted || task.IsCanceled)
                    {
                        onFailCallback?.Invoke();
                    }

                    



                });




            }




        });

        #region Simple Impementation
        //db.RunTransactionAsync(transaction => 
        //{
        //    return transaction.GetSnapshotAsync(tournamentDocRef).ContinueWith(snapshot => 
        //    {
        //        if(snapshot.IsFaulted || snapshot.IsCanceled)
        //        {
        //            return ParticipationStatus.Failed;
        //        }



        //        TournamentDB tournamentObject = snapshot.Result.ConvertTo<TournamentDB>();

        //        if(tournamentObject.Status.Equals("Application Start") && tournamentObject.CurrentParticipants<tournamentObject.MaximumParticipants)
        //        {
        //            //return ParticipationStatus.Participated;

        //             transaction.GetSnapshotAsync(passDocRef).ContinueWith(task => 
        //            {
        //                if(task.Result.Exists)
        //                {
        //                    return ParticipationStatus.AlreadyExist;
        //                }
        //                else
        //                {
        //                    transaction.Set(passDocRef, pass);
        //                    return ParticipationStatus.Participated;
        //                }
        //            });

        //        }

        //        return ParticipationStatus.NotEnoughCapacity;



        //    });


        //}).ContinueWith(transactionResult => 
        //{
        //    if(transactionResult.IsCompleted)
        //    {
        //        Debug.Log(transactionResult.Result);
        //    }



        //});
        #endregion




    }
    #endregion



    //----------------------==============---------------------------------------------------------==========================================================
    //----------------------==============---------------------------------------------------------==========================================================
    //----------------------==============---------------------------------------------------------==========================================================



    #region Playing & Leader Board

    public void ValidatingPlay(string userId, TournamentDB tInfo, System.Action onFailCallback, System.Action<TournamentDB, TournamentPassDB,int> onSuccessCallback)
    {

        Debug.Log(" Interacting Firebase -> Check if player exists as participant ");

        db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tInfo.Id).GetSnapshotAsync().ContinueWithOnMainThread(snapshot =>
        {
            if (snapshot.IsFaulted || snapshot.IsCanceled)
            {
                onFailCallback?.Invoke();
                return;
            }

            if (snapshot.Result.Exists)
            {
                var t = snapshot.Result.ConvertTo<TournamentDB>();

                if (t.Status.Equals("Tournament Start"))
                {
                    db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tInfo.Id)
                    .Collection(ReferencesHolder.FS_TournamentParticipants_Collec).Document(userId).GetSnapshotAsync().ContinueWithOnMainThread(snapshot =>
                    {
                        if (snapshot.IsCompleted)
                        {
                            if (snapshot.Result.Exists)
                            {




                                var pass = snapshot.Result.ConvertTo<TournamentPassDB>();

                                if (pass.tournamentCoins >= t.AnteAmount)
                                {

                                    onSuccessCallback.Invoke(t, pass,0);
                                }
                                else
                                {

                                    Debug.Log("Not Enough Tournament Points");


                                    onSuccessCallback.Invoke(t,pass,1);
                                }


                                Debug.Log(" Player Exists ");

                            }

                            else
                            {
                                Debug.Log(" Player does not exist as participant");
                                onSuccessCallback?.Invoke(t, null,-1);
                            }
                        }
                        else
                        {


                            onFailCallback?.Invoke();



                        }






                    });

                }
                else
                {
                    /// If status is not equal to Tournament Start
                    /// 
                    onSuccessCallback?.Invoke(t, null,-2);

                }

            }


        });






    }



    public void GetBestPlayersInTournament(string tournamentId, System.Action onFailedCallback, System.Action<List<TournamentPassDB>> OnSuccessCallback)
    {
        Query collecRef = db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentId).Collection(ReferencesHolder.FS_TournamentParticipants_Collec)
            .OrderByDescending("points").Limit(15);

        List<TournamentPassDB> topPlayersList = new List<TournamentPassDB>();

        collecRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                onFailedCallback?.Invoke();
                return;
            }


            QuerySnapshot snapshot = task.Result;


            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                TournamentPassDB tPass = document.ConvertTo<TournamentPassDB>();

                topPlayersList.Add(tPass);
            }


            OnSuccessCallback?.Invoke(topPlayersList);


        });
    }

    public void GetMyPassInTournament(string tournamentId, string userId, System.Action onFailedCallback, System.Action<TournamentPassDB> OnSuccessCallback)
    {
        db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentId)
            .Collection(ReferencesHolder.FS_TournamentParticipants_Collec).Document(userId).GetSnapshotAsync().ContinueWithOnMainThread(snapshot =>
            {

                if (snapshot.IsCanceled || snapshot.IsFaulted)
                {
                    onFailedCallback?.Invoke();
                    return;
                }


                if (snapshot.Result.Exists)
                {
                    var tpass = snapshot.Result.ConvertTo<TournamentPassDB>();

                    OnSuccessCallback?.Invoke(tpass);


                }






            });
    }

    public void GetBestPlayersInTournament(string userId, string tournamentId, System.Action onFailedCallback,
        System.Action<List<TournamentPassDB>> OnSuccessCallback1, System.Action<TournamentPassDB> onSuccessCallback2)
    {
        Query collecRef = db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentId).Collection(ReferencesHolder.FS_TournamentParticipants_Collec)
            .OrderByDescending("points").Limit(50);







        List<TournamentPassDB> topPlayersList = new List<TournamentPassDB>();

        collecRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                onFailedCallback?.Invoke();
                return;
            }


            QuerySnapshot snapshot = task.Result;


            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                TournamentPassDB tPass = document.ConvertTo<TournamentPassDB>();

                topPlayersList.Add(tPass);
            }


            OnSuccessCallback1?.Invoke(topPlayersList);


        });
    }

    public void GetWinnerOfTournamentFromFS(string tournamentId, System.Action onFailCallback, System.Action<TournamentPassDB> onSuccessCallback)
    {
        Query collecRef = db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentId)
            .Collection(ReferencesHolder.FS_TournamentParticipants_Collec).OrderByDescending("points").Limit(1);

        List<TournamentPassDB> top5Players = new List<TournamentPassDB>();

        collecRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {



                onFailCallback?.Invoke();
                return;
            }


            QuerySnapshot snapshot = task.Result;


            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                TournamentPassDB tPass = document.ConvertTo<TournamentPassDB>();

                top5Players.Add(tPass);
            }


            onSuccessCallback?.Invoke(top5Players[0]);



                //OnSuccessCallback?.Invoke(topPlayersList);


            });
    }


    public void AvailTournamentPrizeFromFS(string userId, string tournamentId, System.Action onFailedCallback, System.Action SuccessCallback)
    {







    }

    #endregion


    //----------------------==============---------------------------------------------------------==========================================================
    //----------------------==============---------------------------------------------------------==========================================================
    //----------------------==============---------------------------------------------------------==========================================================



    #region  Not Needed
    public void SetPlayerInsideRoom(TournamentPassDB tPass, string tournamentId, System.Action onFailCallback, System.Action onSuccessCallback)
    {
        Debug.Log(" Interacting Firebase -> SetPlayerInsideRoom ");



    }

    #endregion

}








    
