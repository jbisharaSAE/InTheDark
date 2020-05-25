﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JB_ResourceManagement : MonoBehaviour
{
    // UI elements
    public Image energyBar;
    public Image healthBar;
    public Image[] comboPoints;
    public float currentEnergy = 100.0f;
    public float currentHealth = 100.0f;
    private float attackCost = 10.0f;
    private float tempHP = 100.0f;
    public int currentCombo = 1;
    public float energyRefillSpeed = 25.0f;
    public float healthAdjust;
    public int attackPhase;
    public bool bThirdattack = false;
    public bool bAdjustHealth = false;
    public JB_SwordTrigger swordScript;


    // Update is called once per frame
    void Update()
    {
        
        if (currentEnergy <= 100.0f)
        {
            currentEnergy += Time.deltaTime * energyRefillSpeed;
        }

        energyBar.fillAmount = currentEnergy / 100.0f;
        healthBar.fillAmount = currentHealth / 100.0f;

        UpdateHealth();
    }

    // used to adjust health, loss or gain
    public void UpdateHealth()
    {
        

        if (bAdjustHealth)
        {
            tempHP += healthAdjust;
            bAdjustHealth = false;
        }

        currentHealth = Mathf.Lerp(currentHealth, tempHP, 1.0f);
    }


    public void UpdateComboPoints(int combo)
    {
        currentCombo += combo;
        Debug.Log("current combo point = " + currentCombo);

        if (currentCombo <= 0)
        {
            // resetting combo point value to 0 if it goes into negative numbers
            currentCombo = 0;
        }
        else if (currentCombo >= 5)
        {
            // resetting combo point value to 4 if it goes above 4
            currentCombo = 4;
        }


        // turning on / off image combo points
        for (int i = 0; i < currentCombo; ++i)
        {
            comboPoints[i].enabled = true;
            
        }
        for(int i = currentCombo; i <5; ++i)
        {
            comboPoints[i].enabled = false;
        }

    }

    public void BasicSwordAttack(int combo)
    {
        Debug.Log(attackPhase + "sword attack phase");
        Debug.Log(currentCombo + "combo points");
        if (bThirdattack)
        {
            bThirdattack = false;
            UpdateComboPoints(combo);
        }
        
        //swordScript.bThirdattack = false;
        
    }

    public void PlayerAbilities(int abilityNumber)
    {
        switch (abilityNumber)
        {
            case 1:
                // ability 1
                AbilityOne();
                Debug.Log("1st ability activated");
                break;
            case 2:
                // ability 2
                AbilityTwo();
                Debug.Log("2nd ability activated");
                break;
            case 3:
                // ability 3
                AbilityThree();
                Debug.Log("3rd ability activated");
                break;
            case 4:
                // ability 4
                Debug.Log("4th ability activated");
                break;
        }

    }

    private void AbilityOne()
    {
        if(currentCombo >= 2)
        {
            UpdateComboPoints(-2);
        }
    }

    private void AbilityTwo()
    {
        if (currentCombo >= 3)
        {
            UpdateComboPoints(-3);
        }
    }

    private void AbilityThree()
    {
        if(currentCombo >= 4)
        {
            UpdateComboPoints(-4);
        }
    }
}
