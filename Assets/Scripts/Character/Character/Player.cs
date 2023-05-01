using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable] public class Player : Character
{
    public static int NUMBER_PLAYER = 0;
    
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
        m_name = GameManager.instance.playerNames[NUMBER_PLAYER];
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
        var target = m_targets.Find(x => x.target == _attacker);
        target.aggro += _aggro;
        Debug.Log("Taunt ! " + _attacker.name);
        SelectTarget();
    }

    public override void Hit(Character _attacker, float _damage, ActionEffect _effect)
    {
        m_waiting = 0.0f;
        base.Hit(_attacker, _damage, _effect);
        if (m_life.isDead) return;
            
        var target = m_targets.Find(x => x.target == _attacker);
        target.aggro += _damage;
        SelectTarget();
    }

    private void SelectTarget()
    {
        m_target = m_targets.OrderByDescending(x => x.aggro * (x.target.isDead ? 0.0f : 1.0f) + (x.target.isDead ? 0.0f : 1.0f)).ToList()[0].target;
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
            Vector3[] points = new[] {m_sprite.transform.position + Vector3.up * 0.8f, m_target.sprite.transform.position + Vector3.up * 0.8f};
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
        if (lastAction.played && lastAction.duration + lastAction.timePosition <= elapsedTime + 0.1f)
        {
            ChooseAction(math.max(elapsedTime, lastAction.timePosition + lastAction.duration));
        }
    }

    private void ChooseAction(float _timePos)
    {
        if (m_target == null || m_target.isDead) SelectTarget();
        
        float maxWeight = 0.0f;
        var maxWeightAction = m_data.actionDatas[0];
        foreach (var action in m_data.actionDatas)
        {
            float weight = GetWeight(action);
            if (weight > maxWeight)
            {
                maxWeight = weight;
                maxWeightAction = action;
            }
        }
        m_timeline.AddAction(maxWeightAction.timeLineBarPrefab, _timePos + 0.1f, false);
    }

    private float GetWeight(CharacterData.ActionData _action)
    {
        float random = Random.Range(0.6f, 1.4f);
        switch (_action.actionType)
        {
            case ActionType.GUARD :
                return random * m_data.strength  * (m_target.isAttacking ? 5.0f : 0.6f);
            case ActionType.ATTACK :
                return m_data.strength * random;
            default : return 0.0f;
        }
    }

    protected override void BlockAttack(Character _blocker)
    {
        var target = m_targets.Find(x => x.target == _blocker);
        target.aggro = math.max(target.aggro - 0.2f, 0.0f);
    }

    private bool m_alreadyDead = false;
    public override void Dead()
    {
        m_alreadyDead = true;
        base.Dead();
        OnPlayerDead?.Invoke();
    }
}
