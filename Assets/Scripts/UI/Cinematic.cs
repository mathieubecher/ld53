using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cinematic : MonoBehaviour
{
    [SerializeField] private float m_cinematicDuration= 10.0f;
    private CinematicManager m_manager;
    private float m_cooldown;
    public void Play(CinematicManager _manager)
    {
        m_manager = _manager;
        GetComponent<Animator>().SetTrigger("Play");
        m_cooldown = m_cinematicDuration;
    }

    public void Update()
    {
        if (m_cooldown > 0.0f)
        {
            m_cooldown -= Time.deltaTime;
            if (m_cooldown <= 0.0f)
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
