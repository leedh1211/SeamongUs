using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public enum StatType
{
    CurHp,
    MaxHp,
    Stamina,
    MaxStamina
}

public class StatManager : MonoBehaviour
{
    private Dictionary<StatType, ResourceStat> stats = new();
    [SerializeField] private Animator animator;
    private PlayerController controller;

    private int actorNumber;

    private void Awake()
    {
        stats[StatType.CurHp] = new ResourceStat(StatType.CurHp, 100);
        stats[StatType.Stamina] = new ResourceStat(StatType.Stamina, 100);
        Consume(StatType.CurHp, 20f);
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        PhotonView view = GetComponent<PhotonView>();
        if (view != null)
        {
            actorNumber = view.OwnerActorNr;
        }

        if (animator == null)
        {
            Debug.LogWarning("[StatManager] Animator 컴포넌트를 찾을 수 없습니다.");
        }
        if (controller == null)
        {
            Debug.LogWarning("[StatManager] PlayerController 컴포넌트를 찾을 수 없습니다.");
        }
        StartCoroutine(DrainStaminaOverTime());
    }

    private IEnumerator DrainStaminaOverTime()
    {
        while (true)
        {
            Consume(StatType.Stamina, 1f); // 원하는 감소량
            yield return new WaitForSeconds(1f); // 1초 간격
        }
    }

    public float GetValue(StatType type)
    {
        if (stats.TryGetValue(type, out var stat))
            return stat.CurrentValue;
        return 0f;
    }

    public float GetMaxValue(StatType type)
    {
        if (stats.TryGetValue(type, out var stat))
            return stat.MaxValue;
        return 0f;
    }

    public void Recover(StatType type, float amount)
    {
        if (stats.TryGetValue(type, out var stat))
        {
            stat.Recover(amount);
        }
    }

    public void Consume(StatType type, float amount)
    {
        if (stats.TryGetValue(type, out var stat))
        {
            stat.Consume(amount);

           // Debug.Log($"[StatManager] {type} 소모: -{amount} → {stat.CurrentValue}/{stat.MaxValue}");

            if (type == StatType.CurHp && stat.CurrentValue <= 0)
            {
                Debug.Log("[StatManager] 플레이어 사망 처리");
                Die();
            }
        }
    }

    public void SetMaxValue(StatType type, float newMax)
    {
        if (stats.TryGetValue(type, out var stat))
        {
            stat.SetMax(newMax);
        }
    }

    public void SubscribeToStatChange(StatType type, Action<float> callback)
    {
        if (stats.TryGetValue(type, out var stat))
        {
            stat.OnValueChanged += callback;
        }
    }
    private bool isDead = false;

    public void Die()
    {
        if (isDead)
        {
            Debug.Log("[StatManager] 이미 사망 처리된 플레이어입니다.");
            return;
        }

        isDead = true;

        Debug.Log("[StatManager] 플레이어 사망 처리 호출");

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        controller ??= GetComponent<PlayerController>();

        if (controller == null)
        {
            PlayerManager pm = FindObjectOfType<PlayerManager>();
            if (pm != null)
            {
                Debug.Log("사망 플레이어 : " + $"actorNumber: {actorNumber}");
                controller = pm.FindPlayerController(actorNumber);
            }
        }

        if (controller != null)
        {
            controller.Die();
        }
        else
        {
            Debug.LogWarning("[StatManager] PlayerController를 찾을 수 없습니다.");
        }
    }


}
