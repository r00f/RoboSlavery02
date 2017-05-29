using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rewired;

public class GameController : MonoBehaviour {

    Canvas canvas;
    GameObject mainMenu;

    [SerializeField]
    bool restarting;
    public bool paused;
    static bool secDisplayActive;

    [SerializeField]
    float metal;

    public Player rePlayer;

    #region Mono Methods

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()

    {
        HandleInput();
    }

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        rePlayer = ReInput.players.GetPlayer(0);
        restarting = false;
        Time.timeScale = 1;
        if(GameObject.FindGameObjectWithTag("Canvas1"))
            canvas = GameObject.FindGameObjectWithTag("Canvas1").GetComponent<Canvas>();
        mainMenu = GameObject.FindGameObjectWithTag("MainMenu");

        if(SceneManager.GetActiveScene().buildIndex != 0 && mainMenu)
            mainMenu.SetActive(false);
        else
        {
            

        }

        // Display.displays[0] is the primary, default display and is always ON.
        // Check if additional displays are available and activate each.
        if (Display.displays.Length > 1 && !secDisplayActive)
        {
            print("activateSecondDisplay");
            Display.displays[1].Activate();
            secDisplayActive = true;
        }
    }

    void HandleInput()
    {
        if (rePlayer.GetButtonDown("Start"))
        {
            PauseGame();
        }
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
            mainMenu.transform.GetChild(1).GetComponent<Button>().Select();
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

    public void AddSubstractMetal(float amount)
    {
        metal += amount;
        //print(metal);
    }

    public float GetMetalAmount()
    {
        return metal;
    }

    public IEnumerator RestartSceneTimed(float time)
    {
        print("RESTART IN " + time + " SECONDS");
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion


}
