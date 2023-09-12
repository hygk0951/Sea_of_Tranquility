using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vcamera : MonoBehaviour
{
    String cameraName;
    [Header("Connected CameraControls")]
    [SerializeField] CameraControls cameraControls;

    void Start()
    {
        cameraName = this.name;
        ConnectCameraControls();
    }

    private void ConnectCameraControls()
    {
        GameObject[] managers = GameObject.FindGameObjectsWithTag("Manager");
        for (int i = 0; i < managers.Length; i++)
        {
            if (managers[i].name.Contains("Camera"))
            {
                cameraControls = managers[i].GetComponent<CameraControls>();
            }
        }
    }

    void Update()
    {

    }
}
