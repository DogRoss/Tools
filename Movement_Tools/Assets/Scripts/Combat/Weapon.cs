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

    public float scale = 1;

    [Header("Position Dynamics and Offset")]
    public bool useRotationDynamics = true;
    public float rotationFrequency = 2;
    public float rotationDampingCoefficient = .8f;
    public float rotationResponse = .5f;

    public Vector3 startingOffsetRotation = Vector3.zero;

    [Header("Position Dynamics and Offset")]
    public bool usePositionDynamics = true;
    public float movementFrequency = 2;
    public float movementDampingCoefficient = .8f;
    public float movementResponse = .5f;

    public Vector3 startingOffsetPosition = Vector3.zero;

    //private variables
    Vector3 _movementInputVector = Vector3.zero;
    Vector3 _rotationInputVector = Vector3.zero;

    public virtual void Start()
    {
        _rb = GetComponent<Rigidbody>();

        if (transform.parent == null)
            EnableRigidbodyPhysics(true);
        else
            EnableRigidbodyPhysics(false);

        _player = PlayerController.player;

        _movementDynamics = new SecondOrderDynamics();
        _movementDynamics.SecondOrderDynamicsFunction(movementFrequency, movementDampingCoefficient, movementResponse, transform.localPosition);
        _rotationDynamics = new SecondOrderDynamics();
        _rotationDynamics.SecondOrderDynamicsFunction(rotationFrequency, rotationDampingCoefficient, rotationResponse, Vector3.zero);
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
    public virtual void UpdatePositionDynamics()
    {
        //TODO: replace zero vector with offset positional vector
        _movementInputVector = Vector3.zero;
        _movementInputVector += transform.InverseTransformDirection(scale * _holder._velocity);
        _movementInputVector = _movementDynamics.UpdateDynamics(Time.fixedDeltaTime, _movementInputVector);
        transform.localPosition = _movementInputVector;
    }
    public virtual void UpdateRotationDynamics()
    {
        _rotationInputVector = Vector3.zero;
        //_rotationInputVector += transform.InverseTransformDirection(Vector3.Cross(_holder._velocity, Vector3.up));
        _rotationInputVector += transform.InverseTransformDirection(Vector3.Cross(-_holder._velocity, Vector3.up));
        //_rotationInputVector += transform.InverseTransformDirection(Vector3.Cross(-new Vector3(_player.moveVec.x, 0, _player.moveVec.z), Vector3.up));
        _rotationInputVector = _rotationDynamics.UpdateDynamics(Time.fixedDeltaTime, _rotationInputVector);
        transform.localRotation = Quaternion.Euler(_rotationInputVector);
    }

    public virtual void EnableRigidbodyPhysics(bool enable)
    {
        //if true, its a ground object, if not, its meant to be held
        if (enable)
        {
            _rb.isKinematic = false;
            transform.parent = null;
            if (_holder && _holder.weapon == this)
                _holder.weapon = null;

            foreach(Collider c in GetComponents<Collider>())
            {
                c.enabled = true;
            }
        }
        else
        {
            _rb.isKinematic = true;
            foreach (Collider c in GetComponents<Collider>())
            {
                c.enabled = false;
            }
        }
    }

    public virtual void MainAbility(bool enable)
    {

    }

    public virtual void SecondaryAbility(bool enable)
    {

    }

    public virtual void SpecialAbility(bool enable)
    {

    }
}
