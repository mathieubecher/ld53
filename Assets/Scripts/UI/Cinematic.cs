using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cinematic : MonoBehaviour
{
    [SerializeField] private float m_cinematicDuration= 10.0f;
    [SerializeField] private float m_exitOffset= 1.5f;
    private CinematicManager m_manager;
    [SerializeField] private float m_cooldown;
    private bool m_played = false;
    public void Play(CinematicManager _manager)
    {
        m_manager = _manager;
        GetComponent<Animator>().SetTrigger("Play");
        m_cooldown = 0.0f;
        m_played = true;
    }

    public void Update()
    {
        if (m_played && m_cooldown < m_cinematicDuration - m_exitOffset)
        {
            m_cooldown += Time.deltaTime;
            if (m_cooldown >= m_cinematicDuration - m_exitOffset)
            {
                GetComponent<Animator>().SetTrigger("Exit");
            }
        }
    }

    public void Next()
    {
        m_manager.PlayNextScene();
    }
}
