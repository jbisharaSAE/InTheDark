using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component handles how the players character is percepted (sight) by the AI
/// </summary>
public class PlayerSightPerceptionSource : ShadowAreaListener, ISightPerceptible
{
    [SerializeField, Range(0f, 1f)] private float m_hiddenOpacity = 0.6f;                       // Opacity of players sprite when hidden
    [SerializeField, Min(0f)] private float m_hiddenFadeTime = 0.25f;                           // Time it takes to fully change opacity when hidden
    [SerializeField] private List<string> m_layersToIgnoreWhenHidden = new List<string>();      // Layers to ignore when hidden

    private bool m_cachedHidden = false;    // If currently hidden
    private short m_numSeenBy = 0;          // Number of times we have been seen
    private bool m_canGoInvis = false;      // If can go hidden (invis) at this time

    [SerializeField] private HealthComponent m_healthComp = null;                   // Owners health component
    [SerializeField] private AdvancedCharacterMovement m_movementComp = null;       // Owners movement component
    [SerializeField] private Renderer m_rendererComp = null;
    [SerializeField] private JB_PlayerController m_controller = null;

    [SerializeField] private AudioSource m_hideSoundSource = null;
    [SerializeField] private AudioClip m_hideSound = null;
    [SerializeField] private AudioClip m_visibleSound = null;

    private Coroutine m_fadeOpacityRoutine = null;

    void Awake()
    {
        if (!m_healthComp)
            m_healthComp = GetComponent<HealthComponent>();

        if (!m_movementComp)
            m_movementComp = GetComponent<AdvancedCharacterMovement>();

        if (!m_controller)
            m_controller = GetComponent<JB_PlayerController>();

        m_rendererComp = GetComponent<Renderer>();
    }

    void Update()
    {
        if (GameManager.isPaused)
            return;

        UpdateInvisState();
    }

    public bool CanBeDetected()
    {
        // Ignore us if we have died
        if (m_healthComp && m_healthComp.isDead)
            return false;

        return !m_cachedHidden;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void OnVisibleToSightPerception(SightPerception sightPerception)
    {
        ++m_numSeenBy;
        UpdateInvisState();
    }

    public void OnNotVisibleToSightPerception(SightPerception sightPerception)
    {
        --m_numSeenBy;
        UpdateInvisState();
    }

    protected override void OnEnterShadows()
    {
        UpdateInvisState();
    }

    protected override void OnLeftShadows()
    {
        UpdateInvisState();
    }

    private void UpdateInvisState()
    {
        if (m_healthComp && m_healthComp.isDead)
        {
            m_canGoInvis = false;
            m_cachedHidden = false;
            return;
        }

        bool canGoInvis = CheckInvisState();
        if (canGoInvis != m_canGoInvis)
        {
            if (canGoInvis)
                OnBecomeHidden();
            else
                OnBecomeVisible();

            m_canGoInvis = canGoInvis;
        }

        m_cachedHidden = canGoInvis;
    }

    private bool CheckInvisState()
    {
        // Fully visible while dashing
        if (m_movementComp && m_movementComp.isDashing)
            return false;

        // Fully visible while attacking
        if (m_controller && m_controller.isAttacking)
            return false;

        // If already seen by a enemy, we should be visible to all enemies
        if (m_numSeenBy > 0)
            return false;

        return inShadows;
    }

    protected virtual void OnBecomeHidden()
    {
        UpdateLayerCollisions(true);
        StartFadeRoutine(m_hiddenOpacity);

        PlayHideSound(m_hideSound);
    }

    protected virtual void OnBecomeVisible()
    {
        UpdateLayerCollisions(false);

        // If becoming visible due to dashing, instantly revert the opacity
        if (m_movementComp && m_movementComp.isDashing)
        {
            if (m_fadeOpacityRoutine != null)
            {
                StopCoroutine(m_fadeOpacityRoutine);
                m_fadeOpacityRoutine = null;
            }

            UpdateSpriteOpacity(1f);
        }
        else
        {
            StartFadeRoutine(1f);
        }

        PlayHideSound(m_visibleSound);
    }

    private void PlayHideSound(AudioClip clip)
    {
        if (!m_hideSoundSource || !clip)
            return;

        m_hideSoundSource.Stop();
        m_hideSoundSource.clip = clip;
        m_hideSoundSource.Play();
    }

    private void UpdateLayerCollisions(bool ignore)
    {
        if (m_movementComp)
            m_movementComp.SetIgnoreLayers(m_layersToIgnoreWhenHidden, ignore);
    }

    private void UpdateSpriteOpacity(float alpha)
    {
        if (m_rendererComp && m_rendererComp.material)
        {
            Color color = m_rendererComp.material.color;
            color.a = alpha;
            m_rendererComp.material.color = color;
        }
    }

    private void StartFadeRoutine(float alpha)
    {
        if (m_fadeOpacityRoutine != null)
            StopCoroutine(m_fadeOpacityRoutine);

        m_fadeOpacityRoutine = StartCoroutine(FadeRoutine(alpha));
    }

    private IEnumerator FadeRoutine(float alphaToTarget)
    {
        if (!m_rendererComp || !m_rendererComp.material)
            yield break;

        Material mat = m_rendererComp.material;
        Color matCol = mat.color;

        float from = matCol.a;

        float startTime = Time.time;
        float endTime = startTime + m_hiddenFadeTime;
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / m_hiddenFadeTime;
            float newAlpha = Mathf.Lerp(from, alphaToTarget, t);
            matCol.a = newAlpha;
            mat.color = matCol;

            yield return null;

            // Quick hack for fixing issue if fading back to 1 then dashing
            if (m_movementComp && m_movementComp.isDashing)
            {
                alphaToTarget = 1f;
                break;
            }
            
        }

        matCol.a = alphaToTarget;
        mat.color = matCol;

        m_fadeOpacityRoutine = null;
    }
}
