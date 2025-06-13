using System;
using UnityEngine;

public class ResourceStat
{
    public StatType Type { get; private set; }
    public float CurrentValue { get; private set; }
    public float MaxValue { get; private set; }

    public Action<float> OnValueChanged;

    public ResourceStat(StatType type, float maxValue)
    {
        Type = type;
        MaxValue = maxValue;
        CurrentValue = maxValue;
    }

    public void Recover(float amount)
    {
        CurrentValue = Mathf.Min(CurrentValue + amount, MaxValue);
        OnValueChanged?.Invoke(CurrentValue);
    }

    public void Consume(float amount)
    {
        CurrentValue = Mathf.Max(CurrentValue - amount, 0);
        OnValueChanged?.Invoke(CurrentValue);
    }

    public void SetMax(float max)
    {
        MaxValue = max;
        CurrentValue = Mathf.Min(CurrentValue, MaxValue);
        OnValueChanged?.Invoke(CurrentValue);
    }
}
