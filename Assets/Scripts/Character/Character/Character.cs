using System;
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
    public CharacterSprite sprite => m_sprite;
    public CharacterSpriteEvent spriteEvent => m_sprite.spriteEvent;
    public bool isDead => m_life.isDead;
    public CharacterData data => m_data;
    public String name => m_data.characterName;
    public TimeLine timeline => m_timeline;
    public Character target => m_target;

    private Character() { }
    public Character(CharacterData _data, bool _npc)
    {
        m_timeline = GameManager.timelineManager.RequestTimeline(_data.header);
        m_timeline.OnAction += OnAction;
        m_timeline.OnEndAction += OnEndAction;
        m_data = _data;
        m_life = new Life(m_data.life);
        m_guardValue = 0.0f;
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

    public void Heal(float _heal)
    {
        spriteEvent.Healed();
        m_life.Hit(-_heal);
    }
    
    public virtual bool TryHit(Character _attacker, float _damage)
    {
        float damage = _damage;
        if (m_guardValue > 0.0f)
        {
            m_guardValue -= damage;

            if (m_currentActionPlayed && m_currentActionPlayed.type == ActionType.GUARD_ATTACK)
            {
                if (m_guardValue <= 0.0f) CounterAttackThenBreak(_attacker);
                else CounterAttack(_attacker);
            }
            else
            {
                if (m_guardValue <= 0.0f) GuardBreak();
                else spriteEvent.BlockSuccess();
            }
            
            
            return false;
        }

        if (m_guardBreaked) damage *= 2.0f;
        m_life.Hit(damage);
        m_sprite.Hit(m_life.lifeRatio, damage);
        
        if (m_life.isDead) Dead();
        /*else if(m_guardValue <= 0.0f && Random.Range(0.0f, 1.0f) < m_data.hitStunProba)
        {
            AddAction(ActionType.HIT, m_timeline.elapsedTime);
            spriteEvent.Stun();
        }*/

        spriteEvent.DamageReceived();
        return true;
    }

    private void CounterAttack(Character _attacker)
    {
        m_sprite.CounterAttack(_attacker);
        spriteEvent.BlockSuccess();
    }

    private void CounterAttackThenBreak(Character _attacker)
    {
        m_guardBreaked = true;
        m_sprite.CounterAttackThenBreak(_attacker);
        spriteEvent.GuardBreak();
        if (m_timeline.currentAction) m_timeline.currentAction.Break(m_data.GetActionData(ActionType.HIT).color);
    }

    private void GuardBreak()
    {
        m_guardBreaked = true;
        m_sprite.Break();
        
        spriteEvent.GuardBreak();
        if(m_timeline.currentAction) m_timeline.currentAction.Break(m_data.GetActionData(ActionType.HIT).color);
    }

    public virtual void Taunt(Character _attacker, float _aggro) {}

    public void Staggered()
    {
        Debug.Log("Stagger.");
        AddAction(ActionType.HIT, m_timeline.elapsedTime);
        spriteEvent.Stun();
    }

    
    protected void AddAction(ActionType _type, float _timePos)
    {
        float timePos = timeline.GetCellForTimePos(_timePos + 1.0f/timeline.cellsPerUnit);
        ActionData data = m_data.GetActionData(_type);
        m_timeline.AddAction(data, timePos);
        spriteEvent.NewActionReceived(data.actionType);
    }
    
    public virtual void Dead()
    {
        m_sprite.Dead();
        spriteEvent.Die();
    }

    public virtual void StartFight()
    {
        m_timeline.StartTimer();
    }
    public void StopFight()
    {
        m_timeline.StopTimer();
    }

    private TimeLineAction m_currentActionPlayed;

    private void OnAction(TimeLineAction _action)
    {
        if (isDead) return;
     
        m_currentActionPlayed = _action;
        m_currentActionPlayed.PlayAction(this);
        m_guardValue = 0.0f;
        
        spriteEvent.ActionStart(_action.type);
        //m_sprite.PlayAction(m_target, _action);
    }
    private void OnEndAction(TimeLineAction _action)
    {
        m_guardValue = 0.0f;
        spriteEvent.ActionEnd(_action.type);
        
        if (m_currentActionPlayed && m_currentActionPlayed == _action)
        {
            m_currentActionPlayed = null;
            m_sprite.Idle();
        }
    }

    private void PlayActionEffect(ActionEffect _effect, Character _target)
    {
        if (isDead) return;

        var aura = _target.m_timeline.GetCurrentAura();
        Debug.Log(_target.name + " " + aura.invulnerability + " " + aura.attackMultiplier + " " + aura.defenceMultiplier);
        switch (_effect)
        {
            case ActionEffect.ATTACK:
                Debug.Log("Try attack: " + m_currentActionPlayed.type);
                if (_target != null && !_target.timeline.GetCurrentAura().invulnerability)
                {
                    float strength = m_data.strength * (m_currentActionPlayed && m_currentActionPlayed.type == ActionType.ATTACK_ATTACK? 2.0f : 1.0f) * aura.attackMultiplier;

                    if (m_currentActionPlayed.type == ActionType.ATTACK_GUARD)
                    {
                        Debug.Log("Is invulnerable.");
                        m_timeline.AddAura(new Aura(timeline.elapsedTime, m_data.actionSets.invulnerabilityDuration, 1.0f, 1.0f, true));
                    }
                    if (_target.TryHit(this, strength))
                    {
                        Debug.Log("Attack done " + strength + ".");
                        spriteEvent.DamageInflicted();
                    }
                    else
                    {
                        Debug.Log("Attack blocked.");
                        spriteEvent.Blocked();
                    }
                }
                break;
            case ActionEffect.ATTACK_MAGIC:
                if (_target != null)
                {
                    
                }
                break;
            case ActionEffect.TAUNT :
                if (_target != null)
                {
                    _target.Taunt(this, 5.0f * m_data.strength);
                }
                break;
            case ActionEffect.START_GUARD:
                m_guardValue = m_data.guardValue * (m_currentActionPlayed && m_currentActionPlayed.type == ActionType.GUARD_GUARD? 2.0f : 1.0f) * aura.defenceMultiplier;
                
                Debug.Log("Guard start " + m_guardValue + ".");
                break;
            case ActionEffect.STOP_GUARD:
                m_guardValue = 0.0f;
                Debug.Log("Guard stop.");
                break;
            case ActionEffect.INTERRUPT:
                _target.Staggered();
                break;
            case ActionEffect.BUFF:
                
                m_timeline.AddAura(new Aura(timeline.elapsedTime, m_data.actionSets.attackBuffDuration,
                    1.0f, 1.0f, true));
                break;
        }
    }

    public void PlayActionStep(ActionStep _step, float _duration)
    {
        m_sprite.PlayActionStep(_step, _duration);
    }
}
