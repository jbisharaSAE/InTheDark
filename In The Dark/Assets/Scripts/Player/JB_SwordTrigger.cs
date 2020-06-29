using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_SwordTrigger : MonoBehaviour
{
    [SerializeField] private JB_ResourceManagement resourceScript;
    [SerializeField] private LayerMask attackMask;
    [SerializeField] private float attackRange;
    [SerializeField] private DamageInfo firstPhaseAttackDamage;
    [SerializeField] private DamageInfo lastPhaseAttackDamage;

    public void PlayerAttack(int attackPhase)
    {
        Collider2D colInfo = Physics2D.OverlapCircle(transform.position, attackRange, attackMask);

        if (colInfo != null)
        {
            if (attackPhase == 1)
            {
                DamageEnemy(1, colInfo);
            }
            else if (attackPhase == 2)
            {
                RandomGeneratedCombo();
                DamageEnemy(2, colInfo);
            }
        }
            
        
    }


    private void RandomGeneratedCombo()
    {

        float randomN = Random.value;

        // gives a player lower chance to gain 3 combo points
        if (randomN < 0.15f)
        {
            resourceScript.BasicSwordAttack(3);
        }
        else
        {
            int rand = Random.Range(1, 3);
            resourceScript.BasicSwordAttack(rand);
        }

        
        
    }

    private void DamageEnemy(int attackPhase, Collider2D enemy)
    {
        switch (attackPhase)
        {
            case 1:
                enemy.GetComponent<HealthComponent>().ApplyDamage(firstPhaseAttackDamage);
                break;
            case 2:
                enemy.GetComponent<HealthComponent>().ApplyDamage(lastPhaseAttackDamage);
                break;
        }
    }
}
