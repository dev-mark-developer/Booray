using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.Networking;
using System;
using TMPro;
using UnityEngine.UI;
public class GameFireBaseInteractionManager : MonoBehaviour
{
    FirebaseFirestore db;
    public ViewProfileManager ViewProfileManager;
    private void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;
    }


    /// <summary>
    ///  Use Sahi Document Reference
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="amount"></param>
    /// <param name="deductAsMany"></param>
    /// <param name="onFailCallback"></param>
    /// <param name="onSuccessCallback"></param>
    /// 

    #region BOORAY COINS FUNCTIONS

    public void DeductPlayerCoinsForAnte(string userID, int amount, bool deductAsMany, Action onFailCallback, Action<bool, int> onSuccessCallback)
    {

        Debug.Log($" Interacting Firebase -> {userID}  Deducting Coins - {amount} Deduct As Many {deductAsMany} ");

        DocumentReference userInfoDocRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(userID)
            .Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);


        int updatedCoinState = 0;

        db.RunTransactionAsync(transaction =>
        {
            Debug.Log("Inside Transactions");

            return transaction.GetSnapshotAsync(userInfoDocRef).ContinueWith(snapshotResult =>
            {
                if (snapshotResult.IsCompleted)
                {
                    Debug.Log("Inside Transactions -> Found Snapshot");
                    if (snapshotResult.Result.Exists)
                    {
                        var playerInfo = snapshotResult.Result.ConvertTo<PublicInfoDB>();

                        updatedCoinState = playerInfo.Coins;

                        if (playerInfo.Coins >= amount)
                        {
                            Debug.Log("Inside Transactions -> Deducting");

                            transaction.Update(userInfoDocRef, "Coins", FieldValue.Increment(-amount));

                            return 1;

                            //onSuccessCallback?.Invoke(true);
                        }
                        else if (deductAsMany)
                        {
                            Debug.Log("Inside Transactions -> Deducting");

                            amount = playerInfo.Coins;

                            transaction.Update(userInfoDocRef, "Coins", FieldValue.Increment(-amount));

                            return 1;
                            // onSuccessCallback?.Invoke(true);
                        }
                        else
                        {
                            Debug.Log("Inside Transactions -> cannot deduct .. Not many coins");


                            return 0;

                            // onSuccessCallback?.Invoke(false);
                        }
                    }

                }
                else
                {

                    //  onFailCallback?.Invoke();
                    /// Try Again
                    /// 

                    return -1;
                }

                return -1;
            });
        }).ContinueWithOnMainThread(transactionResult =>
        {
            if (transactionResult.Result == 1)
            {
                onSuccessCallback?.Invoke(true, updatedCoinState - amount);
            }
            else if (transactionResult.Result == 0)
            {
                onSuccessCallback?.Invoke(false, 0);
            }
            else
            {
                onFailCallback?.Invoke();
            }
        });
    }

    public void DeductPlayerCoinsOnBooed(string userID, int amount, bool deductAsMany, Action onFailCallback, Action<int, int> onSuccessCallback)  // DeductAmount
    {
        Debug.Log($" Interacting Firebase -> {userID}  Deducting Coins - {amount} Deduct As Many {deductAsMany} ");




        DocumentReference userInfoDocRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(userID)
            .Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);


        int deductAmount = amount;
        int playerCoins = 0;

        db.RunTransactionAsync(transaction =>
        {
            Debug.Log("Inside Transactions");

            return transaction.GetSnapshotAsync(userInfoDocRef).ContinueWith(snapshotResult =>
            {
                if (snapshotResult.IsCompleted)
                {
                    Debug.Log("Inside Transactions -> Found Snapshot");
                    if (snapshotResult.Result.Exists)
                    {
                        var playerInfo = snapshotResult.Result.ConvertTo<PublicInfoDB>();

                        playerCoins = playerInfo.Coins;

                        if (playerInfo.Coins >= deductAmount)
                        {
                            Debug.Log("Inside Transactions -> Deducting");

                            transaction.Update(userInfoDocRef, "Coins", FieldValue.Increment(-deductAmount));

                            return true;

                            //onSuccessCallback?.Invoke(true);
                        }
                        else if (deductAsMany)
                        {
                            Debug.Log("Inside Transactions -> Deducting");

                            deductAmount = playerInfo.Coins;

                            transaction.Update(userInfoDocRef, "Coins", FieldValue.Increment(-deductAmount));

                            return true;
                            // onSuccessCallback?.Invoke(true);
                        }
                        else
                        {
                            Debug.Log("Inside Transactions -> cannot deduct .. Not many coins");


                            return true;

                            // onSuccessCallback?.Invoke(false);
                        }
                    }

                }
                else
                {

                    //  onFailCallback?.Invoke();
                    /// Try Again
                    /// 

                    return false;
                }

                return false;
            });
        }).ContinueWithOnMainThread(transactionResult =>
        {
            if (transactionResult.Result)
            {
                onSuccessCallback?.Invoke(playerCoins - deductAmount, deductAmount);
            }
            else
            {
                onFailCallback?.Invoke();
            }
        });
    }

    public void AddPlayerCoins(string userID, int amount, Action onFailCallback, Action onSuccessCallback)
    {
        DocumentReference userInfoDocRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(userID)
            .Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);

        db.RunTransactionAsync(transaction =>
        {
            return transaction.GetSnapshotAsync(userInfoDocRef).ContinueWith(snapshot =>
            {

                transaction.Update(userInfoDocRef, new Dictionary<string, object> { { "Coins", FieldValue.Increment(amount) } });

            });
        }).ContinueWithOnMainThread(transactionResult =>
        {
            if (transactionResult.IsFaulted || transactionResult.IsCanceled)
            {
                onFailCallback?.Invoke();
            }
            else
            {
                Debug.Log($" Added Coins to Player ->  {amount} ");
                onSuccessCallback?.Invoke();
            }
        });


        //userInfoDocRef.UpdateAsync(new Dictionary<string, object> { { "Coins", FieldValue.Increment(amount) } }).ContinueWithOnMainThread(task => 
        //{
        //    if(task.IsCompleted)
        //    {


        //        Debug.Log($" Added Coins to Player ->  {amount} ");
        //        onSuccessCallback?.Invoke();

        //    }
        //    else
        //    {
        //        onFailCallback?.Invoke();
        //    }
        //});
    }

    public void CheckPlayerCoinsWithAnte(string userID, int ante, Action onFailCallback, Action<bool> onSuccessCallback)
    {
        DocumentReference userInfoDocRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(userID)
           .Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);


        userInfoDocRef.GetSnapshotAsync().ContinueWithOnMainThread(snapshot =>
        {
            if (snapshot.IsCanceled || snapshot.IsFaulted)
            {
                onFailCallback?.Invoke();
                return;
            }

            var playerInfo = snapshot.Result.ConvertTo<PublicInfoDB>();

            if (playerInfo.Coins >= ante)
            {
                onSuccessCallback?.Invoke(true);
            }
            else
            {
                onSuccessCallback?.Invoke(false);
            }

        });


        //db.RunTransactionAsync(transaction => 
        //{
        //    return transaction.GetSnapshotAsync(userInfoDocRef).ContinueWith(snapshot => 
        //    {
        //        if(snapshot.Result.Exists)
        //        {
        //            var playerInfo = snapshot.Result.ConvertTo<PublicInfoDB>();

        //            if(playerInfo.Coins>= ante)
        //            {

        //            }
        //        }
        //    });
        //});
    }

    #endregion


    #region TOURNAMENTFUNCTIONS


    public void AddTournamentPoints(string userID, string tournamentID, int amount, Action onFailCallback, Action onSuccessCallback)
    {


        Debug.Log($" Interacting Firebase -> Adding Tournament Points  {userID } - {tournamentID} - {amount} ");

        DocumentReference userTournamentPass = db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentID)
           .Collection(ReferencesHolder.FS_TournamentParticipants_Collec).Document(userID);

        userTournamentPass.UpdateAsync(new Dictionary<string, object> { { "points", FieldValue.Increment(amount) } }).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                onFailCallback?.Invoke();
                return;
            }

            onSuccessCallback?.Invoke();
        });
    }



    public void DeductPlayerTournamentCoinsForAnte(string userID, string tournamentId, int amount, Action onFailCallback, Action<bool, int> onSuccessCallback)
    {
        Debug.Log($" Interacting Firebase -> {userID}  Deducting Coins - {amount}  ");


        DocumentReference usertPassDocRef = db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentId)
            .Collection(ReferencesHolder.FS_TournamentParticipants_Collec).Document(userID);


        int updatedPoints = 0;

        db.RunTransactionAsync(Transaction =>
        {
            Debug.Log("Inside Transactions");


            return Transaction.GetSnapshotAsync(usertPassDocRef).ContinueWith(snapshotResult =>
            {

                if (snapshotResult.IsCanceled || snapshotResult.IsFaulted)
                {
                    //onFailCallback?.Invoke();
                    return -1;
                }

                if (snapshotResult.Result.Exists)
                {
                    var playerInfo = snapshotResult.Result.ConvertTo<TournamentPassDB>();

                    updatedPoints = playerInfo.tournamentCoins;

                    if (playerInfo.tournamentCoins >= amount)
                    {
                        Debug.Log("Inside Transactions -> Deducting");

                        Transaction.Update(usertPassDocRef, "tournamentCoins", FieldValue.Increment(-amount));

                        return 1;
                    }
                    else
                    {
                        Debug.Log("Inside Transactions -> cannot deduct .. Not many coins");


                        return 0;
                    }
                }

                return -1;

            });

        }).ContinueWithOnMainThread(transactionResult =>
        {
            if (transactionResult.Result == 1)
            {
                onSuccessCallback?.Invoke(true, updatedPoints - amount);
            }
            else if (transactionResult.Result == 0)
            {
                onSuccessCallback?.Invoke(false, 0);
            }
            else
            {
                onFailCallback?.Invoke();
            }
        });


    }

    public void DeductPlayerTournamentCoinsOnBooed(string userID, string tournamentId, int amount, bool deductAsMany, Action onFailCallback, Action<int, int> onSuccessCallback)  // DeductAmount
    {
        Debug.Log($" Interacting Firebase -> {userID}  Deducting Tournament Coins - {amount} Deduct As Many {deductAsMany} ");




        DocumentReference usertPassDocRef = db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentId)
            .Collection(ReferencesHolder.FS_TournamentParticipants_Collec).Document(userID);


        int deductAmount = amount;
        int playerCoins = 0;

        db.RunTransactionAsync(transaction =>
        {
            Debug.Log("Inside Transactions");

            return transaction.GetSnapshotAsync(usertPassDocRef).ContinueWith(snapshotResult =>
            {
                if (snapshotResult.IsCompleted)
                {
                    Debug.Log("Inside Transactions -> Found Snapshot");
                    if (snapshotResult.Result.Exists)
                    {
                        var playerInfo = snapshotResult.Result.ConvertTo<TournamentPassDB>();

                        playerCoins = playerInfo.tournamentCoins;

                        if (playerInfo.tournamentCoins >= deductAmount)
                        {
                            Debug.Log("Inside Transactions -> Deducting");

                            transaction.Update(usertPassDocRef, "tournamentCoins", FieldValue.Increment(-deductAmount));

                            return true;

                            //onSuccessCallback?.Invoke(true);
                        }
                        else if (deductAsMany)
                        {
                            Debug.Log("Inside Transactions -> Deducting");

                            deductAmount = playerInfo.tournamentCoins;

                            transaction.Update(usertPassDocRef, "tournamentCoins", FieldValue.Increment(-deductAmount));

                            return true;
                            // onSuccessCallback?.Invoke(true);
                        }
                        else
                        {
                            Debug.Log("Inside Transactions -> cannot deduct .. Not many coins");


                            return true;

                            // onSuccessCallback?.Invoke(false);
                        }
                    }

                }
                else
                {

                    //  onFailCallback?.Invoke();
                    /// Try Again
                    /// 

                    return false;
                }

                return false;
            });
        }).ContinueWithOnMainThread(transactionResult =>
        {
            if (transactionResult.Result)
            {
                onSuccessCallback?.Invoke(playerCoins - deductAmount, deductAmount);
            }
            else
            {
                onFailCallback?.Invoke();
            }
        });
    }


    public void AddPlayerTournamentCoins(string userID, string tournamentId, int amount, Action onFailCallback, Action onSuccessCallback)
    {
        DocumentReference usertPassDocRef = db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentId)
            .Collection(ReferencesHolder.FS_TournamentParticipants_Collec).Document(userID);

        usertPassDocRef.UpdateAsync(new Dictionary<string, object> { { "tournamentCoins", FieldValue.Increment(amount) } }).ContinueWithOnMainThread(task =>
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




    /// <summary>
    /// Old Methods
    /// </summary>


    //public void DeductTournamentCoins(string userID, string tournamentID,int amount, bool deductAsMany, Action onFailCallback, Action <bool> onSuccessCallback)
    //{
    //    Debug.Log($" Interacting Firebase -> {userID}  Deducting Coins - {amount} Deduct As Many {deductAsMany} ");




    //    DocumentReference userTournamentPass = db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentID)
    //        .Collection(ReferencesHolder.FS_TournamentParticipants_Collec).Document(userID);

    //    db.RunTransactionAsync(transaction =>
    //    {
    //        Debug.Log("Inside Transactions");

    //        return transaction.GetSnapshotAsync(userTournamentPass).ContinueWith(snapshotResult =>
    //        {
    //            if (snapshotResult.IsCompleted)
    //            {
    //                Debug.Log("Inside Transactions -> Found Snapshot");
    //                if (snapshotResult.Result.Exists)
    //                {
    //                    var playertPass = snapshotResult.Result.ConvertTo<TournamentPassDB>();

    //                    if (playertPass.tournamentCoins >= amount)
    //                    {
    //                        Debug.Log("Inside Transactions -> Deducting");

    //                        transaction.Update(userTournamentPass, "tournamentCoins", FieldValue.Increment(-amount));

    //                        return 1;

    //                        //onSuccessCallback?.Invoke(true);
    //                    }
    //                    else if (deductAsMany)
    //                    {
    //                        Debug.Log("Inside Transactions -> Deducting");

    //                        transaction.Update(userTournamentPass, "tournamentCoins", FieldValue.Increment(-playertPass.tournamentCoins));

    //                        return 1;
    //                        // onSuccessCallback?.Invoke(true);
    //                    }
    //                    else
    //                    {
    //                        Debug.Log("Inside Transactions -> cannot deduct .. Not many Coins");


    //                        return 0;

    //                        // onSuccessCallback?.Invoke(false);
    //                    }
    //                }

    //            }
    //            else
    //            {

    //                //  onFailCallback?.Invoke();
    //                /// Try Again
    //                /// 

    //                return -1;
    //            }

    //            return -1;
    //        });
    //    }).ContinueWithOnMainThread(transactionResult =>
    //    {
    //        if (transactionResult.Result == 1)
    //        {
    //            onSuccessCallback?.Invoke(true);
    //        }
    //        else if (transactionResult.Result == 0)
    //        {
    //            onSuccessCallback?.Invoke(false);
    //        }
    //        else
    //        {
    //            onFailCallback?.Invoke();
    //        }
    //    });
    //}

    //public void AddTournamentCoins(string userID, string tournamentID, int amount , Action onFailCallback, Action onSuccessCallback)
    //{
    //    Debug.Log($" Interacting Firebase -> Adding Tournament Coins  {userID } - {tournamentID} - {amount} ");

    //    DocumentReference userTournamentPass = db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentID)
    //       .Collection(ReferencesHolder.FS_TournamentParticipants_Collec).Document(userID);

    //    userTournamentPass.UpdateAsync(new Dictionary<string, object> { { "tournamentCoins", FieldValue.Increment(amount) } }).ContinueWithOnMainThread(task =>
    //    {
    //        if (task.IsFaulted || task.IsCanceled)
    //        {
    //            onFailCallback?.Invoke();
    //            return;
    //        }



    //        onSuccessCallback?.Invoke();


    //    });
    //}

    //public void DeductTournamentPoints(string userID,string tournamentID, int amount, bool deductAsMany, Action onFailCallback, Action<bool> onSuccessCallback)
    //{
    //    Debug.Log($" Interacting Firebase -> {userID}  Deducting Coins - {amount} Deduct As Many {deductAsMany} ");




    //    DocumentReference userTournamentPass = db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentID)
    //        .Collection(ReferencesHolder.FS_TournamentParticipants_Collec).Document(userID);




    //    db.RunTransactionAsync(transaction =>
    //    {
    //        Debug.Log("Inside Transactions");

    //        return transaction.GetSnapshotAsync(userTournamentPass).ContinueWith(snapshotResult =>
    //        {
    //            if (snapshotResult.IsCompleted)
    //            {
    //                Debug.Log("Inside Transactions -> Found Snapshot");
    //                if (snapshotResult.Result.Exists)
    //                {
    //                    var playertPass = snapshotResult.Result.ConvertTo<TournamentPassDB>();

    //                    if (playertPass.points >= amount)
    //                    {
    //                        Debug.Log("Inside Transactions -> Deducting");

    //                        transaction.Update(userTournamentPass, "points", FieldValue.Increment(-amount));

    //                        return 1;

    //                        //onSuccessCallback?.Invoke(true);
    //                    }
    //                    else if (deductAsMany)
    //                    {
    //                        Debug.Log("Inside Transactions -> Deducting");

    //                        transaction.Update(userTournamentPass, "points", FieldValue.Increment(-playertPass.points));

    //                        return 1;
    //                        // onSuccessCallback?.Invoke(true);
    //                    }
    //                    else
    //                    {
    //                        Debug.Log("Inside Transactions -> cannot deduct .. Not many points");


    //                        return 0;

    //                        // onSuccessCallback?.Invoke(false);
    //                    }
    //                }

    //            }
    //            else
    //            {

    //                //  onFailCallback?.Invoke();
    //                /// Try Again
    //                /// 

    //                return -1;
    //            }

    //            return -1;
    //        });
    //    }).ContinueWithOnMainThread(transactionResult =>
    //    {
    //        if (transactionResult.Result == 1)
    //        {
    //            onSuccessCallback?.Invoke(true);
    //        }
    //        else if (transactionResult.Result == 0)
    //        {
    //            onSuccessCallback?.Invoke(false);
    //        }
    //        else
    //        {
    //            onFailCallback?.Invoke();
    //        }
    //    });

    //}




    //public void PromoteTournamentPlayer(string userId, string tournamentID, Action onFailCallback, Action onSuccessCalback)
    //{

    //    Debug.Log(" Interacting Firebase -> Promoting Player ");


    //    DocumentReference tournamentPassDocREf = db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentID).Collection(ReferencesHolder.FS_TournamentParticipants_Collec)
    //        .Document(userId);


    //    db.RunTransactionAsync(transaction =>
    //    {
    //        return transaction.GetSnapshotAsync(tournamentPassDocREf).ContinueWith(snapshotResult =>
    //        {
    //            if (snapshotResult.IsFaulted || snapshotResult.IsCanceled)
    //            {
    //                //onFailCallback?.Invoke();
    //                return false;
    //            }


    //            if (snapshotResult.Result.Exists)
    //            {
    //                var tpass = snapshotResult.Result.ConvertTo<TournamentPassDB>();


    //                if (tpass.level != 0 && tpass.level != -1)
    //                {
    //                    transaction.Update(tournamentPassDocREf, "level", FieldValue.Increment(-1));

    //                    return true;
    //                }

    //                return false;

    //            }

    //            return false;
    //        });
    //    }).ContinueWithOnMainThread(transactionResult =>
    //    {
    //        if (transactionResult.IsCompleted)
    //        {
    //            onSuccessCalback?.Invoke();
    //        }
    //        else
    //        {
    //            onFailCallback?.Invoke();
    //        }
    //    });


    //    //db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentID).Collection(ReferencesHolder.FS_TournamentParticipants_Collec)
    //    //    .Document(userId).GetSnapshotAsync().ContinueWithOnMainThread(snapshotResult => 
    //    //    {





    //    //    });
    //}


    public void RemovePlayerFromTournament(string userId, string tournamentID, Action onFailCallback, Action onSuccessCallback)
    {
        db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentID)
            .Collection(ReferencesHolder.FS_TournamentParticipants_Collec).Document(userId).UpdateAsync(new Dictionary<string, object> { { "level", -1 }, { "roomId", "" } })
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    onFailCallback?.Invoke();
                    return;
                }
                onSuccessCallback?.Invoke();
            });
    }


    public void CheckAndRemovePlayerFromT(string userID, int Ante, string tournamentID, Action onFailCallback, Action onSuccessCallback)
    {

        DocumentReference tournamentPass = db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentID)
            .Collection(ReferencesHolder.FS_TournamentParticipants_Collec).Document(userID);



        db.RunTransactionAsync(transaction =>
        {
            return transaction.GetSnapshotAsync(tournamentPass).ContinueWith(snapshot =>
            {
                if (snapshot.Result.Exists)
                {
                    var tPass = snapshot.Result.ConvertTo<TournamentPassDB>();

                    if (tPass.points < Ante)
                    {
                        //transaction.Update(tournamentPass, "level", -1);

                        return 1;
                    }
                    else
                    {
                        return 0;
                    }



                }
                return -1;
            });
        }).ContinueWithOnMainThread(transactionResult =>
        {
            if (transactionResult.Result == 1)
            {
                onSuccessCallback?.Invoke();
            }
            else if (transactionResult.Result == 0)
            {
                onSuccessCallback?.Invoke();
            }
            else
            {
                onFailCallback?.Invoke();
            }
        });


    }


    public void CheckIfWinnerOfTournamentRound(string roomId, string tournamentID, int level, Action onFailCallback, Action onSuccessCallback)
    {
        db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentID)
            .Collection(ReferencesHolder.FS_TournamentParticipants_Collec).WhereEqualTo("roomId", roomId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                QuerySnapshot querySnapShot = task.Result;

                //foreach(DocumentSnapshot in )

            });
    }

    #endregion


    public void CheckIfPlayerHasEnoughCoinsToPlayHand(string userId, bool inTournament, string tournamentID, int anteAmount, Action onFailCallback, Action<bool> onSuccessCallback)
    {
        DocumentReference userDocRef;

        if (inTournament)
        {
            userDocRef = db.Collection(ReferencesHolder.FS_Tournament_Collec).Document(tournamentID)
                .Collection(ReferencesHolder.FS_TournamentParticipants_Collec).Document(userId);
        }
        else
        {
            userDocRef = db.Collection(ReferencesHolder.FS_users_Collec).Document(userId).
                Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc);
        }


        userDocRef.GetSnapshotAsync().ContinueWithOnMainThread(snapshot =>
            {
                if (snapshot.IsFaulted || snapshot.IsCanceled)
                {
                    onFailCallback?.Invoke();
                    return;
                }


                if (snapshot.Result.Exists)
                {
                    if (inTournament)
                    {
                        TournamentPassDB pass = snapshot.Result.ConvertTo<TournamentPassDB>();

                        if (pass.tournamentCoins >= anteAmount)
                        {
                            onSuccessCallback?.Invoke(true);
                        }
                        else
                        {
                            onSuccessCallback?.Invoke(false);
                        }

                        return;


                    }
                    else
                    {
                        PublicInfoDB pInfo = snapshot.Result.ConvertTo<PublicInfoDB>();

                        if (pInfo.Coins >= anteAmount)
                        {
                            onSuccessCallback?.Invoke(true);
                        }
                        else
                        {
                            onSuccessCallback?.Invoke(false);
                        }

                        return;
                    }
                }
            });


    }

    public void AddStatsOfTournamentOnFireStoreDatabase(string userID, bool isWon, System.Action CallBack)
    {
        string GameTypeWinKey = "TournamentWin";
        string GameTypeLossKey = "TournamentLoss";

        Dictionary<string, object> statUpdate = new Dictionary<string, object>()
        {
            { isWon?GameTypeWinKey:GameTypeLossKey , FieldValue.Increment(1) }
        };


        db.Collection(ReferencesHolder.FS_users_Collec).Document(userID)
            .Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_Stats_Doc)
            .UpdateAsync(statUpdate).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log(" STATS Updated ");
                    CallBack?.Invoke();
                }
                else
                {
                    Debug.Log(" Issue In Overwriting Stats ");
                }
            });
    }

    public void AddStatsOnFireStoreDataBase(string userID, GameModeType gameMode, bool isWon, System.Action CallBack)
    {
        Debug.Log($"Interacting Firebase -> { userID} Adding stats to  -> {gameMode} | is won = {isWon}");
        // Debug.Log($"Interacting Firebase -> { userID} Adding stats to  ");



        string GameTypeWinKey = "";
        string GameTypeLossKey = "";

        switch (gameMode)
        {
            case GameModeType.ClassicBooRay:
                {
                    GameTypeWinKey = "ClassicBorrayWin";
                    GameTypeLossKey = "ClassicBorrayLoss";
                    break;
                }
            case GameModeType.SpeedBet:
                {
                    GameTypeWinKey = "SpeedBetWin";
                    GameTypeLossKey = "SpeedBetLoss";
                    break;
                }
            case GameModeType.FullHouse:
                {
                    GameTypeWinKey = "FullHouseWin";
                    GameTypeLossKey = "FullHouseLoss";
                    break;
                }

        }

        Dictionary<string, object> statUpdate = new Dictionary<string, object>()
        {
            { isWon?GameTypeWinKey:GameTypeLossKey , FieldValue.Increment(1) }
        };


        db.Collection(ReferencesHolder.FS_users_Collec).Document(userID)
            .Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_Stats_Doc)
            .UpdateAsync(statUpdate).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log(" STATS Updated ");
                    CallBack?.Invoke();
                }
                else
                {
                    Debug.Log(" Issue In Overwriting Stats ");
                }
            });
    }

    /// <summary>
    /// Overwrites the Stat Document
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="statsData"></param>
    /// <param name="CallBack"></param>
    /// 

    //  NOT USING IT
    public void AddStatsOnFireStoreDataBase(string userID, StatsDB statsData, System.Action CallBack)
    {
        Debug.Log($"Interacting Firebase -> { userID} Adding stats to  ");

        db.Collection(ReferencesHolder.FS_users_Collec).Document(userID)
            .Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_Stats_Doc)
            .SetAsync(statsData, SetOptions.Overwrite).ContinueWithOnMainThread(task =>
             {


                 if (task.IsCompleted)
                 {


                     Debug.Log(" STATS Updated ");
                     CallBack?.Invoke();
                 }
                 else
                 {

                     Debug.Log(" Issue In Overwriting Stats ");
                 }

             });


    }

    //  NOT USING IT
  

    public void CheckIfFriendExist(string targetUserId, Action<bool> callBackExist, Action failCallBack)
    {
        Debug.Log($"Interacting Firebase -> {ReferencesHolder.playerPublicInfo.UserId} checking if target is friend -> {targetUserId}");

        db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId)
            .Collection(ReferencesHolder.FS_FriendsData_Collec).Document(targetUserId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    failCallBack?.Invoke();
                }



                if (task.Result.Exists)
                {
                    callBackExist?.Invoke(true);
                }
                else
                {
                    callBackExist?.Invoke(false);
                }


            });


    }


    public void SendFriendRequestToPlayer(string toUserID, System.Action CallBackSent, System.Action CallBackRecieve)
    {
        Debug.Log($"Interacting Firebase -> {ReferencesHolder.playerPublicInfo.UserId} Sending Friend Request -> {toUserID}");

        db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId)
            .Collection(ReferencesHolder.FS_FriendReqSent_Collec).Document(toUserID).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result.Exists)
                {
                    CallBackSent?.Invoke();
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

                            CallBackSent?.Invoke();

                        });

                    db.Collection(ReferencesHolder.FS_users_Collec).Document(toUserID)
                        .Collection(ReferencesHolder.FS_FriendReqRecieve_Collec).Document(fromUserID)
                        .SetAsync(FriendReqSentData).ContinueWithOnMainThread(tast =>
                        {
                            Debug.Log("RequestReceived");

                            CallBackRecieve?.Invoke();
                        });
                }
            });
    }

   


    public (bool isSuccess, PublicInfoDB infoOfUser) GetUsersPublicData(string userID)
    {
        PublicInfoDB infoOfUser = new PublicInfoDB();
        bool isSuccess = false;

        db.Collection(ReferencesHolder.FS_users_Collec).Document(userID).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc)
            .GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    if (task.Result.Exists)
                    {
                        infoOfUser = task.Result.ConvertTo<PublicInfoDB>();
                        Debug.Log($" Data Received of Player ID ={userID} - > {infoOfUser.UserName}   ");
                        isSuccess = true;
                    }
                }
                else
                {
                    Debug.Log($"Task is faulted");
                    isSuccess = false;
                }
            });

        return (isSuccess, infoOfUser);

    }


    public void GetUsersPublicData(string userID, System.Action<bool, PublicInfoDB> CallBack)
    {
        Debug.Log($"Interacting Firebase -> Public User Data -> {userID} ");

        PublicInfoDB infoOfUser = new PublicInfoDB();
        bool isSuccess = false;

        db.Collection(ReferencesHolder.FS_users_Collec).Document(userID).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc)
            .GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    if (task.Result.Exists)
                    {
                        infoOfUser = task.Result.ConvertTo<PublicInfoDB>();
                        Debug.Log($" Data Received of Player ID ={userID} - > {infoOfUser.UserName}   ");
                        isSuccess = true;
                    }
                }
                else
                {
                    Debug.Log($"Task is faulted");
                    isSuccess = false;
                }

                CallBack(isSuccess, infoOfUser);
            });
    }


    public IEnumerator GetPlayerAvatarAPI(string url, System.Action<Sprite> Callback)
    {

        Debug.Log($"url of image -> {url}");

        if (string.IsNullOrEmpty(url))
            yield break;


        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            if (tex != null)
            {
                var spritedTex = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

                Debug.Log("Texture Obtained And Sprited");

                Callback(spritedTex);
            }

        }
        yield return null;
    }


    public void UpdatePlayerCountInRoom(string roomId, int amount, Action onFailCallback, Action onSuccessCallback, bool addUp = true)
    {
        DocumentReference gameRoomDocRef = db.Collection(ReferencesHolder.FS_GameRooms_Collec).Document(roomId);

        db.RunTransactionAsync(trans =>
        {
            return trans.GetSnapshotAsync(gameRoomDocRef).ContinueWith(snap =>
            {


                var roomDBObject = snap.Result.ConvertTo<GameRoomDB>();

                if (addUp)
                {
                    trans.Update(gameRoomDocRef, new Dictionary<string, object>() { { "currentPlayingUsers", FieldValue.Increment(amount) } });
                    return true;
                }
                else
                {
                    trans.Update(gameRoomDocRef, new Dictionary<string, object>() { { "currentPlayingUsers", amount } });
                    return true;
                }



                //if (roomDBObject.currentPlayingUsers<roomDBObject.maxPlayingUsers)
                //{

                //}
                //else
                //{
                //    return false;
                //}



            });
        }).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                onFailCallback?.Invoke();
                return;
            }



            if (task.IsCompleted)
            {
                switch (task.Result)
                {
                    case true:
                        {
                            Debug.Log("UpdatePlayerCountInRoom => Task Successful Return True");
                            onSuccessCallback?.Invoke();
                            break;
                        }
                    case false:
                        {
                            Debug.Log("UpdatePlayerCountInRoom => Task Successful Return False");
                            break;
                        }
                }
            }

        });
    }



    public void CreateRoomDocInFB(string roomId, int maxPlayers, Action onFailedCallback, Action onSuccessCAllback)
    {
        var roomDBInfo = new GameRoomDB
        {
            roomId = roomId,
            maxPlayingUsers = maxPlayers,
            currentPlayingUsers = 1,
            isGameActive = false
        };

        db.Collection(ReferencesHolder.FS_GameRooms_Collec).Document(roomId).SetAsync(roomDBInfo).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                onFailedCallback?.Invoke();
            }

            if (task.IsCompleted)
            {
                onSuccessCAllback?.Invoke();
            }
        });
    }


    public void SyncCurrentPlayerCountInRoom(string roomId, int playerCountFromPhoton, Action onFailCallback, Action<int> onSuccessCallback)
    {
        Debug.Log($"SyncCurrentPlayerCountInRoom => {roomId} | {playerCountFromPhoton}");

        DocumentReference gameRoomDocRef = db.Collection(ReferencesHolder.FS_GameRooms_Collec).Document(roomId);

        db.RunTransactionAsync(trans =>
        {
            return trans.GetSnapshotAsync(gameRoomDocRef).ContinueWith(snap =>
            {
                if (snap.Result.Exists)
                {
                    Debug.Log($"SyncCurrentPlayerCountInRoom => | => Inside Transaction => inside Exist");

                    var roomDBObject = snap.Result.ConvertTo<GameRoomDB>();

                    if (roomDBObject.currentPlayingUsers != playerCountFromPhoton)
                    {
                        trans.Update(gameRoomDocRef, new Dictionary<string, object> { { "currentPlayingUsers", playerCountFromPhoton } });

                        Debug.Log($"SyncCurrentPlayerCountInRoom => | => Inside Transaction => inside Exist => Updating Transaction");
                        return true;
                    }
                    else
                    {
                        return true;
                    }




                }
                else
                {
                    return true;
                }



            });
        }).ContinueWithOnMainThread(transResult =>
        {
            if (transResult.IsCanceled || transResult.IsFaulted)
            {
                onFailCallback?.Invoke();
                return;
            }

            if (transResult.Result)
            {
                onSuccessCallback?.Invoke(playerCountFromPhoton);
            }
            else
            {
                Debug.Log("Equal Count");
            }
        });
    }


    public void UpdateGameActiveSystemInFirebase(string roomId, bool isGameActive, Action onFailCallback, Action onSuccessCallback)
    {
        db.Collection(ReferencesHolder.FS_GameRooms_Collec).Document(roomId)
            .UpdateAsync(new Dictionary<string, object> { { "isGameActive", isGameActive } }).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    onFailCallback?.Invoke();
                    return;
                }
                else
                {
                    onSuccessCallback?.Invoke();
                }
            });
    }

    public void DeleteRoomChat(string roomID)
    {
        //  for deleting game chat messages (collections)
        db.Collection(ReferencesHolder.FS_GameRooms_Collec).Document(roomID).Collection(ReferencesHolder.FS_GameChat_Collec).WhereEqualTo("ChatItem", "chat")
            .GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                WriteBatch batch = db.StartBatch();
                QuerySnapshot snapshot = task.Result;
                foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
                {
                    batch.Delete(documentSnapshot.Reference);
                }
                batch.CommitAsync();
            });


        //  for deleting gameroom document
        db.Collection(ReferencesHolder.FS_GameRooms_Collec).Document(roomID).DeleteAsync();
    }
    #region Opponent Profile work

    public void WatchOpponentProfile(string OpponentUserID, TextMeshProUGUI Coins, TextMeshProUGUI PlayerNameTxt)
    {
     
        db.Collection(ReferencesHolder.FS_users_Collec).Document(OpponentUserID).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_publicInfo_Doc)
           .GetSnapshotAsync().ContinueWithOnMainThread(task =>
           {

               PublicInfoDB Info = task.Result.ConvertTo<PublicInfoDB>();
               Coins.text = Info.Coins.ToString();
               PlayerNameTxt.text = Info.UserName;
               


               if (Info.AvatarUsed == true)
               {
                   var sp = ViewProfileManager.avatarAtlus.GetSprite(Info.AvatarID);
                   ViewProfileManager.FriendImg.sprite = sp;
                   ViewProfileManager.Loader.SetActive(false);
               }
               else
               {
                   StartCoroutine(ViewProfileManager.LoadImage(Info.PictureURL));

               }
          



           });
          db.Collection(ReferencesHolder.FS_users_Collec).Document(OpponentUserID).Collection(ReferencesHolder.FS_userData_Collec).Document(ReferencesHolder.FS_DeckSkins_Doc).GetSnapshotAsync().ContinueWithOnMainThread(task =>
          {
             DeckSkinDB SkinInfo = task.Result.ConvertTo<DeckSkinDB>();
              foreach(string skin in SkinInfo.Skins)
              {
                  switch (skin)
                  {
                      case "000":
                          ViewProfileManager.ProfileDeckskinButtons[0].SetActive(true);
                        //  ViewProfileManager.ProfileDeckSkinImage.sprite = ViewProfileManager.ProfileDeckskinSprites[0];
                          break;
                      case "007":
                          ViewProfileManager.ProfileDeckskinButtons[1].SetActive(true);
                          //  ViewProfileManager.ProfileDeckSkinImage.sprite = ViewProfileManager.ProfileDeckskinSprites[1];
                          break;
                      case "002":
                          ViewProfileManager.ProfileDeckskinButtons[2].SetActive(true);
                          // ViewProfileManager.ProfileDeckSkinImage.sprite = ViewProfileManager.ProfileDeckskinSprites[2];
                          Debug.Log("Black and gold skin!!!!!");
                          break;
                      case "003":
                          ViewProfileManager.ProfileDeckskinButtons[3].SetActive(true);
                          // ViewProfileManager.ProfileDeckSkinImage.sprite = ViewProfileManager.ProfileDeckskinSprites[3];
                          break;
                      case "004":
                          ViewProfileManager.ProfileDeckskinButtons[4].SetActive(true);
                          // ViewProfileManager.ProfileDeckSkinImage.sprite = ViewProfileManager.ProfileDeckskinSprites[4];
                          break;
                      case "005":
                          ViewProfileManager.ProfileDeckskinButtons[5].SetActive(true);
                          // ViewProfileManager.ProfileDeckSkinImage.sprite = ViewProfileManager.ProfileDeckskinSprites[5];
                          break;
                      case "006":
                          ViewProfileManager.ProfileDeckskinButtons[6].SetActive(true);
                          //  ViewProfileManager.ProfileDeckSkinImage.sprite = ViewProfileManager.ProfileDeckskinSprites[6];
                          Debug.Log("Black and gold skin!!!!!");
                          break;

                  }
              }
              //switch (SkinInfo.CurrentSkin)
              //{
              //    case "000":
              //        ViewProfileManager.ProfileDeckSkinImage.sprite = ViewProfileManager.ProfileDeckskinSprites[0];
              //        break;
              //    case "001":
              //        ViewProfileManager.ProfileDeckSkinImage.sprite = ViewProfileManager.ProfileDeckskinSprites[1];
              //        break;
              //    case "002":
              //        ViewProfileManager.ProfileDeckSkinImage.sprite = ViewProfileManager.ProfileDeckskinSprites[2];
              //        Debug.Log("Black and gold skin!!!!!");
              //        break;
              //    case "003":
              //        ViewProfileManager.ProfileDeckSkinImage.sprite = ViewProfileManager.ProfileDeckskinSprites[3];
              //        break;
              //    case "004":
              //        ViewProfileManager.ProfileDeckSkinImage.sprite = ViewProfileManager.ProfileDeckskinSprites[4];
              //        break;
              //    case "005":
              //        ViewProfileManager.ProfileDeckSkinImage.sprite = ViewProfileManager.ProfileDeckskinSprites[5];
              //        break;
              //    case "006":
              //        ViewProfileManager.ProfileDeckSkinImage.sprite = ViewProfileManager.ProfileDeckskinSprites[6];
              //        Debug.Log("Black and gold skin!!!!!");
              //        break;
              //}
          });
       // Debug.Log($"Interacting Firebase -> {ReferencesHolder.playerPublicInfo.UserId} checking if target is friend -> {targetUserId}");

        db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId)
            .Collection(ReferencesHolder.FS_FriendsData_Collec).Document(OpponentUserID).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                  //  failCallBack?.Invoke();
                }



                if (task.Result.Exists)
                {
                    ViewProfileManager.AddFriendButton.gameObject.SetActive(false);
                   // callBackExist?.Invoke(true);
                }
                else
                {
                   
                    if (OpponentUserID == ReferencesHolder.playerPublicInfo.UserId)
                    {
                        Debug.Log("khuddddddddddddddddddddddddddddddddddddddddddd");

                        ViewProfileManager.AddFriendButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        db.Collection(ReferencesHolder.FS_users_Collec).Document(ReferencesHolder.playerPublicInfo.UserId)
                       .Collection(ReferencesHolder.FS_FriendReqSent_Collec).Document(OpponentUserID).GetSnapshotAsync().ContinueWithOnMainThread(task =>
                       {

                            if (task.IsFaulted || task.IsCanceled)
                            {

                            }
                              if (task.Result.Exists)
                              {
                                ViewProfileManager.AddFriendButton.gameObject.SetActive(true);
                                ViewProfileManager.AddFriendButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Already Sent";
                              }
                              else
                              {
                                ViewProfileManager.AddFriendButton.gameObject.SetActive(true);
                                ViewProfileManager.AddFriendButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Add Friend";
                              }


                       });
               
                    }

                    //  callBackExist?.Invoke(false);
                }


            });




    }
    //public IEnumerator LoadImage(string MediaUrl)
    //{
    //    Debug.Log("Load img me gya..");
    //    PlayerUIOptionsController.ProfileImage.gameObject.SetActive(false);
    //    UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Create a request
    //    yield return request.SendWebRequest(); //Wait for the request to complete
    //    if (request.result == UnityWebRequest.Result.ConnectionError)
    //    {
    //        Debug.Log(request.error);
    //    }
    //    else
    //    {
    //        Debug.Log("texture mil gya.......!!!!");

    //        var tex = ((DownloadHandlerTexture)request.downloadHandler).texture;


    //        var spriteImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    //        PlayerUIOptionsController.ProfileImage.gameObject.SetActive(true);
    //        PlayerUIOptionsController.ProfileImage.sprite = spriteImage;




    //    }
    //}
 

    #endregion
}

