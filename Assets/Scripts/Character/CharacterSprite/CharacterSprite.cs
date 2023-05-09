using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterSprite : MonoBehaviour
{
    [SerializeField] private LifeBar m_bar;
    private Animator m_animator;
    
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

    public void SetTarget(Character _target)
    {
        m_target = _target;
    }
    
    public void PlayActionStep(ActionStep _step, float _duration)
    {
        Debug.Log("Play " + _step + " " + _duration);
        ResetTriggers();
        switch (_step)
        {
            case ActionStep.IDLE:
                m_animator.SetTrigger("Idle");
                StopSprite();
                break;
            case ActionStep.START_GUARDING:
                m_animator.SetTrigger("StartGuard");
                break;
            case ActionStep.STOP_GUARDING:
                m_animator.SetTrigger("StopGuard");
                break;
            case ActionStep.ATTACK:
                m_animator.SetTrigger("Attack");
                break;
            case ActionStep.SPECIAL:
                m_animator.SetTrigger("Special");
                break;
            case ActionStep.REACH_TARGET:
                m_animator.SetTrigger("ReachTarget");
                ReachTarget(_duration);
                break;
            case ActionStep.RETURN_TO_POSITION:
                m_animator.SetTrigger("ReturnToPosition");
                ReturnToPosition(_duration);
                break;
            case ActionStep.SPECIAL_ANTICIPATION:
                m_animator.SetTrigger("SpecialAnticipation");
                break;
            case ActionStep.HIT:
                m_animator.SetTrigger("Hit");
                ReturnToPosition(0.1f);
                break;
            default:
                return;
        }
    }
    
    public void Break()
    {
        ResetTriggers();
        m_animator.SetTrigger("GuardBreak");
    }

    public void Idle()
    {
        ResetTriggers();
        m_animator.SetTrigger("Idle");
    }

    public void Dead()
    {
        ResetTriggers();
        m_animator.SetBool("dead", true);
    }

    private void ResetTriggers()
    {
        m_animator.ResetTrigger("Idle");
        m_animator.ResetTrigger("Guard");
        m_animator.ResetTrigger("Attack");
        m_animator.ResetTrigger("Special");
        m_animator.ResetTrigger("ReachTarget");
        m_animator.ResetTrigger("ReturnToPosition");
        m_animator.ResetTrigger("SpecialAnticipation");
    }

    public void ReachTarget(float _time)
    {
        //Debug.Log("Reach");
        m_returnToPosition = false;
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

}
