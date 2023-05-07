using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsManager : MonoBehaviour
{

    private static ControlsManager m_instance;
    public static ControlsManager instance
    {
        get
        {
            if (!m_instance)
            {
                m_instance = FindObjectOfType<ControlsManager>();
            }
            return m_instance;
        }
    }
    
    [SerializeField] private Texture2D m_idleCursor;
    [SerializeField] private Texture2D m_hoverCursor;
    [SerializeField] private Texture2D m_selectCursor;
    
    private static ActionSpellButton m_selectedActionSpellButton;
    private bool m_isHover;
    public static ActionSpellButton selectedActionSpellButton => m_selectedActionSpellButton;
    public void SetCurrentActionSpellButton(ActionSpellButton _actionSpellButton)
    {
        m_selectedActionSpellButton = _actionSpellButton;
        SelectCursor();
    }
    private void ResetActionType()
    {
        m_selectedActionSpellButton = null;
        if (m_isHover) HoverCursor();
        else IdleCursor();
    }
    private void OnActionHover(bool _ishover)
    {
        m_isHover = _ishover;
        if (m_selectedActionSpellButton) return;
        if(_ishover) HoverCursor();
        else IdleCursor();
    }

    private void IdleCursor()
    {
        Cursor.SetCursor(m_idleCursor, new Vector2(1.0f, 1.0f), CursorMode.ForceSoftware);
    }
    private void HoverCursor()
    {
        Cursor.SetCursor(m_hoverCursor, new Vector2(16.0f,21.0f), CursorMode.ForceSoftware);
    }

    private void SelectCursor()
    {
        Cursor.SetCursor(m_selectCursor, new Vector2(16.0f,21.0f), CursorMode.ForceSoftware);
    }
    
    public delegate void ClickEvent();
    public static event ClickEvent OnClick;
    public static event ClickEvent OnRelease;
    
    public delegate void SimpleEvent();
    public static event SimpleEvent OnRightClick;
    public static event SimpleEvent OnRightRelease;
    public delegate void SpellInputEvent(int _number);
    public static event SpellInputEvent OnSpellInput;

    public static event SimpleEvent OnAccelTime;
    public static event SimpleEvent OnDecelTime;
    
    void OnEnable()
    {
        ResetActionType();
        ActionSpellsManager.OnHover += OnActionHover;
    }

    void OnDisable()
    {
        ActionSpellsManager.OnHover -= OnActionHover;
        
    }

    void Update()
    {
    }
    
    public void ReadClickInput(InputAction.CallbackContext _context)
    {
        if (_context.performed)
            OnClick?.Invoke();
        else if (_context.canceled)
        {
            OnRelease?.Invoke();
            ResetActionType();
        }
    }
    public void ReadRightClickInput(InputAction.CallbackContext _context)
    {
        if (_context.performed)
        {
            OnRightClick?.Invoke();
            ResetActionType();
        }
        else if(_context.canceled)
            OnRightRelease?.Invoke();
    }
    
    public void One(InputAction.CallbackContext _context)
    {
        if (_context.performed)
            OnSpellInput?.Invoke(1);
    }
    public void Two(InputAction.CallbackContext _context)
    {
        if (_context.performed)
            OnSpellInput?.Invoke(2);
    }
    public void Three(InputAction.CallbackContext _context)
    {
        if (_context.performed)
            OnSpellInput?.Invoke(3);
    }
    public void Four(InputAction.CallbackContext _context)
    {
        if (_context.performed)
            OnSpellInput?.Invoke(4);
    }
    public void Five(InputAction.CallbackContext _context)
    {
        if (_context.performed)
            OnSpellInput?.Invoke(5);
    }
    public void AccelTime(InputAction.CallbackContext _context)
    {
        if (_context.performed)
        {
            OnAccelTime?.Invoke();
        }
        else if (_context.canceled)
        {
            OnDecelTime?.Invoke();
        }
    }
}
