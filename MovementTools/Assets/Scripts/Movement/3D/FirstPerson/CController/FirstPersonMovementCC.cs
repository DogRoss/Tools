using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonMovementCC : MonoBehaviour
{

    [SerializeField] float speed = 10f;
    Vector2 direction = Vector2.zero;
    Vector3 finalMoveDirection = Vector3.zero;

    CharacterController controller;
    [SerializeField] Transform cam;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //finalMoveDirection.x = direction.x; finalMoveDirection.z = direction.y;
        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + cam.eulerAngles.y; //calculates angle for player direction to move and face
            finalMoveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        else
        {
            finalMoveDirection = Vector3.zero;
        }
        
        //finalMoveDirection.z += cam.eulerAngles.y;
        controller.Move(finalMoveDirection.normalized * speed * Time.deltaTime);
    }

    private void OnMove(InputValue value)
    {
        direction = value.Get<Vector2>();
    }
}
