using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance && Instance !=this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] private UIAudio _uiAudio;
    
    [Header("Mixers")]
    [SerializeField] private AudioMixer masterMixerGroup;
    private const string MASTER_VOLUME_PARAMETER = "MasterVolume";
    
    [SerializeField] private AudioMixerGroup backGroundMixer;
    private const string BACKGROUND_VOLUME_PARAMETER = "BackGroundVolume";
    
    [SerializeField] private AudioMixerGroup soundMixer;
    private const string SOUNDS_VOLUME_PARAMETER = "SoundVolume";
    
    
    [Header("AudioSource")]
    [SerializeField] private AudioSource backGroundSource;
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioSource gameAudioSource;
    [SerializeField] private AudioSource enemyAudioSource;
    
    [Header("Clips")]
    [SerializeField] private AudioClip backGroundClip;
    [SerializeField] private AudioClip stepsClip;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip CheckPointClip;
    [SerializeField] private AudioClip VitoryClip;
    [SerializeField] private AudioClip LoseClip;

    
    
    private float currentVolumeValue = 0;
    private float currentMusicVolumeValue = 0;
    private float currentSoundVolumeValue = 0;


    private const string SAVE_GENERAL_VOLUME_KEY = "GeneralVolume";
    private const string SAVE_BACKMUSIC_VOLUME_KEY = "BackMVolume";
    private const string SAVE_SOUND_VOLUME_KEY = "SoundsVolume";
    private void Start()
    {
        LoadVolumeValues();
    }

    public void SaveVolumeValues()
    {
        PlayerPrefs.SetFloat(SAVE_GENERAL_VOLUME_KEY, currentVolumeValue);
        PlayerPrefs.SetFloat(SAVE_BACKMUSIC_VOLUME_KEY, currentMusicVolumeValue);
        PlayerPrefs.SetFloat(SAVE_SOUND_VOLUME_KEY, currentSoundVolumeValue);
        
        PlayerPrefs.Save();
    }

    private void LoadVolumeValues()
    {
        currentVolumeValue = PlayerPrefs.GetFloat(SAVE_GENERAL_VOLUME_KEY, 0f);
        _uiAudio.ChangeGeneralUISlider(currentVolumeValue);
        
        currentMusicVolumeValue = PlayerPrefs.GetFloat(SAVE_BACKMUSIC_VOLUME_KEY, -20f);
        _uiAudio.ChangeMusicUISlider(currentMusicVolumeValue);
        
        currentSoundVolumeValue = PlayerPrefs.GetFloat(SAVE_SOUND_VOLUME_KEY, -20f);
        _uiAudio.ChangeSoundUISlider(currentSoundVolumeValue);
        
        masterMixerGroup.SetFloat(MASTER_VOLUME_PARAMETER, currentVolumeValue);
        backGroundMixer.audioMixer.SetFloat(BACKGROUND_VOLUME_PARAMETER, currentMusicVolumeValue);
        soundMixer.audioMixer.SetFloat(SOUNDS_VOLUME_PARAMETER, currentSoundVolumeValue);
    }

    public void ChangeMasterVolume(float newValue)
    {
        currentVolumeValue = Mathf.Lerp(-80f, 0f, newValue);
        masterMixerGroup.SetFloat(MASTER_VOLUME_PARAMETER, currentVolumeValue);
    }
    
    public void ChangeBackMusicVolume(float newValue)
    {
        currentMusicVolumeValue = (1 - newValue) * -40;
        backGroundMixer.audioMixer.SetFloat(BACKGROUND_VOLUME_PARAMETER, currentMusicVolumeValue);
    }
    
    public void ChangeSoundsVolume(float newValue)
    {
        currentSoundVolumeValue = (1 - newValue) * -40;
        soundMixer.audioMixer.SetFloat(SOUNDS_VOLUME_PARAMETER, currentSoundVolumeValue);
    }
    
    private void PlaySound(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }

    public void PlayStepSounds()
    {
        PlaySound(playerAudioSource, stepsClip);
    }
    
    //for Simulations
    public void PlayStepLoopSounds()
    {
        playerAudioSource.loop = true;
        playerAudioSource.clip = stepsClip;
        playerAudioSource.Play();
    }
    //end for simulation
    
    public void PlayShootSounds()
    {
        PlaySound(enemyAudioSource, shootClip);
    }
    
    public void PlayCheckPointSound()
    {
        PlaySound(gameAudioSource, CheckPointClip);
    }
    
    public void PlayVictorySound()
    {
        PlaySound(gameAudioSource, VitoryClip);
    }
    
    public void PlayLoseSound()
    {
        PlaySound(gameAudioSource, LoseClip);
    }
    

}
