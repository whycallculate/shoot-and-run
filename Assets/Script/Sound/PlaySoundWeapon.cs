using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundWeapon : MonoBehaviour
{
    public void PlaySoundButton(SoundType sound, int type, float volume)
    { SoundManager.PlaySoundOneShot(sound, type, volume); }

}
