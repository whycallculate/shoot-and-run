using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class InputManager
{
    
    public static float VerticalPos;
    public static float HorizontalPos;
    public const KeyCode jumpKey = KeyCode.Space;
    public const KeyCode sprintkey = KeyCode.LeftShift;
    public const KeyCode crouchingKey = KeyCode.LeftControl;
    public const KeyCode getWeaponOnHand = KeyCode.X;
    


    public static void MoveInput()
    {
        
        VerticalPos = Input.GetAxisRaw("Vertical");
        HorizontalPos = Input.GetAxisRaw("Horizontal");
    }
    


}