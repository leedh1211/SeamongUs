using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null) instance = new UIManager();
            return instance;
        }
    }

    public GameObject inGameUI;
    public GameObject missionUI;    //list?
    public GameObject meetingUI;
    public GameObject votingUI;

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

    }

    public void ShowMeetingUI()
    {

    }
}
