using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
    [Header("Ranged Weapon Values")]
    public Vector3 aimingPos;
    bool ads = false;

    float originalRotationScale;
    float originalAngleScale;
    float originalPositionScale;

    public override void Start()
    {
        base.Start();
        RangedWeaponStart();
    }
    public override void MainAbility(bool enable)
    {
        if(enable)
            Recoil();
    }
    public override void SecondaryAbility(bool enable)
    {
        AimDownSights(enable);
    }

    public virtual void RangedWeaponStart()
    {
        originalRotationScale = rotationScale;
        originalAngleScale = angleScale;
        originalPositionScale = movementScale;
    }
    public virtual void AimDownSights(bool enable)
    {
        ads = enable;
        if (ads)
        {
            _holder._offsetPosition = aimingPos;

            rotationScale *= .5f;
            angleScale *= .5f;
            movementScale *= .5f;

        }
        else
        {
            _holder._offsetPosition = offsetPosition;

            rotationScale = originalRotationScale;
            angleScale = originalAngleScale;
            movementScale = originalPositionScale;
        }
    }

    //Shoot
    public virtual void Recoil()
    {
        Vector3 posForce = (Vector3.right * .6f) + (Vector3.up * .4f) + (Vector3.forward * -.5f);
        Vector3 rotForce = Vector3.zero;
        rotForce.x -= 60;
        rotForce.y += 40;
        rotForce.z -= 50;

        _holder.AddForce(posForce, rotForce);
    }
}
