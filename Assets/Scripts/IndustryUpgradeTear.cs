using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndustryUpgradeTear : Singleton<IndustryUpgradeTear>
{
    public bool[] tear;
    // Start is called before the first frame update
    void Start()
    {
        tear = new bool[4];
        for (int i = 0; i < tear.Length; i++)
        {
            if (i == 0) tear[i] = true;
            else tear[i] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
