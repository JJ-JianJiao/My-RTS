using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float currentHealth;
    public float fullHealth;

    public bool isDead;

    public GameObject damagerMaker;

    private void Awake()
    {
        if(gameObject.name.Contains("Soldier"))
            fullHealth = 100;
    }

    // Start is called before the first frame update
    void Start()
    {

        currentHealth = fullHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetDamage(float damageNum, GameObject dM, out bool killTarget, bool Invincible=false) {

        damagerMaker = dM;
        if (isDead)
        {
            killTarget = false;
            return;
        }

        if(!Invincible)
            currentHealth -= damageNum;
        if (currentHealth <= 0) {
            currentHealth = 0;
            isDead = true;
            Debug.Log("I get deadly hurt");
            killTarget = true;
            return;
        }
        killTarget = false;
        Debug.Log("I get " + damageNum + " damage");
    }

    public bool IsUnitDie() {
        if (isDead)
            return true;
        else
            return false;
    }

    public float GetFullHealth() {
        return fullHealth;
    }

    public float GetCurrentHealth() {
        return currentHealth;
    }

    public bool GetHurt() {
        if (fullHealth == currentHealth)
            return false;
        else
            return true;
    }
}
