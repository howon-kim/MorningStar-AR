using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShieldButtonScript : MonoBehaviour
{
    const float SHIELD_ACTIVE_TIME = 9.0f;

    private GameObject shield;

    public Slider shieldActiveTimeSlider;
    private float shieldCurrentActiveTime;
    private float coolTime = 10.0f;
    private float currentTime;


    void Start()
    {

    }

    void Awake()
    {
        shieldCurrentActiveTime = SHIELD_ACTIVE_TIME;
        shieldActiveTimeSlider.maxValue = SHIELD_ACTIVE_TIME;
        shieldActiveTimeSlider.value = SHIELD_ACTIVE_TIME;
        currentTime = coolTime;
        shield = GameObject.FindGameObjectWithTag("Shield");
    }


    public void Update()
    {
        //A public function in the selectable class which button inherits from.
        if (currentTime > coolTime && shield.GetComponent<ShieldHealth>().currentShield >= 0)
        {
            this.GetComponent<Button>().onClick.AddListener(ActivateShield);
            currentTime = 0f;
        }

        if (shieldCurrentActiveTime <= 0f)
        {
            DeactivateShield();
        }

            

        shieldCurrentActiveTime -= Time.deltaTime;
        shieldActiveTimeSlider.value -= Time.deltaTime;
        currentTime += Time.deltaTime;

    }

    public void ActivateShield(){
        shield.GetComponent<Renderer>().enabled = true;
        shield.GetComponent<Collider>().enabled = true;
        shieldCurrentActiveTime = SHIELD_ACTIVE_TIME;
        shieldActiveTimeSlider.value = SHIELD_ACTIVE_TIME;
    }

    public void DeactivateShield(){
        shield.GetComponent<Renderer>().enabled = false;
        shield.GetComponent<Collider>().enabled = false;       
    }

}