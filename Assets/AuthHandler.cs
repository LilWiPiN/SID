using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AuthHandler : MonoBehaviour
{
    [SerializeField] string URL = "https://sid-restapi.onrender.com/";
    string _token;
    string _username;

    void Start()
    {
        _token = PlayerPrefs.GetString("token");
        _username = PlayerPrefs.GetString("username");

        if (string.IsNullOrEmpty(_token) || string.IsNullOrEmpty(_username))
        {
            Debug.Log("No hay token");
        }
        else
        {
            StartCoroutine(GetProfile());
        }
    }

    public void LogIn()
    {
        JsonData jsonData = new JsonData();

        jsonData.username = GameObject.Find("InputUsername").GetComponent<TMP_InputField>().text;
        jsonData.password = GameObject.Find("InputPassword").GetComponent<TMP_InputField>().text;

        string postData = JsonUtility.ToJson(jsonData);

        StartCoroutine(LogInPost(postData));
    }
    public void SignIn()
    {
        JsonData jsonData = new JsonData();

        jsonData.username = GameObject.Find("InputUsername").GetComponent<TMP_InputField>().text;
        jsonData.password = GameObject.Find("InputPassword").GetComponent<TMP_InputField>().text;

        string postData = JsonUtility.ToJson(jsonData);

        StartCoroutine(SignInPost(postData));
    }

    IEnumerator SignInPost(string postData)
    {
        UnityWebRequest request = UnityWebRequest.Post(URL + "api/usuarios/", postData, "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(request.error);
        else
        {
            if (request.responseCode == 200)
            {
                Debug.Log(request.downloadHandler.text);
                StartCoroutine(LogInPost(postData));
            }
            else
            {
                string msj = "status: " + request.responseCode;
                msj += "\nError: " + request.error;
                msj += "\n" + request.downloadHandler.text;
                Debug.Log(msj);
            }
        }
    }

    IEnumerator LogInPost(string postData)
    {
        UnityWebRequest request = UnityWebRequest.Post(URL + "api/auth/login", postData, "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(request.error);
        else
        {
            if (request.responseCode == 200)
            {
                AuthData authData = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);
                Debug.Log(authData.userData.username + " | " + authData.token);

                PlayerPrefs.SetString("token", authData.token);
                PlayerPrefs.SetString("username", authData.userData.username);

                _token = authData.token;
                _username = authData.userData.username;

                GameObject.Find("Canvas").SetActive(false);
            }
            else
            {
                string msj = "status: " + request.responseCode;
                msj += "\nError: " + request.error;
                msj += "\n" + request.downloadHandler.text;
                Debug.Log(msj);
            }
        }
    }

    IEnumerator GetProfile()
    {
        string url = URL + "/api/usuarios/" + _username;
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("x-token", _token);

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(request.error);
        else
        {
            if (request.responseCode == 200)
            {
                GameObject.Find("Canvas").SetActive(false);
            }
            else
            {
                GameObject.Find("Canvas").SetActive(true);
            }
        }
    }
}

public class JsonData
{
    public string username;
    public string password;
}

[System.Serializable]
public class AuthData
{
    public UserData userData;
    public string token;
}

[System.Serializable]
public class UserData
{
    public string _id;
    public string username;
    public bool state;
}