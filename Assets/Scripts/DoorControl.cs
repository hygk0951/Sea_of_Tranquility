using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DoorControl : MonoBehaviour
{
    [Header("Connect Player")]
    [SerializeField] GameObject player;

    [Header("Door status & Control")]
    public bool inDetectArea = false;
    [SerializeField] GameObject lDoor;
    Vector3 lDoorInitPos;
    Vector3 lDoorDestPos;
    [SerializeField] GameObject rDoor;
    Vector3 rDoorInitPos;
    Vector3 rDoorDestPos;
    [SerializeField] bool readyToPlaySFX = true;
    [SerializeField] float openDistance = 1.8f;
    [SerializeField] float doorOpenTime = 1f;
    [SerializeField] float doorCloseWaitTime = 5f;
    [SerializeField] float doorCloseCurrentTime = 0;
    [SerializeField] float lerpPosition = 0f;
    AudioSource doorSFX;

    // Start is called before the first frame update
    void Start()
    {
        ConnectLRDoors();
        ConnectAudioSource();
    }

    private void ConnectAudioSource()
    {
        doorSFX = GetComponent<AudioSource>();
    }

    private void ConnectLRDoors()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Contains("Left"))
            {
                lDoor = transform.GetChild(i).gameObject;
                lDoorInitPos = lDoor.transform.localPosition;
                lDoorDestPos = lDoorInitPos + new Vector3(1, 0, 0) * openDistance;
            }
            if (transform.GetChild(i).name.Contains("Right"))
            {
                rDoor = transform.GetChild(i).gameObject;
                rDoorInitPos = rDoor.transform.localPosition;
                rDoorDestPos = rDoorInitPos - new Vector3(1, 0, 0) * openDistance;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        DoorOpenCloseControls();
        PlaySFX();
    }

    private void DoorOpenCloseControls()
    {
        OpenAndCloseDoor(lDoor.transform, lDoorInitPos, lDoorDestPos);
        OpenAndCloseDoor(rDoor.transform, rDoorInitPos, rDoorDestPos);
    }

    private void OpenAndCloseDoor(Transform door, Vector3 initPos, Vector3 destPos)
    {
        door.localPosition = Vector3.Lerp(initPos, destPos, lerpPosition);
        if (inDetectArea && lerpPosition < doorOpenTime)
        {
            lerpPosition += Time.deltaTime;
            doorCloseCurrentTime = 0;
        }
        else if (!inDetectArea && lerpPosition > 0)
        {
            if (doorCloseCurrentTime < doorCloseWaitTime)
                doorCloseCurrentTime += Time.deltaTime;
            else
                lerpPosition -= Time.deltaTime;
        }
    }

    private void PlaySFX()
    {
        if (inDetectArea && lerpPosition <= 0.1f ||
        !inDetectArea && doorCloseCurrentTime > doorCloseWaitTime && lerpPosition > 0.9f)
        {
            doorSFX.PlayOneShot(doorSFX.clip);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player)
        {
            inDetectArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            inDetectArea = false;
        }
    }

}
