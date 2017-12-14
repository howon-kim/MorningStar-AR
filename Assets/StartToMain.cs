using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StartToMain : MonoBehaviour
{


    private void Awake()
    {
        AudioManager.instance.PlayAudio(BackgroundMusic.Lobby);

    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            //GameObject.FindObjectOfType<LoadingSceneControl>().LoadScreeenExample(1);
        }
    }
}