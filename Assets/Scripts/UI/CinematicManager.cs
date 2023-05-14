using System;
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
    }

    private void OnEnable()
    {
        ChapterManager.OnLoadComplete += Play;
    }

    private void Play()
    {
        if (m_scenes.Count == 0)
        {
            ChapterManager.instance.NextScene();
            return;
        }
        m_scenes[i].Play(this);
    }

    private void OnDisable()
    {
        ChapterManager.OnLoadComplete -= Play;
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

    public void Skip()
    {
        ChapterManager.instance.NextScene();
    }
    public void SkipToLastScene()
    {
        for (int i = 0; i < m_scenes.Count - 1; ++i)
        {
            m_scenes[i].gameObject.SetActive(false);
        }
        i = m_scenes.Count - 1;
        m_scenes[i].Play(this);
        ChapterManager.SkipMeanWhile();
        
    }
}
