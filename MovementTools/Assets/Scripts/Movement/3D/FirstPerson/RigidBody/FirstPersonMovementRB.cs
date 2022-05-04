using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonMovementRB : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float movementMultiplier = 6f;
    public float airMultiplier = 0.25f;
    bool groundCheck = false;

    Vector3 direction = Vector3.zero;
    Vector3 moveVel = Vector3.zero;
    Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        string objectTag = collision.gameObject.tag;

        if (objectTag == "Ground")
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

    /*!
    handles input to direction
    */
    private void OnMove(InputValue value)
    {
        direction = (value.Get<Vector2>().y * transform.forward) + (value.Get<Vector2>().x * transform.right);
    }

    /*!
    handles applying movement to player
    */
    private void Move()
    {
        if(direction.magnitude >= 0.1f)
        {
            if (groundCheck)
            {
                moveVel = direction * moveSpeed;
                moveVel.y = rb.velocity.y;
                rb.velocity = moveVel;
            }
            else
            {
                moveVel = rb.velocity + (direction * airMultiplier);
                moveVel.y = rb.velocity.y;
                rb.velocity = moveVel;
            }
        }
    }


}
