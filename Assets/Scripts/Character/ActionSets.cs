using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class ActionData
{
    public ActionType actionType;
    public string description;
    public Color color;
    public Sprite icone;
    public float duration;
    public List<ActionStepData> actions;
}

[CreateAssetMenu(fileName = "Data", menuName = "Character/New action sets", order = 1)]
public class ActionSets : ScriptableObject
{
    [SerializeField] private float m_invulnerabilityDuration;
    [SerializeField] private float m_tauntDuration;
    [SerializeField] private float m_attackBuffDuration;
    [SerializeField] private float m_attackPotionBuffDuration;
    [SerializeField] private float m_attackPotionDebuffDuration;
    [SerializeField] private float m_healPotionBuffValue;
    [SerializeField] private float m_timeWarpDuration;
    [SerializeField] private List<CharacterActionData> m_actions;
    public List<CharacterActionData> actions => m_actions;
    public float invulnerabilityDuration => m_invulnerabilityDuration;
    public float tauntDuration => m_tauntDuration;
    public float attackBuffDuration => m_attackBuffDuration;
    
    public float attackPotionBuffDuration => m_attackPotionBuffDuration;
    public float attackPotionDebuffDuration => m_attackPotionDebuffDuration;
    public float healPotionBuffValue => m_healPotionBuffValue;
    public float timeWarpDuration => m_timeWarpDuration;
}
