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
        m_timeline.InvertCursor();
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
            if (m_timeline.GetHoverAction(desiredTimePos,out TimeLineAction other, false) 
                && GameManager.CanCombine(other.type, actionSpell.actionSpell.type) )
            {
                Combine(other, actionSpell.actionSpell.type);
                actionSpell.Activate();
            }
            else
            {
                var actionData = m_data.GetActionData(actionSpell.actionSpell.type);
                if (!actionData) return;
                float duration = actionData.duration;
                if(m_timeline.TryAddAction(duration, desiredTimePos, out timePos))
                {
                    AddAction(actionSpell.actionSpell.type, timePos);
                    actionSpell.Activate();
                }
            }
        }
    }

    private void Combine(TimeLineAction _action, ActionType _otherType)
    {
        ActionType type = GameManager.GetCombinedType(_action.type, _otherType);

        var actionData = m_data.GetActionData(type);
        if (actionData)
        {
            _action.SetActionData(actionData);
            _action.SetColor(actionData.color);
            _action.SetIcone(actionData.icone);
        }
    }

    public bool TryDrawAction(ActionType _type, Transform _arrow, RectTransform _overlay)
    {
        var actionData = m_data.GetActionData(_type);
        if (!actionData) return false;
        float duration = actionData.duration;
        
        if (isMouseInTimeline(out float desiredTimePos))
        {
            float timePos;
            if (m_timeline.GetHoverAction(desiredTimePos,out TimeLineAction other, false) && GameManager.CanCombine(other.type, _type) )
            {
                m_timeline.DrawActionOverlay(other.duration, _overlay, other.timePosition);
                _arrow.position = m_sprite.transform.position;
                return true;
            }
            if (m_timeline.TryAddAction(duration, desiredTimePos, out timePos))
            {
                m_timeline.DrawActionOverlay(duration, _overlay, timePos);
                _arrow.position = m_sprite.transform.position;
                return true;
            }
        }

        return false;
    }
    public string GetSelectedSpellDescription(ActionSpell _spell)
    {
        var actionData = m_data.GetActionData(_spell.type);
        if (!actionData) return "";
        if (isMouseInTimeline(out float desiredTimePos))
        {
            if (m_timeline.GetHoverAction(desiredTimePos,out TimeLineAction other, false) && GameManager.CanCombine(other.type, _spell.type) )
            {
                ActionType type = GameManager.GetCombinedType(other.type, _spell.type);
                actionData = m_data.GetActionData(type);
                if (actionData) return m_data.ReplaceDescriptionValues(actionData.description);
            }
            if (m_timeline.TryAddAction(actionData.duration, desiredTimePos, out float _))
            {
                return m_data.ReplaceDescriptionValues(actionData.description);
            }
        }

        return "";
    }

    public override void StartFight()
    {
        m_target = GameManager.instance.player;
        m_sprite.SetTarget(m_target);
        base.StartFight();
    }

    public override void Dead()
    {
        base.Dead();
        OnNPCDead?.Invoke();
    }

    public ActionType SelectActionSpell()
    {
        return m_data.SelectActionSpell();
    }
}
