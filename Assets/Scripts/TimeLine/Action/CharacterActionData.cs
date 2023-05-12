using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "Character/New action data", order = 1)]
public class CharacterActionData : ScriptableObject
{
    public ActionType actionType;
    [TextArea(3, 10)] public string description;
    public Color color;
    public Sprite icone;
    public float duration;
    public List<ActionStepData> actions;
}