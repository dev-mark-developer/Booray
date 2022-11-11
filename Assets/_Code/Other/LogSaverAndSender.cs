using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Text;

public class LogSaverAndSender : MonoBehaviour
{
    public bool enableSave = true;
    public bool enableMailing = true;

    //booraydevteam@gmail.com
    //loanoyxtjnaecdho
    //public string yourEmail = "fromemail@gmail.com";
    public string yourEmail = "booraydevteam@gmail.com";
    public string yourEmailPassword = "loanoyxtjnaecdho";
    public string toEmail = "unity.developer21@gmail.com";


    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    [Serializable]
    public struct Logs
    {
        public string condition;
        public string stackTrace;
        public LogType type;

        public string dateTime;

        public Logs(string condition, string stackTrace, LogType type, string dateTime)
        {
            this.condition = condition;
            this.stackTrace = stackTrace;
            this.type = type;
            this.dateTime = dateTime;
        }
    }

    [Serializable]
    public class LogInfo
    {
        public List<Logs> logInfoList = new List<Logs>();
    }

    LogInfo logs = new LogInfo();

    void OnEnable()
    {


        Debug.Log("- LogSaverAndSender - On Enable");
        //Email last saved log
        if (enableMailing)
        {
            mailLog();
        }

        //Subscribe to Log Event
        Application.logMessageReceived += LogCallback;
    }

    //Called when there is an exception
    void LogCallback(string condition, string stackTrace, LogType type)
    {

       // Debug.Log(" LogCallback Started---");
        //Create new Log
        Logs logInfo = new Logs(condition, stackTrace, type, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));

        //Add it to the List
        logs.logInfoList.Add(logInfo);
    }

    void mailLog()
    {


         
         Debug.Log("mailLog ");

        //Read old/last saved log

        string lastLog = PlayerPrefs.GetString(ReferencesHolder.lastLog_PrefKey, "savelog");
        Debug.Log(lastLog);
        LogInfo loadedData = DataSaver.loadData<LogInfo>(lastLog);
        string date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");

        //Send only if there is something to actually send
        if (loadedData != null && loadedData.logInfoList != null

            && loadedData.logInfoList.Count > 0)
        {

            Debug.Log("Found log to send!");

            //Convert to json
            string messageToSend = JsonUtility.ToJson(loadedData, true);

            //string attachmentPath = Path.Combine(Application.persistentDataPath, "data");
            string attachmentPath = Application.persistentDataPath + "/data";
            attachmentPath = Path.Combine(attachmentPath, $"{lastLog}.txt");

            //Finally send email
            sendMail(yourEmail, yourEmailPassword, toEmail, lastLog + date, messageToSend, attachmentPath);

            //Clear old log
            //DataSaver.deleteData("savelog");
        }
    }

    void sendMail(string fromEmail, string emaiPassword, string toEmail, string eMailSubject, string eMailBody, string attachmentPath = null)
    {

       // Debug.Log(" sendMail");

        try
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(fromEmail);
            mail.To.Add(toEmail);
            mail.Subject = eMailSubject;
            mail.Body = eMailBody;

            if (attachmentPath != null)
            {
                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(attachmentPath);
                mail.Attachments.Add(attachment);
            }

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Credentials = new System.Net.NetworkCredential(fromEmail, emaiPassword) as ICredentialsByHost;
            smtpClient.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };
            smtpClient.Send(mail);
        }
        catch  { Debug.Log("Send Log Mail Failed..."); }
    }

    void OnDisable()
    {
       // DataSaver.saveData(logs, "savelog");

        //Un-Subscribe from Log Event
        Application.logMessageReceived -= LogCallback;
    }

    //Save log  when focous is lost
    void OnApplicationFocus(bool hasFocus)
    {

        if (!hasFocus)
        {
            //Save
            if (enableSave)
            {

               // Debug.Log("OnApplicationFocus( ");

                DataSaver.saveData(logs, $"savelog_{SystemInfo.deviceModel}_{SystemInfo.deviceName}_{DateTime.Now.ToString()}");
            }
        }
    }

    //Save log on exit
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            //Save
            if (enableSave)
            {
               // Debug.Log("OnApplicationFocus( ");
                DataSaver.saveData(logs, $"savelog_{SystemInfo.deviceModel}_{SystemInfo.deviceName}_{DateTime.Now.ToString()}");
            }
        }
    }
}
