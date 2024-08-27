using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class HttpTest : MonoBehaviour
{
    [SerializeField] string APIurl = "https://rickandmortyapi.com/api/character";
    [SerializeField] string DBurl = "https://my-json-server.typicode.com/LilWiPiN/SID";
    [SerializeField] TextMeshProUGUI textName;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button plus;
    [SerializeField] Button minus;
    string idInput;
    int idButton;
    int rnd;

    int index;
    string tagName, tagImage;

    void Start()
    {
        idButton = 0;
        rnd = Random.Range(1, 6);

        index = 0;
    }

    public void SendRequest()
    {
        idInput = inputField.text;

        if (string.IsNullOrEmpty(idInput))
        {
            idButton = rnd;
            StartCoroutine("GetUser", idButton);
            inputField.text = idButton.ToString();
        }
        else
        {
            if (int.TryParse(idInput, out idButton))
                StartCoroutine("GetUser", idButton);
            else
                Debug.Log("error");
        }  
    }

    public void PlusId()
    {
        idButton++;

        if (idButton > 5)
            idButton = 1;

        inputField.text = idButton.ToString();
        StartCoroutine("GetUser", idButton);
    }

    public void MinusId()
    {
        idButton--;

        if (idButton < 1)
            idButton = 5;

        inputField.text = idButton.ToString();
        StartCoroutine("GetUser", idButton);
    }

    IEnumerator GetUser(int id)
    {
        UnityWebRequest request = UnityWebRequest.Get(DBurl + "/users/" + id);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(request.error);
        else
        {
            if (request.responseCode == 200)
            {
                User user = JsonUtility.FromJson<User>(request.downloadHandler.text);
                GameObject.Find("Username").GetComponent<TextMeshProUGUI>().text = "Username: " + user.username;

                Debug.Log(user.username);
                foreach (var card in user.deck)
                {
                    Debug.Log(card);
                    tagName = "Name" + index;
                    tagImage = "RawImage" + index;
                    Debug.Log(tagImage + "/" + tagName);
                    StartCoroutine(GetCharacter(card, tagName, tagImage));
                    index++;

                    if(index > 4)
                        index = 0;
                }
            }

            else
            {
                string msj = "status: " + request.responseCode;
                msj += "\nError: " + request.error;
                Debug.Log(msj);
            }
        }
    }

    IEnumerator GetCharacter(int id, string tagName, string tagImage)
    {
        UnityWebRequest request = UnityWebRequest.Get(APIurl + "/" + id);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if(request.responseCode == 200)
            {
                Character character = JsonUtility.FromJson<Character>(request.downloadHandler.text);
                
                StartCoroutine(GetImage(character.image, tagImage));
                StartCoroutine(GetName(character.name, tagName));
            }
            else
            {
                string msj = "status: " + request.responseCode;
                msj += "\nError: " + request.error;
                Debug.Log(msj);
            }
        }
    }

    IEnumerator GetName(string name, string nameTag)
    {
        UnityWebRequest request = UnityWebRequest.Get(APIurl + "/" + name);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(request.error);
        else
            GameObject.Find(nameTag).GetComponent<TextMeshProUGUI>().text = name;
    }

    IEnumerator GetImage(string image, string imageTag)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(image);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(request.error);
        else
        {
            var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            GameObject.Find(imageTag).GetComponent<RawImage>().texture = texture;
        }
    }
}

class Character
{
    public int id;
    public string name;
    public string species;
    public string image;
}

class User
{
    public int id;
    public string username;
    public bool state;
    public int[] deck;
}