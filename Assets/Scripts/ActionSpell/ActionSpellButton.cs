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

    public ActionSpell actionSpell => m_actionSpell;
    public void SetActionSpell(ActionSpell _actionSpell)
    {
        m_actionSpell = _actionSpell;
    }

    public delegate void ActionSpellClickEvent(ActionType _type);
    public static event ActionSpellClickEvent OnActionSpellClick;
    
    private delegate void ResetByOtherClickEvent();
    private static event ResetByOtherClickEvent OnResetByOtherClick;
    

    void Awake()
    {
        m_originalColor = m_itemColor.color;
    }

    private void OnEnable()
    {
        OnResetByOtherClick += Reset;
        ControlsManager.OnClick += OnClick;
        ControlsManager.OnRelease += Reset;
        ControlsManager.OnRightClick += Reset;
    }
    private void OnDisable()
    {
        OnResetByOtherClick -= Reset;
        ControlsManager.OnClick -= OnClick;
        ControlsManager.OnRelease -= Reset;
        ControlsManager.OnRightClick -= Reset;
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
        ControlsManager.SetCurrentActionSpellButton(this);
        OnResetByOtherClick?.Invoke();

        TimeLine.OnNPCAddAction += Activate;
        m_itemColor.color = Color.gray;
    }

    public void Activate()
    {
        m_cooldown += m_actionSpell.cooldown;
        m_itemColor.color = Color.gray;
        Reset();
    }

    public void Reset()
    {
        TimeLine.OnNPCAddAction -= Activate;
    }
}
