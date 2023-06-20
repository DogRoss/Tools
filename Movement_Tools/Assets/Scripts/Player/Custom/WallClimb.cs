using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerJump))]
[RequireComponent(typeof(PlayerCrouch))]
public class WallClimb : MonoBehaviour
{
    PlayerController _player;
    PlayerJump _jumpEvent;
    PlayerCrouch _crouchEvent;
    Transform _camera;

    public float wallClimbForce;
    public float maxWallClimbSpeed = 5f;

    [Tooltip("max angle you can turn away from the wall before wall climbing turns off")]
    [Range(0,90)]public float wallClimbBreakAngle = 45;
    float wcbaCoefficient;

    //private variables
    bool _waitingForEnd = false;
    Vector3 velocityCap = Vector3.zero;
    float dot = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<PlayerController>();
        _jumpEvent = GetComponent<JumpEvent>();
        _crouchEvent = GetComponent<PlayerCrouch>();
        _camera = _player.Cam.transform;

        wcbaCoefficient = -1 + (wallClimbBreakAngle / 90);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        print(Vector3.Dot(hit.normal, _camera.forward) + " dot prod");

        if (_player.grounded || _crouchEvent.crouching)
            return;

        if (!_waitingForEnd && !_crouchEvent.crouching)
            StartCoroutine(WaitForCollisionEnd(hit));

    }

    private IEnumerator WaitForCollisionEnd(ControllerColliderHit hit)
    {
        _waitingForEnd = true;
        Vector2 excludeYAxis = new Vector2(hit.normal.x, hit.normal.z).normalized;
        Vector2 cameraNoY = new Vector2(_camera.forward.x, _camera.forward.z).normalized;
        dot = Vector2.Dot(excludeYAxis, cameraNoY);
        yield return null;
        while (_player.Controller.collisionFlags != CollisionFlags.None && _player.Controller.collisionFlags != CollisionFlags.Below && !_crouchEvent.crouching)
        {
            excludeYAxis.x = hit.normal.x; excludeYAxis.y = hit.normal.z;
            cameraNoY.x = _camera.forward.x; cameraNoY.y = _camera.forward.z;
            dot = Vector2.Dot(excludeYAxis.normalized, cameraNoY.normalized);
            if (dot > wcbaCoefficient)
                break;

            velocityCap = _player.moveVec;
            velocityCap.y = Mathf.Clamp(velocityCap.y + (wallClimbForce * Time.deltaTime), -Mathf.Infinity, maxWallClimbSpeed);
            _player.SetForce(velocityCap);

            yield return null;
        }
        _jumpEvent.RefreshJump();
        _waitingForEnd = false;
    }
}
