using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class JumpEvent : PlayerJump
{
    [HideInInspector]
    public bool doubleJumped;

    [Header("Inherited Class Values")]
    public float doubleJumpMultiplier = 1.5f;

    public override void Jump(bool enable = true)
    {
        if (!enabled)
            return;

        if (player.grounded || !jumped)
        {
            JEvent();
            if(player.grounded)
                doubleJumped = false;
            player.grounded = false;
            jumped = true;
        }
        else if (jumped && !doubleJumped)
        {
            JEvent();
            doubleJumped = true;
        }
    }

    public override void RefreshJump()
    {
        doubleJumped = false;
        base.RefreshJump();
    }

    public void JEvent()
    {
        Vector3 vec = jumpDirection * jumpMultiplier;
        vec *= jumpForce;
        player.moveVec += player.transform.right * vec.x +
                          player.transform.forward * vec.z;

        player.moveVec.y = jumped ? vec.y * doubleJumpMultiplier : vec.y;
    }
}

