using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraPhysics))]
public class LeanCamera : MonoBehaviour
{
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;

    Vector3 leftLeanPos;
    Vector3 leftLeanRot;
    CameraPhysics camPhys;

    bool rightLean = false;
    bool leftLean = false;

    private void Start()
    {
        camPhys = GetComponent<CameraPhysics>();
        leftLeanPos = positionOffset;
        leftLeanPos.x *= -1;
        leftLeanRot = rotationOffset;
        leftLeanRot.z *= -1;
    }

    public void LeanRight()
    {
        rightLean = true;
        camPhys.offsetPosition = positionOffset;
        camPhys.offsetRotation = rotationOffset;
    }
    public void LeanLeft()
    {
        leftLean = true;
        camPhys.offsetPosition = leftLeanPos;
        camPhys.offsetRotation = leftLeanRot;
    }

    public void ExitLeftLean()
    {
        leftLean = false;

        if (rightLean)
        {
            camPhys.offsetPosition = positionOffset;
            camPhys.offsetRotation = rotationOffset;
        }
        else
        {
            camPhys.offsetPosition = Vector3.zero;
            camPhys.offsetRotation = Vector3.zero;
        }
    }
    public void ExitRightLean()
    {
        rightLean = false;

        if (leftLean)
        {
            camPhys.offsetPosition = leftLeanPos;
            camPhys.offsetRotation = leftLeanRot;
        }
        else
        {
            camPhys.offsetPosition = Vector3.zero;
            camPhys.offsetRotation = Vector3.zero;
        }
    }
}
