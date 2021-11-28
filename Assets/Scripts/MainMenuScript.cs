using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    private float buttonPressDelay = 1;

    private void Start()
    {
        Screen.SetResolution(720, 1280, true);
        Application.targetFrameRate = 60;
    }

    public void StartPressed()
    {
        StartCoroutine(ButtonPressedWait());
    }

    IEnumerator ButtonPressedWait()
    {
        yield return new WaitForSeconds(buttonPressDelay);
        SceneManager.LoadScene("MainScene");
    }
}
