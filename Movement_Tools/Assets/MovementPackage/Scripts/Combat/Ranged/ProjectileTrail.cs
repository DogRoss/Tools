using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProjectileTrail : MonoBehaviour
{
    public float closingTime;
    public float closingRate;
    public LineRenderer trail;
    public Color startColor;

    [HideInInspector] public Vector3 start;
    [HideInInspector] public Vector3 target;

    Color endColor;
    float current = 0;

    private void Awake()
    {
        endColor = startColor;
        endColor.a = 0;
        trail = GetComponent<LineRenderer>();
    }

    public void SetUpTrail(Vector3 startingPosition, Vector3 targetPosition)
    {
        current = 0;
        trail.SetPosition(0, startingPosition);
        trail.SetPosition(1, targetPosition);
        start = startingPosition;
        target = targetPosition;

        closingRate /= Vector3.Distance(startingPosition, targetPosition);
    }

    // Update is called once per frame
    void Update()
    {
        current += Time.deltaTime;
        trail.SetPosition(0, Vector3.Lerp(trail.GetPosition(0), target, Time.deltaTime * closingRate));
        trail.startColor = Color.Lerp(startColor, endColor, current / closingTime);
        trail.endColor = Color.Lerp(startColor, endColor, current / closingTime);
    }
}
