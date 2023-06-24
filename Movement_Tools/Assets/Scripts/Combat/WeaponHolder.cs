using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    PlayerController _player;
    public Weapon weapon;

    public LayerMask weaponMask;
    public float weaponCheckDistance = 5f;

    //private variables
    Transform _camTransform;
    public Vector3 _velocity = Vector3.zero;
    Vector3 _lastPos = Vector3.zero;

    public void Start()
    {
        _player = PlayerController.player;
        _camTransform = _player.Cam.transform;

        if (weapon == null)
            weapon = GetComponentInChildren<Weapon>();

        SetUpWeapon(weapon);
    }
    private void FixedUpdate()
    {
        _velocity = transform.position - _lastPos;
        _lastPos = transform.position;
    }

    public void PickupWeapon()
    {
        Weapon w = null;
        if(Physics.Raycast(_camTransform.position, _camTransform.forward, out RaycastHit hit, weaponCheckDistance, weaponMask))
            w = hit.transform.GetComponent<Weapon>();

        if (w)
            SetUpWeapon(w);
    }
    public void DropWeapon()
    {
        if(weapon != null)
        {
            weapon.EnableRigidbodyPhysics(true);
            //add force here
            weapon = null;
        }
    }

    public void SetUpWeapon(Weapon _weapon)
    {
        if (!_weapon)
            return;

        if (weapon != null)
            weapon.EnableRigidbodyPhysics(true);

        weapon = _weapon;
        weapon.EnableRigidbodyPhysics(false);
        weapon.transform.parent = transform;
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon._holder = this;

        transform.localPosition = weapon.startingOffsetPosition;
        transform.localRotation = Quaternion.Euler(weapon.startingOffsetRotation);
    }

    public void MainInteraction(bool enable)
    {
        if (weapon == null)
            return;

        weapon.MainAbility(enable);
    }
    public void SecondaryInteraction(bool enable)
    {
        if (weapon == null)
            return;

        weapon.SecondaryAbility(enable);
    }
}
