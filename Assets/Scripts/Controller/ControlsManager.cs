using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsManager : MonoBehaviour
{
    private static ActionSpellButton m_selectedActionSpellButton;
    public static ActionSpellButton selectedActionSpellButton => m_selectedActionSpellButton;
    public static void SetCurrentActionSpellButton(ActionSpellButton _actionSpellButton)
    {
        m_selectedActionSpellButton = _actionSpellButton;
    }
    private void ResetActionType()
    {
        m_selectedActionSpellButton = null;
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
