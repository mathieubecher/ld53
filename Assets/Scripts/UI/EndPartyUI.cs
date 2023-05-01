using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPartyUI : MonoBehaviour
{
    private Animator m_animator;
    void OnEnable()
    {
        m_animator = GetComponent<Animator>();
        GameManager.OnWin += Win;
        GameManager.OnLoose += Loose;
    }


    // Update is called once per frame
    void OnDisable()
    {
        GameManager.OnWin -= Win;
        GameManager.OnLoose -= Loose;
    }
    
    private void Win()
    {
        m_animator.SetTrigger("Win");
    }

    private void Loose()
    {
        m_animator.SetTrigger("Loose");
    }
}
