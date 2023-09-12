using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrackMoveControl : MonoBehaviour
{
    float timeChecker;
    Vector3 initPos;
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        timeChecker += Time.deltaTime;
        transform.localPosition = initPos + new Vector3(0, Mathf.Sin(timeChecker) / 10, 0);
    }
}
