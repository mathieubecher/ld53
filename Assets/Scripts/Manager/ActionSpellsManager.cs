using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionSpellsManager : MonoBehaviour
{
    [SerializeField] private Transform m_actionSpellParent;
    [SerializeField] private float m_margin = 10.0f;

    private List<ActionSpellButton> m_buttons;
    private bool m_isHover;
    
    public delegate void HoverEvent(bool _isHover);
    public static event HoverEvent OnHover;

    private void OnEnable()
    {
        ControlsManager.OnSpellInput += OnSpellInput;
    }

    private void OnDisable()
    {
        Reset();
        ControlsManager.OnSpellInput -= OnSpellInput;
    }

    private void Update()
    {
        var buttons = m_buttons.OrderBy(x => x.priority).ToList();
        bool isHover = false;
        int i = 1;
        foreach (var button in buttons)
        {
            if (!isHover && button.IsHover())
            {
                isHover = true;
            }
            button.transform.SetSiblingIndex(i);
            ++i;
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
        m_actionSpellParent.localPosition = new Vector2(-_offset, m_actionSpellParent.localPosition.y);
        float cumulHeight = 0.0f;
        int number = 1;
        foreach (var actionSpell in _actionSpells)
        {
            if (!actionSpell.buttonPrefab) continue;
            
            GameObject button = Instantiate(actionSpell.buttonPrefab, m_actionSpellParent);
            ActionSpellButton actionSpellButton = button.GetComponent<ActionSpellButton>();
            
            button.transform.localPosition += Vector3.down * cumulHeight;
            
            cumulHeight += ((RectTransform)button.transform).rect.height + m_margin;
            actionSpellButton.SetActionSpell(actionSpell);
            actionSpellButton.SetActionSpellNumber(number);
            m_buttons.Add(actionSpellButton);
            ++number;
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

        foreach (var button in m_buttons)
        {
            Destroy(button.gameObject);
        }
        m_buttons = new List<ActionSpellButton>();
        
    }
}
