using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterManager : MonoBehaviour
{
    [SerializeField] private List<Chapter> m_chapters;
    private int m_currentChapterIndex = 0;
    public Chapter currentChapter;
    
    private static ChapterManager m_instance;
    public static ChapterManager instance
    {
        get
        {
            if (!m_instance)
            {
                m_instance = FindObjectOfType<ChapterManager>();
            }
            return m_instance;
        }
    }
    private void Awake()
    {
        currentChapter = m_chapters[0];
        if(instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        ControlsManager.OnAccelTime += OnAccelTime;
        ControlsManager.OnDecelTime += OnDecelTime;
    }

    private void OnDestroy()
    {
        ControlsManager.OnAccelTime -= OnAccelTime;
        ControlsManager.OnDecelTime -= OnDecelTime;
    }

    public void NextScene()
    {
        currentChapter.NextScene();
        if (currentChapter.isLastScene) NextChapter();
    }
    
    public void RestartChapter()
    {
        currentChapter.RestartChapter();
    }
    
    private void NextChapter()
    {
        ++m_currentChapterIndex;
        Debug.Log(m_currentChapterIndex + " " + m_chapters[m_currentChapterIndex]);
        currentChapter = m_chapters[m_currentChapterIndex];
        currentChapter.RestartChapter();
    }
    public void FailScene()
    {
        currentChapter.FailChapter();
    }

    private bool m_accelTime = false;
    private void OnDecelTime()
    {
        m_accelTime = false;
    }

    private void OnAccelTime()
    {
        m_accelTime = true;
    }

    private void Update()
    {
        Time.timeScale = m_accelTime ? 10.0f : 1.0f;
    }
    
    [Serializable] public struct Chapter
    {
        public List<string> chapterScenes;
        public string failScene;
        public int currentScene;
        public bool isLastScene => currentScene >= chapterScenes.Count;
        public void RestartChapter()
        {
            SceneManager.LoadScene(chapterScenes[0]);
            currentScene = 0;
        }

        public void FailChapter()
        {
            SceneManager.LoadScene(failScene);
            currentScene = 0;
        }

        public void NextScene()
        {
            ++currentScene;
            if (currentScene >= chapterScenes.Count) return;
                
            SceneManager.LoadScene(chapterScenes[currentScene]);
        }
    }


}
