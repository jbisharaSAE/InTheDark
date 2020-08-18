using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JB_ResourceManagement : MonoBehaviour
{
    // UI elements
    [SerializeField] private Image energyBar;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image energyIndicatorBar;
    [SerializeField] private Image healthIndicatorBar;
    [SerializeField] private Image[] comboPoints;
    [SerializeField] private float energyRefillSpeed = 25.0f;
    [SerializeField] private float hpRefillSpeed = 5f;
    [SerializeField] private float m_healAbilityAmount;
    [SerializeField] private JB_SwordTrigger swordScript;
    [SerializeField] private GameObject swordSlashPrefab;
    [SerializeField] private GameObject aoeSlashPrefab;
    [SerializeField] private Transform swordSlashSpawn;

    private bool m_isFacingRight;
    private bool bAdjustHealth = false;
    private float m_currentEnergy = 100.0f;
    private float currentHealth = 0f;
    private float maxHealth = 0f;
    private int m_currentCombo = 1;
    private float attackCost = 10.0f;
    private float tempHP = 100.0f;
    private HealthComponent healthScript = null;
    private JB_PlayerController playerScript;

    public int currentCombo { get { return m_currentCombo; } }

    public float currentEnergy { get { return m_currentEnergy; } }

    private void Start()
    {
        healthScript = GetComponent<HealthComponent>();
        playerScript = GetComponent<JB_PlayerController>();

        if(playerScript)
            m_isFacingRight = playerScript.isFacingRight;

        if (healthScript)
        {
            healthScript.OnHealthChanged += OnHealthChanged;
            healthScript.OnDeath += OnDeath;
            currentHealth = healthScript.health;
            maxHealth = healthScript.maxHealth;
        }
    }



    // Update is called once per frame
    void Update()
    {
        
        if (m_currentEnergy <= 100.0f)
        {
            m_currentEnergy += Time.deltaTime * energyRefillSpeed;
        }

        if (energyBar && energyIndicatorBar)
        {
            energyBar.fillAmount = m_currentEnergy / 100.0f;
            energyIndicatorBar.fillAmount = m_currentEnergy / 100.0f;
        }
            

        if (healthBar && healthIndicatorBar)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
            healthIndicatorBar.fillAmount = currentHealth / maxHealth;
        }
            

        UpdateHealth();


    }

    public void UpdateEnergy(float amount)
    {
        m_currentEnergy += amount;
    }


    private void OnHealthChanged(HealthComponent self, float newHealth, float delta)
    {
        bAdjustHealth = true;
    }

    private void OnDeath(HealthComponent self)
    {
        if (PlayerHUD.instance)
            PlayerHUD.instance.DisplayGameOverScreen();
    }

    // used to adjust health, loss or gain
    public void UpdateHealth()
    {

        if (bAdjustHealth)
        {
            currentHealth = Mathf.Lerp(currentHealth, healthScript.health, hpRefillSpeed * Time.deltaTime);
            // Disable ourselves once close to actual health value
            if (Mathf.Approximately(currentHealth, healthScript.health))
            {
                bAdjustHealth = false;
            }
                
        }
        

    }


    public void UpdateComboPoints(int combo)
    {
        m_currentCombo += combo;
        Debug.Log("current combo point = " + m_currentCombo);

        if (m_currentCombo <= 0)
        {
            // resetting combo point value to 0 if it goes into negative numbers
            m_currentCombo = 0;
        }
        else if (m_currentCombo >= 5)
        {
            // resetting combo point value to 4 if it goes above 4
            m_currentCombo = 5;
        }


        // turning on / off image combo points
        for (int i = 0; i < m_currentCombo; ++i)
        {
            comboPoints[i].enabled = true;
            
        }
        for(int i = m_currentCombo; i <5; ++i)
        {
            comboPoints[i].enabled = false;
        }

    }

    // called from JB_SwordTrigger script
    public void BasicSwordAttack(int combo)
    {
        //Debug.Log(attackPhase + "sword attack phase");
        //Debug.Log(currentCombo + "combo points");

        UpdateComboPoints(combo);

      
        
    }

    public void PlayerAbilities(int abilityNumber)
    {
        m_isFacingRight = playerScript.isFacingRight;

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
            
        }

    }

    
    // when players use special abilities, removing correct combo points
    // TODO - add player to enemies

    private void AbilityOne()
    {
        if(m_currentCombo >= 3)
        {
            // frontal slash
            // instantiate sprite
            if(swordSlashPrefab != null)
            {
                GameObject slash = Instantiate(swordSlashPrefab, swordSlashSpawn.position, swordSlashPrefab.transform.rotation);

                // is the player facing left
                if (!m_isFacingRight)
                {
                    slash.transform.Rotate(0f, 180f, 0f);
                }
            }
                

            UpdateComboPoints(-3);
        }
    }

    private void AbilityTwo()
    {
        if (m_currentCombo >= 3)
        {
            // aoe slash
            // instantiate sprite
            if (aoeSlashPrefab != null)
                Instantiate(aoeSlashPrefab, transform.position, Quaternion.identity);

            UpdateComboPoints(-3);
        }
    }

    private void AbilityThree()
    {
        if(m_currentCombo >= 2)
        {
            // heal
            // modify health component  
            if (healthScript)
                healthScript.RestoreHealth(m_healAbilityAmount);

            UpdateComboPoints(-2);
        }
    }
}
