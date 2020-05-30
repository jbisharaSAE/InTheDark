using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardEnemyScript : EnemyScript
{
    [SerializeField] private GuardLookoutRoutine m_lookoutComp = null;      // Guards routine component

    /// <summary>
    /// This guards lookout routine component
    /// </summary>
    public GuardLookoutRoutine lookoutComponent { get { return m_lookoutComp; } }

    protected override void Awake()
    {
        if (!m_lookoutComp)
            m_lookoutComp = GetComponent<GuardLookoutRoutine>();
    }
}
