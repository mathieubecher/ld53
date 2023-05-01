using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTeam : MonoBehaviour
{
    [SerializeField] private Animator m_generalIntroAnimator;
    public void Begin()
    {
        m_generalIntroAnimator.SetTrigger("Begin");
    }

    public void PlayExit()
    {
        GetComponent<Animator>().SetTrigger("Exit");
    }
}
