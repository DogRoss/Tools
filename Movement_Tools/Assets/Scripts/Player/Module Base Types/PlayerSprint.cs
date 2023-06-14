using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class PlayerSprint : PlayerBase
{
    protected Movement player;

    [Header("Base Class Values")]
    [HideInInspector]
    public bool sprinting;
    public float sprintAdditive = 2f;

    private void Start()
    {
        player = GetComponent<Movement>();
        player.sprintEvent = this;
    }

    private void OnEnable()
    {
        player = GetComponent<Movement>();
        player.sprintEvent = this;
    }

    private void OnDisable()
    {
        CancelSprint();
    }

    public virtual void Sprint(bool enable = true)
    {
        if (player.crouchEvent && player.crouchEvent.crouching || !enabled)
            return;

        if (enable)
        {
            player.groundMovementMultiplier += sprintAdditive;
            sprinting = true;
        }
        else if(sprinting)
        {
            player.groundMovementMultiplier -= sprintAdditive;
            sprinting = false;
        }
    }

    public virtual void CancelSprint()
    {
        if (!sprinting)
            return;

        player.groundMovementMultiplier -= sprintAdditive;
        sprinting = false;
    }
}
