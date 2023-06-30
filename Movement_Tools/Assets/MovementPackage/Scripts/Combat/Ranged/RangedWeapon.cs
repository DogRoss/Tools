using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
    [Header("Ranged Weapon Values")]
    public Vector3 aimingPos;
    bool ads = false;

    public override void MainAbility(bool enable)
    {
        Recoil();
    }
    public override void SecondaryAbility(bool enable)
    {
        AimDownSights(enable);
    }

    public void AimDownSights(bool enable)
    {
        ads = enable;
        if (ads)
        {
            _holder._offsetPosition = aimingPos;
        }
        else
        {
            _holder._offsetPosition = offsetPosition;
        }
    }

    //Shoot
    private void Recoil()
    {

    }
}
