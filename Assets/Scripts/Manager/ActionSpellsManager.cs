using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSpellsManager : MonoBehaviour
{
    [SerializeField] private Transform m_actionSpellParent;
    [SerializeField] private float m_margin = 10.0f;

    private List<ActionSpellButton> m_buttons;
    private bool m_isHover;
    
    public delegate void HoverEvent(bool _isHover);
    public static event HoverEvent OnHover;

    private void Awake()
    {
        ControlsManager.OnSpellInput += OnSpellInput;
    }

    private void OnDestroy()
    {
        Reset();
        ControlsManager.OnSpellInput -= OnSpellInput;
    }

    private void Update()
    {
        bool isHover = false;
        foreach (var button in m_buttons)
        {
            if (button.IsHover())
            {
                isHover = true;
                break;
            }
        }

        if (m_isHover != isHover)
        {
            OnHover?.Invoke(isHover);
            m_isHover = isHover;
        }
    }
    
    public void Init(List<ActionSpell> _actionSpells, float _offset)
    {
        m_buttons = new List<ActionSpellButton>();
        m_actionSpellParent.localPosition = Vector3.left * _offset;
        float cumulHeight = 0.0f;
        foreach (var actionSpell in _actionSpells)
        {
            if (!actionSpell.buttonPrefab) continue;
            
            GameObject button = Instantiate(actionSpell.buttonPrefab, m_actionSpellParent);
            ActionSpellButton actionSpellButton = button.GetComponent<ActionSpellButton>();
            
            button.transform.localPosition += Vector3.down * cumulHeight;
            
            cumulHeight += ((RectTransform)button.transform).rect.height + m_margin;
            actionSpellButton.SetActionSpell(actionSpell);
            m_buttons.Add(actionSpellButton);
        }
    }

    private void OnSpellInput(int _number)
    {
        if (_number-1 < m_buttons.Count)
        {
            m_buttons[_number-1].Select();
        }
    }

    public void Reset()
    {
        
    }
}
