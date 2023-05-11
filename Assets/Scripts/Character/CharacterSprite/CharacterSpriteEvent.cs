using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteEvent : MonoBehaviour
{
    public delegate void ActionStepEvent(ActionStep _step);
    public event ActionStepEvent OnActionStep;
    public void ActionStep(ActionStep _step)
    {
        OnActionStep?.Invoke(_step);
    }

    public delegate void ActionStartEvent(ActionType _type);
    public event ActionStartEvent OnActionStart;
    public void ActionStart(ActionType _type)
    {
        OnActionStart?.Invoke(_type);
    }

    public delegate void ActionEndEvent(ActionType _type);
    public event ActionEndEvent OnActionEnd;
    public void ActionEnd(ActionType _type)
    {
        OnActionEnd?.Invoke(_type);
    }
    
    public delegate void DamageInflictedEvent();
    public event DamageInflictedEvent OnDamageInflicted;
    public void DamageInflicted()
    {
        OnDamageInflicted?.Invoke();
    }

    public delegate void BlockedEvent();
    public event BlockedEvent OnBlocked;
    public void Blocked()
    {
        OnBlocked?.Invoke();
    }
    
    public delegate void DamageReceivedEvent();
    public event DamageReceivedEvent OnDamageReceived;
    public void DamageReceived()
    {
        OnDamageReceived?.Invoke();
    }

    public delegate void BlockSuccessEvent();
    public event BlockSuccessEvent OnBlockSuccess;
    public void BlockSuccess()
    {
        OnBlockSuccess?.Invoke();
    }

    public delegate void MagicDamageInflictedEvent();
    public event MagicDamageInflictedEvent OnMagicDamageInflicted;
    public void MagicDamageInflicted()
    {
        OnMagicDamageInflicted?.Invoke();
    }

    public delegate void MagicBlockedEvent();
    public event MagicBlockedEvent OnMagicBlocked;
    public void MagicBlocked()
    {
        OnMagicBlocked?.Invoke();
    }
    
    public delegate void MoveStartEvent();
    public event MoveStartEvent OnMoveStart;
    public void MoveStart()
    {
        OnMoveStart?.Invoke();
    }

    public delegate void MoveEndEvent();
    public event MoveEndEvent OnMoveEnd;
    public void MoveEnd()
    {
        OnMoveEnd?.Invoke();
    }

    public delegate void NewActionReceivesEvent(ActionType _type);
    public event NewActionReceivesEvent OnNewActionReceived;
    public void NewActionReceived(ActionType _type)
    {
        OnNewActionReceived?.Invoke(_type);
    }

    public delegate void HealedEvent();
    public event HealedEvent OnHealed;
    public void Healed()
    {
        OnHealed?.Invoke();
    }
    
    public delegate void DieEvent();
    public event DieEvent OnDie;
    public void Die()
    {
        OnDie?.Invoke();
    }


    public delegate void GuardBreakEvent();
    public event GuardBreakEvent OnGuardBreak;
    public void GuardBreak()
    {
        OnGuardBreak?.Invoke();
    }

    public delegate void StunEvent();
    public event StunEvent OnStun;
    public void Stun()
    {
        OnStun?.Invoke();
    }

    public delegate void FootstepEvent();
    public event FootstepEvent OnFootstep;
    public void Footstep()
    {
        OnFootstep?.Invoke();
    }
}
