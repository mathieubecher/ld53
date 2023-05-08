using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable] public class NPC : Character
{
    public delegate void NPCDeadEvent();

    public static event NPCDeadEvent OnNPCDead;
    public NPC(CharacterData _data) : base(_data, true)
    {
        ControlsManager.OnRelease += OnRelease;
    }

    public override void OnDestroy()
    {
        ControlsManager.OnRelease -= OnRelease;
        base.OnDestroy();
    }
    
    private void OnRelease()
    {
        ActionSpellButton actionSpell = ControlsManager.selectedActionSpellButton;
        if (!actionSpell) return;
        
        float desiredTimePos;
        if (isMouseInTimeline( out desiredTimePos))
        {
            float timePos;
            float duration = m_data.GetActionData(actionSpell.actionSpell.type).duration;
            if(m_timeline.TryAddAction(duration, desiredTimePos, out timePos))
            {
                AddAction(actionSpell.actionSpell.type, timePos);
                actionSpell.Activate();
            }
        }
    }

    public bool TryDrawAction(ActionType _type, Transform _arrow, RectTransform _overlay)
    {
        
        float duration = m_data.GetActionData(_type).duration;
        
        float desiredTimePos;
        if (isMouseInTimeline(out desiredTimePos))
        {
            float timePos;
            if (m_timeline.TryAddAction(duration, desiredTimePos, out timePos))
            {
                m_timeline.DrawActionOverlay(duration, _overlay, timePos);
                _arrow.position = m_sprite.transform.position;
                return true;
            }
        }

        return false;
    }

    private bool isMouseInTimeline(out float _desiredTimePos)
    {
        _desiredTimePos = 0.0f;
        if (!isDead)
        {
            if (m_timeline.IsPointOnTimeLine(Mouse.current.position.ReadValue(), out _desiredTimePos))
            {
                return true;
            }
        }
        return false;
    }

    public override void StartFight()
    {
        m_target = GameManager.instance.player;
        base.StartFight();
    }

    public override void Dead()
    {
        base.Dead();
        OnNPCDead?.Invoke();
    }
}
