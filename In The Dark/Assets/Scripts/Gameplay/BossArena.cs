using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArena : MonoBehaviour
{
    [SerializeField] private List<Collider2D> m_walls = new List<Collider2D>();     // Colliders to enable when activating boss
    [SerializeField] private Transform m_cameraLocation = null;                     // Location to place camera
    [SerializeField] private float m_orthographicSize = 20f;                        // Orthographic size of camera

    void Awake()
    {
        SetWallsEnabled(false);
    }

    void OnEnable()
    {
        SetWallsEnabled(true);

        Camera camera = Camera.main;
        if (camera)
        {
            if (m_cameraLocation)
            {
                camera.transform.parent = m_cameraLocation;
                camera.transform.localPosition = Vector2.zero;
            }

            camera.orthographicSize = m_orthographicSize;
        }
    }

    private void SetWallsEnabled(bool enable)
    {
        foreach (Collider2D wall in m_walls)
            wall.gameObject.SetActive(enable);
    }
}
