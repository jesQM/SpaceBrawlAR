using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private static float TransitionTime;

    public void LoadLevel(string sceneName) {
        StartCoroutine(LoadScene(sceneName, TransitionTime));
    }

    IEnumerator LoadScene(string name, float transitionTime)
    {
        SceneManager.LoadScene(name);
        yield return null;
    }
}
