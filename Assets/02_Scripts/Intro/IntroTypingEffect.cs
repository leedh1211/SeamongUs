using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class IntroTypingEffect : MonoBehaviour
{
    public TextMeshProUGUI targetText; // 출력할 Text UI
    [TextArea]
    public string fullText;            // 출력할 전체 문자열
    public float typingSpeed = 0.05f;  // 글자 하나당 출력 간격 (초)

    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    public GameObject nextObjectToActivate; // 다음 오브젝트

    public Action onTypingComplete; // 콜백 델리게이트

    private void Start()
    {
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        targetText.text = "";
        foreach (char c in fullText)
        {
            targetText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        // 타이핑 완료 → 페이드 아웃 → 다음 오브젝트 활성화
        yield return StartCoroutine(FadeOut());

        if (nextObjectToActivate != null)
            nextObjectToActivate.SetActive(true);
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
    }
}
