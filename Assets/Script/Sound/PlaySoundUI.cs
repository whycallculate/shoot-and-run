using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlaySoundUI : MonoBehaviour
{
    [SerializeField] private SoundType sound;
    [SerializeField, Range(0.1f, 1)] private float volume = 1;
    [SerializeField] private int i;

    public void PlaySoundButton() 
    { SoundManager.PlaySoundOneShot(sound,i, volume); }
    public void PlaySoundRandomButton()
    {
        SoundManager.PlaySoundRandom(sound, volume);
    }
}
