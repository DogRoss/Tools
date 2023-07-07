using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;

/*
 * inherits movement for movement variables
 * handles player interactions like weapons and worldspace interactions
 */

/// <summary>
/// Handles player and player interactions, like guns and grenades.
/// Inherits Movement class and IDamageable interface.
/// </summary>
public class PlayerController : Movement
{
    public static PlayerController player;

    public float health = 100f;

    [Header("Left Mouse")]
    public UnityEvent leftMouseDown;
    public UnityEvent leftMouseUp;

    [Header("Right Mouse")]
    public UnityEvent rightMouseDown;
    public UnityEvent rightMouseUp;

    [Header("Q")]
    public UnityEvent qDown;
    public UnityEvent qUp;

    [Header("E")]
    public UnityEvent eDown;
    public UnityEvent eUp;

    [Header("F")]
    public UnityEvent fDown;

    [Header("R")]
    public UnityEvent rDown;

    public override void Awake()
    {
        player = this;
        base.Awake();
    }
    public override void Update()
    {
        base.Update();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    //--------------------------------------------------------------------------------------------------------------------------
    // Input Functions
    //--------------------------------------------------------------------------------------------------------------------------
    private void OnRestart(InputValue value)
    {
        if(value.Get<float>() > 0)
            GameMaster.instance.Respawn();
    }
    private void OnQ(InputValue value)
    {
        if(value.Get<float>() > 0)
            qDown.Invoke();
        else
            qUp.Invoke();
    }
    private void OnE(InputValue value)
    {
        if (value.Get<float>() > 0)
            eDown.Invoke();
        else
            eUp.Invoke();
    }
    private void OnF(InputValue value)
    {
        if (value.Get<float>() > 0)
            fDown.Invoke();
    }
    private void OnR(InputValue value)
    {
        if (value.Get<float>() > 0)
            rDown.Invoke();
    }

    private void OnLMouseDown(InputValue value)
    {
        if(value.Get<float>() > 0)
            leftMouseDown.Invoke();
        else
            leftMouseUp.Invoke();
    }
    private void OnRMouseDown(InputValue value)
    {
        if(value.Get<float>() > 0)
            rightMouseDown.Invoke();
        else
            rightMouseUp.Invoke();
    }
}

[System.Serializable]
public class PlayerEvent : MonoBehaviour
{
    public UnityEvent buttonDownEvent;
    public UnityEvent buttonUpEvent;
}
