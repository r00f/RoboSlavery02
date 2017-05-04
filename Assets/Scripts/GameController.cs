using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    Canvas canvas;
    GameObject mainMenu;

    bool restarting;
    public bool paused;

    int metal;

    #region Mono Methods

    void Start () {

        Time.timeScale = 1;
        canvas = FindObjectOfType<Canvas>();
        mainMenu = canvas.transform.GetChild(3).gameObject;
        mainMenu.SetActive(false);
	}
	
	void Update () {

        if(Input.GetButtonDown("Cancel"))
        {
            PauseGame();
        }

        canvas.transform.GetChild(5).GetComponentInChildren<Text>().text = "Metal: " + metal;
	}

    #endregion

    #region Methods

    public void LoadScene(int SceneIndex)
    {
        SceneManager.LoadScene(SceneIndex, LoadSceneMode.Single);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1, LoadSceneMode.Single);
    }

    public void PauseGame()
    {
        if (!paused)
        {
            mainMenu.SetActive(true);
            Time.timeScale = 0;
            paused = true;
        }

        else
        {
            mainMenu.SetActive(false);
            Time.timeScale = 1;
            paused = false;
        }

    }

    public void RestartScene(float time = 0)
    {
        if(time == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }
        else if(!restarting)
        {
            StartCoroutine(RestartSceneTimed(time));
            restarting = true;
        }

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void AddMetal(int amount)
    {
        metal += amount;
        //print(metal);
    }

    public IEnumerator RestartSceneTimed(float time)
    {
        print("RESTART IN " + time + " SECONDS");
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion


}
