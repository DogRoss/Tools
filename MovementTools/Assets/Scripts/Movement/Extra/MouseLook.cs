using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;

    //mouse settings
    public float mouseSensitivity = 100f;
    //direction mouse moved towards
    Vector2 mouseDirection = Vector2.zero;
    float xRotation;
    public float verticalLookClamp = 90f;

    // Update is called once per frame
    void Update()
    {
        mouseDirection.x = Mouse.current.delta.x.ReadValue() * mouseSensitivity * Time.deltaTime;
        mouseDirection.y = Mouse.current.delta.y.ReadValue() * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseDirection.y;
        xRotation = Mathf.Clamp(xRotation, -verticalLookClamp, verticalLookClamp);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseDirection.x);
    }
}
