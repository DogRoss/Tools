using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject InGameUI;
    public GameObject EndGameUI;

    [Header("UI Objects")]
    public GameObject movementControls;
    public GameObject jumpControls;
    public GameObject crouchControls;
    public GameObject sprintControls;
    public GameObject doubleJumpControls;
    public GameObject crouchSlideControls;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        if (InGameUI)
            InGameUI.SetActive(true);

        if(EndGameUI)
            EndGameUI.SetActive(false);
    }

    public void TriggerEndGame()
    {
        InGameUI.SetActive(false);
        EndGameUI.SetActive(true);
    }


    public void DisableAll()
    {
        movementControls.SetActive(false);
        jumpControls.SetActive(false);
        crouchControls.SetActive(false);
        sprintControls.SetActive(false);
        doubleJumpControls.SetActive(false);
        crouchSlideControls.SetActive(false);
    }

    /// <summary>
    /// 0 - movement / 1 - jump / 2 - crouch / 3 - sprint / 4 - doubleJump / 5 - crouchSlide
    /// </summary>
    /// <param name="index"></param>
    public void EnableControls(int index)
    {
        DisableAll();

        switch (index)
        {
            case 0:
                movementControls.SetActive(true);
                break;
            case 1:
                jumpControls.SetActive(true);
                break;
            case 2:
                crouchControls.SetActive(true);
                break;
            case 3:
                sprintControls.SetActive(true);
                break;
            case 4:
                doubleJumpControls.SetActive(true);
                break;
            case 5:
                crouchSlideControls.SetActive(true);
                break;
        }
    }
}
