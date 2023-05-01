using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralEvents : MonoBehaviour
{
    public void Restart()
    {
        ChapterManager.instance.RestartChapter();
    }
    
    public void Continue()
    {
        ChapterManager.instance.NextScene();
    }
    public void Fail()
    {
        ChapterManager.instance.FailScene();
    }
    public void StartFight()
    {
        GameManager.instance.StartFight();
    }
}
