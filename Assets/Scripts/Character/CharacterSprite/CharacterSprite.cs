using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterSprite : MonoBehaviour
{
    [SerializeField] private LifeBar m_bar;
    private Animator m_animator;
    
    private TimeLineAction m_action;
    private Character m_target;
    
    [SerializeField] private bool m_reachTarget = false;
    [SerializeField] private bool m_returnToPosition = false;
    [SerializeField] private float m_displacementTime = 0.0f;

    
    public delegate void PlayActionEffectEvent(ActionEffect _effect, Character _target);
    public event PlayActionEffectEvent OnPlayActionEffect;
    
    void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_animator.speed = GameManager.timelineManager.timelineScale;
    }

    void Update()
    {
        if (m_returnToPosition)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, m_displacementTime >= Time.deltaTime ? Time.deltaTime / m_displacementTime : 1.0f);
        }
        else if (m_reachTarget && m_target != null)
        {
            var position = transform.position;
            var targetPos = m_target.sprite.transform.position;
            Vector3 desiredPosition = position + (targetPos - position).normalized * ((targetPos - position).magnitude - 3.0f);
            position = Vector3.Lerp(position, desiredPosition, m_displacementTime >= Time.deltaTime ? Time.deltaTime / m_displacementTime : 1.0f);
            
            transform.position = position;
        }

        if(m_displacementTime > 0.0f) m_displacementTime -= Time.deltaTime * GameManager.timelineManager.timelineScale;
        m_animator.speed = GameManager.timelineManager.timelineScale;
    }
    public void Hit(float _lifeRatio, float _damage)
    {
        if (!m_bar) m_bar.GetComponentInChildren<LifeBar>();
        m_bar.SetLifeRatio(_lifeRatio);
    }

    public void PlayAction(Character _target, TimeLineAction _action)
    {
        m_animator.ResetTrigger("Attack");
        m_animator.ResetTrigger("Guard");
        m_animator.ResetTrigger("Buff");
        m_animator.ResetTrigger("Hit");
        switch (_action.type)
        {
            case ActionType.ATTACK:
                m_animator.SetTrigger("Attack");
                break;
            case ActionType.GUARD:
                m_animator.SetTrigger("Guard");
                break;
            case ActionType.BUFF:
                m_animator.SetTrigger("Buff");
                break;
            case ActionType.HIT:
                m_animator.SetTrigger("Hit");
                break;
            default:
                return;
        }
        m_returnToPosition = false;
        m_action = _action;
        m_target = _target;
    }

    public void ReachTarget(float _time)
    {
        //Debug.Log("Reach");
        m_reachTarget = true;
        m_displacementTime = _time;
    }
    public void StopSprite()
    {
        m_reachTarget = false;
        m_returnToPosition = false;
    }
    public void ReturnToPosition(float _time)
    {
        //Debug.Log("Return");
        m_returnToPosition = true;
        m_reachTarget = false;
        m_displacementTime = _time;
    }

    public void PlayActionEffect(ActionEffect _effect)
    {
        OnPlayActionEffect?.Invoke(_effect, m_target);
    }

    public void Dead()
    {
        m_animator.ResetTrigger("Attack");
        m_animator.ResetTrigger("Guard");
        m_animator.ResetTrigger("Buff");
        m_animator.ResetTrigger("Hit");
        m_animator.SetBool("dead", true);
        
    }
}
