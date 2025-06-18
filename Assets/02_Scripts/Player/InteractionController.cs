using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float killDamage = 100f;

    public void TryInteract()
    {
        // �̼� ���� ����
    }

    public void TryKill()
    {
        // �ֺ� �÷��̾� Ž��
        Collider2D target = Physics2D.OverlapCircle(transform.position, 1.2f, LayerMask.GetMask("Player"));

        if (target != null && target.gameObject != this.gameObject)
        {
            // ��� StatManager ��������
            StatManager targetStats = target.GetComponent<StatManager>();

            if (targetStats != null)
            {
                targetStats.Consume(StatType.CurHp, killDamage);
                Debug.Log($"[Kill] {target.name}���� {killDamage} ���ظ� �������ϴ�.");
            }
        }
    }

    public void OpenInventory()
    {
        // �κ��丮 UI ���� ����
    }
}
