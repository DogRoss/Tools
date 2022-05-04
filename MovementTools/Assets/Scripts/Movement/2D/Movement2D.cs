using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement2D : MonoBehaviour
{

    //Variables
    //----------------------------------------------
    //speed variables
    [SerializeField] float speed = 10f;
    [SerializeField] float maxSpeed = 50f;
    [SerializeField] float dragCoefficient = 10f;

    //collision and velocity
    Rigidbody2D rb;
    public Vector2 direction = Vector2.zero;
    Vector2 storedVelocity = Vector2.zero;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float gravityMultiplier = 10f;
    
    //----------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
        HandleDrag();
        Move();
    }

    private void OnMove(InputValue value)
    {
        direction.x = value.Get<Vector2>().x;
    }

    private void OnJump(InputValue value)
    {
        if(value.Get<float>() > 0)
        {
            rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
        }
        else if(value.Get<float>() == 0)
        {
        }
    }

    private void Move()
    {
        storedVelocity = rb.velocity + (direction * speed);
        storedVelocity.x = Mathf.Clamp(storedVelocity.x, -maxSpeed, maxSpeed);
        storedVelocity.y = Mathf.Clamp(storedVelocity.y, -maxSpeed, maxSpeed);
        rb.velocity = storedVelocity;
    }

    private void HandleDrag()
    {
        rb.AddForce(Vector2.right * -(rb.velocity.x * dragCoefficient));

        rb.velocity += Vector2.up * Physics2D.gravity.y * (Time.deltaTime / gravityMultiplier);
    }
}
