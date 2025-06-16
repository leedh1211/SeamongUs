using TMPro;
using UnityEngine;

public class ReportPopupController : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Animator animator;

    public void Init(string deadName)
    {
        titleText.text = $"{deadName}���� ��ü�� �߰ߵǾ����ϴ�.";
    }

    public void PlayEnter()
    {
        animator.SetTrigger("Enter");
    }

    public void PlayExit()
    {
        animator.SetTrigger("Exit");
    }
}

