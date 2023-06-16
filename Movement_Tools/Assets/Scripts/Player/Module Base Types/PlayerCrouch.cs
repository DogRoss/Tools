using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class type used in Movement class to crouch and slow down player.
/// </summary>
[RequireComponent(typeof(Movement))]
public class PlayerCrouch : PlayerBase
{
    protected Movement player;

    [Header("Base Class Values")]
    public float crouchAccelCoefficient = .5f;
    public float crouchJumpDeductive = .5f;

    [HideInInspector]
    public bool crouching;

    //Raycasting
    protected Ray ray = new Ray();
    protected float rayDistance;
    protected bool coroutineRunning = false;

    private void Start()
    {
        player = GetComponent<Movement>();
        player.crouchEvent = this;
    }

    private void OnEnable()
    {
        player = GetComponent<Movement>();
        player.crouchEvent = this;
    }

    private void OnDisable()
    {
        player.restrictAccelCoefficient = false;
        player.restrictFrictionCoefficient = false;

        player.Controller.height = player.originalControllerHeight;
    }

    public virtual void Crouch(bool enableCrouch)
    {
        if (!enabled)
            return;

        if (player.sprintEvent && player.sprintEvent.sprinting)
            player.sprintEvent.CancelSprint();

        if (enableCrouch)
        {
            if (coroutineRunning)
            {
                StopCoroutine(UnCrouch());
                coroutineRunning = false;
            }

            player.restrictAccelCoefficient = true;
            player.currentAccelCoefficient = crouchAccelCoefficient;

            player.Controller.height = player.originalControllerHeight / 2;
            crouching = true;
            player.currentAccelCoefficient = crouchAccelCoefficient;

            if(player.jumpEvent)
                player.jumpEvent.jumpMultiplier -= crouchJumpDeductive;
        }
        else
        {
            if (coroutineRunning)
            {
                StopCoroutine(UnCrouch());
                StartCoroutine(UnCrouch());
            }
            else
                StartCoroutine(UnCrouch());
        }
    }

    public bool CheckAboveHead()
    {
        ray.origin = player.transform.position;
        ray.direction = Vector3.up;
        rayDistance = player.originalControllerHeight / 2;
        if (Physics.SphereCast(ray, .5f, rayDistance, LayerMask.GetMask("Default")))
            return true;
        else
            return false;
    }

    public virtual IEnumerator UnCrouch()
    {
        coroutineRunning = true;

        //check if object above us
        bool waitForGetup = CheckAboveHead();

        //if so begin loop until we have left the object blocking us from getting up
        if (waitForGetup)
        {
            while (CheckAboveHead())
            {
                yield return null;
            }
        }

        //uncrouch
        player.restrictAccelCoefficient = false;

        float current = 0;
        player.Controller.enableOverlapRecovery = true;
        while(current < 1f)
        {
            current += Time.fixedDeltaTime;
            player.Controller.height = Mathf.Lerp(player.originalControllerHeight / 2, player.originalControllerHeight, current / 1f);
            yield return null;
        }
        player.Controller.height = player.originalControllerHeight;
        crouching = false;

        if (player.jumpEvent)
            player.jumpEvent.jumpMultiplier += crouchJumpDeductive;

        coroutineRunning = false;
    }
}
