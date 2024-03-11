using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField]
    float pauseTransitionTime;

    [SerializeField]
    RectTransform pauseMenu;

    public static PauseManager instance;

    float speed = 0;
    bool paused;
    float pauseTimer;
    //Reference UI parent this needs to enable and disable along with individual parts it needs to control.


    // Declares itself instance. Do *NOT* give this DontDestroyOnLoad.
    // This should be destroyed and recreated when swapping from main menu to gameplay.
    // Game data and settings should be stored in common scripts.
    private void Awake()
    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<PauseManager>();
        }
        else
        {
            Destroy(gameObject);
        }

        GameplayEvents.SetPause.AddListener(PauseGame);
    }

    private void Update()
    {
        if (paused)
        {
            pauseTimer = Mathf.SmoothDamp(pauseTimer, 1, ref speed, pauseTransitionTime);
            if (pauseTimer > 0.998f)
            {
                pauseTimer = 1;
            }
        }

        if (!paused && pauseTimer > 0)
        {
            pauseTimer = Mathf.SmoothDamp(pauseTimer, 0, ref speed, pauseTransitionTime);
            if (pauseTimer <= 0.002f)
            {
                pauseTimer = 0;
                GameplayEvents.SetPause.Invoke(false);
            }
        }

        pauseMenu.anchorMax = Vector3.Lerp(new Vector3(0, 1), new Vector3(1, 1), pauseTimer);
        pauseMenu.anchorMin = Vector3.Lerp(new Vector3(-1, 0), new Vector3(0, 0), pauseTimer);
    }

    public void PauseGame(bool pausing)
    {
        paused = pausing;
        if (pausing)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
