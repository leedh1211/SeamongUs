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
        SoundManager.Instance.PlaySFX(SFXType.Click);
        Time.timeScale = 1f;
        SceneManager.LoadScene("Loginscene");
    }
}
