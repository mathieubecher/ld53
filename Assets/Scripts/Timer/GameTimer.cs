using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer
{
    private float m_timeScale = 1.0f;
    private float m_startTime;
    private float m_stopTime;
    private float m_timeOffset;
    private bool m_isStarted = false;

    public bool isStarted => m_isStarted;
    public float timeScale => m_timeScale;
    public float elapsedTime
    {
        get
        {
            double elapsedTime = 0.0f;
            if(m_isStarted) elapsedTime = (Time.time - m_startTime);
            else elapsedTime = (m_stopTime - m_startTime);
            
            return (float)(elapsedTime + m_timeOffset) * m_timeScale;
        }
    }

    public GameTimer()
    {
        Reset();
    }
    public void Reset()
    {
        m_isStarted = false;
        m_timeOffset = 0.0f;
        
        m_startTime = Time.time;
        m_stopTime = Time.time;
    }
    
    public void Start()
    {
        Reset();
        m_isStarted = true;
    }
    
    public void Stop()
    {
        if (!m_isStarted) return;
        
        m_stopTime = Time.time;
        m_isStarted = false;
    }

    public void Resume()
    {
        if (m_isStarted) return;
        
        m_isStarted = true;
        m_startTime += Time.time - m_stopTime;
    }

    public void SetTimeScale(float _timeScale)
    {
        if (_timeScale <= 0.0f)
        {
            Debug.LogError("Try to set GameTimer timescale to zero or less");
            return;
        }

        var timeReference = !m_isStarted ? m_stopTime : Time.time;
        m_timeOffset = (timeReference - m_startTime + m_timeOffset) * m_timeScale / _timeScale;
        m_startTime = timeReference;
        
        m_timeScale = _timeScale;
    }
}
