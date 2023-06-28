using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;

    public Vector3 currentSpawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        StartCoroutine(SetSpawn());
    }
    public void SetRespawnAsCurrent()
    {
        if (!PlayerController.player)
            print("no player");
        else if (!PlayerController.player.Controller)
            print("no caracter controller");

        currentSpawnPosition = PlayerController.player.Controller.transform.position;
    }

    public void Respawn()
    {
        PlayerController.player.Controller.enabled = false;
        PlayerController.player.Controller.transform.position = currentSpawnPosition;
        PlayerController.player.Controller.enabled = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator SetSpawn()
    {
        while(!PlayerController.player)
            yield return null;

        SetRespawnAsCurrent();
    }
}
