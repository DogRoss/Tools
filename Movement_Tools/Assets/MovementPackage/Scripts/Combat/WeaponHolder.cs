using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    PlayerController _player;
    [HideInInspector] public CameraPhysics _camPhys;
    public Weapon weapon;

    public LayerMask weaponMask;
    public float weaponCheckDistance = 5f;

    [Header("Recoil & Centering")]
    public float posCenteringForce = 50;
    public float rotCenteringForce = 50;
    public float snappiness = 50;

    //private variables
    Transform _camTransform;
    [HideInInspector] public Vector3 _velocity = Vector3.zero;
    Vector3 _lastPos = Vector3.zero;

    //position
    Vector3 _currentPosition;    
    Vector3 _targetPosition;
    [HideInInspector] public Vector3 _offsetPosition;

    //rotation
    Vector3 _currentRotation = Vector3.zero;
    Vector3 _targetRotation = Vector3.zero;
    [HideInInspector] public Vector3 _offsetRotation;

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

        _currentRotation = Vector3.Slerp(_currentRotation, _targetRotation, snappiness * Time.fixedDeltaTime);
        _targetRotation = Vector3.Lerp(_targetRotation, _offsetRotation, rotCenteringForce * Time.fixedDeltaTime);

        _currentPosition = Vector3.Lerp(_currentPosition, _targetPosition, snappiness * Time.fixedDeltaTime);
        _targetPosition = Vector3.Lerp(_targetPosition, _offsetPosition, posCenteringForce * Time.fixedDeltaTime);

        //apply
        transform.localRotation = Quaternion.Euler(_currentRotation);
        transform.localPosition = _currentPosition;
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
            weapon.DisableDynamics();
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
        weapon.transform.localPosition = new Vector3(-.5f, -1);
        weapon.transform.localRotation = Quaternion.identity;
        weapon._holder = this;

        _offsetPosition = weapon.offsetPosition;
        transform.localPosition = _offsetPosition;
        _offsetRotation = weapon.offsetRotation;
        transform.localRotation = Quaternion.Euler(_offsetRotation);

        weapon.StartDynamics();
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

    public void AddForce(Vector3 positionForce, Vector3 rotationForce)
    {
        _targetPosition += positionForce;
        _targetRotation += rotationForce;
    }
}
