using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable] public struct ActionStepData
{
    public ActionStep steps;
    public int numberOfCells;
}
[Serializable] public struct RandomAction
{
    public ActionType actionType;
    public float weight;
}

[CreateAssetMenu(fileName = "Data", menuName = "Character/New type", order = 1)]
public class CharacterData : ScriptableObject
{

    [SerializeField] private string m_characterName;
    [SerializeField] private string m_faction;
    [SerializeField] private Sprite m_timeLineHeader;
    [SerializeField] private GameObject m_spritePrefab;
    
    [SerializeField] private ActionSets m_actionSets;
    [SerializeField] private float  m_life = 10.0f;
    [SerializeField] private float  m_strength = 1.0f;
    [SerializeField] private float  m_magica = 0.0f;
    [SerializeField] private float m_guardValue = 1.0f;
    
    [SerializeField] private List<RandomAction> m_randomActions;
    [SerializeField] private ActionData m_hitAction;

    public string characterName => m_characterName;
    public string faction => m_faction;
    public GameObject spritePrefab => m_spritePrefab;
    public Sprite header => m_timeLineHeader;
    public float life => m_life;
    public float strength => m_strength;
    public float magica => m_magica;
    public float guardValue => m_guardValue;
    
    public ActionSets actionSets => m_actionSets;
    public List<ActionData> actionDatas => m_actionSets.actions;
    
    public ActionData GetActionData(ActionType actionType)
    {
        if (actionType == ActionType.HIT) return m_hitAction;
        return actionDatas.Find(x => x.actionType == actionType);
    }

    public ActionType SelectActionSpell()
    {
        float totalWeight = 0.0f;
        foreach (RandomAction action in m_randomActions)
        {
            totalWeight += action.weight;
        }

        float randomValue = Random.Range(0f, totalWeight);

        foreach (RandomAction action in m_randomActions)
        {
            randomValue -= action.weight;
            if (randomValue <= 0f)
            {
                return action.actionType;
            }
        }

        return ActionType.ATTACK;
    }
}
