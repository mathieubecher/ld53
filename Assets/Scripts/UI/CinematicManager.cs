using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CinematicManager : MonoBehaviour
{
    [SerializeField] private List<Cinematic> m_scenes;

    private int i = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (m_scenes.Count == 0)
        {
            ChapterManager.instance.NextScene();
            return;
        }
        m_scenes[i].Play(this);
    }

    public void PlayNextScene()
    {
        ++i;
        if (i < m_scenes.Count)
        {
            m_scenes[i].Play(this);
        }
        else ChapterManager.instance.NextScene();
    }
}
