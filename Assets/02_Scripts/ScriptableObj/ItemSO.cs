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
    public int Id;  // 네트워크에서 식별용 고유 번호 추가
    public string itemName;
    public Sprite icon;
    public ItemEffectType effectType;
    public float effectValue;
    public float duration; // 지속 시간 (0이면 즉시 효과)
}
