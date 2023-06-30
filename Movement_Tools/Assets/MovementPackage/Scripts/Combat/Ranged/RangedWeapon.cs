using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
    [Header("Ranged Weapon Values")]
    public Vector3 aimingPos;
    Vector3 startingPos;
    bool ads = false;

    public override void Start()
    {
        base.Start();

        startingPos = offsetPosition;
    }

    public void AimDownSights(bool enable)
    {
        ads = enable;
        if (ads)
        {
            _holder.transform.localPosition = aimingPos;
        }
        else
        {
            _holder.transform.localPosition = startingPos;
        }
    }
}
