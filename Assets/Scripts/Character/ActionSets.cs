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
    [SerializeField] private float m_attackBuffDuration;
    [SerializeField] private float m_attackPotionBuffDuration;
    [SerializeField] private float m_guardPotionBuffDuration;
    [SerializeField] private float m_healPotionBuffValue;
    [SerializeField] private List<CharacterActionData> m_actions;
    public List<CharacterActionData> actions => m_actions;
    public float invulnerabilityDuration => m_invulnerabilityDuration;
    public float attackBuffDuration => m_attackBuffDuration;
    
    public float attackPotionBuffDuration => m_attackPotionBuffDuration;
    public float guardPotionBuffDuration => m_guardPotionBuffDuration;
    public float healPotionBuffValue => m_healPotionBuffValue;
}
