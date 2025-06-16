using System.Collections;
using System.Text;
using _02_Scripts.Alert;
using _02_Scripts.Login.Player;
using _02_Scripts.Ung_Managers;
using Newtonsoft.Json;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine.Networking;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerVisual : MonoBehaviourPunCallbacks
{
    public SpriteRenderer bodyRenderer;
    public Animator animator;
    private string setVisualUrl = "http://121.162.172.253:3000/api/users/setVisual";

    private void Start()
    {
        ApplyAvatar(); // 최초 반영
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player target, Hashtable changedProps)
    {
        if (target == photonView.Owner && changedProps.ContainsKey(PlayerPropKey.Spr))
        {
            ApplyAvatar();
        }
            
    }

    private void ApplyAvatar()
    {
        if (photonView.Owner.CustomProperties.TryGetValue(PlayerPropKey.Spr, out object idxObj))
        {
            int idx = (int)idxObj;
            bodyRenderer.sprite = AvatarManager.Instance.GetSprite(idx);
            animator.runtimeAnimatorController = AvatarManager.Instance.GetAnim(idx);
            
            photonView.Owner.CustomProperties.TryGetValue(PlayerPropKey.Id, out var Id);
            StartCoroutine(SendVisualRequest(Id.ToString(), idx));
        }
    }
    
    private IEnumerator SendVisualRequest(string userId, int index)
    {
        // JSON 형식으로 전송할 데이터 생성
        string jsonData = JsonConvert.SerializeObject(new { userId = userId, visual = index });

        // 요청 객체 생성
        UnityWebRequest request = new UnityWebRequest(setVisualUrl, "POST");
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 전송
        yield return request.SendWebRequest();

        // 응답 처리
        JsonConvert.DeserializeObject<LoginResponse>(request.downloadHandler.text);
    }
}