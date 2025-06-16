using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public event Action OnReportPopupClosed; //이게 게임매니저에 아까 콜백 구독한애들 
    public event Action OnEndGamePopupClosed; //마찬가지
    void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);

    [Header("엔딩팝업")]
    [SerializeField] private GameObject crewWinPopupPrefab;
    [SerializeField] private GameObject imposterWinPopupPrefab;

    [Header("시체 발견 팝업")]
    [SerializeField] private GameObject reportPopupPrefab;

    [Header("팝업을 띄울 부모(Canvas)")]
    [SerializeField] private RectTransform popupParent;

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


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        
        Debug.Log(targetActor);
    }
}
