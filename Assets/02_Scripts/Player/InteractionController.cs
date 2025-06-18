using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float killDamage = 100f;

    public void TryInteract()
    {
        // 미션 관련 로직
    }

    public void TryKill()
    {
        // 주변 플레이어 탐색
        Collider2D target = Physics2D.OverlapCircle(transform.position, 1.2f, LayerMask.GetMask("Player"));

        if (target != null && target.gameObject != this.gameObject)
        {
            // 상대 StatManager 가져오기
            StatManager targetStats = target.GetComponent<StatManager>();

            if (targetStats != null)
            {
                targetStats.Consume(StatType.CurHp, killDamage);
                Debug.Log($"[Kill] {target.name}에게 {killDamage} 피해를 입혔습니다.");
            }
        }
    }

    public void OpenInventory()
    {
        // 인벤토리 UI 열기 로직
    }
}
