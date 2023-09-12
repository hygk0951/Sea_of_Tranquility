using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerColliderForCamera : MonoBehaviour
{
    [Header("Connected CameraControls")]
    [SerializeField] CameraControls cameraControls;

    [Header("Connected Camera")]
    [SerializeField] public GameObject myCamera;

    void Start()
    {
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

    void FixedUpdate()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        cameraControls.currentCollider = this.gameObject;
    }
}
