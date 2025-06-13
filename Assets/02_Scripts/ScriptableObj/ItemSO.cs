using UnityEngine;

public enum ItemEffectType
{
    HealHp,
    HealStamina,
    BuffSpeed,
    RegenHp,
    Invisibility
}

[CreateAssetMenu(menuName = "SO/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemEffectType effectType;
    public float effectValue;
    public float duration; // 지속 시간 (0이면 즉시 효과)
}
