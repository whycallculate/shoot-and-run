using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    
    UI_BUTTON,
    UI_NEXT,
    WEAPON,
    PLAYER_FOOT_STEP,
    PLAYER_DAMAGE,
    PLAYER_STUFF
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    #region singleton
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundManager>();
            }
            return instance;
        }
    }
    #endregion

    [SerializeField] private SoundList[] soundList;
    [SerializeField] private AudioClip[] backgroundMucisList;
    [SerializeField] public AudioSource backgroundAudioSource;
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();



    }

    public static void PlaySoundOneShot(SoundType sound ,int i , float volume = 1)
    {
        AudioClip[] clip = Instance.soundList[(int)sound].Sounds;
        AudioClip playClip = clip[i];
        Instance.audioSource.PlayOneShot(playClip, volume);
    }
    public static void PlaySoundRandom(SoundType sound, float volume = 1)
    {
        AudioClip[] clip = Instance.soundList[(int)sound].Sounds;
        AudioClip playClip = clip[UnityEngine.Random.Range(0, clip.Length)];
        Instance.audioSource.PlayOneShot(playClip, volume);

    }

    public void PlayBackgroundMusic()
    {
        backgroundAudioSource.clip = backgroundMucisList[UnityEngine.Random.Range(0, backgroundMucisList.Length)];
        backgroundAudioSource.Play();
        backgroundAudioSource.PlayDelayed(2f);
    }
    public void StopBackgroundMusic()
    {
        backgroundAudioSource.clip = null;
        backgroundAudioSource.Stop();

    }


#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length );
        for(int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }

#endif
}
[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}