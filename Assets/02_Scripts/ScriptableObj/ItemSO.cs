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
    public float duration; // ���� �ð� (0�̸� ��� ȿ��)
}
