using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] public GameObject target;

    void Update()
    {
        transform.position = target.transform.position;
    }
}
