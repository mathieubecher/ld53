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
    [SerializeField] private List<ActionData> m_actionDatas;
    public List<ActionData> actions => m_actionDatas;
    public float invulnerabilityDuration => m_invulnerabilityDuration;
    public float attackBuffDuration => m_attackBuffDuration;
}
