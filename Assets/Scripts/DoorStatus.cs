using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorStatus : MonoBehaviour
{
    [Header("Basement Controls")]
    [SerializeField] BasementControls basementControls;
    [SerializeField] Transform player;

    [Header("Door Controls")]
    [SerializeField] bool playerDetected = false;
    [SerializeField] bool doorIsNowMoving = false;
    [SerializeField] Transform pairDoor;
    [SerializeField] AudioSource doorSFX;

    Vector3 initPosition;
    Vector3 targetPosition;
    float time = 0;
    // Start is called before the first frame update
    void Start()
    {
        basementControls = GetComponentInParent<BasementControls>();
        doorSFX = basementControls.doorSFX;
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if (this.name.Contains("Left") && transform.parent.GetChild(i).name.Contains("Right"))
            {
                pairDoor = transform.parent.GetChild(i);
            }
            else if (this.name.Contains("Right") && transform.parent.GetChild(i).name.Contains("Left"))
            {
                pairDoor = transform.parent.GetChild(i);
            }
        }

        player = GameObject.FindWithTag("Player").transform;
        // player = basementControls.player;
        initPosition = this.transform.localPosition;
        if (this.name.Contains("Left"))
        {
            targetPosition = initPosition + transform.right * 1.1f;
        }
        else if (this.name.Contains("Right"))
        {
            targetPosition = initPosition - transform.right * 1.1f;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        ControlDoors();
    }

    public float doorToPlayerDistance()
    {
        if (player == null) return 0;
        float distance = (player.position - transform.position).magnitude;
        return distance;
    }

    private void ControlDoors()
    {
        DetectPlayerFromDoor();
        OpenAndCloseDoor();
        PlayDoorSFX();
    }

    private void DetectPlayerFromDoor()
    {
        if (basementControls.doorOpenDistance < doorToPlayerDistance() &&
                basementControls.doorOpenDistance < pairDoor.GetComponent<DoorStatus>().doorToPlayerDistance())
        {
            playerDetected = false;
            pairDoor.GetComponent<DoorStatus>().playerDetected = false;
        }
        else
        {
            playerDetected = true;
            pairDoor.GetComponent<DoorStatus>().playerDetected = true;
        }
    }

    private void OpenAndCloseDoor()
    {
        transform.localPosition = Vector3.Lerp(initPosition, targetPosition, time);
        if (playerDetected && time < 1.0f)
        {
            time += Time.deltaTime;
            doorIsNowMoving = true;
        }
        else if (playerDetected && time >= 1.0f)
        {
            doorIsNowMoving = false;
        }
        else if (!playerDetected && time > 0.0f)
        {
            time -= Time.deltaTime;
            doorIsNowMoving = true;
        }
        else
        {
            doorIsNowMoving = false;
        }
    }

    private void PlayDoorSFX()
    {
        if (doorIsNowMoving && !doorSFX.isPlaying)
        {
            doorSFX.Play();
        }
    }
}
