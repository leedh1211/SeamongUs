using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("엔딩팝업")]
    [SerializeField] private GameObject crewWinPopupPrefab;
    [SerializeField] private GameObject imposterWinPopupPrefab;

    [Header("시체 발견 팝업")]
    [SerializeField] private GameObject reportPopupPrefab;

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

    void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);

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
        byte code = (byte)ev.Code;

        if(code == EventCodes.BodyReported) // OnBodyReportedPopup을 호출 // ChangeState는 콜백으로 처리하고 일단은 안에 함수넣지말기.
        {
            var data = ev.CustomData as object[];
            string reporter = (string)data[0];
            string deadBody = (string)data[1];

            //StartCoroutine(ReportPopupCoroutine)
        }
        else if (code == EventCodes.GameEnded)
        {
          
            byte winner = (byte)ev.CustomData;
            GameObject prefab = (winner == (byte)EndGameCategory.CitizensWin)
                ? crewWinPopupPrefab
                : imposterWinPopupPrefab;

            StartCoroutine(EndGamePopupRoutine(prefab,10f));
        }
    }


    private IEnumerator EndGamePopupRoutine(GameObject prefab, float displaySec)
    {
        var go = Instantiate(prefab, transform);
        var anim = go.GetComponent<Animator>();
        if (anim) anim.SetTrigger("Enter");

        yield return new WaitForSeconds(displaySec);

        if (anim) anim.SetTrigger("Exit");
        yield return new WaitForSeconds(0.5f);

        Destroy(go);
    
        //GameManager.Instance.ChangeState(GameState.Result);
    }

}
