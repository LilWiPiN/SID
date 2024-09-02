using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.PackageManager.Requests;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class AuthHandler : MonoBehaviour
{
    [SerializeField] string URL = "https://sid-restapi.onrender.com/";
    [SerializeField] string _token;
    [SerializeField] string _username;

    public GameObject mainMenu;
    public GameObject menuScreen;
    public GameObject signupScreen;
    public GameObject loginScreen;
    public GameObject ProfileScreen;
    public GameObject LeaderboardScreen;

    public GameObject backButton;

    public GameObject inputErrors;
    public GameObject inputError1;
    public GameObject inputError2;
    public GameObject inputError3;

    public GameObject inputError4;

    public GameObject inputCorrect1;
    public GameObject inputCorrect2;

    public GameObject invalidToken;
    public GameObject username;
    public RawImage imgProfile;
    public Texture[] rndTexture;

    public GameObject inputScore;
    public GameObject inputColor;
    public GameObject inputLevel;
    public GameObject inputExp;

    public GameObject inputUpdate;

    public GameObject leaderboardUsername;
    public GameObject leaderboardScore;
    public GameObject leaderboardLevel;
    public GameObject leaderboardExp;
    string tagUsername, tagScore, tagLevel, tagExp;
    int index;

    void Start()
    {
        index = 0;

        SetOff();

        _token = PlayerPrefs.GetString("token");
        _username = PlayerPrefs.GetString("username");

        if (string.IsNullOrEmpty(_token) || string.IsNullOrEmpty(_username))
            Debug.Log("No hay token");
        else
            StartCoroutine(GetProfile());
    }

    public void SetOff()
    {
        mainMenu = GameObject.Find("Main");
        menuScreen = GameObject.Find("Menu");
        signupScreen = GameObject.Find("SignUpScreen");
        loginScreen = GameObject.Find("LogInScreen");
        ProfileScreen = GameObject.Find("Profile");
        LeaderboardScreen = GameObject.Find("Leaderboard");

        backButton = GameObject.Find("Back");

        inputErrors = GameObject.Find("Errors");
        inputError1 = GameObject.Find("ExistingUser");
        inputError2 = GameObject.Find("EnterPassword");
        inputError3 = GameObject.Find("EnterUser");

        inputError4 = GameObject.Find("NoExistingUser");

        inputCorrect1 = GameObject.Find("AutheticatedUser"); 
        inputCorrect2 = GameObject.Find("CreatedUser");

        username = GameObject.Find ("User");
        invalidToken = GameObject.Find("InvalidToken");

        inputScore = GameObject.Find("InputScore");
        inputColor = GameObject.Find("InputColor");
        inputLevel = GameObject.Find("InputLevel");
        inputExp = GameObject.Find("InputExp");

        inputUpdate = GameObject.Find("UpdatedInfo");

        leaderboardUsername = GameObject.Find($"UserText{index}");
        leaderboardScore = GameObject.Find($"UserScore{index}");
        leaderboardLevel = GameObject.Find($"UserLevel{index}");
        leaderboardExp = GameObject.Find($"UserExp{index}");

        menuScreen.SetActive(false);
        ProfileScreen.SetActive(false);
        LeaderboardScreen.SetActive(false);
    }

    public void BackMain()
    {
        mainMenu.SetActive(true);
        menuScreen.SetActive(false);
        ProfileScreen.SetActive(false);
        LeaderboardScreen.SetActive(false);
    }

    public void ExitProfile()
    {
        mainMenu.SetActive(true);
        ProfileScreen.SetActive(false);
        LeaderboardScreen.SetActive(false);

        TextMeshProUGUI tmpText = invalidToken.GetComponent<TextMeshProUGUI>();
        tmpText.text = "La sesion de " + _username + " ha sido suspendida";
        invalidToken.SetActive(true);
    }

    public void Res()
    {
        inputError1.SetActive(false);
        inputError2.SetActive(false);
        inputError3.SetActive(false);
        inputError4.SetActive(false);

        inputCorrect1.SetActive(false);
        inputCorrect2.SetActive(false);

        invalidToken.SetActive(false);
        username.SetActive(false);

        inputUpdate.SetActive(false);
    }

    public void SignUpScreen() 
    {
        mainMenu.SetActive(false);
        menuScreen.SetActive(true);
        signupScreen.SetActive(true);
        loginScreen.SetActive(false);

        Res();
    }
    
    public void LogInScreen()
    {
        mainMenu.SetActive(false);
        menuScreen.SetActive(true);
        loginScreen.SetActive(true);
        signupScreen.SetActive(false);

        Res();
    }

    public void SignUp()
    {
        JsonData jsonData = new JsonData();

        jsonData.username = GameObject.Find("InputUsername").GetComponent<TMP_InputField>().text;
        jsonData.password = GameObject.Find("InputPassword").GetComponent<TMP_InputField>().text;

        string postData = JsonUtility.ToJson(jsonData);

        StartCoroutine(SignUpPost(postData));
    }

    public void LogIn()
    {
        JsonData jsonData = new JsonData();

        jsonData.username = GameObject.Find("InputUsername").GetComponent<TMP_InputField>().text;
        jsonData.password = GameObject.Find("InputPassword").GetComponent<TMP_InputField>().text;

        string postData = JsonUtility.ToJson(jsonData);

        StartCoroutine(LogInPost(postData));
    }

    public void UpdateData()
    {
        UserData userData = new UserData();

        TMP_Dropdown dropdown = GameObject.Find("InputColor").GetComponent<TMP_Dropdown>();

        userData.score = int.Parse(GameObject.Find("InputScore").GetComponent<TMP_InputField>().text);
        userData.color = dropdown.options[dropdown.value].text;
        userData.level = (int)GameObject.Find("InputLevel").GetComponent<Slider>().value;
        userData.exp = (int)GameObject.Find("InputExp").GetComponent<Slider>().value;
        
        string postData = JsonUtility.ToJson(userData);

        StartCoroutine(PatchUsers(postData));
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
                inputCorrect1.SetActive(false);
                inputCorrect2.SetActive(true);
                StartCoroutine(LogInPost(postData));
            }
            else
            {
                inputErrors.SetActive(true);
                inputCorrect2.SetActive(false);
                string msj = "status: " + request.responseCode;
                msj += "\nError: " + request.error;
                msj += "\n" + request.downloadHandler.text;
                Debug.Log(msj);

                switch (request.downloadHandler.text)
                {
                    case "{\"msg\":\"Debe enviar el usuario\"}":
                        inputError1.SetActive(false);
                        inputError2.SetActive(false);
                        inputError3.SetActive(true);
                        break;
                    case "{\"msg\":\"Debe enviar el password\"}":
                        inputError1.SetActive(false);
                        inputError2.SetActive(true);
                        inputError3.SetActive(false);
                        break;
                    case "{\"msg\":\"Ya existe usuario con ese username\"}":
                        inputError1.SetActive(true);
                        inputError2.SetActive(false);
                        inputError3.SetActive(false);
                        break;
                }
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

                inputCorrect2.SetActive(false);
                inputCorrect1.SetActive(true);
                inputErrors.SetActive(false);

                StartCoroutine(GetProfile());
            }
            else
            {
                inputErrors.SetActive(true);
                inputCorrect1.SetActive(false);
                string msj = "status: " + request.responseCode;
                msj += "\nError: " + request.error;
                msj += "\n" + request.downloadHandler.text;
                Debug.Log(msj);

                switch (request.downloadHandler.text)
                {
                    case "{\"msg\":\"debe enviar el campo username en la petición\",\"field\":\"username\"}":
                        inputError2.SetActive(false);
                        inputError3.SetActive(true);
                        inputError4.SetActive(false);
                        break;
                    case "{\"msg\":\"debe enviar el campo password en la petición\",\"field\":\"password\"}":
                        inputError2.SetActive(true);
                        inputError3.SetActive(false);
                        inputError4.SetActive(false);
                        break;
                    case "{\"msg\":\"Usuario o contraseña no son correctos - password\"}":
                        inputError2.SetActive(false);
                        inputError3.SetActive(false);
                        inputError4.SetActive(true);
                        break;
                    case "{\"msg\":\"Usuario o contraseña no son correctos - correo\"}":
                        inputError2.SetActive(false);
                        inputError3.SetActive(false);
                        inputError4.SetActive(true);
                        break;
                }
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

                ProfileScreen.SetActive(true);
                username.SetActive(true);

                mainMenu.SetActive(false);
                menuScreen.SetActive(false);
                loginScreen.SetActive(false);

                TextMeshProUGUI tmpText = username.GetComponent<TextMeshProUGUI>();
                tmpText.text = _username;
                
                int rnd = Random.Range(0, 10);
                imgProfile.texture = rndTexture[rnd];
            }
            else
            {
                Debug.Log("El token del usuario " + _username + " ya no es valido - 401");
                TextMeshProUGUI tmpText = invalidToken.GetComponent<TextMeshProUGUI>();
                tmpText.text = "La sesion de " + _username + " ha sido suspendida";

                mainMenu.SetActive(true);
                invalidToken.SetActive(true);
            }
        }
    }

    IEnumerator GetUsers()
    {
        string url = URL + "api/usuarios";
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
                //User userK = data.usuario;
                //Debug.Log(userK.username + " esta en nivel " + userK.data.level + " y tiene puntaje " + userK.data.score);

                Debug.Log(request.downloadHandler.text);

                TextMeshProUGUI usernameText = leaderboardUsername.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI scoreText = leaderboardScore.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI levelText = leaderboardLevel.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI expText = leaderboardExp.GetComponent<TextMeshProUGUI>();

                int rnd = Random.Range(0, 10);

                LeaderboardScreen.SetActive(true);
                ProfileScreen.SetActive(false);

                UserList users = JsonUtility.FromJson<UserList>(request.downloadHandler.text);

                foreach (var user in users.usuarios)
                {
                    imgProfile.texture = rndTexture[rnd];

                    tagUsername = usernameText.text = $"<color={user.data.color.ToLower()}>{user.username}</color>";
                    tagScore = scoreText.text = user.data.score.ToString();
                    tagLevel = levelText.text = user.data.level.ToString();
                    tagExp = expText.text = user.data.exp.ToString();

                    index++;
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

    IEnumerator PatchUsers(string postData)
    {
        string url = URL + "api/usuarios";
        string body = "{\"username\":\"" + _username + "\",\"data\":" + postData + "}";
        
        UnityWebRequest request = UnityWebRequest.Put(url, body);
        request.method = "PATCH";
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-token", _token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(request.error);
        else
        {
            if (request.responseCode == 200)
            {
                Debug.Log(request.downloadHandler.text);

                inputUpdate.SetActive(true);

                StartCoroutine(GetUsers());
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
        public string color;
        public int level;
        public int exp;
    }
}