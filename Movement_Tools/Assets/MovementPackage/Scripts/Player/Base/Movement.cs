using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary>
/// FPS movement character controller 3D.
/// simulates rigidbody-like behaviour with adding and transferring of forces
/// there is no set max speed for anything, drag and other counteracting forces calculate a max speed at runtime based off multiple factors(environment, stat, player)
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class Movement : MonoBehaviour
{
    public PlayerJump jumpEvent;
    public PlayerCrouch crouchEvent;
    public PlayerSprint sprintEvent;
        
    public bool input = true;

    protected CharacterController controller;
    protected GameObject cam;
    protected bool movementEnabled = true;

    //----------------------------------------------------------------------

    #region Inspector Values
    [Tooltip("Weight of player character (in kilograms)")]
    public float playerMass = 70f;
    [Tooltip("Rate at which Player gains speed.")]
    public float acceleration = 1f;
    [Tooltip("acceleration rate of DownForce the player will experience when in the air. Coefficient to gravity, if at 1: base gravity is applied.")]
    public float gravity = 0.5f;
    public LayerMask groundMask;



    //[HideInInspector]
    public float currentFrictionCoefficient;
    [HideInInspector]
    public bool restrictFrictionCoefficient = false;
    [HideInInspector]
    public float currentAccelCoefficient;
    [HideInInspector]
    public bool restrictAccelCoefficient = false;

    //----------------------------------------------------------------------

    [Header("Ground Movement Values")]
    [Tooltip("multiplied by 'Acceleration' to get the acceleration of Player when touching the ground.")]
    public float groundAccelerationCoefficient = 1f;
    [Tooltip("Coefficient of forces that act against the forces imposed by Player movement on the ground.")]
    [Range(0, 1)]
    public float groundFrictionCoefficient = 0.1f;



    [HideInInspector]
    public float groundMovementMultiplier = 1f;
    [HideInInspector]
    public bool restrictGroundMovement;

    //----------------------------------------------------------------------

    [Header("Air Movement Values")]
    [Tooltip("multiplied by 'Acceleration' to get the acceleration of Player when in the air.")]
    public float airAccelerationCoefficient = 1f;
    [Tooltip("how much air drag affects the forces imposed by gravity.")]
    [Range(0, 1)]
    public float verticalDragCoefficient = .01f;
    [Tooltip("how much air drag affects the forces imposed by Player movement.")]
    [Range(0, 1)]
    public float horizontalDragCoefficient = .03f;



    [HideInInspector]
    public bool restrictAirMovement;

    //----------------------------------------------------------------------

    [Header("Camera Values")]
    public float mouseAcceleration = 1;
    public float sensitivity = 0.5f;
    public float maxLookAngle = 90;
    public float minLookAngle = -90;
    private float clampX = 0f;
    [SerializeField] private float inputLagPeriod;
    private Vector2 velocity = Vector2.zero;
    private Vector2 rotation = Vector2.zero;

    private Vector2 lastInputEvent;
    private float inputLagTimer;

    //----------------------------------------------------------------------

    //Raycast/Ground Check
    private Ray groundRay = new Ray();
    private RaycastHit groundHit = new RaycastHit();
    private float groundRayDistance = 1f;
    public bool grounded;

    //----------------------------------------------------------------------

    //Input
    [HideInInspector]
    public Vector3 direction = Vector3.zero;
    public Vector3 mouseVector = Vector3.zero;
    [HideInInspector]
    public Vector3 moveVec = Vector3.zero;
    [HideInInspector]
    public Vector3 playerInputDirec = Vector3.zero;
    [HideInInspector]
    public float originalControllerHeight;
    #endregion

    //----------------------------------------------------------------------

    #region Built In Engine Functions
    public virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = GameObject.FindGameObjectWithTag("PlayerCamera");

        if(cam == null)
        {
            cam = GetComponentInChildren<Camera>().gameObject;
        }

        //Ray Setup
        groundRay.direction = Vector3.down;
        originalControllerHeight = controller.height;
    }
    public virtual void Update()
    {
        HandleCamera();
    }
    public virtual void FixedUpdate()
    {
        if (movementEnabled)
            CalculateMovement();
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Handle colliding off objects and the friction that happens between em
        float newMag = moveVec.magnitude;
        float dot = Vector3.Dot(moveVec.normalized, hit.normal);
        if (dot < 0)
        {
            newMag *= Mathf.Abs(dot);
            moveVec += hit.normal * newMag;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + (groundRay.direction * groundRayDistance));
    }
    #endregion

    #region Input System
    private void OnMove(InputValue value)
    {
        direction = value.Get<Vector3>().normalized;
    }
    private void OnJump(InputValue value)
    {
        if (!input || jumpEvent == null)
            return;

        if (value.Get<float>() > 0)
        {
            jumpEvent.Jump(true);
        }
    }
    private void OnMouseChange(InputValue value)
    {
        if (!input)
            return;

        inputLagTimer += Time.deltaTime;

        mouseVector.x = -value.Get<Vector2>().y;
        mouseVector.y = value.Get<Vector2>().x;
    }

    private void OnCrouch(InputValue value)
    {
        if (!input || crouchEvent == null)
            return;

        if (value.Get<float>() > 0)
            crouchEvent.Crouch(true);
        else
            crouchEvent.Crouch(false);
    }

    private void OnSprint(InputValue value)
    {
        if (!input || sprintEvent == null)
            return;

        if (value.Get<float>() > 0)
            sprintEvent.Sprint(true);
        else
            sprintEvent.Sprint(false);

    }
    #endregion

    #region Getter/Setter Functions

    public float CurrentSpeed
    {
        get
        {
            return controller.velocity.magnitude;
        }
    }
    public float CurrentHorizontalSpeed
    {
        get
        {
            Vector2 vec = new Vector2(controller.velocity.x, controller.velocity.z);
            return vec.magnitude;
        }
    }
    public Vector3 Velocity
    {
        get
        {
            return moveVec;
        }
        set
        {
            moveVec = value;
        }
    }
    public Vector3 LocalVelocity
    {
        get
        {
            return transform.InverseTransformDirection(moveVec);
        }
    }
    public GameObject Cam
    {
        get
        {
            return cam;
        }
    }
    public CharacterController Controller
    {
        get
        {
            return controller;
        }
    }
    public bool MoveInputRead
    {
        get
        {
            return direction.magnitude > 0 ? true : false;
        }
    }
    #endregion

    #region Player Functions
    private void CheckGround()
    {
        groundRayDistance = controller.height * .5f;
        groundRay.origin = controller.transform.position;
        //if (Physics.Raycast(groundRay, out groundHit, groundRayDistance, groundMask.value))
        if (Physics.SphereCast(groundRay, controller.radius, out groundHit, groundRayDistance, groundMask.value))
        {
            grounded = true;
            transform.position = groundHit.point + (Vector3.up * ((controller.height / 2) - 0.05f));
        }
        else
            grounded = false;
    }
    /// <summary>
    /// Handles camera movement for looking around
    /// </summary>
    private void HandleCamera()
    {
        inputLagTimer += Time.deltaTime;
    
        Vector2 wantedVelocity = lastInputEvent * sensitivity;
    
        velocity = new Vector2(Mathf.MoveTowards(velocity.x, wantedVelocity.x, mouseAcceleration * Time.deltaTime), Mathf.MoveTowards(velocity.y, wantedVelocity.y, mouseAcceleration * Time.deltaTime));
        rotation += velocity * Time.deltaTime;
        cam.transform.localEulerAngles = new Vector3(rotation.x, rotation.y, 0);
    
        if((Mathf.Approximately(0, mouseVector.x) && Mathf.Approximately(0, mouseVector.y)) == false || inputLagTimer >= inputLagPeriod)
        {
            lastInputEvent = mouseVector;
            inputLagTimer = 0;
            mouseVector = Vector3.zero;
        }
    }
    //private void HandleCamera()
    //{
    //    Vector3 vel = mouseVector * sensitivity;
    //    Vector3 axis = mouseVector;
    //    
    //    axis.y = mouseVector.y;
    //    axis.x = 0;
    //    transform.Rotate(axis, Mathf.Abs(vel.y));
    //    
    //    float mouseX = vel.x;
    //    clampX += mouseX;
    //    if (Mathf.Abs(clampX) < maxLookAngle)
    //    {
    //        cam.transform.Rotate(Vector3.right * (mouseX));
    //    }
    //    else
    //    {
    //        if (clampX > maxLookAngle) clampX = 90f;
    //        else if (clampX < minLookAngle) clampX = -90f;
    //    
    //        Vector3 eulerRot = cam.transform.eulerAngles;
    //        eulerRot.x = clampX;
    //        cam.transform.eulerAngles = eulerRot;
    //    }
    //    mouseVector = Vector2.zero;
    //}
    
   

    /// <summary>
    /// Handles what direction the player will move in based on being in the air and input direction
    /// </summary>
    private void AirMovement()
    {
        /* grab current velocity, this will be the value that forces act against;
         * take directional input and account for transform.forward;
         * with direction, use that and air acceleration to act against the stored velocity;
         * return velocity; */

        //make sure we are not grounded
        if (grounded || restrictAirMovement)
        {
            return;
        }

        moveVec.y -= gravity;

        //get directional input and account for player forward
        Vector3 forward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
        Vector3 right = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up);
        Vector3 forces = forward * direction.z + right * direction.x;
        //get acceleration rate
        forces *= acceleration * airAccelerationCoefficient;

        moveVec += forces;
    }

    /// <summary>
    /// Handles what direction the player will move in based on being grounded and input direction
    /// </summary>
    private void GroundMovement()
    {
        /* grab current velocity, this will be the value that forces act against;
         * take directional input and account for transform.forward;
         * with direction, use that and ground acceleration to act against the stored velocity;
         * return velocity; */


        //make sure we are grounded
        if (!grounded)
            return;


        //get acceleration rate and apply to 'forces'
        if(!restrictAccelCoefficient)
            currentAccelCoefficient = groundAccelerationCoefficient;

        currentAccelCoefficient *= groundMovementMultiplier;

        //get directional input and account for player forward
        //then apply acceleration coefficient
        Vector3 forward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
        Vector3 right = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up);
        Vector3 forces = (forward * direction.z + right * direction.x) * currentAccelCoefficient;

        //Apply forces
        moveVec += forces;
    }
    /// <summary>
    /// Handles forces that oppose Player's movement on the ground
    /// </summary>
    private void GroundFriction()
    {
        if (!grounded)
            return;

        Vector3 counterForce = Vector3.zero;

        if(!restrictFrictionCoefficient)
            currentFrictionCoefficient = groundFrictionCoefficient;

        if (controller.velocity.x != 0)
            counterForce.x = controller.velocity.x * currentFrictionCoefficient;
        if (controller.velocity.z != 0)
            counterForce.z = controller.velocity.z * currentFrictionCoefficient;

        moveVec -= counterForce;
    }
    /// <summary>
    /// Handles Forces in air that oppose Player's movement through the air 
    /// </summary>
    private void AirDrag()
    {
        if (grounded || restrictAirMovement)
            return;

        Vector3 counterForce = Vector3.zero;

        if (controller.velocity.magnitude > 0)
        {
            counterForce.x = controller.velocity.x * horizontalDragCoefficient;
            counterForce.y = controller.velocity.y * verticalDragCoefficient;
            counterForce.z = controller.velocity.z * horizontalDragCoefficient;
        }

        moveVec -= counterForce;
    }

    public void AddForce(Vector3 force)
    {
        moveVec += force;
    }

    public void SetForce(Vector3 force)
    {
        moveVec = force;
    }

    public void CalculateMovement()
    {
        CheckGround();
        if(!restrictGroundMovement)
        { 
            GroundMovement();
            GroundFriction();
        }
        AirMovement();
        AirDrag();

        if (input)
            controller.Move(moveVec * Time.fixedDeltaTime);
    }
    #endregion
}
