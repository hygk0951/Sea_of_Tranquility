using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioTower : MonoBehaviour
{
    [SerializeField] float rotateSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RatateTelescope();
    }

    void RatateTelescope()
    {
        transform.Rotate(0,Time.deltaTime * rotateSpeed,0);
    }
}
