using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimelineManager : MonoBehaviour
{
    
    [SerializeField] private GameObject m_timelinePrefab;
    [SerializeField] private Transform m_timelineParent;
    [SerializeField] private float m_timelineScale = 0.5f;
    
    [SerializeField] private List<TimeLine> m_timeLines;
    private float m_width = 0.0f;
    
    public float width => m_width;
    public float timelineScale => m_timelineScale;

    void Awake()
    {
        m_timeLines = new List<TimeLine>();
    }
    
    public void StartAllTimeline()
    {
        foreach (var timeline in m_timeLines)
        {
            timeline.StartTimer();
        }
    }

    public void StopAllTimelines()
    {
        foreach (var timeline in m_timeLines)
        {
            timeline.StopTimer();
        }
    }
    
    public TimeLine RequestTimeline()
    {
        GameObject timelineInstance = Instantiate(m_timelinePrefab, m_timelineParent);
        TimeLine timeline = timelineInstance.GetComponentInChildren<TimeLine>();
        timeline.SetTimeScale(m_timelineScale);
        //timeline.StartTimer();

        timeline.parent.localPosition += Vector3.left * m_width;
        m_width += timeline.parent.rect.width;
        m_timeLines.Add(timeline);
        
        return timeline;
    }

    public void RemoveTimeLine(TimeLine _timeline)
    {
        m_timeLines.Remove(_timeline);
        if(_timeline && _timeline.parent)
            Destroy(_timeline.parent.gameObject);
    }

    public void ResetWidth()
    {
        m_width = 0.0f;
    }
}
