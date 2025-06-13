using System;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public enum StatType
{
    CurHp,
    Stamina
}

public class StatManager : MonoBehaviour
{
    private Dictionary<StatType, ResourceStat> stats = new();

    private void Awake()
    {
        // �⺻�� �ʱ�ȭ (�׽�Ʈ��)
        stats[StatType.CurHp] = new ResourceStat(StatType.CurHp, 100);
        stats[StatType.Stamina] = new ResourceStat(StatType.Stamina, 100);
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
            Debug.Log($"[StatManager] {type} ȸ��: +{amount} �� {stat.CurrentValue}/{stat.MaxValue}");
        }
    }

    public void Consume(StatType type, float amount)
    {
        if (stats.TryGetValue(type, out var stat))
        {
            stat.Consume(amount);
            Debug.Log($"[StatManager] {type} �Ҹ�: -{amount} �� {stat.CurrentValue}/{stat.MaxValue}");
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
        Debug.Log("[StatManager] �÷��̾� ��� ó�� ȣ��");
        // TODO: �׾��� �� �ؾ� �� �ൿ�� ���� (�ִϸ��̼�, UI, ��Ȱ��ȭ ��)
    }
}
