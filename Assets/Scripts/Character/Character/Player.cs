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

        m_waiting = 30.0f;

        SelectTarget();
    }

    public override void Taunt(Character _attacker, float _aggro)
    {
        base.Taunt(_attacker, _aggro);
        var attackerTarget = m_targets.Find(x => x.target == _attacker);
        attackerTarget.aggro += _aggro;
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
                attackerTarget.aggro += _damage;
            }
            else
            {
                attackerTarget.aggro = math.max(attackerTarget.aggro - 0.2f, 1.0f);
            }
            SelectTarget();
        }
        return hitSuccess;
    }

    private void SelectTarget()
    {
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
        
        if (m_life.isDead) return;
        
        m_waiting -= Time.deltaTime * GameManager.timelineManager.timelineScale;
        if (m_waiting > 0.0f) return;
        
        var actions = m_timeline.actions;
        float elapsedTime = m_timeline.elapsedTime;
        if (actions.Count == 0)
        {
            ChooseAction(elapsedTime);
            return;
        }

        var lastAction = actions[^1];
        if (lastAction.duration + lastAction.timePosition <= elapsedTime + 10.0f)
        {
            ChooseAction(math.max(elapsedTime, lastAction.timePosition + lastAction.duration));
        }
    }

    private void ChooseAction(float _timePos)
    {
        if (m_target == null || m_target.isDead) SelectTarget();
        AddAction(GetRandomAction().actionType, _timePos);
    }
    public ActionData GetRandomAction()
    {
        m_waiting = 0.1f;
        float totalWeight = 0f;
        foreach (ActionData action in m_data.actionDatas)
        {
            totalWeight += GetWeight(action);
        }

        float randomValue = Random.Range(0f, totalWeight);

        foreach (ActionData action in m_data.actionDatas)
        {
            randomValue -= GetWeight(action);
            if (randomValue <= 0f)
            {
                return action;
            }
        }

        return m_data.actionDatas[0];
    }
    private float GetWeight(ActionData _action)
    {
        switch (_action.actionType)
        {
            case ActionType.GUARD :
                return 1.0f;
            case ActionType.ATTACK :
                return 3.0f;
            default : return 0.0f;
        }
    }

    private bool m_alreadyDead = false;
    public override void Dead()
    {
        m_alreadyDead = true;
        base.Dead();
        OnPlayerDead?.Invoke();
    }
}
