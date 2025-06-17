using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    private void Awake()
    {
        SoundManager.Instance.PlayBGM(BGMType.Intro);
    }
}
