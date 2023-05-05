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

    [SerializeField] private bool m_isGuarding = false;
    [SerializeField] private bool m_isAttacking = false;
    [SerializeField] private bool m_buffDamage = false;
    public CharacterSprite sprite => m_sprite;
    public bool isDead => m_life.isDead;
    public CharacterData data => m_data;
    public String name => m_data.characterName;
    public bool isAttacking => m_isAttacking;
    public TimeLine timeline => m_timeline;

    private Character() { }
    public Character(CharacterData _data, bool _npc)
    {
        m_timeline = GameManager.timelineManager.RequestTimeline();
        m_timeline.OnAction += OnAction;
        m_data = _data;
        m_life = new Life(m_data.life);
        m_isAttacking = false;
        m_isGuarding = false;
        m_buffDamage = false;
        if(_npc) m_sprite = GameManager.characterSpriteManager.RequestNPCSprite(m_data.spritePrefab);
        else m_sprite = GameManager.characterSpriteManager.RequestPlayerSprite(m_data.spritePrefab);
        m_sprite.OnPlayActionEffect += PlayActionEffect;
    }
    
    public virtual void OnDestroy()
    {
        m_sprite.OnPlayActionEffect -= PlayActionEffect;
        m_timeline.OnAction -= OnAction;
        GameManager.timelineManager.RemoveTimeLine(m_timeline);
        GameManager.characterSpriteManager.RemoveCharacterSprite(m_sprite);
    }

    private void OnAction(TimeLineAction _action)
    {
        if (isDead) return;
        m_isGuarding = false;
        m_sprite.PlayAction(m_target, _action);
        if (_action.type == ActionType.ATTACK) m_isAttacking = true;

    }

    public virtual void Hit(Character _attacker, float _damage, ActionEffect _effect)
    {
        float damage = _damage;
        m_isAttacking = false;
        if (m_isGuarding)
        {
            damage *= _effect == ActionEffect.MELEE_ATTACK || _effect == ActionEffect.DISTANCE_ATTACK ? m_data.resistance : 1.0f;
            damage *= _effect == ActionEffect.MAGIC_ATTACK ? m_data.magicaResistance : 1.0f;
            _attacker.BlockAttack(this);
        }
        m_life.Hit(damage);
        m_sprite.Hit(m_life.lifeRatio, damage);
        
        if (m_life.isDead) Dead();
        else if(!m_isGuarding && Random.Range(0.0f, 1.0f) < m_data.hitStunProba)
        {
            m_timeline.AddAction(m_data.GetActionData(ActionType.HIT).timeLineBarPrefab, m_timeline.elapsedTime);
        }
    }

    protected virtual void BlockAttack(Character _blocker)
    {
        
    }

    public virtual void Taunt(Character _attacker, float _aggro) {}

    public virtual void Dead()
    {
        Debug.Log(name + " is dead.");
        m_sprite.Dead();
        
    }
    public virtual void StartFight()
    {
        m_timeline.StartTimer();
    }
    public void StopFight()
    {
        Debug.Log(m_timeline);
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
                    m_isAttacking = false;
                }
                break;
            case ActionEffect.TAUNT :
                if (_target != null)
                {
                    _target.Taunt(this, 5.0f * m_data.strength);
                }
                break;
            case ActionEffect.START_GUARD:
                m_isGuarding = true;
                break;
            case ActionEffect.END_GUARD:
                m_isGuarding = false;
                break;
            case ActionEffect.BUFF:
                m_buffDamage = true;
                break;
        }
    }
}
