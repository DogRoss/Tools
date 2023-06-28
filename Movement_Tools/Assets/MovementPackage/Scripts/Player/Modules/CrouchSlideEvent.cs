using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchSlideEvent : PlayerCrouch
{
    [Header("Inherited Class Values")]

    public float slideBoostMultiplier = 2f;
    public float minimumSlideVelocity = 10f;
    public float slideFrictionCoefficient = .05f;

    
    private bool canSlide = false;
    private bool slideCooldown = false;
    public float slideCooldownTime = .5f;

    private void Update()
    {
        if (!enabled)
            return;

        if (player.CurrentHorizontalSpeed > minimumSlideVelocity)
        {
            canSlide = true;
        }
        else
        {
            canSlide = false;
        }
    }
    public override void Crouch(bool enableCrouch)
    {
        if (!enabled)
            return;

        if (enableCrouch)
        {
            if (slideCooldown)
                return;

            if (player.sprintEvent && player.sprintEvent.sprinting)
                player.sprintEvent.CancelSprint();

            if (player.jumpEvent)
                player.jumpEvent.jumpMultiplier -= crouchJumpDeductive;

            player.restrictAccelCoefficient = true;
            player.restrictFrictionCoefficient = true;
            player.currentAccelCoefficient = crouchAccelCoefficient;
            player.currentFrictionCoefficient = slideFrictionCoefficient;

            if (canSlide && player.grounded)
            {
                player.AddForce((player.Controller.velocity * slideBoostMultiplier) - player.Controller.velocity);
                StartCoroutine(SlideCooldown());
            }

            player.Controller.height = player.originalControllerHeight / 2;
            crouching = true;
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

    private IEnumerator SlideCooldown()
    {
        slideCooldown = true;

        float current = 0f;

        while(current < slideCooldownTime)
        {
            current += Time.deltaTime;
            yield return null;
        }

        slideCooldown = false;
    }

    public override IEnumerator UnCrouch()
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
        player.restrictFrictionCoefficient = false;
        player.currentFrictionCoefficient = player.groundFrictionCoefficient;

        player.Controller.height = player.originalControllerHeight;
        crouching = false;

        if (player.jumpEvent)
            player.jumpEvent.jumpMultiplier += crouchJumpDeductive;

        coroutineRunning = false;
    }
}
