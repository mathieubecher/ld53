using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager m_instance;
    private SoundComponent m_soundComponent;

    public EventReference chapterIntro1;
    public EventReference chapterMeanwhile1;
    public EventReference victory1;
    public EventReference defeat1;

    public EventReference chapterIntro2;
    public EventReference chapterMeanwhile2;
    public EventReference victory2;
    public EventReference defeat2;

    public EventReference chapterIntro3;
    public EventReference chapterMeanwhile3;
    public EventReference victory3;
    public EventReference defeat3;

    public EventReference chapterIntro4;
    public EventReference chapterMeanwhile4;
    public EventReference victory4;
    public EventReference defeat4;

    public EventReference chapterIntro5;
    public EventReference chapterMeanwhile5;
    public EventReference victory5;
    public EventReference defeat5;

    public EventReference fightMusic;

    private int m_curChapIndex;
    private bool m_prevSceneSkipped;

    public bool startFightMusic;
    public float meanwhileTitleDuration;

    private void Awake()
    {
        if (m_instance)
            Destroy(this);
        else
            m_instance = this;

        DontDestroyOnLoad(gameObject);

        if (!TryGetComponent<SoundComponent>(out m_soundComponent))
            m_soundComponent = gameObject.AddComponent<SoundComponent>();
    }

    private void OnEnable()
    {
        ChapterManager.OnChapterIntro += OnChapterIntro;
        ChapterManager.OnChapterMeanwhile += OnChapterMeanwhile;
        ChapterManager.OnSkipMeanwhile += OnSkipMeanwhile;
        ChapterManager.OnFightStart += OnFightStart;
        ChapterManager.OnChapterWin += OnVictory;
        ChapterManager.OnChapterDefeat += OnDefeat;
        ChapterManager.OnChapterTitleScreen += OnChapterTitleScreen;
        ChapterManager.OnSkipScene += OnSkipScene;
    }

    private void OnDisable()
    {
        ChapterManager.OnChapterIntro -= OnChapterIntro;
        ChapterManager.OnChapterMeanwhile -= OnChapterMeanwhile;
        ChapterManager.OnSkipMeanwhile -= OnSkipMeanwhile;
        ChapterManager.OnFightStart -= OnFightStart;
        ChapterManager.OnChapterWin -= OnVictory;
        ChapterManager.OnChapterDefeat -= OnDefeat;
        ChapterManager.OnChapterTitleScreen -= OnChapterTitleScreen;
        ChapterManager.OnSkipScene -= OnSkipScene;
    }

    private void Start()
    {
        if (startFightMusic)
            m_soundComponent.PlaySound(fightMusic);
    }

    private void OnChapterIntro(int _index)
    {
        m_curChapIndex = _index;
        switch (_index)
        {
            case 0:
                m_soundComponent.PlaySound(chapterIntro1);
                break;
            case 1:
                m_soundComponent.PlaySound(chapterIntro2);
                break;
            case 2:
                m_soundComponent.PlaySound(chapterIntro3);
                break;
            case 3:
                m_soundComponent.PlaySound(chapterIntro4);
                break;
            case 4:
                m_soundComponent.PlaySound(chapterIntro5);
                break;
        }
    }

    private void OnChapterMeanwhile(int _index)
    {
        if (m_prevSceneSkipped)
        {
            switch (_index)
            {
                case 0:
                    m_soundComponent.StopSound(chapterIntro1);
                    break;
                case 1:
                    m_soundComponent.StopSound(chapterIntro2);
                    break;
                case 2:
                    m_soundComponent.StopSound(chapterIntro3);
                    break;
                case 3:
                    m_soundComponent.StopSound(chapterIntro4);
                    break;
                case 4:
                    m_soundComponent.StopSound(chapterIntro5);
                    break;
            }
        }

        StartCoroutine(StartMeanwhile(_index));
        
        m_prevSceneSkipped = false;
    }

    private void OnSkipMeanwhile()
    {
        switch (m_curChapIndex)
        {
            case 0:
                m_soundComponent.StopSound(chapterMeanwhile1);
                break;
            case 1:
                m_soundComponent.StopSound(chapterMeanwhile2);
                break;
            case 2:
                m_soundComponent.StopSound(chapterMeanwhile3);
                break;
            case 3:
                m_soundComponent.StopSound(chapterMeanwhile4);
                break;
            case 4:
                m_soundComponent.StopSound(chapterMeanwhile5);
                break;
        }
        m_prevSceneSkipped = false;
    }

    private void OnFightStart()
    {
        m_soundComponent.PlaySound(fightMusic);
    }

    private void OnVictory(int _index)
    {
        m_soundComponent.StopSound(fightMusic);

        switch (m_curChapIndex)
        {
            case 0:
                m_soundComponent.PlaySound(victory1);
                break;
            case 1:
                m_soundComponent.PlaySound(victory2);
                break;
            case 2:
                m_soundComponent.PlaySound(victory3);
                break;
            case 3:
                m_soundComponent.PlaySound(victory4);
                break;
            case 4:
                m_soundComponent.PlaySound(victory5);
                break;
        }
    }

    private void OnDefeat(int _index)
    {
        m_soundComponent.StopSound(fightMusic);

        switch (m_curChapIndex)
        {
            case 0:
                m_soundComponent.PlaySound(defeat1);
                break;
            case 1:
                m_soundComponent.PlaySound(defeat2);
                break;
            case 2:
                m_soundComponent.PlaySound(defeat3);
                break;
            case 3:
                m_soundComponent.PlaySound(defeat4);
                break;
            case 4:
                m_soundComponent.PlaySound(defeat5);
                break;
        }
    }

    private void OnChapterTitleScreen(int _index)
    {
        switch (_index)
        {
            case 1:
                m_soundComponent.StopSound(victory1);
                break;
            case 2:
                m_soundComponent.StopSound(victory2);
                break;
            case 3:
                m_soundComponent.StopSound(victory3);
                break;
            case 4:
                m_soundComponent.StopSound(victory4);
                break;
            case 0:
                m_soundComponent.StopSound(victory5);
                break;
        }

        switch (_index)
        {
            case 1:
                m_soundComponent.StopSound(defeat1);
                break;
            case 2:
                m_soundComponent.StopSound(defeat2);
                break;
            case 3:
                m_soundComponent.StopSound(defeat3);
                break;
            case 4:
                m_soundComponent.StopSound(defeat4);
                break;
            case 0:
                m_soundComponent.StopSound(defeat5);
                break;
        }
    }

    private void OnSkipScene()
    {
        m_prevSceneSkipped = true;
    }

    private IEnumerator StartMeanwhile(int _index)
    {
        yield return new WaitForSeconds(meanwhileTitleDuration);
        switch (_index)
        {
            case 0:
                m_soundComponent.PlaySound(chapterMeanwhile1);
                break;
            case 1:
                m_soundComponent.PlaySound(chapterMeanwhile2);
                break;
            case 2:
                m_soundComponent.PlaySound(chapterMeanwhile3);
                break;
            case 3:
                m_soundComponent.PlaySound(chapterMeanwhile4);
                break;
            case 4:
                m_soundComponent.PlaySound(chapterMeanwhile5);
                break;
        }
    }
}
