using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float m_timeScale = 1.0f;
    private DateTime m_startTime;
    private DateTime m_stopTime;
    private TimeSpan m_timeOffset;
    private bool m_isStarted = false;

    public bool isStarted => m_isStarted;
    public float elapsedTime
    {
        get
        {
            double elapsedTime = 0.0f;
            if(m_isStarted) elapsedTime = (DateTime.Now - m_startTime).TotalSeconds;
            else elapsedTime = (m_stopTime - m_startTime).TotalSeconds;
            
            return (float)(elapsedTime + m_timeOffset.TotalSeconds) * m_timeScale;
        }
    }

    public Timer()
    {
        Reset();
    }
    public void Reset()
    {
        m_isStarted = false;
        m_timeOffset = TimeSpan.Zero;
        
        m_startTime = DateTime.Now;
        m_stopTime = DateTime.Now;
    }
    
    public void Play()
    {
        Reset();
        m_isStarted = true;
    }
    public void Stop()
    {
        if (!m_isStarted) return;
        m_stopTime = DateTime.Now;
        m_isStarted = false;
    }
    
    public void Resume()
    {
        if (m_isStarted) return;
        m_isStarted = true;
        m_startTime += DateTime.Now - m_stopTime;
    }

    public void SetTimeScale(float _timeScale)
    {
        if (_timeScale <= 0.0f)
        {
            Debug.LogError("Try to set Timer timescale to zero or less");
            return;
        }

        var timeReference = !m_isStarted ? m_stopTime : DateTime.Now;
        m_timeOffset = (timeReference - m_startTime + m_timeOffset) * m_timeScale / _timeScale;
        m_startTime = timeReference;
        
        m_timeScale = _timeScale;
    }
}
