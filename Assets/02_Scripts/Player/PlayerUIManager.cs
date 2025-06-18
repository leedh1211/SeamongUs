
using System.Collections;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private Button killBtn;
    [SerializeField] private Button useBtn;
    [SerializeField] private Button reportBtn;
    [SerializeField] private Button inventoryBtn;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private TMP_Text killCollDownText;
    [SerializeField] private Image KillCoolDownMask;
    private PlayerController player;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private StatManager statManager;
    [SerializeField] private Button useButton;
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

    public IEnumerator SetKillButtonCooldown(float MaxCoolDown)
    {
        killCollDownText.gameObject.SetActive(true);
        float timer = MaxCoolDown;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            float fill = Mathf.Clamp01((MaxCoolDown - timer) / MaxCoolDown);
            KillCoolDownMask.fillAmount = fill;
            killCollDownText.text = timer.ToString("0.0");
            yield return null;
        }
        KillCoolDownMask.fillAmount = 0f;
        killCollDownText.text = "";
        killCollDownText.gameObject.SetActive(false);
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
        }
        else if (role == 2)
        {
            killBtn.gameObject.SetActive(true);
            useBtn.gameObject.SetActive(false);
        }

        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        player = playerManager.FindPlayerController(actorNumber);
        killBtn.onClick.RemoveAllListeners(); // 기존 리스너 제거 (선택)
        killBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySFX(SFXType.Kill);
            player.TryKill();
        });

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
    public void SetUseButtonInteractable(bool interactable)
    {
        if (useButton != null)
            useButton.interactable = interactable;
    }
}