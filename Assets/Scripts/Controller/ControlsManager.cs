using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsManager : MonoBehaviour
{
    [SerializeField] private ActionType currentActionType = ActionType.NULL;
    
    public delegate void ClickEvent(ActionType actionType);
    public static event ClickEvent OnClick;
    public static event ClickEvent OnRelease;
    
    public delegate void SimpleEvent();
    public static event SimpleEvent OnRightClick;
    public static event SimpleEvent OnRightRelease;
    public delegate void SpellInputEvent(int _number);
    public static event SpellInputEvent OnSpellInput;

    public static event SimpleEvent OnAccelTime;
    public static event SimpleEvent OnDecelTime;
    void Awake()
    {
        ResetActionType();
        TimeLine.OnNPCAddAction += OnNpcAddAction;
        ActionSpellButton.OnActionSpellClick += OnActionSpellClick;
    }
    private void OnDestroy()
    {
        TimeLine.OnNPCAddAction -= OnNpcAddAction;
        ActionSpellButton.OnActionSpellClick -= OnActionSpellClick;
    }

    private void OnNpcAddAction()
    {
        ResetActionType();
    }

    private void OnActionSpellClick(ActionType _type)
    {
        currentActionType = _type;
    }

    private void ResetActionType()
    {
        currentActionType = ActionType.NULL;
    }

    public void ReadClickInput(InputAction.CallbackContext _context)
    {
        if (_context.performed)
            OnClick?.Invoke(ActionType.NULL);
        else if (_context.canceled)
        {
            OnRelease?.Invoke(currentActionType);
            currentActionType = ActionType.NULL;
        }
    }
    public void ReadRightClickInput(InputAction.CallbackContext _context)
    {
        if (_context.performed)
        {
            ResetActionType();
            OnRightClick?.Invoke();
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
