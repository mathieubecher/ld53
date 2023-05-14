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
        //ControlsManager.OnAccelTime += OnAccelTime;
        //ControlsManager.OnDecelTime += OnDecelTime;
    }

    private void OnDestroy()
    {
        //ControlsManager.OnAccelTime -= OnAccelTime;
        //ControlsManager.OnDecelTime -= OnDecelTime;
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
/*
    private bool m_accelTime = false;
    private void OnDecelTime()
    {
        m_accelTime = false;
    }

    private void OnAccelTime()
    {
        m_accelTime = true;
    }
*/
    private void Update()
    {
        //Time.timeScale = m_accelTime ? 10.0f : 1.0f;
    }
    
    [Serializable] public struct Chapter
    {
        public List<string> chapterScenes;
        public string failScene;
        public int currentScene;
        public bool isLastScene => currentScene >= chapterScenes.Count;
        public void RestartChapter()
        {
            ChapterManager.InformLoadingScene(chapterScenes[0]);
            SceneManager.LoadScene(chapterScenes[0]);
            currentScene = 0;
        }

        public void FailChapter()
        {
            ChapterManager.InformLoadingScene(failScene);
            SceneManager.LoadScene(failScene);
            currentScene = 0;
        }

        public void NextScene()
        {
            ++currentScene;
            if (currentScene >= chapterScenes.Count) return;

            ChapterManager.InformLoadingScene(chapterScenes[currentScene]);
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
            Debug.Log(_chapterScene + " " + name + " " + number);
            switch (name.Replace(" ", ""))
            {
                case "Intro" :
                    Debug.Log("Intro " + number);
                    OnChapterIntro?.Invoke(number);
                    break;
                case "Level" :
                    Debug.Log("Level " + number);
                    OnChapterMeanwhile?.Invoke(number);
                    break;
                case "Success" :
                    Debug.Log("Success " + number);
                    OnChapterWin?.Invoke(number);
                    break;
                case "Fail" :
                    Debug.Log("Fail " + number);
                    OnChapterDefeat?.Invoke(number);
                    break;
                case "StartMenu" :
                    Debug.Log("StartMenu " + number);
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
