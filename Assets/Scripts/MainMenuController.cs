using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        SceneManager.LoadScene("Level1");
    }

    public float Volume
    {
        get { return AudioListener.volume; }
        set { AudioListener.volume = value; }
    }
}
