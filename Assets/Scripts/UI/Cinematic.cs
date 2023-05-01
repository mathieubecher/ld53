using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cinematic : MonoBehaviour
{
    private CinematicManager m_manager;
    public void Play(CinematicManager _manager)
    {
        m_manager = _manager;
        GetComponent<Animator>().SetTrigger("Play");
    }

    public void Next()
    {
        m_manager.PlayNextScene();
    }
}
