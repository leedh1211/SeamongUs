﻿using Photon.Pun;
using System;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get;
        //{
        //    if (instance == null) instance = new UIManager();
        //    return instance;
        //}

        private set;
    }

    //public GameObject inGameUI;
    //public GameObject missionUI;    //list?
    //public GameObject meetingUI;
    public GameObject votingUI;

    [Header("VoteResult")]
    public GameObject voteResult;
    public TextMeshProUGUI voteText;
    public Image playerImage;
    private string roletext;

    [Header("killed")]
    public GameObject killedPopup;
    public Image imposterImage;
    public TextMeshProUGUI killedText;

    public PlayerManager playerManager;

    public event Action OnReportPopupClosed; //이게 게임매니저에 아까 콜백 구독한애들 
    public event Action OnEndGamePopupClosed; //마찬가지
    new void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    new void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);

    [Header("엔딩팝업")]
    [SerializeField] private GameObject crewWinPopupPrefab;
    [SerializeField] private GameObject imposterWinPopupPrefab;

    [Header("시체 발견 팝업")]
    [SerializeField] private GameObject reportPopupPrefab;

    [Header("팝업을 띄울 부모(Canvas)")]
    [SerializeField] private RectTransform popupParent;
    
    private Action voteResultCallback;
    private Action playerDiedCallback;
    

    bool popupActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        votingUI.SetActive(false);
    }

    public void ShowInGameUI()
    {

    }

    public void ShowMissionUI()
    {

    }

    public void ShowVotingUI()
    {
        votingUI.SetActive(true);
        return;
    }

    public void HideVotingUI()
    {
        votingUI.SetActive(false);
        return;
    }

    public void ShowMeetingUI()
    {

    }

    public void OnEvent(EventData ev) 
    {
        if (popupActive) return;
        byte code = (byte)ev.Code;

        if (code == EventCodes.GameEnded) 
        {
            // ev.CustomData 에서 승자 카테고리를 꺼냅니다
            EndGameCategory winner = (EndGameCategory)(byte)ev.CustomData;

            // 팝업용 prefab 선택
            GameObject prefab = (winner == EndGameCategory.CitizensWin)
                ? crewWinPopupPrefab
                : imposterWinPopupPrefab;

          
            StartCoroutine(DoEndGamePopup(prefab, winner, 3f));
        }
    }
    public void RaiseLocalReportPopup(int deadActorNumber)
    {
        if (popupActive) return;
        StartCoroutine(DoReportPopup(deadActorNumber, 2f));
    }

    IEnumerator DoReportPopup(int deadActorNumber, float duration)
    {
       
        popupActive = true;

        // 1) 인스턴스화
        var go = Instantiate(reportPopupPrefab, popupParent);

        // 2) 컨트롤러 가져오기
        var controller = go.GetComponent<ReportPopupController>();

        // 3) Init + Enter 애니
        string deadName = PhotonNetwork.CurrentRoom
                               .Players[deadActorNumber]
                               .NickName;
       
        controller.Init(deadName);
        controller.PlayEnter();

        // 4) 대기
        yield return new WaitForSeconds(duration);
      
        // 5) Exit 애니
      
        controller.PlayExit();
        yield return new WaitForSeconds(0.5f);

        // 6) 정리
        Destroy(go);
        
        popupActive = false;
        OnReportPopupClosed?.Invoke();       
    }

    IEnumerator DoEndGamePopup(GameObject prefab, EndGameCategory winner, float duration) 
    {
        popupActive = true;
        var go = Instantiate(prefab, popupParent);
        var controller = go.GetComponent<EndingPopupController>();

        // 이제 받은 winner 를 Init 에 넘겨줍니다
        controller.Init(winner);
        controller.PlayEnter();

        yield return new WaitForSeconds(duration);

        controller.PlayExit();
        yield return new WaitForSeconds(0.5f);

        Destroy(go);
        popupActive = false;
        OnEndGamePopupClosed?.Invoke();
    }

    public void ShowVoteResultPopup(int targetActor, Action callback)
    {
        SoundManager.Instance.PlaySFX(SFXType.Eject);
        voteResultCallback = callback; // 저장
        //팝업을 킴
        votingUI.SetActive(false);
        voteResult.SetActive(true);
        
        if (targetActor != -1)
        {
            Player player = PhotonNetwork.CurrentRoom.Players[targetActor];
            playerImage.rectTransform.localEulerAngles = Vector3.zero;

            //임포스터 아닐경우
            player.CustomProperties.TryGetValue(PlayerPropKey.Nick, out object nick);
            player.CustomProperties.TryGetValue(PlayerPropKey.Role, out object role);
            if ((int)role == 1)
            {
                roletext = ($"{(string)nick}은 임포스터가 아닙니다. \n 남은 임포스터는 {CountImposter()}명입니다.");
            }
            else if ((int)role == 2)//임포스터일 경우
            {
                roletext = ($"{(string)nick}은 임포스터가 맞습니다. \n 남은 임포스터는 {CountImposter()-1}명입니다.");
            }else
            {
                roletext = ($"nickname은 임포스터가 맞습니다. \n 남은 임포스터는 n명입니다.");    
            }    
        }
        else
        {
            roletext = ($"누군가의 죽음이 쓸모없어졌습니다. \n 남은 임포스터는 {CountImposter()}명입니다.");
        }
        StartCoroutine(TextEffect(roletext, 0.1f));
    }
    IEnumerator StartRotateImage(RectTransform target, float speed)
    {
        while (true)
        {
            target.Rotate(0f, speed * Time.deltaTime, 0f);
            yield return null;
        }
    }
    public int CountImposter()
    {
        int AliveImposter = 0;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            player.CustomProperties.TryGetValue(PlayerPropKey.IsDead, out object IsDead);
            if (!(bool)IsDead)
            {
                player.CustomProperties.TryGetValue(PlayerPropKey.Role, out object role);
                if ((int)role == 2)
                {
                    AliveImposter += 1;
                }
            }
        }
        return AliveImposter;
    }

    IEnumerator TextEffect(string text, float delay)
    {
        Coroutine rotate = StartCoroutine(StartRotateImage(playerImage.rectTransform, 360f));
        voteText.text = string.Empty;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < text.Length; i++)
        {
            sb.Append(text[i]);
            voteText.text = sb.ToString();
            float currentDelay;
            if (i == text.LastIndexOf("가"))
            {
                currentDelay = 1f;
            }
            else if (i == text.LastIndexOf("가") + 6)
            {
                StopCoroutine(rotate);
                currentDelay = 0.5f;
            }
            else
            {
                currentDelay = delay;
            }
            yield return new WaitForSeconds(currentDelay);
        }
        yield return new WaitForSeconds(2f);
        voteResult.SetActive(false);
        voteResultCallback?.Invoke();
    }

    public void KilledPopup(Action callback)
    {
        playerDiedCallback = callback;
        var go = Instantiate(killedPopup, popupParent);
        StartCoroutine(UpImageScale(go, 3f, 3f, 3, 0.5f, 0.2f ));
    }

    IEnumerator UpImageScale(GameObject go, float size, float duration, int repeat, float delay, float downdelay)
                            //타겟 이미지 위치, 커지는 사이즈, 커지는 정도, 반복 횟수, 커지는 딜레이, 커지는 딜레이 감소정도, 복사되는 텍스트
    {
        var targetArray = go.GetComponentsInChildren<RectTransform>();
        RectTransform target = targetArray.FirstOrDefault(r => r.name == "imposterImage");
        var textArray = go.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI text = textArray.FirstOrDefault(r => r.name == "killed");
        float originDelay = delay;
        go.SetActive(true);
        text.text = ("저벅..");
        for (int i = 0; i < repeat; i++)
        {
            target.localScale = Vector3.Lerp(target.localScale, target.localScale * size, 1/duration);
            text.text += text.text;
            yield return new WaitForSeconds(delay);
            delay -= downdelay;
        }
        yield return new WaitForSeconds(originDelay);
        text.fontSize = 100;
        text.text = ("푹");
        yield return new WaitForSeconds(originDelay*2);
        Destroy(go);
        playerDiedCallback?.Invoke();
    }
}
