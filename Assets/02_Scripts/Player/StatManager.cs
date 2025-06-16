using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.UI;

public enum StatType
{
    CurHp,
    Stamina
}

public class StatManager : MonoBehaviour
{
    private Dictionary<StatType, ResourceStat> stats = new();
    [SerializeField] private Animator animator;
    private void Awake()
    {
        stats[StatType.CurHp] = new ResourceStat(StatType.CurHp, 20);
        stats[StatType.Stamina] = new ResourceStat(StatType.Stamina, 100);
        animator = GetComponent<Animator>();
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
            Debug.Log($"[StatManager] {type} 회복: +{amount} → {stat.CurrentValue}/{stat.MaxValue}");
        }
    }

    public void Consume(StatType type, float amount)
    {
        if (stats.TryGetValue(type, out var stat))
        {
            stat.Consume(amount);
            Debug.Log($"[StatManager] {type} 소모: -{amount} → {stat.CurrentValue}/{stat.MaxValue}");

            if (type == StatType.CurHp && stat.CurrentValue <= 0)
            {
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
    private void Die()
    {
        Debug.Log("[StatManager] 플레이어 사망 처리 호출");

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
    }

}
