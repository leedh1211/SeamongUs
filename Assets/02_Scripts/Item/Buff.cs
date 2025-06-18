using UnityEngine;

public enum BuffType
{
    Speed,
    RegenHp,
    Invisibility
}

public class Buff
{
    public BuffType Type { get; private set; }
    public float Value { get; private set; }
    public float Duration { get; private set; }
    private float timeRemaining;

    public Buff(BuffType type, float value, float duration)
    {
        Type = type;
        Value = value;
        Duration = duration;
        timeRemaining = duration;
    }

    public void Update(GameObject player, float deltaTime)
    {
        timeRemaining -= deltaTime;

        if (Type == BuffType.RegenHp)
        {
            var statManager = player.GetComponent<StatManager>();
            if (statManager != null)
            {
                statManager.Recover(StatType.CurHp, Value * deltaTime);
            }
        }
    }

    public bool IsExpired => timeRemaining <= 0f;

    public void Apply(GameObject player)
    {
        switch (Type)
        {
            case BuffType.Speed:
                player.GetComponent<PlayerController>()?.ModifySpeed(Value);
                break;

            case BuffType.Invisibility:
                player.layer = LayerMask.NameToLayer("Invisible"); // 레이어는 프로젝트에 따라 다를 수 있음
                break;
        }
    }

    public void Remove(GameObject player)
    {
        switch (Type)
        {
            case BuffType.Speed:
                player.GetComponent<PlayerController>()?.ModifySpeed(-Value);
                break;

            case BuffType.Invisibility:
                player.layer = LayerMask.NameToLayer("Player"); // 원래 레이어로 복귀
                break;
        }
    }
}
