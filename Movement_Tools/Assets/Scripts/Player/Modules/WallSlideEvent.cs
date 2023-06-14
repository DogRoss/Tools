using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(CharacterController))]
public class WallSlideEvent : MonoBehaviour
{
    public Movement player;
    public ControllerColliderHit wallPointHit;
    public bool touchingWall;

    [Tooltip("Wall Gravity Coefficient: Speed at which the Player falls while sliding on the wall.")]
    public float WGCoefficient = 0.1f;
    [Tooltip("Max Wall Gravity Coefficient: max speed the player can travel down on the Y Axis")]
    public float MWGCoefficient = .5f;
    [Tooltip("Wall Acceleration Coefficient: put desc here.")]
    public float WACoefficient = 1f;
    [Tooltip("Wall Jump Coefficient: multiplied by jump force to get force when jumping on wall, z is forward of player")]
    public Vector3 WJCoefficient = Vector3.one;
    [Tooltip("Coefficient of forces that act against the Player when in motion against a wall.")]
    [Range(0, 1)]
    public float wallFrictionCoefficient = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!enabled)
            return;

        CheckWall();

        if (touchingWall)
        {
           WallSlide();
           WallFriction();
        }

        if(player.crouchEvent.crouching)
            touchingWall = false;
    }

    private void OnEnable()
    {
        player = GetComponent<Movement>();
    }

    private void OnDisable()
    {
        player.restrictGroundMovement = false;
        player.jumpEvent.jumpMultiplier = 1;
        player.jumpEvent.jumpDirection = Vector3.up;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!enabled)
            return;

        if (hit.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            if (!player.grounded)
            {
                player.restrictGroundMovement = true;
                touchingWall = true;
                player.grounded = false;
                wallPointHit = hit;
                if(player.jumpEvent)
                    player.jumpEvent.RefreshJump();
            }
            else
                player.restrictGroundMovement = false;


            if (player.jumpEvent && wallPointHit != null)
            {
                player.jumpEvent.jumpMultiplier = WJCoefficient.magnitude;
                float dotprod = Vector3.Dot(player.transform.right, wallPointHit.normal);
                if(dotprod > 0) //left side wall
                {
                    player.jumpEvent.jumpDirection = player.transform.TransformDirection(WJCoefficient.normalized);
                }
                else
                {
                    Vector3 vec = Vector3.Reflect(WJCoefficient.normalized, Vector3.left);
                    player.jumpEvent.jumpDirection = player.transform.TransformDirection(vec);
                }
            }
        }
        else
        {
            player.restrictGroundMovement = false;

            if (player.jumpEvent)
            {
                player.jumpEvent.jumpDirection = Vector3.up;
                player.jumpEvent.jumpMultiplier = 1;
            }
        }
    }

    public void CheckWall()
    {

        if (player.Controller.collisionFlags != CollisionFlags.Sides)
        {
            print("call this thing");
            touchingWall = false;
        }
    }

    public void WallSlide()
    {
        print("wall slide");

        //store input in relation to where the player is facing
        player.playerInputDirec = transform.forward * player.direction.z + transform.right * player.direction.x;
        player.playerInputDirec.y = 0;

        //take current velocity and transfer to wall plane
        player.moveVec = Vector3.ProjectOnPlane(player.moveVec, wallPointHit.normal);

        //measure forces and apply to plane
        Vector3 forces = player.playerInputDirec * (player.acceleration * WACoefficient);
        forces = Vector3.ProjectOnPlane(forces, wallPointHit.normal);
        //forces -= wallPointHit.normal;

        //add to movement vector
        player.moveVec += forces;
    }

    public void WallFriction()
    {
        //calculate gravity on wall
        if (player.moveVec.y > -player.gravity * MWGCoefficient)
            player.moveVec.y = player.Controller.velocity.y - (player.gravity * WGCoefficient);
        else
            player.moveVec.y -= player.moveVec.y * wallFrictionCoefficient;

        Vector3 counterForce = Vector3.zero;

        if (player.CurrentHorizontalSpeed > 0)
        {
            counterForce.x = player.Controller.velocity.x * wallFrictionCoefficient;
            counterForce.z = player.Controller.velocity.z * wallFrictionCoefficient;
        }

        player.moveVec -= counterForce;
    }
}
