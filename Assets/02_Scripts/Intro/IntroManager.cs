using UnityEngine;

public class IntroManager : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayBGM(BGMType.Intro);
    }
}
