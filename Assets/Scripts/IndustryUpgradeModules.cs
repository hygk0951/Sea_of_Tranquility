using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndustryUpgradeModules : MonoBehaviour
{
    float powerConsumption = 2.0f;
    int mineralConsumption = 0;
    int stability = 0;

    IndustryUpgradeTear openedTear;
    // Start is called before the first frame update
    void Start()
    {
        openedTear = IndustryUpgradeTear.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
