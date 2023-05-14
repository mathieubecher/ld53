using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        SceneManager.sceneLoaded += OnSceneLoaded;
        
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    } 

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InformLoadingScene(currentChapter.chapterScenes[currentChapter.currentScene]);
    }


    private void Start()
    {
        InformLoadingScene(currentChapter.chapterScenes[currentChapter.currentScene]);
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

    public delegate void LoadSceneEvent(int _index);
    public static event LoadSceneEvent OnChapterTitleScreen;
    public static event LoadSceneEvent OnChapterIntro;
    public static event LoadSceneEvent OnChapterMeanwhile;
    public static event LoadSceneEvent OnChapterWin;
    public static event LoadSceneEvent OnChapterDefeat;
    private static void InformLoadingScene(string _chapterScene)
    {
        DecomposeText(_chapterScene, out string name, out int number);
        {
            switch (name.Replace(" ", ""))
            {
                case "Intro" :
                    OnChapterIntro?.Invoke(number);
                    break;
                case "Level" :
                    OnChapterMeanwhile?.Invoke(number);
                    break;
                case "Success" :
                    OnChapterWin?.Invoke(number);
                    break;
                case "Fail" :
                    OnChapterDefeat?.Invoke(number);
                    break;
                case "StartMenu" :
                    OnChapterTitleScreen?.Invoke(number);
                    break;
            }
        }
    }
    
    public delegate void SimpleEvent();
    public static event SimpleEvent OnFightStart;
    public static event SimpleEvent OnSkipMeanwhile;
    public static void StartFight()
    {
        OnFightStart?.Invoke();
    }
    public static void SkipMeanWhile()
    {
        OnSkipMeanwhile?.Invoke();
    }
    
    private static void DecomposeText(string _text, out string _name, out int _number)
    {
        string[] splitString = _text.Split(' ');

        _name = "";
        _number = 0;
        if (splitString.Length != 2) return;

        for (int i = 0; i < splitString.Length - 1; ++i)
        {
            _name += splitString[i];
        }
        int.TryParse(splitString[^1], out _number);
    }

}
