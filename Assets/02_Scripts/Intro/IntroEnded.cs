using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroEnded : MonoBehaviour
{
    private void OnEnable()
    {
        OnstartGame();
    }

    public void OnstartGame()
    {
        SceneManager.LoadScene("Loginscene");
    }
}
