using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    private float buttonPressDelay = 1;

    [SerializeField] GameObject transition;

    private void Start()
    {
        transition.SetActive(true);
        Screen.SetResolution(720, 1280, true);
        Application.targetFrameRate = 60;
    }

    public void StartPressed()
    {
        StartCoroutine(ButtonPressedWait());
    }

    IEnumerator ButtonPressedWait()
    {
        transition.GetComponent<Animator>().SetBool("IsMenu", true);
        yield return new WaitForSeconds(buttonPressDelay);
        SceneManager.LoadScene("MainScene");
    }
}
