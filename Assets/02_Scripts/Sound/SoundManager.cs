using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SfxType
{
    Click, // 0
    Start, // 1
    End, // 2
    Report,
    Eject,
    //Walk, Jump, Dance, Run, Win, Lose, Hit, Damage, Heal, Death, ...
}

public enum BgmType
{
    Login,
    Room,
    Intro,
    Ending, 
    Map01, //Map02, Map03,
}

/// <summary>
/// 사운드 사용 법 참고사항:
/// 방 입장 시
/// SoundManager.Instance.PlayBGM(BgmType.Room);

/// 버튼 클릭 시
/// SoundManager.Instance.PlaySfx(SfxType.Click);

/// 리포트 버튼 클릭 시
/// SoundManager.Instance.PlaySfx(SfxType.Report);

/// 추방될 때
/// SoundManager.Instance.PlaySfx(SfxType.Eject);
/// </summary>

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("SFX Clips (enum 인덱스와 맞춰서)")]
    [SerializeField] private List<AudioClip> sfxClips;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.7f;

    [Header("BGM Clips (enum 인덱스와 맞춰서)")]
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField, Range(0f, 1f)] private float bgmVolume = 0.5f;

    private AudioSource sfxSource;
    private AudioSource bgmSource;

    private Dictionary<SfxType, AudioClip> _sfxList;
    private Dictionary<BgmType, AudioClip> _bgmList;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // AudioSource 세팅
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;

        // 매핑
        InitializeMappings();

        // 초기 BGM 재생 (로그인 화면 BGM)
        PlayBGM(BgmType.Login);
    }

    // 배경음악
    public void PlayBGM(BgmType track)
    {
        if (!_bgmList.TryGetValue(track, out var clip) || clip == null)
        {
            Debug.LogWarning($"BGM type {track} does not have a corresponding clip or the clip is null.");
            return;
        }

        if (bgmSource.clip == clip && bgmSource.isPlaying)
        {
            return;
        }

        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void stopBGM()
    {
        bgmSource.Stop();
    }

    // 효과음
    public void PlaySFX(SfxType type)
    {
        if (!_sfxList.TryGetValue(type, out var clip) || clip == null)
        {
            Debug.LogWarning($"SFX type {type} does not have a corresponding clip or the clip is null.");
            return;
        }

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    // 매핑 초기화
    public void InitializeMappings()
    {
        // 효과음
        _sfxList = new Dictionary<SfxType, AudioClip>();
        foreach (var type in Enum.GetValues(typeof(SfxType)).Cast<SfxType>())
        {
            int index = (int)type;
            if (index < sfxClips.Count)
            {
                _sfxList[type] = sfxClips[index];
            }
            else
            {
                Debug.LogWarning($"SfxType {type} does not have a corresponding clip in the list.");
            }
        }

        // 배경음악
        _bgmList = new Dictionary<BgmType, AudioClip>();
        foreach (BgmType type in Enum.GetValues(typeof(BgmType)))
        {
            int index = (int)type;
            if (index < bgmClips.Length)
            {
                _bgmList[type] = bgmClips[index];
            }
            else
            {
                Debug.LogWarning($"BgmType {type} does not have a corresponding clip in the array.");
            }
        }
    }
}
