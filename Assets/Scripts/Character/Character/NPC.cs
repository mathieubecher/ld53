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
        GameObject actionPrefab = m_data.GetActionData(ControlsManager.currentActionType).timeLineBarPrefab;
        if(actionPrefab && !isDead) m_timeline.Click(actionPrefab, Mouse.current.position.ReadValue());
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
