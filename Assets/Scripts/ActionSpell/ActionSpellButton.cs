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
    [SerializeField] private TextMeshProUGUI m_actionSpellNumber;
    
    private ActionSpell m_actionSpell;
    private bool m_isSelected;
    private Vector3 m_originalPos;
    [SerializeField] private int m_priority = 0;
    
    private Camera m_cameraRef;

    public ActionSpell actionSpell => m_actionSpell;
    public int priority => m_priority;
    public void SetActionSpell(ActionSpell _actionSpell)
    {
        m_actionSpell = _actionSpell;
    }
    public void SetActionSpellNumber(int _number)
    {
        m_actionSpellNumber.text = _number.ToString();
    }

    public delegate void ActionSpellEvent();
    private static event ActionSpellEvent OnSelected;
    private static event ActionSpellEvent OnDropped;
    private static event ActionSpellEvent OnActivated;
    
    
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
        ControlsManager.OnRelease += Dropped;
        ControlsManager.OnRightClick += Dropped;
    }

    private void OnDisable()
    {
        m_button.transform.localPosition = m_originalPos;
        OnResetByOtherClick -= OtherSelected;
        ControlsManager.OnClick -= OnClick;
        ControlsManager.OnRelease -= Dropped;
        ControlsManager.OnRightClick -= Dropped;
    }

    private void FixedUpdate()
    {
        Vector3 position = m_button.transform.localPosition;
        if (m_isSelected)
        {
            Vector2 localPoint;
            isMouseOnButton(out localPoint);
            m_button.transform.localPosition = Vector3.Lerp(position, localPoint, 0.3f);
            GameManager.instance.DrawHoverAction(actionSpell);
        }
        else
        {
            m_button.transform.localPosition = Vector3.Lerp(position, m_originalPos, 0.3f);
        }

    }

    public void OnClick()
    {
        Vector2 localPoint;
        if (isMouseOnButton(out localPoint))
        {
            Select();
        }

    }

    public void Select()
    {
        ControlsManager.instance.SetCurrentActionSpellButton(this);
        OnResetByOtherClick?.Invoke(this);

        m_isSelected = true;
        
        m_button.SetTrigger("Select");
        m_priority = 1000;
        
        OnSelected?.Invoke();
    }

    public void Activate()
    {
        Destroy(gameObject);
        
        OnActivated?.Invoke();
    }

    public void Dropped()
    {
        m_isSelected = false;
        m_button.SetTrigger("UnSelect");
        m_priority = 0;
        OnDropped?.Invoke();
    }
    
    public void Remove()
    {
        m_isSelected = false;
        m_button.SetTrigger("Remove");
        m_button.transform.localPosition = m_originalPos;
    }

    private void OtherSelected(ActionSpellButton _other)
    {
        if(m_isSelected && this != _other) Dropped();
    }
    
    public void Reset()
    {
        m_button.SetTrigger("Reset");
        m_priority = 10;
        StartCoroutine(ResetPrio());
    }

    private IEnumerator ResetPrio()
    {
        yield return new WaitForSeconds(0.3f);
        m_priority = 0;
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

    public bool IsHover()
    {
        return isMouseOnButton(out _);
    }

    public bool IsSelected()
    {
        return m_isSelected;
    }
}
