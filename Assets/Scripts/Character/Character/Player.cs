using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable] public class Player : Character
{
    public delegate void PlayerDeadEvent();
    public static event PlayerDeadEvent OnPlayerDead;
    
    [Serializable] private class NPCTarget
    {
        public NPC target;
        public float aggro;

        public NPCTarget(NPC _target, int _aggro)
        {
            target = _target;
            aggro = _aggro;
        }
    }
    

    private float m_waiting;
    [SerializeField] private List<NPCTarget> m_targets;
    public Player(CharacterData _data) : base(_data, false)
    {
        m_timeline.EnableBarrier(true);
        m_line = m_sprite.GetComponent<LineRenderer>();
    }

    public override void StartFight()
    {
        base.StartFight();
        m_targets = new List<NPCTarget>();
        foreach (var character in GameManager.instance.npcs)
        {
            m_targets.Add(new NPCTarget(character, 0));
        }

        m_waiting = 3.0f;
        SelectTarget();
    }

    public override void Taunt(Character _attacker, float _aggro)
    {
        base.Taunt(_attacker, _aggro);
        var attackerTarget = m_targets.Find(x => x.target == _attacker);
        attackerTarget.aggro += _aggro;
        SelectTarget();
    }

    protected override void OnEndAction(TimeLineAction _action)
    {
        base.OnEndAction(_action);
        SelectTarget();
    }

    public override bool TryHit(Character _attacker, float _damage)
    {
        m_waiting = 0.0f;
        bool hitSuccess = base.TryHit(_attacker, _damage);
        
        if (!m_life.isDead)
        {
            var attackerTarget = m_targets.Find(x => x.target == _attacker);
            if (hitSuccess)
            {
                attackerTarget.aggro += 1.0f;
            }
            SelectTarget();
        }
        return hitSuccess;
    }

    private void SelectTarget()
    {
        foreach (var target in m_targets)
        {
            if (target.target.HasTaunt())
            {
                m_target = target.target;
                m_sprite.SetTarget(m_target);
                return;
            }   
        }
        
        m_target = m_targets.OrderByDescending(x => x.aggro * (x.target.isDead ? 0.0f : 1.0f) + (x.target.isDead ? 0.0f : 1.0f)).ToList()[0].target;
        m_sprite.SetTarget(m_target);
    }

    [SerializeField] private LineRenderer m_line;
    public void Update()
    {
        if (m_life.isDead || m_target == null)
        {
            m_line.enabled = false;
        }
        else
        {
            Vector3[] points = {m_target.sprite.transform.position + Vector3.up * 0.8f, m_sprite.transform.position + Vector3.up * 0.8f};
            m_line.SetPositions(points);
            m_line.enabled = true;
        }
        
        if (m_life.isDead ||  m_data.patterns.Count == 0) return;
        
        m_waiting -= Time.deltaTime * GameManager.timelineManager.timelineScale;
        if (m_waiting > 0.0f) return;
        
        var actions = m_timeline.actions;
        float elapsedTime = m_timeline.elapsedTime;
        if (actions.Count == 0)
        {
            ChooseActionPattern(elapsedTime);
            return;
        }

        var lastAction = actions[^1];
        if (lastAction.duration + lastAction.timePosition <= elapsedTime + 10.0f)
        {
            ChooseActionPattern(math.max(elapsedTime, lastAction.timePosition + lastAction.duration));
        }
    }

    private void ChooseActionPattern(float _timePos)
    {
        AddActions(GetRandomPattern(), _timePos);
    }
    private ActionPatternData GetRandomPattern()
    {
        m_waiting = 0.1f;
        float totalWeight = 0f;
        foreach (ActionPatternData action in m_data.patterns)
        {
            totalWeight += action.weight;
        }

        float randomValue = Random.Range(0f, totalWeight);

        foreach (ActionPatternData action in m_data.patterns)
        {
            randomValue -= action.weight;
            if (randomValue <= 0f)
            {
                return action;
            }
        }
        return m_data.patterns[0];
    }
    
    public override void Dead()
    {
        base.Dead();
        OnPlayerDead?.Invoke();
    }
    
    private void AddActions(ActionPatternData _pattern, float _timePos)
    {
        float currentPos = _timePos;
        foreach (var action in _pattern.actions)
        {
            m_timeline.AddAction(action, currentPos);
            spriteEvent.NewActionReceived(action.actionType);
            currentPos += action.duration;
        }
    }
}
