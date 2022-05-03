using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovementCC : MonoBehaviour
{

    public CharacterController controller;
    public Transform cam;

    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    Vector3 direction = Vector3.zero;

    private void OnMove(InputValue value)
    {
        direction.x = value.Get<Vector2>().x;
        direction.z = value.Get<Vector2>().y;
    }

    // Update is called once per frame
    void Update()
    {

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y; //calculates angle for player direction to move and face
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); //smoothes the angle transfer between current and target angle
            transform.rotation = Quaternion.Euler(0f, angle, 0f); //sets the rotation to the current angle

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        
    }
}
