using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StartToMain : MonoBehaviour
{
    // ** HTC VIVE SETTING ** //
    public GameObject steamController;

    private SteamVR_TrackedController controller;
    private SteamVR_Controller.Device device;
    private SteamVR_TrackedObject trackedObj;

    private void Awake()
    {
        AudioManager.instance.PlayAudio(BackgroundMusic.Lobby);
        controller = GetComponent<SteamVR_TrackedController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<SteamVR_TrackedController>();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if ((Input.GetKey(KeyCode.Mouse0) || controller.triggerPressed))
        {
            GameObject.FindObjectOfType<LoadingSceneControl>().LoadScreeenExample(1);
        }
    }
}