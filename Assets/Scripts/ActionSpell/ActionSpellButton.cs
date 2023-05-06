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
    [SerializeField] private Animator m_button;
    [SerializeField] private Image m_cooldownUIImage;
    [SerializeField] private TextMeshProUGUI m_cooldownUIText;
    
    private ActionSpell m_actionSpell;
    private float m_cooldown = 0.0f;
    private bool m_isSelected;
    private Vector3 m_originalPos;
    
    private Camera m_cameraRef;

    public ActionSpell actionSpell => m_actionSpell;
    public void SetActionSpell(ActionSpell _actionSpell)
    {
        m_actionSpell = _actionSpell;
    }

    public delegate void ActionSpellClickEvent(ActionType _type);
    public static event ActionSpellClickEvent OnActionSpellClick;
    
    private delegate void ResetByOtherClickEvent(ActionSpellButton _other);
    private static event ResetByOtherClickEvent OnResetByOtherClick;
    

    void Awake()
    {
        m_cameraRef = FindObjectOfType<Canvas>().worldCamera;
    }

    private void OnEnable()
    {
        m_originalPos = m_button.transform.localPosition;
        OnResetByOtherClick += OtherSelected;
        ControlsManager.OnClick += OnClick;
        ControlsManager.OnRelease += UnSelect;
        ControlsManager.OnRightClick += UnSelect;
    }

    private void OnDisable()
    {
        m_button.transform.localPosition = m_originalPos;
        OnResetByOtherClick -= OtherSelected;
        ControlsManager.OnClick -= OnClick;
        ControlsManager.OnRelease -= UnSelect;
        ControlsManager.OnRightClick -= UnSelect;
    }

    void Update()
    {
        Vector3 position = m_button.transform.localPosition;
        if (m_isSelected)
        {
            Vector2 localPoint;
            isMouseOnButton(out localPoint);
            m_button.transform.localPosition = Vector3.Lerp(position, localPoint, 0.1f);
            GameManager.instance.DrawHoverAction(actionSpell);
        }
        else if(m_cooldown <= 0.0f)
        {
            m_button.transform.localPosition = Vector3.Lerp(position, m_originalPos, 0.1f);
        }
        else
        {
            m_button.transform.localPosition = m_originalPos;
        }
        
        if (m_cooldown > 0.0f)
        {
            m_cooldown -= Time.deltaTime * GameManager.timelineManager.timelineScale;
            m_cooldownUIImage.fillAmount = m_cooldown / m_actionSpell.cooldown;
            m_cooldownUIText.text = math.ceil(m_cooldown).ToString();
            if (m_cooldown <= 0.0f)
            {
                Reset();
            }
        }
        else
        {
            m_cooldownUIImage.fillAmount = 0.0f;
            m_cooldownUIText.text = "";
        }
    }

    public void OnClick()
    {
        if (m_cooldown > 0.0f) return;

        Vector2 localPoint;
        if (isMouseOnButton(out localPoint))
        {
            Select();
        }

    }

    public void Select()
    {
        if (m_cooldown > 0.0f) return;
        ControlsManager.SetCurrentActionSpellButton(this);
        OnResetByOtherClick?.Invoke(this);

        m_isSelected = true;
        
        m_button.SetTrigger("Select");
    }

    public void Activate()
    {
        m_cooldown += m_actionSpell.cooldown;
        m_button.SetTrigger("Activate");
    }

    public void UnSelect()
    {
        m_isSelected = false;
        m_button.SetTrigger("UnSelect");
    }

    private void OtherSelected(ActionSpellButton _other)
    {
        if(m_isSelected && this != _other) UnSelect();
    }
    
    public void Reset()
    {
        m_button.SetTrigger("Reset");
        m_cooldown = 0.0f;
    }

    
    private bool isMouseOnButton(out Vector2 _localPoint)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Mouse.current.position.ReadValue(), m_cameraRef, out _localPoint))
        {
            if (((RectTransform)transform).rect.Contains(_localPoint))
            {
                return true;
            }
        }
        return false;
    }
}
