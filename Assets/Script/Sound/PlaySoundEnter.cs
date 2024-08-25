using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundEnter : StateMachineBehaviour
{
    [SerializeField] private SoundType sound;
    [SerializeField,Range(0.1f,1)] private float volume = 1;
    [SerializeField] private int i;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        SoundManager.PlaySoundOneShot(sound,i,volume );
    }
}
