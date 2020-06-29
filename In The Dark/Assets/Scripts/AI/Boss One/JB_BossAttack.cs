using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_BossAttack : MonoBehaviour
{
    public float attackDamage = 20f;

    public Vector3 attackOffset;
    public float attackRange = 3f;
    public LayerMask attackMask;

    public void BossAttack()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.y;
        pos += transform.up * attackOffset.y;

        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if(colInfo != null)
        {
            colInfo.GetComponent<HealthComponent>().ApplyDamage(attackDamage);
        }

    }
}
