using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Aura : MonoBehaviour
{
    public float timePosition = 0.0f;
    public float duration = 0.0f;
    public AuraEffect effect;
    [HideInInspector] public bool played = false;
    [HideInInspector] public bool stop = false;

    [Serializable] public class AuraEffect
    {
        public float attackMultiplier = 1.0f;
        public float defenceMultiplier = 1.0f;
        public bool invulnerability = false;
        public AuraEffect(){}

        public AuraEffect(float _attackMultiplier, float _defenceMultiplier, bool _invulnerability)
        {
            attackMultiplier = _attackMultiplier;
            defenceMultiplier = _defenceMultiplier;
            invulnerability = _invulnerability;
        }
        
        public static AuraEffect operator +(AuraEffect a, AuraEffect b)
        {
            AuraEffect result = new AuraEffect();
            result.attackMultiplier =  math.max(a.attackMultiplier, b.attackMultiplier);
            result.defenceMultiplier = math.max(a.defenceMultiplier, b.defenceMultiplier);
            result.invulnerability = a.invulnerability || b.invulnerability;
            return result;
        }
    }
    public void SetAura(float _timePosition, float _duration, float _attackMultiplier, float _defenceMultiplier, bool _invulnerability)
    {
        timePosition = _timePosition;
        duration = _duration;
        effect = new AuraEffect(_attackMultiplier, _defenceMultiplier, _invulnerability);
    }
    public void SetSize(float _size)
    {
        RectTransform rect = (RectTransform)transform;
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, _size);
    }

}
