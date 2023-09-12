using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerColliderManager : MonoBehaviour
{
    GameObject[] triggerColliders;
    // Start is called before the first frame update
    void Start()
    {
        triggerColliders = GameObject.FindGameObjectsWithTag("Trigger Collider");
        foreach (GameObject triggerCollider in triggerColliders)
        {
            triggerCollider.AddComponent<TriggerColliderForCamera>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
