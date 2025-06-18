using System.Collections;
using UnityEngine;

public class IntroUIFader : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;

    public void FadeIn() => StartCoroutine(Fade(0f, 1f));
    public void FadeOut() => StartCoroutine(Fade(1f, 0f));

    private IEnumerator Fade(float from, float to)
    {
        float time = 0f;
        canvasGroup.alpha = from;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, time / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = to;
        canvasGroup.interactable = to == 1;
        canvasGroup.blocksRaycasts = to == 1;
    }

    private void OnEnable()
    {
       FadeIn();
    }
}
