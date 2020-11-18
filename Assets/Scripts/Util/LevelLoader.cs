using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime;

    public void LoadLevel(string sceneName) {
        StartCoroutine(LoadScene(sceneName, transitionTime));
    }

    IEnumerator LoadScene(string name, float transitionTime)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(name);
    }
}
