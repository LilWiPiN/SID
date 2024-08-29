using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.PackageManager.Requests;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AuthHandler : MonoBehaviour
{
    [SerializeField] string URL = "https://sid-restapi.onrender.com/";
    [SerializeField] string _token;
    [SerializeField] string _username;

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
    public void SignUp()
    {
        JsonData jsonData = new JsonData();

        jsonData.username = GameObject.Find("InputUsername").GetComponent<TMP_InputField>().text;
        jsonData.password = GameObject.Find("InputPassword").GetComponent<TMP_InputField>().text;

        string postData = JsonUtility.ToJson(jsonData);

        StartCoroutine(SignUpPost(postData));
    }

    IEnumerator SignUpPost(string postData)
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
                Debug.Log(authData.usuario.username + " | " + authData.token);

                PlayerPrefs.SetString("token", authData.token);
                PlayerPrefs.SetString("username", authData.usuario.username);

                _token = authData.token;
                _username = authData.usuario.username;

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
        string url = URL + "api/usuarios/" + _username;
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("x-token", _token);

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(request.error);
        else
        {
            if (request.responseCode == 200)
            {
                AuthData data = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);
                User user = data.usuario;
                Debug.Log(request.downloadHandler.text);
                Debug.Log(user.username + " esta en nivel " + user.data.level + " y tiene puntaje " + user.data.score);

                GameObject.Find("Canvas").SetActive(false);
            }
            else
            {
                Debug.Log("El token del usuario " + _username + " ya no es valido");
                GameObject.Find("Canvas").SetActive(true);
            }
        }
    }

    IEnumerator GetUsers()
    {
        string url = URL + "/api/usuarios";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("x-token", _token);

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(request.error);
        else
        {
            if (request.responseCode == 200)
            {
                UserList users = JsonUtility.FromJson<UserList>(request.downloadHandler.text);

                foreach (var user in users.usuarios)
                {

                }

                List<User> leaderboard = users.usuarios.OrderByDescending(u => u.data.score).ToList();
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

    IEnumerator PatchUsers(int score)
    {
        string url = URL + "api/usuarios";
        UnityWebRequest request = UnityWebRequest.Put(url, "body");
        request.method = "PATCH";
        request.SetRequestHeader("x-token", _token);

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(request.error);
        else
        {
            if (request.responseCode == 200)
            {
                //ESCRIBIR
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

    public class JsonData
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class AuthData
    {
        public User usuario;
        public string token;
    }

    public class UserList
    {
        public User[] usuarios;
    }

    [System.Serializable]
    public class User
    {
        public string _id;
        public string username;
        public bool state;
        public UserData data;
    }

    [System.Serializable]
    public class UserData
    {
        public int score;
        public int level;
        public int exp;
        public string color;
    }
}