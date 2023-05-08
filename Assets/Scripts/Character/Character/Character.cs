using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable] public abstract class Character
{
    [Serializable] protected class Life
    {
        public Life(float _maxLife)
        {
            if (_maxLife <= 0.0f)
            {
                Debug.LogError("Try to set life to " + _maxLife);
                _maxLife = 1.0f;
            }
            
            m_currentLife = _maxLife;
            m_maxLife = _maxLife;
        }
        
        [SerializeField] private float m_currentLife;
        [SerializeField] private float m_maxLife;
        
        public bool isDead => m_currentLife <= 0.0f;
        public float lifeRatio => m_currentLife / m_maxLife;

        public void Hit(float _damage)
        {
            m_currentLife -= _damage;
            m_currentLife = math.clamp(m_currentLife, 0.0f, m_maxLife);
        }

    }
    
    [SerializeField] protected Life m_life;
    [SerializeField] protected Character m_target;
    [Header("Serialized Data")]
    [SerializeField] protected CharacterData m_data;
    [SerializeField] protected TimeLine m_timeline;
    [SerializeField] protected CharacterSprite m_sprite;

    [SerializeField] private float m_guardValue = 0.0f;
    [SerializeField] private bool m_guardBreaked = false;
    [SerializeField] private bool m_buffDamage = false;
    public CharacterSprite sprite => m_sprite;
    public bool isDead => m_life.isDead;
    public CharacterData data => m_data;
    public String name => m_data.characterName;
    public TimeLine timeline => m_timeline;

    private Character() { }
    public Character(CharacterData _data, bool _npc)
    {
        m_timeline = GameManager.timelineManager.RequestTimeline(_data.header);
        m_timeline.OnAction += OnAction;
        m_timeline.OnEndAction += OnEndAction;
        m_data = _data;
        m_life = new Life(m_data.life);
        m_guardValue = 0.0f;
        m_buffDamage = false;
        if(_npc) m_sprite = GameManager.characterSpriteManager.RequestNPCSprite(m_data.spritePrefab);
        else m_sprite = GameManager.characterSpriteManager.RequestPlayerSprite(m_data.spritePrefab);
        m_sprite.OnPlayActionEffect += PlayActionEffect;
    }
    
    public virtual void OnDestroy()
    {
        m_sprite.OnPlayActionEffect -= PlayActionEffect;
        m_timeline.OnAction -= OnAction;
        m_timeline.OnEndAction -= OnEndAction;
        GameManager.timelineManager.RemoveTimeLine(m_timeline);
        GameManager.characterSpriteManager.RemoveCharacterSprite(m_sprite);
    }

    private TimeLineAction m_lastActionPlayed;
    private void OnAction(TimeLineAction _action)
    {
        if (isDead) return;
        m_lastActionPlayed = _action;
        m_guardValue = 0.0f;
        m_sprite.PlayAction(m_target, _action);
    }
    private void OnEndAction(TimeLineAction _action)
    {
        m_guardValue = 0.0f;
        if (m_lastActionPlayed && m_lastActionPlayed == _action)
        {
            m_sprite.Idle();
        }
    }


    public virtual void Hit(Character _attacker, float _damage, ActionEffect _effect)
    {
        float damage = _damage;
        if (m_guardValue > 0.0f)
        {
            m_guardValue -= damage;
            _attacker.BlockAttack(this);
            if (m_guardValue <= 0.0f) GuardBreak();
            return;
        }

        if (m_guardBreaked) damage *= 2.0f;
        m_life.Hit(damage);
        m_sprite.Hit(m_life.lifeRatio, damage);
        
        if (m_life.isDead) Dead();
        else if(m_guardValue <= 0.0f && Random.Range(0.0f, 1.0f) < m_data.hitStunProba)
        {
            AddAction(ActionType.HIT, m_timeline.elapsedTime);
        }
    }

    protected void AddAction(ActionType _type, float _timePos)
    {
        CharacterData.ActionData data = m_data.GetActionData(_type);
        m_timeline.AddAction(data.actionType, GameManager.GetColor(data.actionType), GameManager.GetIcone(data.actionType),
            data.duration, _timePos);
    }


    private void GuardBreak()
    {
        m_guardBreaked = true;
        m_sprite.Break();
        if(m_timeline.currentAction) m_timeline.currentAction.Break();
    }

    protected virtual void BlockAttack(Character _blocker)
    {
        
    }

    public virtual void Taunt(Character _attacker, float _aggro) {}

    public virtual void Dead()
    {
        m_sprite.Dead();
        
    }
    public virtual void StartFight()
    {
        m_timeline.StartTimer();
    }
    public void StopFight()
    {
        m_timeline.StopTimer();
    }
    
    private void PlayActionEffect(ActionEffect _effect, Character _target)
    {
        if (isDead) return;
        switch (_effect)
        {
            case ActionEffect.MELEE_ATTACK:
                if (_target != null)
                {
                    _target.Hit(this, m_data.strength + (m_buffDamage ? 1.0f : 0.0f), _effect);
                }
                break;
            case ActionEffect.TAUNT :
                if (_target != null)
                {
                    _target.Taunt(this, 5.0f * m_data.strength);
                }
                break;
            case ActionEffect.START_GUARD:
                m_guardValue = m_data.guardValue;
                break;
            case ActionEffect.BUFF:
                m_buffDamage = true;
                break;
        }
    }
}
