using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField] public UnityEvent _enterEvent;
    [SerializeField] public UnityEvent _exitEvent;

    private void OnTriggerEnter(Collider other)
    {
        TriggerEnter();
    }
    private void OnTriggerExit(Collider other)
    {
        TriggerExit();
    }

    public void TriggerEnter()
    {
        _enterEvent.Invoke();
    }
    public void TriggerExit()
    {
        _exitEvent.Invoke();
    }
}


