using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Spell/New spell", order = 1)]
public class ActionSpell : ScriptableObject
{
    
    [SerializeField] private ActionType m_type = ActionType.NULL;
    [SerializeField] private float m_cooldown = 10.0f;
    [SerializeField] private GameObject m_buttonPrefab;

    public ActionType type => m_type;
    public float cooldown => m_cooldown;
    public GameObject buttonPrefab => m_buttonPrefab;
    
}
