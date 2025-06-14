using System.Collections;
using UnityEngine;

public class ItemEffectHandler : MonoBehaviour
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

    private IEnumerator ApplySpeedBuff(float value, float duration)
    {
        // 필요 시 PlayerMovement 등에서 속도 증감 적용
        Debug.Log($"[ItemEffectHandler] 이동속도 +{value} (지속 {duration}초)");
        yield return new WaitForSeconds(duration);
        Debug.Log($"[ItemEffectHandler] 이동속도 버프 종료");
    }

    private IEnumerator RegenHpOverTime(float totalAmount, float duration)
    {
        float tick = 0.5f;
        float elapsed = 0f;
        float amountPerTick = totalAmount / (duration / tick);

        while (elapsed < duration)
        {
            statManager.Recover(StatType.CurHp, amountPerTick);
            elapsed += tick;
            yield return new WaitForSeconds(tick);
        }
    }

    private IEnumerator ApplyInvisibility(float duration)
    {
        // 예시 처리 (머티리얼 조정, 레이어 변경 등)
        Debug.Log("[ItemEffectHandler] 은신 시작");
        yield return new WaitForSeconds(duration);
        Debug.Log("[ItemEffectHandler] 은신 종료");
    }
}
