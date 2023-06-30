using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    PlayerController _player;
    CameraPhysics _camPhys;
    public Weapon weapon;

    public LayerMask weaponMask;
    public float weaponCheckDistance = 5f;

    //private variables
    Transform _camTransform;
    [HideInInspector] public Vector3 _velocity = Vector3.zero;
    Vector3 _lastPos = Vector3.zero;

    [HideInInspector] public Vector3 angularVelocity;
    Quaternion _lastRotation;
    Quaternion _deltaRot;
    Vector3 _eulerRot;

    public void Start()
    {
        _player = PlayerController.player;
        _camTransform = _player.Cam.transform;
        _camPhys = GetComponentInParent<CameraPhysics>();

        if (weapon == null)
            weapon = GetComponentInChildren<Weapon>();

        SetUpWeapon(weapon);
    }
    private void FixedUpdate()
    {
        _velocity = transform.position - _lastPos;
        _lastPos = transform.position;

        _deltaRot = transform.rotation * Quaternion.Inverse(_lastRotation);
        _eulerRot = new Vector3(Mathf.DeltaAngle(0, _deltaRot.eulerAngles.x), Mathf.DeltaAngle(0, _deltaRot.eulerAngles.y), Mathf.DeltaAngle(0, _deltaRot.eulerAngles.z));

        angularVelocity = _eulerRot / Time.fixedDeltaTime;
        _lastRotation = transform.rotation;
    }

    //Checks to see if item equipped or not, if equipped, drop, if not, try and equip
    public void ContextEquipWeapon()
    {
        if(weapon == null)
            PickupWeapon();
        else
            DropWeapon();
    }
    public void PickupWeapon()
    {
        Weapon w = null;
        if(Physics.SphereCast(_camTransform.position, 2f, _camTransform.forward, out RaycastHit hit, weaponCheckDistance, weaponMask))
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
        weapon.transform.parent = transform.parent;
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon._holder = this;

        transform.localPosition = weapon.offsetPosition;
        transform.localRotation = Quaternion.Euler(weapon.offsetRotation);
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
