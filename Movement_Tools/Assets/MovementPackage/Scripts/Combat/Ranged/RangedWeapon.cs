using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
    [Header("Ranged Weapon Values")]
    public Transform firePoint;
    public Vector3 aimingPosition;
    public Vector3 positionRecoil;
    public Vector3 rotationRecoil;
    public float cameraRecoilCoefficient = .5f;
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
            Shoot();
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
            _holder._offsetPosition = aimingPosition;

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

    public virtual void Shoot()
    {
        //raycast for fps distance


        //handle recoil
        Vector3 posForce = positionRecoil;
        posForce.x = Random.Range(-posForce.x, posForce.x);
        Vector3 rotForce = Vector3.zero;
        rotForce.x -= rotationRecoil.x;
        rotForce.y += Random.Range(-rotationRecoil.y, rotationRecoil.y);
        rotForce.z += Random.Range(-rotationRecoil.z, rotationRecoil.z);

        _holder._camPhys.AddForce(rotForce.magnitude * cameraRecoilCoefficient, rotForce.normalized);
        _holder.AddForce(posForce, rotForce);
    }
}
