using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroSkip : MonoBehaviour
{
    public GameObject SkipPanel;

    public void SkipCheck()
    {
        SoundManager.Instance.PlaySFX(SFXType.Click);
        SkipPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    public void SkipCheckCancled()
    {
        SoundManager.Instance.PlaySFX(SFXType.Click);
        SkipPanel.SetActive(false);
        Time.timeScale = 1f; // Pause the game
    }
}
