using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Skip : MonoBehaviour
{
    [SerializeField] private float m_holdDuration = 1.0f;
    [SerializeField] private UnityEvent m_skipEvent;

    private Animator m_animator;
    private bool m_hold;
    private float m_holdTimer;
    private bool m_canSkip = true;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        ControlsManager.OnPressSkip += OnPressSkip;
        ControlsManager.OnReleaseSkip += OnReleaseSkip;
    }
    private void OnDisable()
    {
        ControlsManager.OnPressSkip -= OnPressSkip;
        ControlsManager.OnReleaseSkip -= OnReleaseSkip;
    }

    private void Update()
    {
        if (m_hold && m_canSkip)
        {
            m_holdTimer += Time.deltaTime;
            if (m_holdTimer > m_holdDuration)
            {
                m_skipEvent?.Invoke();
                m_canSkip = false;
                m_animator.SetTrigger("Exit");
            }
        }
    }

    private void OnReleaseSkip()
    {
        if (!m_canSkip) return;

        m_hold = false;
        m_animator.SetTrigger("Release");
    }

    private void OnPressSkip()
    {
        if (!m_canSkip) return;
        
        m_hold = true;
        m_holdTimer = 0.0f;
        m_animator.SetTrigger("Press");
    }

}
