using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerJump))]
public class WallRunning : MonoBehaviour
{

    PlayerController _player;
    PlayerJump _jumpEvent;
    Transform _camera;
    CameraPhysics _campPhys;

    public float wallRunningSpeed= 20;
    public float verticalSpeedCap = 1;
    public float slowdownRate = 100;

    bool _waitingForEnd = false;
    float dot;

    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<PlayerController>();
        _jumpEvent = GetComponent<PlayerJump>();
        _camera = _player.Cam.transform;
        _campPhys = GetComponentInChildren<CameraPhysics>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (_player.grounded)
            return;

        Vector3 movementAxis = Vector3.Cross(hit.normal, Vector3.up);
        dot = Vector3.Dot(movementAxis, _camera.forward);

        if (dot < .5f && dot > -.5f)
        {
            if (_waitingForEnd)
            {
                StopAllCoroutines();
                _waitingForEnd = false;
                _jumpEvent.jumpDirection = Vector3.up;
                _campPhys.offsetRotation.z = 0;
            }

            return;
        }

        if (!_waitingForEnd)
            StartCoroutine(WaitForWallRunEnd(movementAxis, hit));
    }

    private IEnumerator WaitForWallRunEnd(Vector3 movementAxis, ControllerColliderHit hit)
    {
        _waitingForEnd = true;


        Vector3 movementDirection = Vector3.Project(_player.direction, movementAxis).normalized;

        while (_player.Controller.collisionFlags != CollisionFlags.None && _player.Controller.collisionFlags != CollisionFlags.Below)
        {
            movementDirection = Vector3.Project(_player.direction, movementAxis).normalized;
            _player.AddForce((movementDirection * dot) * wallRunningSpeed * Time.deltaTime);

            if(_player.moveVec.y < -verticalSpeedCap)
                _player.moveVec.y = Mathf.Lerp(_player.moveVec.y, -verticalSpeedCap, Time.deltaTime * slowdownRate);

            if (_jumpEvent.jumped)
                _jumpEvent.RefreshJump();

            _jumpEvent.jumpDirection = transform.TransformDirection((hit.normal + Vector3.up)).normalized;

            if (!_player.grounded)
                _player.grounded = true;

            dot = Vector3.Dot(movementAxis, _camera.forward);
            if (dot > 0)
                _campPhys.offsetRotation.z = -45;
            else
                _campPhys.offsetRotation.z = 45;

            yield return null;
        }

        _waitingForEnd = false;
        _jumpEvent.jumpDirection = Vector3.up;
        _campPhys.offsetRotation.z = 0;
    }

}
