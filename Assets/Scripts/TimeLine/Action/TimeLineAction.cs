using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLineAction : MonoBehaviour
{
    [SerializeField] private ActionType m_type;
    private float m_timePosition;
    private float m_duration;
    [HideInInspector] public bool played = false;
    
    public ActionType type => m_type;
    public float timePosition => m_timePosition;
    public float duration => m_duration;


    public void SetActionType(ActionType _type)
    {
        m_type = _type;
    }
    public void SetTimePosition(float _timePosition)
    {
        m_timePosition = _timePosition;
    }
    public void SetDuration(float _duration)
    {
        m_duration = _duration;
    }
}
