using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable] public class AuraEffect
{
    public float attackMultiplier = 1.0f;
    public float attackDeMultiplier = 1.0f;
    public bool taunt = false;
    public bool invulnerability = false;
    public AuraEffect(){}

    public AuraEffect(float _attackMultiplier, float _attackDeMultiplier, bool _taunt, bool _invulnerability)
    {
        attackMultiplier = _attackMultiplier;
        attackDeMultiplier = _attackDeMultiplier;
        taunt = _taunt;
        invulnerability = _invulnerability;
    }
        
    public static AuraEffect operator +(AuraEffect a, AuraEffect b)
    {
        AuraEffect result = new AuraEffect();
        result.attackMultiplier =  math.max(a.attackMultiplier, b.attackMultiplier);
        result.attackDeMultiplier =  math.min(a.attackDeMultiplier, b.attackDeMultiplier);
        result.invulnerability = a.invulnerability || b.invulnerability;
        result.taunt = a.taunt || b.taunt;
        return result;
    }
    public static bool operator ==(AuraEffect a, AuraEffect b)
    {
        return a.invulnerability == b.invulnerability && a.taunt == b.taunt && Math.Abs(a.attackMultiplier - b.attackMultiplier) < 0.001f && Math.Abs(a.attackDeMultiplier - b.attackDeMultiplier) < 0.001f;
    }
    public static bool operator !=(AuraEffect a, AuraEffect b)
    {
        return a.invulnerability != b.invulnerability || a.taunt != b.taunt || Math.Abs(a.attackMultiplier - b.attackMultiplier) >= 0.001f;
    }
}
public class Aura : MonoBehaviour
{
    public float timePosition = 0.0f;
    public float duration = 0.0f;
    public AuraEffect effect;
    [HideInInspector] public bool played = false;
    [HideInInspector] public bool stop = false;

    
    public void SetAura(float _timePosition, float _duration, float _attackMultiplier, bool _taunt, bool _invulnerability)
    {
        timePosition = _timePosition;
        duration = _duration;
        effect = new AuraEffect(_attackMultiplier > 1.0f? _attackMultiplier : 1.0f, _attackMultiplier < 1.0f? _attackMultiplier : 1.0f, _taunt, _invulnerability);
    }
    public void SetSize(float _size)
    {
        RectTransform rect = (RectTransform)transform;
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, _size);
    }

}
