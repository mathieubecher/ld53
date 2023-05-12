using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;


[Serializable] public class Aura
{
    public float timePosition = 0.0f;
    public float duration = 0.0f;
    public AuraEffect effect;

    [Serializable] public class AuraEffect
    {
        public float attackMultiplier = 1.0f;
        public float defenceMultiplier = 1.0f;
        public bool invulnerability = false;
        public AuraEffect(){}

        public AuraEffect(float _attackMultiplier, float _defenceMultiplier, bool _invulnerability)
        {
            attackMultiplier = _attackMultiplier;
            defenceMultiplier = _defenceMultiplier;
            invulnerability = _invulnerability;
        }
        
        public static AuraEffect operator +(AuraEffect a, AuraEffect b)
        {
            AuraEffect result = new AuraEffect();
            result.attackMultiplier = a.attackMultiplier * b.attackMultiplier;
            result.defenceMultiplier = a.defenceMultiplier * b.defenceMultiplier;
            result.invulnerability = a.invulnerability || b.invulnerability;
            return result;
        }
    }
    public Aura(){}
    public Aura(float _timePosition, float _duration, float _attackMultiplier, float _defenceMultiplier, bool _invulnerability)
    {
        timePosition = _timePosition;
        duration = _duration;
        effect = new AuraEffect(_attackMultiplier, _defenceMultiplier, _invulnerability);
    }

}

public class TimeLine : MonoBehaviour
{
    const float EPSILON = 0.00001f;
    
    [SerializeField] private float m_timeScale = 0.5f;
    
    [SerializeField] private Image m_header;
    [SerializeField] private RectTransform m_parent;
    [SerializeField] private RectTransform m_barrier;
    [SerializeField] private RectTransform m_actionsParent;
    [SerializeField] private RectTransform m_overlayParent;
    [Header("Cursor")]
    [SerializeField] private RectTransform m_cursor;
    [SerializeField] private Animator m_cursorAnimator;
    [SerializeField, Range(0,10)] private float m_cursorTimeOffset = 0.0f;
    [Header("Prefab")]
    [SerializeField] private GameObject m_barPrefab;
    [SerializeField] private float m_barSize = 60.0f;

    private GameTimer m_timer;
    private List<TimeLineBar> m_bars;
    private List<TimeLineAction> m_actions;
    [SerializeField] private List<Aura> m_auras;
    private int m_cellsPerUnit;
    
    public RectTransform parent => m_parent;
    public float elapsedTime => m_timer.elapsedTime;
    public List<TimeLineAction> actions => m_actions;
    public int cellsPerUnit => m_cellsPerUnit;

    public TimeLineAction currentAction
    {
        get
        {
            foreach (var action in m_actions)
            {
                if (action.played && action.timePosition + action.duration >= elapsedTime)
                    return action;
            }

            return null;
        }
    }

    public delegate void OnActionEvent(TimeLineAction _action);
    public event OnActionEvent OnAction;
    public event OnActionEvent OnEndAction;
    
    private void Awake()
    {
        m_timer = new GameTimer();
        m_timer.SetTimeScale(m_timeScale);

        m_actions = new List<TimeLineAction>();
        m_auras = new List<Aura>();
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

    public void TickCursor()
    {
        m_cursorAnimator.SetTrigger("Tick");
    }
    
    public void EnableBarrier(bool _enable)
    {
        m_barrier.gameObject.SetActive(_enable);
    }

    private int m_currentTick = 0;
    private void Update()
    {
        float dt = m_timer.timeScale * Time.deltaTime;
        Rect parentRect = ((RectTransform)transform).rect;
        Vector2 zeroPos = Vector2.up * ((parentRect.height) / 2.0f + m_barSize);

        float TOLERANCE = 0.0001f;
        int nextTick = (int) math.floor((elapsedTime + 0.05f) * m_cellsPerUnit);
        if (nextTick != m_currentTick)
        {
            m_currentTick = nextTick;
            TickCursor();
        }

        ManageCursor(zeroPos);
        ManageBars(elapsedTime, zeroPos);
        ManageActions(elapsedTime, dt, zeroPos);
    }

    private void ManageCursor(Vector2 _zeroPos)
    {
        if (m_cursor)
        {
            m_cursor.localPosition = new Vector2(m_cursor.localPosition.x, _zeroPos.y - (m_cursorTimeOffset + 1.0f) * m_barSize);
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
        if (!m_timer.isStarted) return;
        foreach (TimeLineAction action in m_actions)
        {
            if (!action.played && action.timePosition <= _elapsedTime + _dt)
            {
                action.played = true;
                OnAction?.Invoke(action);
            }
            else if (!action.stop && action.timePosition + action.duration <= elapsedTime + _dt)
            {
                action.stop = true;
                OnEndAction?.Invoke(action);
            }
            else if (_elapsedTime - 10.0f > action.timePosition)
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
    
    public void DrawActionOverlay(float _duration, RectTransform _overlay, float _timePos)
    {
        _overlay.SetParent(m_overlayParent);
        _overlay.sizeDelta = new Vector2(_overlay.sizeDelta.x, _duration * m_barSize);
        
        Rect parentRect = ((RectTransform)transform).rect;
        Vector2 zeroPos = Vector2.up * ((parentRect.height) / 2.0f + m_barSize);
        float actionTime = _timePos - m_timer.elapsedTime + 1.0f + m_cursorTimeOffset;
        Vector2 barRelativePos = Vector2.down * (actionTime * m_barSize);
        _overlay.localPosition = zeroPos + barRelativePos;
        
    }

    public bool TryCombineAction(float _desiredTimePos, out TimeLineAction _other)
    {
        _other = null;
        foreach (TimeLineAction other in m_actions)
        {
            if (!other.played && _desiredTimePos >= other.timePosition && _desiredTimePos <= other.timePosition + other.duration - 0.2f)
            {
                _other = other;
                return true;
            }
        }

        return false;
    }
    
    public bool TryAddAction(float _duration, float _desiredTimePos, out float _timePos)
    {
        _timePos = GetCellForTimePos(_desiredTimePos);
        if (!m_timer.isStarted) return false;
        
        float delaTimePos = _timePos - m_timer.elapsedTime;
        if (delaTimePos < -1.0f) return false;
        if (delaTimePos < 0.0f)
        {
            _timePos = GetCellForTimePos(m_timer.elapsedTime);
        }

        foreach (TimeLineAction other in m_actions)
        {
            float otherDuration = ((RectTransform)other.transform).rect.height / m_barSize;
            if (_timePos >= other.timePosition && _timePos < other.timePosition + otherDuration)
            {
                float testTimePos = other.timePosition + otherDuration;
                if (testTimePos - _timePos > 0.5f) return false;
                foreach (TimeLineAction otherTest in m_actions)
                {
                    if (testTimePos <= otherTest.timePosition && testTimePos + _duration >= otherTest.timePosition) return false;
                }
                _timePos = GetCellForTimePos(testTimePos);
                return true;
            }
            else if (_timePos + _duration >= other.timePosition && _timePos + _duration < other.timePosition + otherDuration)
            {
                return false;
            }
            else if (_timePos >= other.timePosition && _timePos + _duration <= other.timePosition + otherDuration)
            {
                return false;
            }
            else if (_timePos <= other.timePosition && _timePos + _duration >= other.timePosition + otherDuration)
            {
                return false;
            }
        }

        if (_timePos - m_timer.elapsedTime < 0.0f) return false;
        return true;
    }

    public float GetCellForTimePos(float _timePos)
    {
        float floorPos = math.floor(_timePos);
        float posInUnit = _timePos - floorPos;
        float cellSize = 1.0f / m_cellsPerUnit;
        for (int i = 0; i < m_cellsPerUnit; ++i)
        {
            if (posInUnit <= i * cellSize) return floorPos + i * cellSize;
        }
        return floorPos + 1.0f;
    }

    public void AddAction(CharacterActionData _data, float _timePos)
    {
        GameObject actionObject = Instantiate(TimelineManager.GetGenericAction(), m_actionsParent);
        TimeLineAction action = actionObject.GetComponent<TimeLineAction>();
        
        action.SetTimeline(this);
        action.SetActionData(_data);
        action.SetTimePosition(_timePos);
        action.SetDuration(_data.duration);
        action.SetColor(_data.color);
        action.SetSize(m_barSize * _data.duration);
        action.SetIcone(_data.icone);
        
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

    
    public void AddAura(Aura _aura)
    {
        m_auras.Add(_aura);
    }

    public Aura.AuraEffect GetCurrentAura()
    {
        Aura.AuraEffect currentAuraEffect = new Aura.AuraEffect();
        foreach (var aura in m_auras)
        {
            if (elapsedTime >= aura.timePosition && aura.timePosition + aura.duration >= elapsedTime)
            {
                currentAuraEffect += aura.effect;
            }
        }

        return currentAuraEffect;
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
    public void SetHeader(Sprite _sprite)
    {
        m_header.sprite = _sprite;
    }
    public void SetCellsPerUnit(int _cellsPerUnit)
    {
        m_cellsPerUnit = _cellsPerUnit;
    }
    
    private float Mod(float x, int m) {
        float r = x % m;
        return r < 0.0f ? r + m : r;
    }

}
