using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMaster : MonoBehaviour
{
    public float timeBetweenDoors = .5f;

    public List<ClosingDoor> doorList = new List<ClosingDoor>();

    //Coroutines
    private bool closingCoroutine;
    private bool openingCoroutine;

    [ContextMenu("CloseDoors")]
    public void CloseDoors()
    {
        foreach (ClosingDoor d in doorList)
        {
            d.EnableDoor();
        }

        if (closingCoroutine)
            StopCoroutine(CloseAllDoors());

        StartCoroutine(CloseAllDoors());
    }
    [ContextMenu("OpenDoors")]
    public void OpenDoors()
    {
        foreach(ClosingDoor d in doorList)
        {
            d.EnableDoor();
        }

        if (openingCoroutine)
            StopCoroutine(OpenAllDoors());

        StartCoroutine(OpenAllDoors());
    }

    public IEnumerator CloseAllDoors()
    {
        if (openingCoroutine)
            StopCoroutine(OpenAllDoors());

        closingCoroutine = true;

        int currentOnList = 0;
        float current = 0f;

        while (true)
        {
            current += Time.deltaTime;

            if (current > timeBetweenDoors)
            {
                current = 0;
                doorList[currentOnList].Close();
                if (currentOnList < doorList.Count - 1)
                    currentOnList++;
                else
                    break;
            }

            yield return null;
        }
    }
    public IEnumerator OpenAllDoors()
    {
        if (closingCoroutine)
            StopCoroutine(CloseAllDoors());

        openingCoroutine = true;

        int currentOnList = 0;
        float current = 0f;

        while (true)
        {
            current += Time.deltaTime;

            if(current > timeBetweenDoors)
            {
                current = 0;
                doorList[currentOnList].Open();
                if (currentOnList < doorList.Count - 1)
                    currentOnList++;
                else
                    break;
            }

            yield return null;
        }
    }
}
