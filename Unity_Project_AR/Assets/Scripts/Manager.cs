using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager instance;

    public bool detected;

    private bool firstOpened = true;


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onDetected()
    {
        detected = true;
    }

    public void offDetected()
    {
        if(firstOpened)
        {
            firstOpened = false;
            return;
        }
        detected = false;
    }
}
