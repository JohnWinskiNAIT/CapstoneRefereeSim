using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneLoader : MonoBehaviour
{
    public string nextSceneName;

    public void LoadSceneBttn()
    {
        SceneManager.LoadScene(nextSceneName);
    }


}
