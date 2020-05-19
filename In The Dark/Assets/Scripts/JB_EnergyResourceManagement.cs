using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JB_EnergyResourceManagement : MonoBehaviour
{
    // UI elements
    public Image energyBar;
    public Image[] comboPoints;
    public float currentEnergy = 100.0f;
    private float attackCost = 10.0f;
    public int currentCombo = 1;
    public float energyRefillSpeed = 25.0f;
    public int attackPhase;
    public bool bThirdattack;
    public JB_SwordTrigger swordScript;

    // Start is called before the first frame update
    void Start()
    {
        //playerScript = GetComponent<JB_PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentEnergy <= 100.0f)
        {
            currentEnergy += Time.deltaTime * energyRefillSpeed;
        }

        energyBar.fillAmount = currentEnergy / 100.0f;
    }

    private void UpdateComboPoints()
    {
        ++currentCombo;

        if (currentCombo < 1)
        {
            // resetting combo point value to 0 if it goes into negative numbers
            currentCombo = 1;
        }
        else if (currentCombo > 5)
        {
            // resetting combo point value to 4 if it goes above 4
            currentCombo = 4;
        }

        for (int i = 0; i < currentCombo; ++i)
        {
            comboPoints[i].enabled = true;
        }

    }

    public void BasicSwordAttack()
    {
        Debug.Log(attackPhase + "sword attack phase");
        Debug.Log(currentCombo + "combo points");
        if (bThirdattack)
        {
            bThirdattack = false;
            UpdateComboPoints();
        }
        
        //swordScript.bThirdattack = false;
        
    }

}
