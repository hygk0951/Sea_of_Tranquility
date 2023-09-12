using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasementControls : MonoBehaviour
{
    [Header("Position of player")]
    [SerializeField] public Transform player;

    [Header("Doors")]
    [SerializeField] List<Transform> doors;

    [Header("Doors Settings")]
    [SerializeField] public float doorOpenDistance = 3.5f;
    [SerializeField] public AudioSource doorSFX;

    // [Header("Debug")]

    void Start()
    {
        Transform doors = transform.Find("Doors");
        for (int i = 1; i < doors.childCount; i++)
        {
            for (int j = 0; j < doors.GetChild(i).childCount; j++)
            {
                Transform newDoor = doors.GetChild(i).GetChild(j);
                newDoor.gameObject.AddComponent<DoorStatus>();
                this.doors.Add(newDoor);
            }
        }

        doorSFX = transform.Find("Doors").Find("Door SFX").GetComponent<AudioSource>();
        doorSFX.playOnAwake = false;
    }

    void FixedUpdate()
    {

    }
    private void DoorControl()
    {

    }
}
