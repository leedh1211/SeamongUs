using System.Collections;
using System.Collections.Generic;
using System.Text;
using _02_Scripts.Alert;
using _02_Scripts.Player;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private TMP_InputField pwInputField;

    private string loginUrl = "http://localhost:3000/api/login/onLogin";

    public void OnLoginButtonPressed()
    {
        string userId = idInputField.text;
        string password = pwInputField.text;

        Debug.Log($"로그인 시도: ID={userId}, PW={password}");
        StartCoroutine(SendLoginRequest(userId, password));
    }

    private IEnumerator SendLoginRequest(string userId, string password)
    {
        string HashPass = ConvertHash.StringToHash(password);
        // JSON 형식으로 전송할 데이터 생성
        string jsonData = JsonConvert.SerializeObject(new LoginData(userId, HashPass));

        // 요청 객체 생성
        UnityWebRequest request = new UnityWebRequest(loginUrl, "POST");
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 전송
        yield return request.SendWebRequest();

        // 응답 처리
        LoginResponse response = JsonConvert.DeserializeObject<LoginResponse>(request.downloadHandler.text);
        if (response.isLogin)
        {
            PlayerData loginData = response.data;
            login(loginData);
        }
        else
        {
            AlertUIManager.Instance.OnAlert(response.message);
        }
    }

    private void login(PlayerData loginData)
    {
        Debug.Log(loginData.seq);
        // PlayerManager 생성
        GameObject obj = new GameObject("PlayerManager");
        PlayerManager manager = obj.AddComponent<PlayerManager>();
        manager.Init(loginData);

        // 씬 이동
        SceneManager.LoadScene("LobbyScene");
    }
}