using UnityEngine;
using System.Collections;

public class PreLoad : MonoBehaviour
{
    public GameObject[] maps;
    public GameObject game;
    // Use this for initialization
    void Awake()
    {
        Instantiate(maps[GameController.currentMap], new Vector3(0, 0, 0), Quaternion.identity);
        Instantiate(game);
     //   StartCoroutine(LoadMap());
    }

    IEnumerator LoadMap()
    {
        Instantiate(maps[GameController.currentMap], new Vector3(0,0,0), Quaternion.identity);
        yield return new WaitForSeconds(1);
        Instantiate(game);
    }


}
