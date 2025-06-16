using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class ReportManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static ReportManager Instance { get; private set; }
    public int LastReporter { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    public void ReportBody(int deadPlayerActNum)
    {
        object[] eventData = new object[] { deadPlayerActNum };
        PhotonNetwork.RaiseEvent(
            EventCodes.PlayerReport,
            eventData,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable
        );
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == EventCodes.PlayerReport)
        {
            // 신고 요청을 보낸 ActorNum을 그대로 저장
            LastReporter = photonEvent.Sender;
            VoteUI.Instance?.PopulateSlots();
        }
    }
}
