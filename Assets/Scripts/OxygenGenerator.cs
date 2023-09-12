using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenGenerator : IndustryUpgradeModules
{
    private static OxygenGenerator instance;

    public static OxygenGenerator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (OxygenGenerator)FindObjectOfType(typeof(OxygenGenerator));
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(OxygenGenerator).Name, typeof(OxygenGenerator));
                    instance = obj.GetComponent<OxygenGenerator>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (transform.parent != null && transform.root != null)
        {
            DontDestroyOnLoad(this.transform.root.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
