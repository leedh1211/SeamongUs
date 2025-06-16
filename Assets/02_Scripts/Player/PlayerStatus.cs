using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public float maxHp = 100f;
    public float currentHp = 100f;
    public float stamina = 50f;
    public float speed = 5f;

    public void ApplyItemEffect(ItemSO item)
    {
        switch (item.effectType)
        {
            case ItemEffectType.HealHp:
                currentHp = Mathf.Min(currentHp + item.effectValue, maxHp);
                break;

            case ItemEffectType.HealStamina:
                stamina += item.effectValue;
                break;

            case ItemEffectType.BuffSpeed:
                StartCoroutine(ApplySpeedBuff(item.effectValue, item.duration));
                break;

            case ItemEffectType.RegenHp:
                StartCoroutine(RegenHpOverTime(item.effectValue, item.duration));
                break;

            case ItemEffectType.Invisibility:
                StartCoroutine(ApplyInvisibility(item.duration));
                break;
        }
    }

    private IEnumerator ApplySpeedBuff(float value, float duration)
    {
        speed += value;
        yield return new WaitForSeconds(duration);
        speed -= value;
    }

    private IEnumerator RegenHpOverTime(float totalAmount, float duration)
    {
        float time = 0f;
        float tick = 0.5f;
        float amountPerTick = totalAmount / (duration / tick);

        while (time < duration)
        {
            currentHp = Mathf.Min(currentHp + amountPerTick, maxHp);
            time += tick;
            yield return new WaitForSeconds(tick);
        }
    }

    private IEnumerator ApplyInvisibility(float duration)
    {
        // 예시: 콜라이더 비활성화 또는 머터리얼 투명화
        // GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(duration);
        // GetComponent<Renderer>().enabled = true;
    }
}

