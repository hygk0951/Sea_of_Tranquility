using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdlePose
{
    private String idlePose;
    private float poseNumber;
    private float poseTime;

    public IdlePose(String idlePose, float poseNumber, float poseTime)
    {
        this.idlePose = idlePose;
        this.poseNumber = poseNumber;
        this.poseTime = poseTime;
    }

    public String IdlePoseName
    {
        get
        {
            return idlePose;
        }
    }

    public float PoseNumber
    {
        get
        {
            return poseNumber;
        }
    }

    public float PoseTime
    {
        get
        {
            return poseTime;
        }
    }
}