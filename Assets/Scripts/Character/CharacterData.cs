using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Character/New type", order = 1)]
public class CharacterData : ScriptableObject
{
    [Serializable] public struct ActionData
    {
        [SerializeField] public ActionType actionType;
        [SerializeField] public GameObject timeLineBarPrefab;
    }

    [SerializeField] private string m_className = "Bandit";
    [SerializeField] private string m_characterName;
    [SerializeField] private GameObject m_spritePrefab;
    [SerializeField] private float  m_life = 10.0f;
    [SerializeField] private float  m_strength = 1.0f;
    [SerializeField] private float  m_magica = 0.0f;
    [SerializeField] private float  m_resistance = 0.0f;
    [SerializeField] private float  m_magicaResistance = 0.0f;
    [SerializeField] private float  m_hitStunProba = 0.0f;
    
    [SerializeField] private List<ActionData> m_actionDatas;
    [SerializeField] private ActionData m_hitAction;
    
    public GameObject spritePrefab => m_spritePrefab;
    public float life => m_life;
    public float strength => m_strength;
    public float magica => m_magica;
    public float resistance => m_resistance;
    public float magicaResistance => m_magicaResistance;
    public float hitStunProba => m_hitStunProba;
    
    public List<ActionData> actionDatas => m_actionDatas;
    public string characterName => m_characterName;

    public ActionData GetActionData(ActionType actionType)
    {
        if (actionType == ActionType.HIT) return m_hitAction;
        return m_actionDatas.Find(x => x.actionType == actionType);
    }
}
