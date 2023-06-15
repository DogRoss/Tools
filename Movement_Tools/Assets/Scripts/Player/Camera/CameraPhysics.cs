using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPhysics : MonoBehaviour
{
    public PlayerController player;
    SecondOrderDynamics dynamics;

    public float centerForce = 10f;
    public float snappiness = 10f;
    public float randomForce = 50f;

    Vector3 targetRecoilRotation = Vector3.zero;
    Vector3 currentRecoilRotation = Vector3.zero;

    public float frequency = 2;
    public float dampingCoefficient = .8f;
    public float response = .5f;
    public float scale = 1f;

    private void Start()
    {
        player = PlayerController.player;
        dynamics = new SecondOrderDynamics();
        dynamics.SecondOrderDynamicsFunction(frequency, dampingCoefficient, response, transform.localPosition);
    }
    // Update is called once per frame
    void Update()
    {
        ApplyForce();
    }

    public void ApplyForce()
    {
        Vector3 yeah = dynamics.UpdateDynamics(Time.deltaTime, player.Controller.velocity.normalized);
        yeah.x = 0; yeah.z = 0;
        transform.localPosition = transform.InverseTransformDirection(yeah) * scale;

        AddForce(player.direction.magnitude * scale, Vector3.Cross(-player.direction, Vector3.up));

        targetRecoilRotation = Vector3.Lerp(targetRecoilRotation, Vector3.zero, centerForce * Time.fixedDeltaTime);
        currentRecoilRotation = Vector3.Slerp(currentRecoilRotation, targetRecoilRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRecoilRotation);
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
