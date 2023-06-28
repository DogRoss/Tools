using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosingDoor : MonoBehaviour
{
    [SerializeField]
    public float closingRate = 1f;

    private Vector3 startingPosition;
    [SerializeField]
    public Vector3 endingPositionOffset;

    private Vector3 endingPosition;
    private bool closingActive = true;
    public bool opening = false;

    private Vector3 dir = Vector3.zero;

    public bool Opening
    {
        get
        {
            return opening;
        }
        set
        {
            opening = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        endingPosition = transform.position + endingPositionOffset;

        opening = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (closingActive)
        {
            if (opening)
            {
                dir = endingPosition - transform.position;
                transform.position = transform.position + (dir * closingRate * Time.deltaTime);
            }
            else
            {
                dir = startingPosition - transform.position;
                transform.position = transform.position + (dir * closingRate * Time.deltaTime);
            }
        }    
    }

    public void Open()
    {
        opening = true;
    }

    public void Close()
    {
        opening = false;
    }

    public void DisableDoor()
    {
        closingActive = false;
    }

    public void EnableDoor()
    {
        closingActive = true;
    }
}
