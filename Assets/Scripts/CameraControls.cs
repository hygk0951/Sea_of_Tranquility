using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class CameraControls : Singleton<CameraControls>
{
    [Header("Player")]
    [SerializeField] GameObject player;
    RaycastHit hitInfoUp;
    [SerializeField] LayerMask structureRoof;
    [SerializeField] bool indoor = false;
    Animator animator;

    [Header("Structure")]
    [SerializeField] GameObject[] allTriggerColliders;
    [SerializeField] public GameObject currentCollider;

    [Header("Cameras")]
    [SerializeField] GameObject[] allCinemachineCameras;
    Dictionary<GameObject, GameObject> vCameraMap = new Dictionary<GameObject, GameObject>();
    [SerializeField] GameObject currentCamera;
    [SerializeField] GameObject playerCamera;
    [SerializeField] GameObject idleCamera;
    [SerializeField] GameObject idleCameraOxygenModule;

    [SerializeField] float idleCamYPosition;

    [Header("Settings")]
    [SerializeField] Transform idleDollyTrack;
    [SerializeField] CinemachineDollyCart idleDollyCart1;
    [SerializeField] CinemachineDollyCart idleDollyCart2;
    [SerializeField] public float idlePoseStartTime;
    [SerializeField] float currentIdleTime;

    void Start()
    {
        // vCameraMap = new Dictionary<Vcamera, TriggerColliderForCamera>();
        animator = PlayerMovement.animator;
        CameraSettings();
    }

    void FixedUpdate()
    {
        FixIdleCameraTrackPosition();
        SwitchCameraControls();
        GetCurrentCamera();
    }

    private void GetCurrentCamera()
    {
        ICinemachineCamera currentCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera;
        this.currentCamera = currentCamera.VirtualCameraGameObject;
    }

    private void CameraSettings()
    {
        AddAllTriggerColliders();
        AddAllVcameras();
        AddAllVcameraMap();
        SetAllCameraPriorityToZero();
        foreach (GameObject camera in allCinemachineCameras)
        {
            switch (camera.name)
            {
                case "Player Camera":
                    playerCamera = camera;
                    camera.GetComponent<CinemachineFreeLook>().Priority = 1;
                    break;
                case "Idle Camera":
                    idleCamera = camera;
                    break;
                case "idleCamera OxygenModule":
                    idleCameraOxygenModule = camera;
                    break;
                default:
                    break;
            }
        }
    }

    private void AddAllTriggerColliders()
    {
        allTriggerColliders = GameObject.FindGameObjectsWithTag("Trigger Collider");
        foreach (GameObject triggerCollider in allTriggerColliders)
        {
            triggerCollider.AddComponent<TriggerColliderForCamera>();
        }
    }

    private void AddAllVcameras()
    {
        allCinemachineCameras = GameObject.FindGameObjectsWithTag("Camera");
        foreach (GameObject camera in allCinemachineCameras)
        {
            camera.AddComponent<Vcamera>();
        }
    }

    private void AddAllVcameraMap()
    {
        for (int i = 0; i < allCinemachineCameras.Length; i++)
        {
            // vCameraMap.Add(allCinemachineCameras[i], allTriggerColliders[i]);
        }
    }

    private void SetAllCameraPriorityToZero()
    {
        foreach (GameObject camera in allCinemachineCameras)
        {
            SetCamreaPriority(camera.transform, 0);
        }
    }

    private void FixIdleCameraTrackPosition()
    {
        idleDollyTrack.transform.position = player.transform.position + new Vector3(0, idleCamYPosition, 0);
        idleDollyTrack.transform.forward = player.transform.forward;
    }

    private void SetCamreaPriority(Transform Vcam, int priority)
    {
        if (Vcam.GetComponent<CinemachineFreeLook>() != null)
            Vcam.GetComponent<CinemachineFreeLook>().Priority = priority;
        if (Vcam.GetComponent<CinemachineVirtualCamera>() != null)
            Vcam.GetComponent<CinemachineVirtualCamera>().Priority = priority;
    }

    public void SwitchCamera(GameObject camera)
    {
        foreach (GameObject others in allCinemachineCameras)
        {
            if (others.GetComponent<CinemachineFreeLook>() != null)
            {
                others.GetComponent<CinemachineFreeLook>().Priority = 0;
            }
            if (others.GetComponent<CinemachineVirtualCamera>() != null)
            {
                others.GetComponent<CinemachineVirtualCamera>().Priority = 0;
            }
        }
        if (camera.GetComponent<CinemachineFreeLook>() != null)
        {
            camera.GetComponent<CinemachineFreeLook>().Priority = 1;
        }
        if (camera.GetComponent<CinemachineVirtualCamera>() != null)
        {
            camera.GetComponent<CinemachineVirtualCamera>().Priority = 1;
        }
    }

    private void SwitchCameraControls()
    {
        if (indoor)
        {
            IndoorCameraControl();
        }
        else
        {
            IdleCameraControl();
        }
    }

    private void IndoorCameraControl()
    {
        // Debug.Log(currentCollider.name);
        // SwitchCamera(currentCollider.GetComponent<TriggerColliderForCamera>().myCamera);
        // switch (currentCollider.name)
        // {
        //     case "Entry Module Enter Trigger":
        //         SwitchCamera(baseCamera1);
        //         break;
        //     case "Oxygen Module Enter Trigger":
        //         SwitchCamera(baseCamera2);
        //         break;
        //     default:
        //         break;
        // }
    }

    private void IdleCameraControl()
    {
        currentIdleTime = animator.GetFloat("IdleTime");
        if (currentIdleTime > idlePoseStartTime)
        {
            if (UIManager.Instance.currentLocation == "Oxygen Module")
            {
                SwitchCamera(idleCameraOxygenModule);
                UIManager.Instance.transform.Find("GeneralUI").GetComponent<Animator>().SetBool("IsIdle", true);
            }
            else if (UIManager.Instance.currentLocation == "Lunar Surface")
            {
                SwitchCamera(idleCamera);
                UIManager.Instance.transform.Find("GeneralUI").GetComponent<Animator>().SetBool("IsIdle", true);
            }
        }
        else
        {
            UIManager.Instance.transform.Find("GeneralUI").GetComponent<Animator>().SetBool("IsIdle", false);
            SwitchCamera(playerCamera);
            // idleDollyCart1.m_Position = 0;
            // idleDollyCart2.m_Position = 0;
        }
    }
}
