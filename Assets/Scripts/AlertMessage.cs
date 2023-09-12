using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AlertMessage : MonoBehaviour
{
    public bool alertSignal = false;
    bool test = false;
    public String message;
    public int times;
    int alertCount;
    float timeChecker;
    public float blinkFadeinTime = 2f;
    public float blinkStayTime = 4f;
    public float blinkFadeoutTime = 2f;
    Image alertImage;
    TMP_Text alertMessage;
    [SerializeField] Image warningIcon1;
    [SerializeField] Image warningIcon2;

    // Start is called before the first frame update
    void Start()
    {
        alertImage = transform.GetComponent<Image>();
        alertMessage = transform.GetChild(0).GetComponent<TMP_Text>();
        VisibleSettings();
    }

    // Update is called once per frame
    void Update()
    {
        if (alertSignal)
        {
            Alert(message, times);
        }
    }

    private void VisibleSettings()
    {
        var imageTempColor = alertImage.color;
        var messageTempColor = alertMessage.color;

        imageTempColor.a = 0;
        messageTempColor.a = 0;

        alertImage.color = imageTempColor;
        alertMessage.color = messageTempColor;

        if (warningIcon1 != null && warningIcon2 != null)
        {
            warningIcon1.color = imageTempColor;
            warningIcon2.color = imageTempColor;
        }
    }

    public void Alert(String message, int times)
    {
        SetMessage(message);
        Blink();
    }

    private void SetMessage(string message)
    {
        alertMessage.text = message;
    }

    private void Blink()
    {
        timeChecker += Time.deltaTime;

        var imageTempColor = alertImage.color;
        var messageTempColor = alertMessage.color;

        if (timeChecker < blinkFadeinTime)
        {
            imageTempColor.a = timeChecker / blinkFadeinTime;
            messageTempColor.a = timeChecker / blinkFadeinTime;
        }
        else if (timeChecker < blinkFadeinTime + blinkStayTime + blinkFadeoutTime)
        {
            imageTempColor.a = (blinkFadeinTime + blinkStayTime + blinkFadeoutTime - timeChecker) / blinkFadeoutTime;
            messageTempColor.a = (blinkFadeinTime + blinkStayTime + blinkFadeoutTime - timeChecker) / blinkFadeoutTime;
            if ((blinkFadeinTime + blinkStayTime + blinkFadeoutTime - timeChecker) / blinkFadeoutTime < 0.05f)
            {
                imageTempColor.a = 0;
                messageTempColor.a = 0;
            }
        }
        else
        {
            alertCount++;
            timeChecker = 0;
            if (alertCount == times)
            {
                alertCount = 0;
                alertSignal = false;
            }
        }
        alertImage.color = imageTempColor;
        alertMessage.color = messageTempColor;

        if (warningIcon1 != null && warningIcon2 != null)
        {
            warningIcon1.color = imageTempColor;
            warningIcon2.color = imageTempColor;
        }
    }
}
