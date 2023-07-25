using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Weapon : MonoBehaviour
{
    PlayerController _player;
    Rigidbody _rb;
    SecondOrderDynamics _movementDynamics;
    SecondOrderDynamics _rotationDynamics;
    [HideInInspector] public WeaponHolder _holder;


    [Header("Rotation Dynamics and Offset")]
    public bool useRotationDynamics = true;
    public float rotationFrequency = 2;
    public float rotationDampingCoefficient = .8f;
    public float rotationResponse = .5f;
    public Vector3 rotationScale = Vector3.one;
    public Vector3 angleScale = Vector3.one;

    public Vector3 offsetRotation = Vector3.zero;

    [Header("Position Dynamics and Offset")]
    public bool usePositionDynamics = true;
    public float movementFrequency = 2;
    public float movementDampingCoefficient = .8f;
    public float movementResponse = .5f;
    public Vector3 movementScale = Vector3.one;

    public Vector3 offsetPosition = Vector3.zero;

    //private variables
    Vector3 _movementInputVector = Vector3.zero;
    Vector3 _rotationInputVector = Vector3.zero;
    protected bool _mainAbilityEnabled;
    protected bool _secondaryAbilityEnabled;
    public virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (transform.parent == null)
            EnableRigidbodyPhysics(true);
        else
            EnableRigidbodyPhysics(false);
    }
    public virtual void Start()
    {
        _player = PlayerController.player;
    }
    public virtual void FixedUpdate()
    {
        if (_rb.isKinematic)
        {
            if (usePositionDynamics)
                UpdatePositionDynamics();
            if (useRotationDynamics)
                UpdateRotationDynamics();
        }
    }
    public void DisableDynamics()
    {
        _movementDynamics = null;
        _rotationDynamics = null;
    }
    public void StartDynamics()
    {
        _movementDynamics = new SecondOrderDynamics();
        _movementDynamics.SecondOrderDynamicsFunction(movementFrequency, movementDampingCoefficient, movementResponse, transform.localPosition);
        _rotationDynamics = new SecondOrderDynamics();
        _rotationDynamics.SecondOrderDynamicsFunction(rotationFrequency, rotationDampingCoefficient, rotationResponse, transform.localRotation.eulerAngles);
    }
    public virtual void UpdatePositionDynamics()
    {
        //TODO: replace zero vector with offset positional vector
        //_movementInputVector = transform.InverseTransformDirection(movementScale * _holder._velocity) + _holder.transform.localPosition;
        _movementInputVector = CalculateMovementScale() + _holder.transform.localPosition;
        _movementInputVector = _movementDynamics.UpdateDynamics(Time.fixedDeltaTime, _movementInputVector);
        transform.localPosition = _movementInputVector;
    }
    public virtual void UpdateRotationDynamics()
    {
        _rotationInputVector = _holder.transform.localEulerAngles;
        if (_rotationInputVector.x > 180)
            _rotationInputVector.x -= 360;
        if (_rotationInputVector.y > 180)
            _rotationInputVector.y -= 360;
        if (_rotationInputVector.z > 180)
            _rotationInputVector.z -= 360;
        _holder.transform.localEulerAngles = _rotationInputVector;

        //_rotationInputVector += Vector3.Cross(rotationScale * transform.InverseTransformDirection(_holder._velocity), _holder.transform.up);
        _rotationInputVector += CalculateRotationScale();
        //_rotationInputVector += angleScale * transform.InverseTransformDirection(_holder.angularVelocity);
        _rotationInputVector += CalculateAngleScale();
        _rotationInputVector = _rotationDynamics.UpdateDynamics(Time.fixedDeltaTime, _rotationInputVector);

        transform.localRotation = Quaternion.Euler(_rotationInputVector);
    }

    public virtual void EnableRigidbodyPhysics(bool enable)
    {
        //if true, its a ground object, if not, its meant to be held
        if (enable)
        {
            StopAllCoroutines();

            _rb.isKinematic = false;
            transform.parent = null;
            if (_holder && _holder.weapon == this)
                _holder.weapon = null;

            foreach(Collider c in GetComponents<Collider>())
                c.enabled = true;
        }
        else
        {
            _rb.isKinematic = true;
            foreach (Collider c in GetComponents<Collider>())
                c.enabled = false;
        }
    }

    public virtual void MainAbility(bool enable)
    {
        _mainAbilityEnabled = enable;
    }

    public virtual void SecondaryAbility(bool enable)
    {
        _secondaryAbilityEnabled = enable;
    }

    public virtual void SpecialAbility(bool enable)
    {

    }

    public Vector3 CalculateMovementScale()
    {
        //Vector3 scaledVel = transform.InverseTransformDirection(_holder._velocity);
        Vector3 scaledVel = _holder._velocity;
        scaledVel.x *= movementScale.x;
        scaledVel.y *= movementScale.y;
        scaledVel.z *= movementScale.z;
        return transform.InverseTransformDirection(scaledVel);
    }
    public Vector3 CalculateRotationScale()
    {
        Vector3 scaledVel = transform.InverseTransformDirection(_holder._velocity);
        scaledVel.x *= rotationScale.x;
        scaledVel.y *= rotationScale.y;
        scaledVel.z *= rotationScale.z;
        return Vector3.Cross(scaledVel, _holder.transform.up);
    }
    public Vector3 CalculateAngleScale()
    {
        Vector3 scaledVel = transform.InverseTransformDirection(_holder.angularVelocity);
        scaledVel.x *= angleScale.x;
        scaledVel.y *= angleScale.y;
        scaledVel.z *= angleScale.z;
        return scaledVel;
    }
}
