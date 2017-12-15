using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AttackButtonScript : Button
{
    private FireProjectile playerGun;

    protected override void Awake()
    {
        playerGun = GameObject.FindGameObjectWithTag("PlayerGun").GetComponent<FireProjectile>();
    }
    public void Update()
    {
        //A public function in the selectable class which button inherits from.
        if (IsPressed())
        {
            Debug.Log("Hello!!!!!!!!!");
            WhilePressed();
        }
    }

    public void WhilePressed()
    {
        playerGun.Shoot();
    }
}