using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameObject[] finalDoor;
    GameObject[] turrets;
    
    
    // Start is called before the first frame update
    void Start()
    {
        finalDoor = GameObject.FindGameObjectsWithTag("finalDoors");
        turrets = GameObject.FindGameObjectsWithTag("turrets");
        StartCoroutine(gameOver());
    }
    // ppermet de sortir du niveau quand toutes les tourrels sont détruites;
    IEnumerator gameOver()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);
        while (true)
        {
            yield return wait;

            bool IsGameOver = true;

            foreach (GameObject go in turrets)
            {
                if (go.activeInHierarchy)
                {
                    IsGameOver = false;
                    break;
                }            }

            if (IsGameOver)
            {
                foreach (GameObject go in finalDoor)
                {
                    go.SetActive(false);
                }
                break;
            }

        }
    }

}
