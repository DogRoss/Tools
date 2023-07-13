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

    [Header("Projectile Values")]
    public LineRenderer projectileTrail;
    public ShootingType type;
    [Tooltip("Rounds Per Minute, projectiles able to be fired within a minute")]
    public float rpm;
    [Tooltip("Feet Per Second, this is the initial velocity when the projectile leaves the weapon")]
    public float fps;
    public float damage;
    [Tooltip("weight in grains")]
    public float weight;
    public int currentMag;
    public int magazineSize;
    public int ammo;

    
    bool _ads = false;
    bool _cooldown = false;
    bool _firingProjectile = false;
    bool _autoCoroutine = false;
    float _originalRotationScale;
    float _originalAngleScale;
    float _originalPositionScale;
    float _timeBetweenProjectiles;
    float _calculatedWeight;

    public override void Start()
    {
        base.Start();
        RangedWeaponStart();
    }
    public override void MainAbility(bool enable)
    {
        _mainAbilityEnabled = enable;
        if (enable && !_cooldown)
        {
            _firingProjectile = true;
            switch (type)
            {
                case ShootingType.Semi:
                    Shoot();
                    StartCoroutine(Cooldown());
                    break;
                case ShootingType.Auto:
                    _firingProjectile = true;
                    if(!_autoCoroutine)
                        StartCoroutine(AutoShoot());
                    break;
            }
        }
    }
    public override void SecondaryAbility(bool enable)
    {
        _secondaryAbilityEnabled = enable;
        AimDownSights(enable);
    }

    public virtual void RangedWeaponStart()
    {
        _originalRotationScale = rotationScale;
        _originalAngleScale = angleScale;
        _originalPositionScale = movementScale;
        _timeBetweenProjectiles = 1 / (rpm / 60);
        _calculatedWeight = weight / 7000;
    }
    public virtual void AimDownSights(bool enable)
    {
        _ads = enable;
        if (_ads)
        {
            _holder._offsetPosition = aimingPosition;

            rotationScale *= .5f;
            angleScale *= .5f;
            movementScale *= .5f;
        }   
        else
        {
            _holder._offsetPosition = offsetPosition;

            rotationScale = _originalRotationScale;
            angleScale = _originalAngleScale;
            movementScale = _originalPositionScale;
        }
    }

    [ContextMenu("Reload")]
    public virtual void Reload()
    {
        if (_cooldown || _firingProjectile)
            return;

        int difference = magazineSize - currentMag;
        if(difference > ammo)
        {
            currentMag += ammo;
            ammo = 0;
        }
        else
        {
            currentMag = magazineSize;
            ammo -= difference;
        }
    }
    public virtual void Shoot()
    {
        if (currentMag <= 0)
            return;
        currentMag--;

        //raycast for fps distance
        Ray ray = new Ray();
        ray.origin = firePoint.position;
        ray.direction = firePoint.forward;
        if (Physics.Raycast(ray, out RaycastHit hit, fps))
        {
            GameObject h = hit.collider.gameObject;

            //Projectile Collision Interactions
            if (h.TryGetComponent(out IDamageable dmg))
                dmg.Damage(damage);
            if (h.TryGetComponent(out Rigidbody rb) && !rb.isKinematic)
                rb.AddForceAtPosition(ray.direction * (_calculatedWeight * fps), hit.point, ForceMode.Impulse);

            //spawn bullet trail
            //TODO: pool bullet trail
            GameObject g = Instantiate(projectileTrail.gameObject, firePoint.position, Quaternion.identity);
            ProjectileTrail pt = g.GetComponent<ProjectileTrail>();
            pt.SetUpTrail(firePoint.position, hit.point);
            Destroy(g, 3);
        }
        else
        {
            //spawn bullet trail
            //TODO: pool bullet trail
            GameObject g = Instantiate(projectileTrail.gameObject, firePoint.position, Quaternion.identity);
            ProjectileTrail pt = g.GetComponent<ProjectileTrail>();
            pt.SetUpTrail(firePoint.position, firePoint.position + (firePoint.forward * fps));
            Destroy(g, 3);
        }


        //handle recoil
        Vector3 posForce = positionRecoil;
        posForce.x = Random.Range(-posForce.x, posForce.x);
        Vector3 rotForce = Vector3.zero;
        rotForce.x -= Random.Range(0.75f, 1.25f) * rotationRecoil.x;
        rotForce.y += Random.Range(-rotationRecoil.y, rotationRecoil.y);
        rotForce.z += Random.Range(-rotationRecoil.z, rotationRecoil.z);

        _holder._camPhys.AddForce(rotForce.magnitude * cameraRecoilCoefficient, rotForce.normalized);
        _holder.AddForce(posForce, rotForce);
    }

    public virtual IEnumerator AutoShoot()
    {
        _autoCoroutine = true;
        Vector3 originalRecoil = rotationRecoil;
        Vector3 []shots = new Vector3[5];
        shots[0] = rotationRecoil / 5;
        shots[1] = shots[0] * 2;
        shots[2] = rotationRecoil / 2;
        shots[3] = shots[2] + shots[0];
        shots[4] = rotationRecoil;

        int current = 0;

        //climb really high after 3 shots
        while (currentMag > 0 && _mainAbilityEnabled)
        {
            Shoot();
            if(current < 4)
            {
                current++;
                rotationRecoil = shots[current];
            }
            yield return new WaitForSeconds(_timeBetweenProjectiles);
        }

        rotationRecoil = originalRecoil;
        _firingProjectile = false;
        _autoCoroutine = false;
        yield return null;
    }
    public virtual IEnumerator Cooldown()
    {
        _cooldown = true;
        yield return new WaitForSeconds(_timeBetweenProjectiles);
        _cooldown = false;


    }
}

public enum ShootingType
{
    Semi,
    Auto
}