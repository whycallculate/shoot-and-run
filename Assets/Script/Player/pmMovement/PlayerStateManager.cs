using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerStateManager : MonoBehaviour
{
    public float moveSpeed = 3;
    [HideInInspector] public Vector3 dir;
    float hzInput,vInput;
    CharacterController controller;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        GetDirectionAndMove();
    }
    void GetDirectionAndMove()
    {
        hzInput = InputManager.HorizontalPos;
        vInput = InputManager.VerticalPos;

        dir =  transform.forward * vInput + transform.right * hzInput;
        controller.Move(dir * moveSpeed * Time.deltaTime);
    }
}
