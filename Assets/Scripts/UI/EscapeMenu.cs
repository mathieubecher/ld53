using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeMenu : MonoBehaviour
{
    private Animator m_animator;
    private bool m_isOpen = false;
    void Awake()
    {
        m_animator = GetComponent<Animator>();
    }
    void OnEnable()
    {
        ControlsManager.OnEscape += OnEscape;
    }


    void OnDisable()
    {
        ControlsManager.OnEscape -= OnEscape;
        
    }
    
    public void OnEscape()
    {
        if (!GameManager.instance.isPlaying) return;
        if(m_isOpen) ResumeGame();
        else PauseGame();
    }

    private void PauseGame()
    {
        m_animator.SetTrigger("Enter");
        GameManager.instance.PauseGame();
        m_isOpen = true;
    }

    private void ResumeGame()
    {
        m_animator.SetTrigger("Exit");
        GameManager.instance.ResumeGame();
        m_isOpen = false;
    }
}
