using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : Singleton<ResourcesManager>
{
    [SerializeField] float solarium = 1000f;
    [SerializeField] int batteryPack = 1;
    [SerializeField] float oxygen = 1000f;
    [SerializeField] int oxygenTank = 1;
    [SerializeField] int oxygenCapsule = 3;
    [SerializeField] int mineral = 0;
    [SerializeField] int plastic = 0;

    [SerializeField] int elixer = 0;
    [SerializeField] int researchPoint = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
