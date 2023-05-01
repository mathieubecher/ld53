using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ActionSpellButton : MonoBehaviour
{
    [SerializeField] private Image m_cooldownUIImage;
    [SerializeField] private Image m_itemColor;
    private Color m_originalColor;
    
    private ActionSpell m_actionSpell;
    private float m_cooldown = 0.0f;

    public delegate void ActionSpellClickEvent(ActionType _type);
    public static event ActionSpellClickEvent OnActionSpellClick;
    
    private delegate void ResetByOtherClickEvent();
    private static event ResetByOtherClickEvent OnResetByOtherClick;
    
    public void SetActionSpell(ActionSpell _actionSpell)
    {
        m_actionSpell = _actionSpell;
    }

    void Awake()
    {
        m_originalColor = m_itemColor.color;
    }

    private void OnEnable()
    {
        OnResetByOtherClick += Reset;
        ControlsManager.OnClick += OnClick;
        ControlsManager.OnRelease += OnRelease;
    }
    private void OnDisable()
    {
        OnResetByOtherClick -= Reset;
        ControlsManager.OnClick -= OnClick;
        ControlsManager.OnRelease -= OnRelease;
    }

    void Update()
    {
        if (m_cooldown > 0.0f)
        {
            m_cooldown -= Time.deltaTime * GameManager.timelineManager.timelineScale;
            m_cooldownUIImage.fillAmount = m_cooldown / m_actionSpell.cooldown;
        }
        else
        {
            m_itemColor.color = m_originalColor;
            m_cooldownUIImage.fillAmount = 0.0f;
        }
    }

    public void OnClick()
    {
        if (m_cooldown > 0.0f) return;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Mouse.current.position.ReadValue(), FindObjectOfType<Canvas>().worldCamera, out localPoint))
        {
            if (((RectTransform)transform).rect.Contains(localPoint))
            {
                Select();
            }
        }

    }

    public void Select()
    {
        if (m_cooldown > 0.0f) return;
        OnActionSpellClick?.Invoke(m_actionSpell.type);
        OnResetByOtherClick?.Invoke();

        TimeLine.OnNPCAddAction += Activate;
        ControlsManager.OnRightClick += Reset;
        m_itemColor.color = Color.gray;
    }

    private void OnRelease()
    {
        Reset();
    }


    private void Activate()
    {
        m_cooldown += m_actionSpell.cooldown;
        m_itemColor.color = Color.gray;
        Reset();
    }

    private void Reset()
    {
        TimeLine.OnNPCAddAction -= Activate;
        ControlsManager.OnRightClick -= Reset;
    }
}
