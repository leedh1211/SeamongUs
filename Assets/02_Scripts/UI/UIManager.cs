using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
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

    public void ShowVoteResultPopup(int targetActor, Action callback)
    {
        Debug.Log(targetActor);
    }
}
