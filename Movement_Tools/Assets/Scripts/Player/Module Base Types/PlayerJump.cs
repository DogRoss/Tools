using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : PlayerBase
{
    protected Movement player;

    [Header("Base Class Values")]
    public float jumpForce = 10f;
    public float jumpMultiplier = 1f;
    [HideInInspector]
    public Vector3 jumpDirection = Vector3.up;

    [HideInInspector]
    public bool jumped = false;

    private void Start()
    {
        player = GetComponent<Movement>();
        player.jumpEvent = this;
    }

    private void OnEnable()
    {
        player = GetComponent<Movement>();
        player.jumpEvent = this;
    }

    public virtual void Jump(bool enable = true) 
    {
        if (!enabled)
            return;

        if (player.grounded)
        {
            Vector3 vec = jumpDirection * jumpMultiplier;
            player.moveVec += player.transform.right * (vec.x * jumpForce) +
                              player.transform.forward * (vec.z * jumpForce) +
                              Vector3.up * (vec.y * jumpForce);
        }
    }

    public virtual void RefreshJump()
    {
        jumped = false;
    }
}
