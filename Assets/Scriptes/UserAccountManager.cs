﻿using System.Collections;
using UnityEngine;
using DatabaseControl;

public class UserAccountManager : MonoBehaviour {

    public static UserAccountManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    public static string LoggedIn_Username { get; protected set; }
    private static string LoggedIn_Password;
    public static string LoggedIn_Data { get; protected set; }

    public static bool IsLoggedIn { get; protected set; }


    public void LogOut()
    {
        LoggedIn_Username = "";
        LoggedIn_Password = "";

        IsLoggedIn = false;

        Debug.Log("User Loggeout");
    }

    public void LogIn(string username,string password)
    {
        LoggedIn_Username = username;
        LoggedIn_Password = password;

        IsLoggedIn = true;

        Debug.Log("User Logged in as " + username);
    }

    public void SendData(string data)
    { 
        if (IsLoggedIn)
        {
            StartCoroutine(sendSendDataRequest(LoggedIn_Username, LoggedIn_Password,data));
        }
    }

    IEnumerator sendSendDataRequest(string username, string password, string data)
    {
        IEnumerator eee = DatabaseControl.DCF.SetUserData(username, password, data);
        while (eee.MoveNext())
        {
            yield return eee.Current;
        }

        WWW returneddd = eee.Current as WWW;

        if (returneddd.text == "ContainsUnsupportedSymbol")
        {
            //One of the parameters contained a - symbol
            Debug.Log("Data Upload Error. Could be a server error. To check try again, if problem still occurs, contact us.");
        }

        if (returneddd.text == "Error")
        {
            //Error occurred. For more information of the error, DC.Login could
            //be used with the same username and password
            Debug.Log("Data Upload Error: Contains Unsupported Symbol '-'");
        }
        
    }

    public void GetData()
    { //called when the 'Get Data' button on the data part is pressed
        if (IsLoggedIn == true)
        {
            //ready to send request
            StartCoroutine(sendGetDataRequest(LoggedIn_Username, LoggedIn_Password)); //calls function to send get data request

            
        }
    }

    IEnumerator sendGetDataRequest(string username, string password)
    {
        string data = "ERROR";

        IEnumerator eeee = DatabaseControl.DCF.GetUserData(username, password);
        while (eeee.MoveNext())
        {
            yield return eeee.Current;
        }

        WWW returnedddd = eeee.Current as WWW;

        if (returnedddd.text == "Error")
        {
            //Error occurred. For more information of the error, DC.Login could
            //be used with the same username and password
               
            Debug.Log("Data Upload Error. Could be a server error. To check try again, if problem still occurs, contact us.");
        }
        else
        {
            if (returnedddd.text == "ContainsUnsupportedSymbol")
            {
                //One of the parameters contained a - symbol
                    
                Debug.Log("Get Data Error: Contains Unsupported Symbol '-'");
            }
            else
            {
                //Data received in returned.text variable
                string DataRecieved = returnedddd.text;
                data = DataRecieved;
            }
        }

        LoggedIn_Data = data;
        
    }
}
