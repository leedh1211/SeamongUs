
using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private Button killBtn;
    [SerializeField] private Button useBtn;
    [SerializeField] private Button reportBtn;
    [SerializeField] private Button inventoryBtn;
    [SerializeField] private PlayerManager playerManager;
    private PlayerController player;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private StatManager statManager;
    public static PlayerUIManager Instance { get; private set; }

    private Dictionary<int, UIInventory> playerInventories = new();

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        hpSlider.maxValue = statManager.GetMaxValue(StatType.CurHp);
        staminaSlider.maxValue = statManager.GetMaxValue(StatType.Stamina);

        statManager.SubscribeToStatChange(StatType.CurHp, val => hpSlider.value = val);
        statManager.SubscribeToStatChange(StatType.Stamina, val => staminaSlider.value = val);
    }
    public void Init()
    {
        int role = 0;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerPropKey.Role, out object roleObj))
        {
            role = System.Convert.ToInt32(roleObj);
        }

        if (role == 1)
        {
            killBtn.gameObject.SetActive(false);
            useBtn.gameObject.SetActive(true);
        }else if (role == 2)
        {
            killBtn.gameObject.SetActive(true);
            useBtn.gameObject.SetActive(false);
        }
        
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        player = playerManager.FindPlayerController(actorNumber);
        killBtn.onClick.RemoveAllListeners(); // 기존 리스너 제거 (선택)
        killBtn.onClick.AddListener(() => player.TryKill());

        useBtn.onClick.RemoveAllListeners();
        useBtn.onClick.AddListener(() => player.OnInteractAction());

        reportBtn.onClick.RemoveAllListeners();
        reportBtn.onClick.AddListener(() => player.OnReportAction());

        // inventoryBtn.onClick.RemoveAllListeners();
        // inventoryBtn.onClick.AddListener(() => player.OnInventoryInputWrapper());


    }

    public void Initialize(StatManager statManager)
    {
        this.statManager = statManager;

        hpSlider.maxValue = statManager.GetMaxValue(StatType.CurHp);
        hpSlider.value = statManager.GetValue(StatType.CurHp);

        statManager.SubscribeToStatChange(StatType.CurHp, val =>
        {
            hpSlider.value = val;
            Debug.Log($"[UIStatDisplay] 체력 UI 갱신: {val}");
        });
    }
}