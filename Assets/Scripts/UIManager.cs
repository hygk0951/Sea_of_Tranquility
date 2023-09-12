using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] Texture2D cursorImg;
    bool cursorLock = true;
    [SerializeField] GameObject generalUI;
    [SerializeField] GameObject researchUI;
    [SerializeField] GameObject industryUI;

    float timeChecker;
    [SerializeField] String startLocation;
    public String currentLocation;
    [SerializeField] TMP_Text location;
    [SerializeField] TMP_Text fuel;
    [SerializeField] Slider fuelSlider;
    [SerializeField] TMP_Text exp;
    [SerializeField] Slider expSlider;
    int playerLevel;
    [SerializeField] List<GameObject> levelUI;
    public bool readyToAlert = false;
    Vector3 lastGravity;

    // Start is called before the first frame update
    void Start()
    {
        lastGravity = Physics.gravity;
        Cursor.SetCursor(cursorImg, new Vector2(), CursorMode.ForceSoftware);
        currentLocation = startLocation;
        generalUI.SetActive(true);
        researchUI.SetActive(false);
        industryUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timeChecker += Time.deltaTime;
        CursorControls();
    }

    void FixedUpdate()
    {
        LocationUI();
        IndoorCheck();
        FuelUI();
        ExpUI();
    }

    private void IndoorCheck()
    {
        generalUI.GetComponent<Animator>().SetBool("Indoor", PlayerMovement.Instance.indoor);
        transform.Find("GeneralUI").Find("AlertSign").GetComponent<Animator>().SetBool("Indoor", PlayerMovement.Instance.indoor);
    }

    private void CursorControls()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            cursorLock = !cursorLock;
        }
        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerMovement.Instance.freeLockCamera.m_XAxis.m_MaxSpeed = 500;
            PlayerMovement.Instance.freeLockCamera.m_YAxis.m_MaxSpeed = 6;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PlayerMovement.Instance.idleTime = 0;
            PlayerMovement.Instance.freeLockCamera.m_XAxis.m_MaxSpeed = 0;
            PlayerMovement.Instance.freeLockCamera.m_YAxis.m_MaxSpeed = 0;
        }
    }

    private void LocationUI()
    {
        location.text = currentLocation;
    }

    private void FuelUI()
    {
        int currentFuel = Mathf.FloorToInt(PlayerMovement.Instance.currentFuel);
        if (currentFuel < 0)
            currentFuel = 0;
        fuel.text = currentFuel.ToString();
        fuelSlider.maxValue = PlayerMovement.Instance.maxFuel;
        fuelSlider.value = PlayerMovement.Instance.currentFuel;
    }

    private void ExpUI()
    {
        playerLevel = PlayerMovement.Instance.playerLevel;
        if (playerLevel < 4)
        {
            int exp = Mathf.FloorToInt(PlayerMovement.Instance.exp);
            if (exp < 0)
                exp = 0;
            this.exp.text = exp.ToString();
            expSlider.maxValue = PlayerMovement.Instance.maxExp;
            expSlider.value = PlayerMovement.Instance.exp;
        }
        else
        {
            this.exp.text = "MAX";
        }
        for (int i = 0; i < levelUI.Count; i++)
        {
            if (playerLevel - 1 == i)
                levelUI[i].SetActive(true);
            else
                levelUI[i].SetActive(false);
        }
    }

    public void ChangeUI(GameObject ui)
    {
        generalUI.SetActive(ui == generalUI);
        researchUI.SetActive(ui == researchUI);
        industryUI.SetActive(ui == industryUI);
    }
}
