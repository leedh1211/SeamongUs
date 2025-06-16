using TMPro;
using UnityEngine;

public class ReportPopupController : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Animator animator;

    public void Init(string deadName)
    {
        titleText.text = $"{deadName}님의 시체가 발견되었습니다.";
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

