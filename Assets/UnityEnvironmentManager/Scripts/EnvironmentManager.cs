
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnitySingleton;

#if TAPTIC
using TapticPlugin;
#endif

public class EnvironmentManager : MonoBehaviourProtectedSingleton<EnvironmentManager>
{
    #region FrameRate
    [SerializeField]
    private bool m_IsShowFrameRate = false;

    [SerializeField]
    private FrameRate m_AwakeFrameRate = FrameRate.FPS_30;
    public static FrameRate AwakeFrameRate { get { return _Instance.m_AwakeFrameRate; } }

    public enum FrameRate : int
    {
        FPS_10 = 10,
        FPS_15 = 15,
        FPS_20 = 20,
        FPS_30 = 30,
        FPS_60 = 60
    }
    #endregion

    #region Audios
    [SerializeField]
    private AudioSource[] m_AudioSources = default;
    [SerializeField]
    private AudioClip[] m_ClipsSE = default;
    [SerializeField]
    private AudioClip[] m_ClipsBGM = default;

    public enum AudioType : int { SE = 0, BGM }
    public enum ClipBGMType { }
    public enum ClipSEType { Wall, Broad, Moving, TargetBreak }

    private readonly AudioModels[] m_AudioModels = { new AudioModels(), new AudioModels() };

    private List<Button> m_MuteSwitchButtons;
    [SerializeField]
    private Sprite m_SoundOnSprite = default;
    [SerializeField]
    private Sprite m_SoundOffSprite = default;

    private bool m_IsMute = false;
    #endregion

#if TAPTIC
    public static bool IsTapticEnable { get { return TapticTrigger.IsEnable; } set { TapticTrigger.IsEnable = value; } }
#endif

    protected override void Awake()
    {
        base.Awake();
#if UNITY_STANDALONE || UNITY_EDITOR
        QualitySettings.vSyncCount = 0;
#endif
        SetTargetFrameRate(m_AwakeFrameRate);

        if (m_IsShowFrameRate)
            gameObject.AddComponent<FPSDisplay>();
        SceneManager.sceneLoaded += OnSceneLoaded;

#if TAPTIC
        IsTapticEnable = true;
#endif
    }

    public static void PlayAudio(AudioType audioType)
    {
        _Instance.m_AudioSources[(int)audioType].Play();
    }

    public static void PlayAudio(AudioType audioType, AudioClip clip)
    {
        StopAudio(audioType);
        _Instance.m_AudioSources[(int)audioType].PlayOneShot(clip);
    }

    public static void PlayBGM(ClipBGMType clipBGMType)
    {
        AudioClip audioClip = _Instance.m_ClipsBGM[(int)clipBGMType];
        if (!_Instance.m_AudioSources[(int)AudioType.BGM].isPlaying || _Instance.m_AudioSources[(int)AudioType.BGM].clip != audioClip)
        {
            _Instance.m_AudioSources[(int)AudioType.BGM].clip = audioClip;
            _Instance.m_AudioSources[(int)AudioType.BGM].Play();
        }
    }

    public static void PlaySE(ClipSEType clipSEType)
    {
        AudioClip audioClip = _Instance.m_ClipsSE[(int)clipSEType];
        PlayAudio(AudioType.SE, audioClip);
    }

    public static void PlaySE(ClipSEType clipSEType, AudioSource audioSource)
    {
        AudioClip audioClip = _Instance.m_ClipsSE[(int)clipSEType];
        audioSource.PlayOneShot(audioClip);
        PlayAudio(AudioType.SE, audioClip);
    }

    public static void PauseAudio(AudioType audioType)
    {
        _Instance.m_AudioSources[(int)audioType].Pause();
    }

    public static void StopAudio(AudioType audioType)
    {
        _Instance.m_AudioSources[(int)audioType].Stop();
    }

    public static void SetAudioMute(AudioType audioType, bool isMute)
    {
        _Instance.m_AudioModels[(int)audioType].IsMute = isMute;
        _Instance.SetAudioVolume(audioType);
    }

    public static void SetAudioVolume(AudioType audioType, float volume)
    {
        _Instance.m_AudioModels[(int)audioType].Volume = volume;
        _Instance.SetAudioVolume(audioType);
    }

    public static bool IsAudioMute(AudioType audioType)
    {
        return _Instance.m_AudioModels[(int)audioType].IsMute;
    }

    private void SetAudioVolume(AudioType audioType)
    {
        AudioModels models = _Instance.m_AudioModels[(int)audioType];
        float newVolume = (models.IsMute) ? -80f : models.Volume;
        SetAudioMixerGroupVolume(audioType, newVolume);
    }

    private void SetAudioMixerGroupVolume(AudioType audioType, float volume)
    {
        m_AudioSources[(int)audioType].outputAudioMixerGroup.audioMixer.SetFloat(audioType.ToString(), volume);
    }

    public static void SetTargetFrameRate(FrameRate frameRate)
    {
        Application.targetFrameRate = (int)frameRate;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_Instance.m_MuteSwitchButtons == null)
        {
            return;
        }
        for (int i = 0; i < _Instance.m_MuteSwitchButtons.Count; i++)
        {
            if (_Instance.m_MuteSwitchButtons[i] == null)
            {
                _Instance.m_MuteSwitchButtons.RemoveAt(i);
            }
        }
    }

    public static void AddMuteButton(Button button)
    {
        if (_Instance.m_MuteSwitchButtons == null)
        {
            _Instance.m_MuteSwitchButtons = new List<Button>();
        }
        button.image.sprite = _Instance.m_IsMute ? _Instance.m_SoundOffSprite : _Instance.m_SoundOnSprite;
        button.onClick.AddListener(MuteSwitch);
        _Instance.m_MuteSwitchButtons.Add(button);
    }

    private static void MuteSwitch()
    {
        _Instance.m_IsMute = !_Instance.m_IsMute;

        SetAudioMute(AudioType.SE, _Instance.m_IsMute);
        SetAudioMute(AudioType.BGM, _Instance.m_IsMute);
        for (int i = 0; i < _Instance.m_MuteSwitchButtons.Count; i++)
        {
            _Instance.m_MuteSwitchButtons[i].image.sprite = _Instance.m_IsMute ? _Instance.m_SoundOffSprite : _Instance.m_SoundOnSprite;
        }
    }

    //public 
}

public class AudioModels
{
    public bool IsMute = false;

    private float _volume = 0f;
    public float Volume
    {
        get { return _volume; }
        set
        {
            if (value < -80f)
                _volume = -80f;
            else if (value > 20f)
                _volume = 20f;
            else
                _volume = value;
        }
    }
}