using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonMovementRB : MonoBehaviour
{
    //Variables
    //--------------------------------------
    public Transform cam;

    public float speed = 6f;
    public float airMultiplier = 0.25f;
    [SerializeField] float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    Rigidbody rb;
    bool groundCheck = false;
    Vector3 inputDirection = Vector3.zero;
    Vector3 moveDirection = Vector3.zero;
    Vector3 moveVel = Vector3.zero;
    //--------------------------------------
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (inputDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y; //calculates angle for player direction to move and face
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); //smoothes the angle transfer between current and target angle
            transform.rotation = Quaternion.Euler(0f, angle, 0f); //sets the rotation to the current angle

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward; //creates a movement direction off of the given angle
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        if (groundCheck) //on ground
        {
            moveVel = moveDirection * speed;
            moveVel.y = rb.velocity.y;
            rb.velocity = moveVel;
        }
        else //in air
        {
            moveVel = rb.velocity + (moveDirection * airMultiplier);
            moveVel.y = rb.velocity.y;
            rb.velocity = moveVel;
        }
    }

    private void OnMove(InputValue value)
    {
        inputDirection.x = value.Get<Vector2>().x;
        inputDirection.z = value.Get<Vector2>().y;
    }

    private void OnJump()
    {
        if (groundCheck)
        {
            rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
        }
    }

    private void OnCamera()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        string objectTag = collision.gameObject.tag;

        if(objectTag == "Ground")
        {
            groundCheck = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        string objectTag = collision.gameObject.tag;

        if (objectTag == "Ground")
        {
            groundCheck = false;
        }
    }
}
