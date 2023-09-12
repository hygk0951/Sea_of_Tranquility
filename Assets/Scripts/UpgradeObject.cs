using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using TMPro;

public class UpgradeObject : MonoBehaviour
{
    [SerializeField] TMP_Text level;
    string upgradeName;
    string effects;
    string neededResources;
    int time;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LevelUp()
    {
        int currentLevel = int.Parse(level.text);
        if (currentLevel < 5)
        {
            currentLevel++;
            level.text = currentLevel.ToString();
        }
    }
}