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

    [Header("Bullet Values")]
    public LineRenderer projectileTrail;
    public float rpm;
    public float fps;

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
        Ray ray = new Ray();
        ray.origin = firePoint.position;
        ray.direction = firePoint.forward;
        if(Physics.Raycast(ray, out RaycastHit hit, fps))
        {
            GameObject g = Instantiate(projectileTrail.gameObject, firePoint.position, Quaternion.identity);
            ProjectileTrail pt = g.GetComponent<ProjectileTrail>();
            pt.SetUpTrail(firePoint.position, hit.point);
        }
        else
        {
            GameObject g = Instantiate(projectileTrail.gameObject, firePoint.position, Quaternion.identity);
            ProjectileTrail pt = g.GetComponent<ProjectileTrail>();
            pt.SetUpTrail(firePoint.position, firePoint.position + (firePoint.forward * fps));
        }


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
