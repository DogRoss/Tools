using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPhysics : MonoBehaviour
{
    public PlayerController player;
    SecondOrderDynamics movementDynamics;
    SecondOrderDynamics rotationDynamics;

    [Header("Centering")]
    public float centerForce = 10f;
    public float snappiness = 10f;
    public float randomForce = 50f;

    [Header("Second Order Dynamics")]
    public float movementFrequency = 2;
    public float movementDampingCoefficient = .8f;
    public float movementResponse = .5f;
    public float movementScale = 1f;

    public float rotationFrequency = 2;
    public float rotationDampingCoefficient = .8f;
    public float rotationResponse = .5f;
    public float rotationScale = 1f;


    Vector3 targetRecoilRotation = Vector3.zero;
    Vector3 currentRecoilRotation = Vector3.zero;
    Vector3 leaningVector = Vector3.zero;


    [HideInInspector] public Vector3 offsetPosition;
    [HideInInspector] public Vector3 offsetRotation;

    private void Start()
    {
        player = PlayerController.player;
        movementDynamics = new SecondOrderDynamics();
        movementDynamics.SecondOrderDynamicsFunction(movementFrequency, movementDampingCoefficient, movementResponse, transform.localPosition);

        rotationDynamics = new SecondOrderDynamics();
        rotationDynamics.SecondOrderDynamicsFunction(rotationFrequency, rotationDampingCoefficient, rotationResponse, Vector3.zero);
    }
    // Update is called once per frame
    void Update()
    {
        ApplyForce();
    }

    public void ApplyForce()
    {

        //MOVEMENT DYNAMICS
        //------------------------------------------------------------------------------------------------------------------------
        Vector3 movementInputVector = Vector3.zero;
        movementInputVector += transform.InverseTransformDirection(new Vector3(0, player.Controller.velocity.normalized.y, 0));

        movementInputVector += offsetPosition;
        movementInputVector = movementDynamics.UpdateDynamics(Time.deltaTime, movementInputVector);
        transform.localPosition = movementScale * movementInputVector;
        //------------------------------------------------------------------------------------------------------------------------

        //ROTATION DYNAMICS
        //------------------------------------------------------------------------------------------------------------------------
        Vector3 rotationInputVector = Vector3.zero;
        rotationInputVector += Vector3.Cross(-new Vector3(player.moveVec.x, 0, player.moveVec.z), Vector3.up);

        rotationInputVector += offsetRotation;
        rotationInputVector = rotationDynamics.UpdateDynamics(Time.deltaTime, rotationInputVector) * rotationScale;

        //RECOIL
        targetRecoilRotation = Vector3.Lerp(targetRecoilRotation, Vector3.zero, centerForce * Time.fixedDeltaTime);
        currentRecoilRotation = Vector3.Slerp(currentRecoilRotation, targetRecoilRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRecoilRotation + rotationInputVector);
        //------------------------------------------------------------------------------------------------------------------------
    }
    public void AddForce(float force, Vector3 axis)
    {
        targetRecoilRotation += axis * force;
    }

    [ContextMenu("Apply Random Force")]
    public void ApplyRandomForce()
    {
        AddForce(randomForce, Random.insideUnitSphere);
    }
}
