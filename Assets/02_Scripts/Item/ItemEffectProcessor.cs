using UnityEngine;

public class ItemEffectProcessor : MonoBehaviour
{
    [SerializeField] private StatManager statManager;
    [SerializeField] private BuffManager buffManager;

    public void UseItem(ItemSO item)
    {
        switch (item.effectType)
        {
            case ItemEffectType.HealHp:
                statManager.Recover(StatType.CurHp, item.effectValue);
                break;

            case ItemEffectType.HealStamina:
                statManager.Recover(StatType.Stamina, item.effectValue);
                break;

            case ItemEffectType.BuffSpeed:
                buffManager.ApplyBuff(new Buff(BuffType.Speed, item.effectValue, item.duration));
                break;

            case ItemEffectType.RegenHp:
                buffManager.ApplyBuff(new Buff(BuffType.RegenHp, item.effectValue, item.duration));
                break;

            case ItemEffectType.Invisibility:
                buffManager.ApplyBuff(new Buff(BuffType.Invisibility, 0f, item.duration));
                break;
        }
    }
}
