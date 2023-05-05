using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeLine : MonoBehaviour
{
    const float EPSILON = 0.00001f;
    public delegate void SimpleEvent();
    public static event SimpleEvent OnNPCAddAction;
    
    [SerializeField] private float m_timeScale = 0.5f;
    
    [SerializeField] private RectTransform m_parent;
    [SerializeField] private RectTransform m_barrier;
    [SerializeField] private RectTransform m_actionsParent;
    [SerializeField] private RectTransform m_overlayParent;
    [Header("Cursor")]
    [SerializeField] private RectTransform m_cursor;
    [SerializeField, Range(0,10)] private float m_cursorTimeOffset = 0.0f;
    [Header("Prefab")]
    [SerializeField] private GameObject m_barPrefab;
    [SerializeField] private float m_barSize = 60.0f;
    
    private GameTimer m_timer;
    private List<TimeLineBar> m_bars;
    private List<TimeLineAction> m_actions;
    private int m_priority = 0;
    
    public RectTransform parent => m_parent;
    public float elapsedTime => m_timer.elapsedTime;
    public List<TimeLineAction> actions => m_actions;

    public delegate void OnActionEvent(TimeLineAction _action);
    public event OnActionEvent OnAction;
    
    private void Awake()
    {
        m_timer = new GameTimer();
        m_timer.SetTimeScale(m_timeScale);

        m_actions = new List<TimeLineAction>();
        InstantiateBar();
    }
    private void InstantiateBar()
    {
        if(m_bars != null)
        {
            foreach (TimeLineBar bar in m_bars)
                Destroy(bar.gameObject);
        }
    
        Rect parentRect = ((RectTransform)transform).rect;
        int nbBar = (int)math.ceil(parentRect.height / m_barSize) + 1;
        m_bars = new List<TimeLineBar>();
        for (int i = 0; i < nbBar; ++i)
        {
            GameObject bar = Instantiate(m_barPrefab, transform);
            TimeLineBar timeLineBar = bar.GetComponent<TimeLineBar>();
            m_bars.Add(timeLineBar);
        }
    }

    public void EnableBarrier(bool _enable)
    {
        m_barrier.gameObject.SetActive(_enable);
    }
    
    private void Update()
    {
        float dt = m_timer.timeScale * Time.deltaTime;
        Rect parentRect = ((RectTransform)transform).rect;
        Vector2 zeroPos = Vector2.up * ((parentRect.height) / 2.0f + m_barSize);

        ManageCursor(zeroPos);
        ManageBars(elapsedTime, zeroPos);
        ManageActions(elapsedTime, dt, zeroPos);
    }

    private void ManageCursor(Vector2 _zeroPos)
    {
        if (m_cursor)
        {
            m_cursor.localPosition = _zeroPos + Vector2.down * ((m_cursorTimeOffset + 1.0f) * m_barSize);
        }
    }

    private void ManageBars(float _elapsedTime, Vector2 _zeroPos)
    {
        for (int i = 0; i < m_bars.Count; ++i)
        {
            TimeLineBar bar = m_bars[i];
            float barTime = Mod(i - _elapsedTime + 1.0f + m_cursorTimeOffset, m_bars.Count);
            Vector2 barRelativePos = Vector2.down * (barTime * m_barSize);
            bar.transform.localPosition = _zeroPos + barRelativePos;
            bar.SetText(math.floor(barTime + _elapsedTime - m_cursorTimeOffset - 1.0f + EPSILON).ToString());
        }
    }

    private void ManageActions(float _elapsedTime, float _dt, Vector2 _zeroPos)
    {
        m_actions.RemoveAll(x => x == null);
        foreach (TimeLineAction action in m_actions)
        {
            if (!action.played && m_timer.isStarted && action.timePosition <= _elapsedTime + _dt)
            {
                action.played = true;
                OnAction?.Invoke(action);
            }
            else if (m_timer.isStarted && _elapsedTime - 10.0f > action.timePosition)
            {
                Destroy(action.gameObject);
                continue;
            }

            float actionTime = action.timePosition - _elapsedTime + 1.0f + m_cursorTimeOffset;
            Vector2 barRelativePos = Vector2.down * (actionTime * m_barSize);
            action.transform.localPosition = _zeroPos + barRelativePos;
        }
    }

    public bool IsPointOnTimeLine(Vector2 _mousePos, out float _timePos)
    {
        _timePos = 0.0f;
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform as RectTransform, _mousePos, FindObjectOfType<Canvas>().worldCamera, out localPoint))
        {
            if (((RectTransform)transform).rect.Contains(localPoint))
            {
                _timePos = GetTimePosForPoint(localPoint);
                return true;
            }
        }

        return false;
    }
    
    private float GetTimePosForPoint(Vector2 _localPoint)
    {
        float cursorPos = -(_localPoint.y  - m_cursor.localPosition.y) / m_barSize + m_timer.elapsedTime;
        return cursorPos;
    }
    
    public void DrawActionOverlay(GameObject _actionPrefab, RectTransform _overlay, float _timePos)
    {
        _overlay.SetParent(m_overlayParent);
        _overlay.sizeDelta = ((RectTransform)_actionPrefab.transform).sizeDelta;
        
        Rect parentRect = ((RectTransform)transform).rect;
        Vector2 zeroPos = Vector2.up * ((parentRect.height) / 2.0f + m_barSize);
        float actionTime = _timePos - m_timer.elapsedTime + 1.0f + m_cursorTimeOffset;
        Vector2 barRelativePos = Vector2.down * (actionTime * m_barSize);
        _overlay.localPosition = zeroPos + barRelativePos;
        
    }

    public bool TryAddAction(GameObject _actionPrefab, float _desiredTimePos, out float _timePos)
    {
        _timePos = _desiredTimePos;
        if (!m_timer.isStarted) return false;
        
        float delaTimePos = _timePos - m_timer.elapsedTime;
        if (delaTimePos < -1.0f) return false;
        if (delaTimePos < 0.0f)
        {
            _timePos = m_timer.elapsedTime;
        }

        Rect actionRect = ((RectTransform)_actionPrefab.transform).rect;
        float duration = actionRect.height / m_barSize;
        foreach (TimeLineAction other in m_actions)
        {
            float otherDuration = ((RectTransform)other.transform).rect.height / m_barSize;
            if (_timePos >= other.timePosition && _timePos < other.timePosition + otherDuration)
            {
                float testTimePos = other.timePosition + otherDuration;
                if (testTimePos - _timePos > 0.5f) return false;
                foreach (TimeLineAction otherTest in m_actions)
                {
                    if (testTimePos <= otherTest.timePosition && testTimePos + duration >= otherTest.timePosition) return false;
                }
                _timePos = testTimePos;
                return true;
            }
            else if (_timePos + duration >= other.timePosition && _timePos + duration < other.timePosition + otherDuration)
            {
                return false;
            }
            else if (_timePos >= other.timePosition && _timePos + duration <= other.timePosition + otherDuration)
            {
                return false;
            }
            else if (_timePos <= other.timePosition && _timePos + duration >= other.timePosition + otherDuration)
            {
                return false;
            }
        }

        if (_timePos - m_timer.elapsedTime < 0.0f) return false;
        return true;
    }

    public void AddAction(GameObject _actionPrefab, float _timePos)
    {
        GameObject actionObject = Instantiate(_actionPrefab, m_actionsParent);
        TimeLineAction action = actionObject.GetComponent<TimeLineAction>();
        action.SetTimePosition(_timePos);
        action.SetDuration(((RectTransform)actionObject.transform).rect.height / m_barSize);
        
        if (action.type == ActionType.HIT)
        {
            ClearAllActions();
        }
        m_actions.Add(action);
    }

    private void ClearAllActions()
    {
        foreach (var action in m_actions)
        {
            if(action) Destroy(action.gameObject);
        }
        m_actions = new List<TimeLineAction>();
    }


    public void StartTimer()
    {
        m_timer.Start();
    }
    
    public void ResetTimer()
    {
        m_timer.Reset();
    }
    
    public void StopTimer()
    {
        Debug.Log(m_timer);
        m_timer.Stop();
    }
    
    public void ResumeTimer()
    {
        m_timer.Resume();
    }
    
    
    public void SetTimeScale(float _timeScale)
    {
        if (_timeScale <= 0.0f) return;
        m_timeScale = _timeScale;
        m_timer.SetTimeScale(_timeScale);
    }
    
    private float Mod(float x, int m) {
        float r = x % m;
        return r < 0.0f ? r + m : r;
    }
}
