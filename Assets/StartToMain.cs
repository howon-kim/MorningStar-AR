using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class StartToMain : MonoBehaviour
{
    public GameObject waitingText;
    public GameObject loadingText;
    public Slider slider;

    private float currentTime;
    private Animator animator;
    AsyncOperation async;



    private void Awake()
    {
        AudioManager.instance.PlayAudio(BackgroundMusic.Lobby);
        animator = this.GetComponent<Animator>();

    }

    // Update is called once per frame
    private void Update()
    {
        currentTime += Time.deltaTime;
 
        if (currentTime >= 7.0f)
            animator.speed = 0;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            StartCoroutine(LoadingScreen());
        
    }

    IEnumerator LoadingScreen(){
        animator.speed = 1;
        waitingText.SetActive(false);
        loadingText.SetActive(true);

        async = SceneManager.LoadSceneAsync(1);
        async.allowSceneActivation = false;

        while (async.isDone == false){
            slider.value = async.progress;
            if(async.progress == 0.9f){
                slider.value = 1f;
                async.allowSceneActivation = true;
                
            }
            yield return null;
        }
    }
}