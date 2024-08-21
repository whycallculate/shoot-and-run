using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region singleton
    private static InputManager instance;
    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InputManager>();
            }
            return instance;
        }
    }
    #endregion

    public float VerticalPos;
    public float HorizontalPos;
    public KeyCode jumpKey = KeyCode.Space;

    private void Update()
    {
        MoveInput();
    }
    public void MoveInput()
    {
        VerticalPos = Input.GetAxisRaw("Vertical");
        HorizontalPos = Input.GetAxisRaw("Horizontal");
        Debug.Log("Input manager move input");
        Debug.Log(VerticalPos);
    }


}