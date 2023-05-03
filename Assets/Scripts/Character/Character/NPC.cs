using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable] public class NPC : Character
{
    public delegate void NPCDeadEvent();
    public static event NPCDeadEvent OnNPCDead;
    
    public static int NUMBER_NPC = 0;
    public NPC(CharacterData _data) : base(_data, true)
    {
        m_name = GameManager.instance.npcNames[NUMBER_NPC];
        ++NUMBER_NPC;
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
        
        GameObject actionPrefab = m_data.GetActionData(actionSpell.actionSpell.type).timeLineBarPrefab;
        if (actionPrefab && !isDead)
        {
            float desiredTimePos;
            if (m_timeline.IsPointOnTimeLine(Mouse.current.position.ReadValue(), out desiredTimePos))
            {
                float timePos;
                if(m_timeline.TryAddAction(actionPrefab, desiredTimePos, out timePos))
                {
                    m_timeline.AddAction(actionPrefab, timePos);
                    actionSpell.Activate();
                }
            }
        }
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
