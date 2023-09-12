using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : Singleton<GravityManager>
{
    [SerializeField] public bool artificialGravity = false;
    Vector3 onEarth = new Vector3(0, -9.81f, 0);
    Vector3 onMoon = new Vector3(0, -1.62f, 0);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (artificialGravity)
        {
            Physics.gravity = onEarth;
        }
        else
        {
            Physics.gravity = onMoon;
        }
    }
}
