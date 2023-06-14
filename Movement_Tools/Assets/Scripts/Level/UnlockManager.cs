using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    public static UnlockManager instance;
    public PlayerController player;

    public PlayerJump jump;
    public PlayerCrouch crouch;
    public PlayerSprint sprint;
    public JumpEvent doubleJump;
    public CrouchSlideEvent crouchSlide;

    private void Start()
    {
        instance = this;

        if (player)
        {
            jump = player.GetComponent<PlayerJump>();
            crouch = player.GetComponent<PlayerCrouch>();
            sprint = player.GetComponent<PlayerSprint>();
            doubleJump = player.GetComponent<JumpEvent>();
            crouchSlide = player.GetComponent<CrouchSlideEvent>();
        }
        else
            Debug.LogError("player reference is null");
    }

    public void SetPlayerMovement(bool enable)
    {
        if (player)
        {
            player.input = enable;
        }
    }


    public void UnlockJump()
    {
        jump.enabled = true;
    }

    public void UnlockCrouch()
    {
        crouch.enabled = true;
    }

    public void UnlockSprint()
    {
        sprint.enabled = true;
    }

    public void UnlockDoubleJump()
    {
        jump.enabled = false;
        doubleJump.enabled = true;
    }

    public void UnlockCrouchSlide()
    {
        crouch.enabled = false;
        crouchSlide.enabled = true;
    }
}
