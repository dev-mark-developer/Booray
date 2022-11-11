using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class LogErrorUIHandler : MonoBehaviour
{
    [SerializeField] GameObject errorPanel;
    [SerializeField] TextMeshProUGUI errorMsgTxt;

    [SerializeField] Button cancleBtn;


    public static LogErrorUIHandler instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }


    }

    private void Start()
    {
        cancleBtn.onClick.AddListener(delegate { errorPanel.SetActive(false); });
   

    }
   

    public void OpenErrorPanel(string msg)
    {

        if(errorMsgTxt ==null || errorPanel == null)
        {
            return;
        }
        errorMsgTxt.text = msg;
        errorPanel.SetActive(true);

    }
    //public void CheckNetFunction()
    //{
      
    //    else
    //    {

    //    }
  // }
    // Action<bool> action

    

    public void CheckForInternet()
    {

        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            OpenErrorPanel("Please Check Your Internet Connection");
            Debug.Log("InternetStatus");
            ReferencesHolder.InternetStatus = false;
        }
        else
        {
            Debug.Log("InternetStatus");
            ReferencesHolder.InternetStatus = true;
        }

        

        #region Comment
        //ReferencesHolder.InternetStatus = false;
        //Debug.Log("Check internet function k ander heinnnnnnnnnnnnnnnnnnnnnn");
        //if (Application.internetReachability != NetworkReachability.NotReachable)
        //{
        //    Debug.Log("blah");
        //    ReferencesHolder.InternetStatus = true;
        //}
        //else
        //{
        //    WWW www = new WWW("http://google.com");

        //    yield return www;
        //    if (www.error != null)
        //    {
        //        Debug.Log("Not Connected to internet");
        //        OpenErrorPanel("Internet not Connected");
        //        ReferencesHolder.InternetStatus = false;
        //    }
        //    else
        //    {
        //        Debug.Log("  Connected to internet");
        //        ReferencesHolder.InternetStatus = true;

        //    }
        //}

        //if (Application.internetReachability == NetworkReachability.NotReachable)
        //{
        //    Debug.Log("not rechaaaaaaaa");
        //    OpenErrorPanel("Internet not reachable");
        //    ReferencesHolder.InternetStatus = false;
        //}
        //else
        //{
        //    WWW www = new WWW("http://google.com");

        //    yield return www;
        //    if (www.error != null)
        //    {
        //        Debug.Log("Not Connected to internet");
        //        OpenErrorPanel("Internet not Connected");
        //        ReferencesHolder.InternetStatus = false;
        //    }
        //    else
        //    {
        //        Debug.Log("  Connected to internet");
        //        ReferencesHolder.InternetStatus = true;

        //    }
        //}
        #endregion



    }



}
