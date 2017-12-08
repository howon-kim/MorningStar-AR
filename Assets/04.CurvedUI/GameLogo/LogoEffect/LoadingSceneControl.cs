using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneControl : MonoBehaviour
{
    public Animator animator;
    public float stopTime = 6.1f;

    public GameObject loadingScreenObj;

    //public Slider slider;
    private AsyncOperation async;

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(stopTime);
        GameObject.FindObjectOfType<LoadingSceneControl>().LoadScreeenExample(1);
        animator.speed = 0f;
    }

    public void LoadScreeenExample(int sceneLevel)
    {
        SceneManager.LoadScene(sceneLevel, LoadSceneMode.Single);
        //StartCoroutine(LoadingScreen(sceneLevel));
    }

    private IEnumerator LoadingScreen(int sceneLevel)
    {
        loadingScreenObj.transform.Find("Description").gameObject.SetActive(false);
        loadingScreenObj.transform.Find("Loading").gameObject.SetActive(true);
        animator.speed = 1f;

        SceneManager.LoadScene(sceneLevel, LoadSceneMode.Single);
        /*async.allowSceneActivation = false;

        while (async.progress < 0.9f)
        {
            yield return new WaitForSeconds(0.1f);
            Debug.Log(async.progress);
        }

        async.allowSceneActivation = true;*/
        yield break;
    }
}