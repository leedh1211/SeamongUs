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

    public event Action OnReportPopupClosed;
    public event Action OnEndGamePopupClosed;

    [Header("엔딩팝업")]
    [SerializeField] private GameObject crewWinPopupPrefab;
    [SerializeField] private GameObject imposterWinPopupPrefab;

    [Header("시체 발견 팝업")]
    [SerializeField] private GameObject reportPopupPrefab;

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
            byte winner = (byte)ev.CustomData;
            GameObject prefab = (winner == (byte)EndGameCategory.CitizensWin)
                ? crewWinPopupPrefab
                : imposterWinPopupPrefab;
            StartCoroutine(DoEndGamePopup(prefab, 3f));
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
        var go = Instantiate(reportPopupPrefab, transform);
        var txt = go.GetComponentInChildren<TMP_Text>();
        string deadName = PhotonNetwork.CurrentRoom.Players[deadActorNumber].NickName;
        txt.text = $"{deadName}님의 시체가 발견되었습니다.";

        var anim = go.GetComponent<Animator>();
        if (anim) anim.SetTrigger("Enter");

        yield return new WaitForSeconds(duration);

        if (anim) anim.SetTrigger("Exit");
        yield return new WaitForSeconds(0.5f);

        Destroy(go);
        popupActive = false;
        OnReportPopupClosed?.Invoke();
    }

    IEnumerator DoEndGamePopup(GameObject prefab, float duration)
    {
        popupActive = true;
        var go = Instantiate(prefab, transform);
        var anim = go.GetComponent<Animator>();
        if (anim) anim.SetTrigger("Enter");

        yield return new WaitForSeconds(duration);

        if (anim) anim.SetTrigger("Exit");
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
