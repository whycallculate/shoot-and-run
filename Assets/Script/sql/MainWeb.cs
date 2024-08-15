using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainWeb : MonoBehaviour
{
    private static MainWeb instance;
    public static MainWeb Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<MainWeb>();
            }
            return instance;
        }
    }
    public Web web;
    // Start is called before the first frame update
    void Start()
    {
        web = GetComponent<Web>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
